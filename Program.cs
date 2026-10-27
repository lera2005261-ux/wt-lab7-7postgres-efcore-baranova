app.MapPost("/api/products", async (Product product, AppDbContext db) =>
{
    var errors = Validate(product);
    if (errors.Count > 0)
        return Results.BadRequest(new { errors });

    product.Id = 0;                        // Id создаёт БД
    // product.CreatedAt = DateTime.UtcNow; // раскомментировать на шаге AddCreatedAt
    db.Products.Add(product);
    await db.SaveChangesAsync();

    return Results.Created($"/api/products/{product.Id}", product);
})
.WithTags("Products").Produces<Product>(StatusCodes.Status201Created).Produces(StatusCodes.Status400BadRequest);