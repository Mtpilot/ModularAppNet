# Auth Frontend для ModularAppNet

Простой веб-интерфейс для авторизации в ModularAppNet API с интеграцией Swagger UI.

## Возможности

- Авторизация с тестовыми учетными данными (`admin@test.com/Test1234!`)
- Получение JWT токена
- Автоматическое копирование токена
- Интеграция со Swagger UI
- Тестирование авторизации
- Сохранение настроек в localStorage
- Адаптивный дизайн
- Docker контейнеризация

## Быстрый старт

### Способ 1: Docker Compose (рекомендуется)

```bash
# Клонируйте или создайте файлы проекта
git clone <repository-url>
cd auth-frontend

# Запустите проект
docker-compose up -d

# Откройте в браузере
open http://localhost:8080