# FIAS Microservices

Микросервисная архитектура для получения KLADR кодов через Dadata API с сохранением результатов в MSSQL.

## Описание

Проект реализует двухсервисную архитектуру для:
- Поиска адресов через Dadata Suggestions API
- Получения KLADR кодов для адресов
- Сохранения истории запросов в базе данных

## Архитектура

### Сервисы:
- FIAS API (`:8080`) — прокси к Dadata Suggestions API, поиск адресов
- *lient API (`:8081`) — бизнес-логика, валидация, сохранение результатов в БД
- MSSQL Server (`:1433`) — хранение результатов запросов

### Технологии:
- ASP.NET Core 8.0
- Docker & Docker Compose
- Entity Framework Core
- Dadata API
- Swagger/OpenAPI

## Быстрый старт

### Предварительные требования:
- Docker Desktop
- API ключ от Dadata (получить на https://dadata.ru/profile/apikey/)

### Запуск:

1. Создайте файл `.env` в корне проекта:
```bash
DADATA_API_KEY=ваш_api_ключ
SQL_PASSWORD=YourPassword123!

Запустите контейнеры:
docker compose up --build

Откройте Swagger:
FIAS API: http://localhost:8080/swagger
Client API: http://localhost:8081/swagger

Blazor API: http://localhost:5000
