# Как деплоить обновления mauth на стенд

**Стенд:** `https://ru01-vm23.gal.lan/`  
**Расположение на сервере:** `/etc/app/mauth`  
**Репозиторий:** `https://gitlab.galaktika.local/ovchinnikov/mauth` (ветка `SeSZh_Workflows_Deploy`)

---

## Быстрый деплой (обычный случай — только код C#)

```bash
cd /etc/app/mauth
git pull gitlab SeSZh_Workflows_Deploy
docker compose up -d --build --remove-orphans app
```

- `--build` — пересобирает образ `mauth_app` из исходников
- `--remove-orphans` — убирает контейнеры, которых больше нет в compose
- `app` — пересобирает и перезапускает только приложение, не трогая postgres/seq/jaeger/front

---

## Если изменился фронтенд (`frontend/`)

```bash
cd /etc/app/mauth
git pull gitlab SeSZh_Workflows_Deploy
docker compose up -d --build --remove-orphans front
```

---

## Если изменился `docker-compose.yml` (новые сервисы, переменные, порты)

```bash
cd /etc/app/mauth
git pull gitlab SeSZh_Workflows_Deploy
docker compose down --remove-orphans
docker compose up -d --wait
```

`--wait` держит команду до тех пор, пока все контейнеры с healthcheck не станут healthy
(или завершится с ошибкой, если что-то не поднялось). Seq поднимается дольше остальных —
у него `start_period: 60s`, так что команда будет ждать около минуты.

---

## Полный пересброс (например, нужно почистить БД)

> Осторожно: удаляет все данные Postgres и Seq.

```bash
cd /etc/app/mauth
git pull gitlab SeSZh_Workflows_Deploy
docker compose down --remove-orphans
rm -rf docker_data/pgdata docker_data/seq
docker compose up -d --build --wait
```

---

## Полезные команды

```bash
# Статус контейнеров
docker compose ps

# Логи приложения (последние 100 строк + следить)
docker compose logs -f --tail=100 app

# Зайти в контейнер приложения
docker compose exec app bash

# Подключиться к БД
docker compose exec postgres psql -U postgres -d modular_monolith

# Принудительно пересобрать все образы без кэша
docker compose build --no-cache
docker compose up -d --remove-orphans
```

---

## Что где доступно после деплоя

| Сервис              | URL                                                  |
|---------------------|------------------------------------------------------|
| Frontend / Auth test| https://ru01-vm23.gal.lan/external/mauth/            |
| Swagger UI          | https://ru01-vm23.gal.lan/external/mauth/swagger/    |
| API                 | https://ru01-vm23.gal.lan/external/mauth/api/        |
| Seq (логи)          | https://ru01-vm23.gal.lan/external/mauth/seq/        |
| Jaeger (трейсы)     | https://ru01-vm23.gal.lan/external/mauth/jaeger/     |

---

## Примечания

- Postgres, Seq и Jaeger при обычном деплое **не трогаем** — их данные хранятся в `./docker_data/`.
- Образ приложения всегда собирается локально из исходников (`build: context: .`), готового образа в registry нет.
- После `git pull` убедитесь, что нет конфликтов (`git status`). Ветка деплоя — `SeSZh_Workflows_Deploy`.
