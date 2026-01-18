# Архитектурная и функциональная схема проекта

## Содержание

1. [Общая архитектура](#общая-архитектура)
2. [Модули системы](#модули-системы)
3. [Взаимодействие модулей](#взаимодействие-модулей)
4. [База данных](#база-данных)
5. [Функциональные потоки](#функциональные-потоки)
6. [Технологический стек](#технологический-стек)

---

## Общая архитектура

Проект построен на основе **Modular Monolith** архитектуры с использованием принципов **Clean Architecture** и **Vertical Slices**.

### Архитектурные принципы

- **Modular Monolith**: Все модули развернуты как единое приложение, но изолированы друг от друга
- **Clean Architecture**: Разделение на слои (Domain, Application, Infrastructure)
- **Vertical Slices**: Функциональность организована по бизнес-сценариям, а не по техническим слоям
- **Dependency Inversion**: Модули взаимодействуют только через PublicApi интерфейсы

### Структура модуля

Каждый модуль следует единой структуре:

```mermaid
graph TB
    subgraph Module["Модуль (например, Shipments)"]
        subgraph Domain["Domain Layer"]
            Entities["Entities<br/>Shipment, ShipmentItem"]
            ValueObjects["Value Objects<br/>Address"]
            Enums["Enums<br/>ShipmentStatus"]
            Policies["Policies<br/>Business Rules"]
        end
        
        subgraph Features["Features Layer (Vertical Slices)"]
            CreateShipment["CreateShipment/<br/>- Endpoint<br/>- Handler<br/>- Validator<br/>- Mapping"]
            ProcessShipment["ProcessShipment/<br/>- Endpoint<br/>- Handler"]
            Events["Events/<br/>- ShipmentCreatedEvent<br/>- EventHandlers"]
        end
        
        subgraph Infrastructure["Infrastructure Layer"]
            Database["Database/<br/>- DbContext<br/>- Migrations<br/>- Configurations"]
            Policies["Policies/<br/>- Authorization"]
        end
        
        subgraph PublicApi["PublicApi Layer"]
            Interface["ICarrierModuleApi<br/>IStockModuleApi"]
            Contracts["Contracts/<br/>- DTOs<br/>- Requests"]
        end
        
        subgraph InternalApi["InternalApi (Features)"]
            ModuleApi["CarrierModuleApi<br/>StockModuleApi"]
            Decorators["Decorators/<br/>- TracedCarrierModuleApi"]
        end
    end
    
    Features -->|"uses"| Domain
    Features -->|"implements"| InternalApi
    InternalApi -->|"implements"| PublicApi
    Infrastructure -->|"persists"| Domain
    Features -->|"depends on"| PublicApi
```

**Ключевые файлы:**
- Domain: [Shipments/Modules.Shipments.Domain/Entities/Shipment.cs](Shipments/Modules.Shipments.Domain/Entities/Shipment.cs)
- Features: [Shipments/Modules.Shipments.Features/DependencyInjection.cs](Shipments/Modules.Shipments.Features/DependencyInjection.cs)
- Infrastructure: [Shipments/Modules.Shipments.Infrastructure/DependencyInjection.cs](Shipments/Modules.Shipments.Infrastructure/DependencyInjection.cs)
- PublicApi: [Carriers/Modules.Carriers.PublicApi/ICarrierModuleApi.cs](Carriers/Modules.Carriers.PublicApi/ICarrierModuleApi.cs)

---

## Модули системы

### Users Module

**Назначение**: Аутентификация и авторизация пользователей

**Основные компоненты:**
- JWT токены и refresh tokens
- Identity Core для управления пользователями
- Роли и политики авторизации

**API Endpoints:**
- `POST /api/users/register` - Регистрация пользователя
- `POST /api/users/login` - Аутентификация
- `POST /api/users/refresh-token` - Обновление токена
- `GET /api/users/{id}` - Получение пользователя
- `PUT /api/users/{id}` - Обновление пользователя
- `DELETE /api/users/{id}` - Удаление пользователя

**Структура:**
```
Users/
├── Modules.Users.Domain/          # User, Role, RefreshToken entities
├── Modules.Users.Features/         # Login, Register, RefreshToken handlers
└── Modules.Users.Infrastructure/   # Identity, DbContext, Authorization
```

### Shipments Module

**Назначение**: Управление жизненным циклом отправлений

**Основные компоненты:**
- Создание и управление отправлениями
- Отслеживание статусов (Created → Processing → Dispatched → InTransit → Delivered → Received)
- Интеграция с Carriers и Stocks модулями

**API Endpoints:**
- `POST /api/shipments` - Создание отправления
- `GET /api/shipments/{number}` - Получение отправления
- `POST /api/shipments/process/{number}` - Обработка
- `POST /api/shipments/dispatch/{number}` - Отправка
- `POST /api/shipments/transit/{number}` - В пути
- `POST /api/shipments/deliver/{number}` - Доставка
- `POST /api/shipments/receive/{number}` - Получение
- `POST /api/shipments/cancel/{number}` - Отмена

**Структура:**
```
Shipments/
├── Modules.Shipments.Domain/       # Shipment, ShipmentItem entities
├── Modules.Shipments.Features/     # Vertical slices для каждого use case
└── Modules.Shipments.Infrastructure/ # DbContext, Migrations
```

### Carriers Module

**Назначение**: Управление перевозчиками

**Основные компоненты:**
- Создание и управление перевозчиками
- Создание отправлений перевозчиков
- Проверка активности перевозчиков

**API Endpoints:**
- `POST /api/carriers` - Создание перевозчика
- `GET /api/carriers/active` - Получение активных перевозчиков

**Структура:**
```
Carriers/
├── Modules.Carriers.Domain/         # Carrier, CarrierShipment entities
├── Modules.Carriers.Features/      # CreateCarrier, CreateShipment handlers
├── Modules.Carriers.Infrastructure/ # DbContext
└── Modules.Carriers.PublicApi/     # ICarrierModuleApi для межмодульного взаимодействия
```

### Stocks Module

**Назначение**: Управление складскими запасами

**Основные компоненты:**
- Создание и управление товарными запасами
- Проверка наличия товаров
- Уменьшение запасов при создании отправлений

**API Endpoints:**
- `POST /api/stocks` - Создание запаса
- `GET /api/stocks/by-product-name` - Получение по имени продукта
- `POST /api/stocks/increase` - Увеличение запаса

**Структура:**
```
Stocks/
├── Modules.Stocks.Domain/           # ProductStock entity
├── Modules.Stocks.Features/        # CheckStock, DecreaseStock handlers
├── Modules.Stocks.Infrastructure/  # DbContext
└── Modules.Stocks.PublicApi/       # IStockModuleApi для межмодульного взаимодействия
```

### Common Module

**Назначение**: Общие компоненты и утилиты

**Основные компоненты:**
- **Common.Domain**: Result pattern, Events, IHandler
- **Common.API**: IApiEndpoint, ErrorHandling, Extensions
- **Common.Application**: EventPublisher, Handler registration
- **Common.Infrastructure**: Database migrations, Policies

**Ключевые файлы:**
- [Common/Modules.Common.Domain/Results/Result.cs](Common/Modules.Common.Domain/Results/Result.cs)
- [Common/Modules.Common.Application/EventPublisher.cs](Common/Modules.Common.Application/EventPublisher.cs)
- [Common/Modules.Common.API/Extensions/MapEndpointExtensions.cs](Common/Modules.Common.API/Extensions/MapEndpointExtensions.cs)

---

## Взаимодействие модулей

### Правила зависимостей

Модули могут взаимодействовать только через **PublicApi** интерфейсы. Прямые зависимости на Domain, Features или Infrastructure других модулей запрещены.

```mermaid
graph LR
    subgraph Shipments["Shipments Module"]
        SFeatures["Features"]
        SInternalApi["InternalApi"]
    end
    
    subgraph Carriers["Carriers Module"]
        CPublicApi["PublicApi<br/>ICarrierModuleApi"]
        CFeatures["Features"]
        CDomain["Domain"]
    end
    
    subgraph Stocks["Stocks Module"]
        StPublicApi["PublicApi<br/>IStockModuleApi"]
        StFeatures["Features"]
        StDomain["Domain"]
    end
    
    subgraph Users["Users Module"]
        UFeatures["Features"]
    end
    
    SFeatures -->|"can use"| CPublicApi
    SFeatures -->|"can use"| StPublicApi
    SFeatures -.->|"cannot use"| CDomain
    SFeatures -.->|"cannot use"| StDomain
    
    SInternalApi -->|"implements"| CPublicApi
    CFeatures -->|"implements"| CPublicApi
    StFeatures -->|"implements"| StPublicApi
```

### Межмодульное взаимодействие

**1. Через PublicApi интерфейсы (синхронное)**

Shipments модуль вызывает Carriers и Stocks через их PublicApi:

```mermaid
sequenceDiagram
    participant Client
    participant Shipments as Shipments Module
    participant Carriers as Carriers Module<br/>(PublicApi)
    participant Stocks as Stocks Module<br/>(PublicApi)
    
    Client->>Shipments: POST /api/shipments
    Shipments->>Carriers: ICarrierModuleApi.CreateShipmentAsync()
    Carriers-->>Shipments: Result<Success>
    Shipments->>Stocks: IStockModuleApi.CheckStockAsync()
    Stocks-->>Shipments: Result<Success>
    Shipments->>Stocks: IStockModuleApi.DecreaseStockAsync()
    Stocks-->>Shipments: Result<Success>
    Shipments-->>Client: ShipmentResponse
```

**2. Через события (асинхронное)**

При создании shipment публикуется событие, которое обрабатывается несколькими handlers:

```mermaid
sequenceDiagram
    participant Handler as CreateShipmentHandler
    participant EventPub as EventPublisher
    participant CarrierHandler as CreateCarrierEventHandler
    participant StockHandler as UpdateStockEventHandler
    
    Handler->>Handler: Create Shipment entity
    Handler->>EventPub: Publish(ShipmentCreatedEvent)
    
    par Параллельная обработка
        EventPub->>CarrierHandler: HandleAsync(event)
        CarrierHandler->>CarrierHandler: Create CarrierShipment
    and
        EventPub->>StockHandler: HandleAsync(event)
        StockHandler->>StockHandler: Decrease Stock
    end
```

**Ключевые файлы:**
- [Shipments/Modules.Shipments.Features/Features/CreateShipment/Events/CreateCarrierEventHandler.cs](Shipments/Modules.Shipments.Features/Features/CreateShipment/Events/CreateCarrierEventHandler.cs)
- [Shipments/Modules.Shipments.Features/Features/CreateShipment/Events/UpdateStockEventHandler.cs](Shipments/Modules.Shipments.Features/Features/CreateShipment/Events/UpdateStockEventHandler.cs)
- [Common/Modules.Common.Application/EventPublisher.cs](Common/Modules.Common.Application/EventPublisher.cs)

### Аутентификация

Модули не вызывают Users модуль напрямую. Вместо этого:
1. Клиент получает JWT токен через `/api/users/login`
2. Токен используется в заголовке `Authorization: Bearer {token}`
3. ASP.NET Core middleware валидирует токен
4. Модули проверяют политики авторизации

```mermaid
sequenceDiagram
    participant Client
    participant Users as Users Module
    participant Auth as Auth Middleware
    participant Module as Any Module<br/>(Shipments/Carriers/Stocks)
    
    Client->>Users: POST /api/users/login
    Users-->>Client: JWT Token + Refresh Token
    
    Client->>Module: POST /api/shipments<br/>Authorization: Bearer {token}
    Module->>Auth: Validate Token
    Auth->>Auth: Check Policies
    Auth-->>Module: Authorized
    Module-->>Client: Response
```

---

## База данных

### Архитектура базы данных

Все модули используют одну PostgreSQL базу данных, но с изолированными схемами:

```mermaid
erDiagram
    subgraph users_schema["users schema"]
        users_users ||--o{ users_user_roles : has
        users_roles ||--o{ users_user_roles : assigned_to
        users_roles ||--o{ users_role_claims : has
        users_users ||--o{ users_refresh_tokens : has
    end
    
    subgraph shipments_schema["shipments schema"]
        shipments_shipments ||--o{ shipments_shipment_items : contains
    end
    
    subgraph carriers_schema["carriers schema"]
        carriers_carriers ||--o{ carriers_carrier_shipments : has
    end
    
    subgraph stocks_schema["stocks schema"]
        stocks_product_stocks
    end
```

### Схемы модулей

**Users Schema (`users`)**
- `users` - Пользователи (Identity)
- `roles` - Роли
- `user_roles` - Связь пользователей и ролей
- `role_claims` - Права доступа ролей
- `refresh_tokens` - Refresh токены

**Shipments Schema (`shipments`)**
- `shipments` - Отправления
  - `id` (UUID)
  - `number` (string)
  - `order_id` (string)
  - `carrier` (string)
  - `receiver_email` (string)
  - `status` (enum)
  - `address_street`, `address_city`, `address_zip`
  - `created_at`, `updated_at`
- `shipment_items` - Товары в отправлении
  - `id` (int)
  - `shipment_id` (UUID, FK)
  - `product` (string)
  - `quantity` (int)

**Carriers Schema (`carriers`)**
- `carriers` - Перевозчики
  - `id` (UUID)
  - `name` (string)
  - `is_active` (boolean)
- `carrier_shipments` - Отправления перевозчиков
  - `id` (UUID)
  - `carrier_id` (UUID, FK)
  - `order_id` (string)
  - `shipping_address_street`, `shipping_address_city`, `shipping_address_zip`
  - `created_at` (timestamp)

**Stocks Schema (`stocks`)**
- `product_stocks` - Товарные запасы
  - `id` (UUID)
  - `product_name` (string)
  - `available_quantity` (int)
  - `last_updated_at` (timestamp)

### Миграции

Каждый модуль имеет свои миграции в отдельной таблице истории:
- `users.migration_history`
- `shipments.migration_history`
- `carriers.migration_history`
- `stocks.migration_history`

Миграции выполняются автоматически при запуске в режиме Development.

**Ключевые файлы:**
- [Common/Modules.Common.Infrastructure/Database/DatabaseMigrationExtensions.cs](Common/Modules.Common.Infrastructure/Database/DatabaseMigrationExtensions.cs)
- [Users/Modules.Users.Infrastructure/Database/UsersDbContext.cs](Users/Modules.Users.Infrastructure/Database/UsersDbContext.cs)
- [Shipments/Modules.Shipments.Infrastructure/Database/ShipmentsDbContext.cs](Shipments/Modules.Shipments.Infrastructure/Database/ShipmentsDbContext.cs)

---

## Функциональные потоки

### 1. Создание Shipment

Полный поток создания отправления с взаимодействием всех модулей:

```mermaid
sequenceDiagram
    participant Client
    participant ShipmentsAPI as Shipments API
    participant CreateHandler as CreateShipmentHandler
    participant CarrierAPI as ICarrierModuleApi
    participant StockAPI as IStockModuleApi
    participant EventPub as EventPublisher
    participant CarrierHandler as CreateCarrierEventHandler
    participant StockHandler as UpdateStockEventHandler
    participant DB as Database
    
    Client->>ShipmentsAPI: POST /api/shipments<br/>{orderId, carrier, items, address}
    
    ShipmentsAPI->>CreateHandler: HandleAsync(request)
    
    Note over CreateHandler: 1. Валидация запроса
    
    CreateHandler->>DB: Check if shipment exists for orderId
    DB-->>CreateHandler: No existing shipment
    
    CreateHandler->>StockAPI: CheckStockAsync(products)
    StockAPI->>DB: Check available quantities
    DB-->>StockAPI: Stock available
    StockAPI-->>CreateHandler: Success
    
    CreateHandler->>DB: Create Shipment entity
    DB-->>CreateHandler: Shipment created
    
    CreateHandler->>EventPub: Publish(ShipmentCreatedEvent)
    
    par Параллельная обработка событий
        EventPub->>CarrierHandler: HandleAsync(event)
        CarrierHandler->>CarrierAPI: CreateShipmentAsync(request)
        CarrierAPI->>DB: Create CarrierShipment
        DB-->>CarrierAPI: Success
        CarrierAPI-->>CarrierHandler: Success
        CarrierHandler-->>EventPub: Completed
    and
        EventPub->>StockHandler: HandleAsync(event)
        StockHandler->>StockAPI: DecreaseStockAsync(request)
        StockAPI->>DB: Update product_stocks<br/>(decrease quantity)
        DB-->>StockAPI: Success
        StockAPI-->>StockHandler: Success
        StockHandler-->>EventPub: Completed
    end
    
    CreateHandler-->>ShipmentsAPI: ShipmentResponse
    ShipmentsAPI-->>Client: 200 OK + Shipment data
```

**Ключевые файлы:**
- [Shipments/Modules.Shipments.Features/Features/CreateShipment/CreateShipment.Handler.cs](Shipments/Modules.Shipments.Features/Features/CreateShipment/CreateShipment.Handler.cs)
- [Shipments/Modules.Shipments.Features/Features/CreateShipment/Events/ShipmentCreatedEvent.cs](Shipments/Modules.Shipments.Features/Features/CreateShipment/Events/ShipmentCreatedEvent.cs)

### 2. Аутентификация пользователя

```mermaid
sequenceDiagram
    participant Client
    participant UsersAPI as Users API
    participant LoginHandler as LoginUserHandler
    participant SignInManager as SignInManager
    participant UserStore as UserStore
    participant TokenService as Token Service
    participant DB as Database
    
    Client->>UsersAPI: POST /api/users/login<br/>{email, password}
    
    UsersAPI->>LoginHandler: HandleAsync(request)
    
    LoginHandler->>LoginHandler: Validate request
    
    LoginHandler->>UserStore: FindByEmailAsync(email)
    UserStore->>DB: SELECT * FROM users WHERE email = ?
    DB-->>UserStore: User entity
    UserStore-->>LoginHandler: User
    
    LoginHandler->>SignInManager: CheckPasswordSignInAsync(user, password)
    SignInManager->>DB: Verify password hash
    DB-->>SignInManager: Password valid
    SignInManager-->>LoginHandler: Success
    
    LoginHandler->>TokenService: GenerateJwtToken(user, roles)
    TokenService-->>LoginHandler: JWT Token
    
    LoginHandler->>TokenService: GenerateRefreshToken()
    TokenService-->>LoginHandler: Refresh Token
    
    LoginHandler->>DB: INSERT INTO refresh_tokens
    DB-->>LoginHandler: Token saved
    
    LoginHandler-->>UsersAPI: LoginUserResponse<br/>{token, refreshToken}
    UsersAPI-->>Client: 200 OK + Tokens
```

**Ключевые файлы:**
- [Users/Modules.Users.Features/Users/LoginUser/LoginUser.Handler.cs](Users/Modules.Users.Features/Users/LoginUser/LoginUser.Handler.cs)

### 3. Обновление статуса Shipment

```mermaid
sequenceDiagram
    participant Client
    participant ShipmentsAPI as Shipments API
    participant ProcessHandler as ProcessShipmentHandler
    participant Shipment as Shipment Entity
    participant DB as Database
    
    Client->>ShipmentsAPI: POST /api/shipments/process/{number}
    
    ShipmentsAPI->>ProcessHandler: HandleAsync(shipmentNumber)
    
    ProcessHandler->>DB: SELECT * FROM shipments<br/>WHERE number = ?
    DB-->>ProcessHandler: Shipment entity
    
    ProcessHandler->>Shipment: Process()
    
    alt Status is Created
        Shipment->>Shipment: Set Status = Processing
        Shipment->>Shipment: Set UpdatedAt = Now
        Shipment-->>ProcessHandler: Success
    else Status is not Created
        Shipment-->>ProcessHandler: Validation Error
        ProcessHandler-->>ShipmentsAPI: 400 Bad Request
        ShipmentsAPI-->>Client: Error response
    end
    
    ProcessHandler->>DB: UPDATE shipments<br/>SET status = 'Processing'
    DB-->>ProcessHandler: Updated
    
    ProcessHandler-->>ShipmentsAPI: Success
    ShipmentsAPI-->>Client: 204 No Content
```

**Ключевые файлы:**
- [Shipments/Modules.Shipments.Features/Features/ProcessShipment/ProcessShipment.Handler.cs](Shipments/Modules.Shipments.Features/Features/ProcessShipment/ProcessShipment.Handler.cs)
- [Shipments/Modules.Shipments.Domain/Entities/Shipment.cs](Shipments/Modules.Shipments.Domain/Entities/Shipment.cs) (метод `Process()`)

### 4. Жизненный цикл Shipment

Полный цикл от создания до получения:

```mermaid
stateDiagram-v2
    [*] --> Created: POST /api/shipments
    
    Created --> Processing: POST /api/shipments/process/{number}
    Processing --> Dispatched: POST /api/shipments/dispatch/{number}
    Dispatched --> InTransit: POST /api/shipments/transit/{number}
    InTransit --> Delivered: POST /api/shipments/deliver/{number}
    Delivered --> Received: POST /api/shipments/receive/{number}
    
    Created --> Cancelled: POST /api/shipments/cancel/{number}
    Processing --> Cancelled: POST /api/shipments/cancel/{number}
    Dispatched --> Cancelled: POST /api/shipments/cancel/{number}
    InTransit --> Cancelled: POST /api/shipments/cancel/{number}
    
    Delivered --> [*]: Cannot cancel
    Received --> [*]: Final state
    Cancelled --> [*]: Final state
```

---

## Технологический стек

### Основные технологии

| Технология | Назначение | Версия |
|-----------|-----------|--------|
| .NET | Платформа разработки | 10.0 |
| ASP.NET Core | Web framework | 10.0 |
| Entity Framework Core | ORM | 10.0 |
| PostgreSQL | База данных | Latest |
| FluentValidation | Валидация | Latest |
| Serilog | Логирование | Latest |
| OpenTelemetry | Observability | Latest |

### Инфраструктура

| Сервис | Назначение | Порт |
|--------|-----------|------|
| PostgreSQL | База данных | 5432 |
| Seq | Централизованное логирование | 5341, 8081 |
| Jaeger | Distributed tracing | 4317, 4318, 16686 |

### Архитектурные паттерны

- **Result Pattern**: Обработка ошибок без исключений
  - [Common/Modules.Common.Domain/Results/Result.cs](Common/Modules.Common.Domain/Results/Result.cs)
- **Handler Pattern**: Обработка use cases
  - [Common/Modules.Common.Domain/Handlers/IHandler.cs](Common/Modules.Common.Domain/Handlers/IHandler.cs)
- **Event-Driven**: Асинхронная обработка событий
  - [Common/Modules.Common.Domain/Events/IEvent.cs](Common/Modules.Common.Domain/Events/IEvent.cs)
- **Decorator Pattern**: Трейсинг для межмодульных вызовов
  - [Carriers/Modules.Carriers.Features/InternalApi/Decorators/TracedCarrierModuleApi.cs](Carriers/Modules.Carriers.Features/InternalApi/Decorators/TracedCarrierModuleApi.cs)

### Конфигурация

**Точка входа:** [ModularMonolith.Host/Program.cs](ModularMonolith.Host/Program.cs)

**Регистрация модулей:**
- [Users/Modules.Users.Features/DependencyInjection.cs](Users/Modules.Users.Features/DependencyInjection.cs)
- [Shipments/Modules.Shipments.Features/DependencyInjection.cs](Shipments/Modules.Shipments.Features/DependencyInjection.cs)
- [Carriers/Modules.Carriers.Features/DependencyInjection.cs](Carriers/Modules.Carriers.Features/DependencyInjection.cs)
- [Stocks/Modules.Stocks.Features/DependencyInjection.cs](Stocks/Modules.Stocks.Features/DependencyInjection.cs)

---

## Заключение

Проект демонстрирует современный подход к построению модульных монолитов с четким разделением ответственности, изоляцией модулей и гибким механизмом межмодульного взаимодействия. Архитектура позволяет легко масштабировать отдельные модули в будущем при необходимости перехода на микросервисы.
