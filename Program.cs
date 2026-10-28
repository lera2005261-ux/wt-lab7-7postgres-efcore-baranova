using Microsoft.EntityFrameworkCore;
using wt_lab7_7postgres_efcore_baranova.Data;
using wt_lab7_7postgres_efcore_baranova.Models;
var builder = WebApplication.CreateBuilder(args);

// ---- Строка подключения: локально ConnectionStrings:DefaultConnection, на Render DATABASE_URL ----
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var databaseUrl = builder.Configuration["DATABASE_URL"];

if (!string.IsNullOrWhiteSpace(databaseUrl))
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':', 2);
    var dbPort = uri.Port > 0 ? uri.Port : 5432;   // во внутреннем URL Render порта может не быть
    connectionString =
        $"Host={uri.Host};" +
        $"Port={dbPort};" +
        $"Database={uri.AbsolutePath.TrimStart('/')};" +
        $"Username={Uri.UnescapeDataString(userInfo[0])};" +
        $"Password={Uri.UnescapeDataString(userInfo[1])};" +
        "SSL Mode=Disable";   // только для Internal Database URL (private network Render)
}

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

// ---- OpenAPI (Swagger) ----
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
    o.SwaggerDoc("v1", new()
    {
        Title = "Products API",
        Version = "v1",
        Description = "REST API интернет-магазина на ASP.NET Core Minimal API + EF Core + PostgreSQL"
    }));

var app = builder.Build();

// ---- Автоматическое применение миграций при запуске (EnsureCreated не использовать!) ----
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();

// Проверка данных из тела запроса -> 400 Bad Request
static List<string> Validate(Product p)
{
    var errors = new List<string>();
    if (string.IsNullOrWhiteSpace(p.Name)) errors.Add("Name: введите название товара");
    if (string.IsNullOrWhiteSpace(p.Category)) errors.Add("Category: введите категорию");
    if (p.Price <= 0) errors.Add("Price: цена должна быть больше 0");
    return errors;
}

// ---- Информация ----
app.MapGet("/", () => "Minimal API работает!");

app.MapGet("/api/info", () => new
{
    Application = "Web Technologies",
    Technology = "ASP.NET Core Minimal API + EF Core + PostgreSQL",
    Version = "2.0"
});

// ---- GET: список ----
app.MapGet("/api/products", async (AppDbContext db) =>
    Results.Ok(await db.Products.ToListAsync()))
.WithTags("Products").Produces<List<Product>>(StatusCodes.Status200OK);

// ---- GET: поиск по названию (LINQ -> SQL на стороне БД) ----
app.MapGet("/api/products/search", async (string? search, AppDbContext db) =>
{
    var query = db.Products.AsQueryable();
    if (!string.IsNullOrWhiteSpace(search))
        query = query.Where(x => x.Name.ToLower().Contains(search.ToLower()));
    return Results.Ok(await query.ToListAsync());
})
.WithTags("Products").Produces<List<Product>>(StatusCodes.Status200OK);

// ---- GET: LINQ-запросы ----
// Фильтрация
app.MapGet("/api/products/expensive", async (AppDbContext db) =>
    Results.Ok(await db.Products.Where(x => x.Price > 100).ToListAsync()))
.WithTags("LINQ");

// Сортировка
app.MapGet("/api/products/sorted", async (AppDbContext db) =>
    Results.Ok(await db.Products.OrderBy(x => x.Price).ToListAsync()))
.WithTags("LINQ");

// Выборка с условием
app.MapGet("/api/products/available", async (AppDbContext db) =>
    Results.Ok(await db.Products.Where(x => x.IsAvailable).OrderBy(x => x.Name).ToListAsync()))
.WithTags("LINQ");

// Собственный запрос: статистика по категориям
app.MapGet("/api/products/stats", async (AppDbContext db) =>
    Results.Ok(await db.Products
        .GroupBy(x => x.Category)
        .Select(g => new { Category = g.Key, Count = g.Count(), AvgPrice = g.Average(x => x.Price) })
        .OrderBy(x => x.Category)
        .ToListAsync()))
.WithTags("LINQ");

// ---- GET: по id ----
app.MapGet("/api/products/{id:int}", async (int id, AppDbContext db) =>
{
    var product = await db.Products.FirstOrDefaultAsync(x => x.Id == id);
    return product == null ? Results.NotFound() : Results.Ok(product);
})
.WithTags("Products").Produces<Product>(StatusCodes.Status200OK).Produces(StatusCodes.Status404NotFound);

// ---- POST ----
app.MapPost("/api/products", async (Product product, AppDbContext db) =>
{
    var errors = Validate(product);
    if (errors.Count > 0)
        return Results.BadRequest(new { errors });

    product.Id = 0;                        // Id создаёт БД
    product.CreatedAt = DateTime.UtcNow; // раскомментировать на шаге AddCreatedAt
    db.Products.Add(product);
    await db.SaveChangesAsync();

    return Results.Created($"/api/products/{product.Id}", product);
})
.WithTags("Products").Produces<Product>(StatusCodes.Status201Created).Produces(StatusCodes.Status400BadRequest);

// ---- PUT ----
app.MapPut("/api/products/{id:int}", async (int id, Product updatedProduct, AppDbContext db) =>
{
    var product = await db.Products.FirstOrDefaultAsync(x => x.Id == id);
    if (product == null)
        return Results.NotFound();

    var errors = Validate(updatedProduct);
    if (errors.Count > 0)
        return Results.BadRequest(new { errors });

    product.Name = updatedProduct.Name;
    product.Category = updatedProduct.Category;
    product.Price = updatedProduct.Price;
    product.IsAvailable = updatedProduct.IsAvailable;
    // после шага 35 добавьте: product.Description = updatedProduct.Description;

    await db.SaveChangesAsync();
    return Results.Ok(product);
})
.WithTags("Products").Produces<Product>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest).Produces(StatusCodes.Status404NotFound);

// ---- DELETE ----
app.MapDelete("/api/products/{id:int}", async (int id, AppDbContext db) =>
{
    var product = await db.Products.FirstOrDefaultAsync(x => x.Id == id);
    if (product == null)
        return Results.NotFound();

    db.Products.Remove(product);
    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithTags("Products").Produces(StatusCodes.Status204NoContent).Produces(StatusCodes.Status404NotFound);

// ---- Запуск: порт из переменной окружения PORT (Render) ----
var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
app.Run($"http://0.0.0.0:{port}");
//rjvvbn