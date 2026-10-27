# WT-6 Minimal API

REST API интернет-магазина на ASP.NET Core Minimal API (.NET 8). Данные хранятся в памяти (`ProductStore`), после перезапуска возвращаются к исходным.

## API

| Метод | URL | Назначение | Статусы |
|-------|-----|-----------|---------|
| GET | / | проверить работу приложения | 200 |
| GET | /api/info | информация о приложении | 200 |
| GET | /api/products | получить список (`?search=` — поиск по названию) | 200 |
| GET | /api/products/{id} | получить объект | 200, 404 |
| POST | /api/products | создать объект | 201, 400 |
| PUT | /api/products/{id} | изменить объект | 200, 400, 404 |
| DELETE | /api/products/{id} | удалить объект | 204, 404 |

Документация OpenAPI: `/swagger/v1/swagger.json`, Swagger UI: `/swagger`.

## Примеры

POST /api/products
```json
{ "name": "Планшет", "category": "Компьютеры", "price": 1200, "isAvailable": true }
```
Ответ `201 Created`, заголовок `Location: /api/products/7`:
```json
{ "id": 7, "name": "Планшет", "category": "Компьютеры", "price": 1200, "isAvailable": true }
```
Некорректные данные — `400 Bad Request`:
```json
{ "errors": ["Name: введите название товара", "Price: цена должна быть больше 0"] }
```

## Репозиторий

https://github.com/lera2005261-ux/wt-lab6-6minimalapi-baranova

## Опубликованное приложение

https://wt-lab6-6minimalapi-baranova.onrender.com/
