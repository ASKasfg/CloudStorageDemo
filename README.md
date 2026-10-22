# CloudStorageDemo

Демонстрационное консольное приложение (.NET 6+) — клиент облачного хранилища
с двумя провайдерами и разными API, единым интерфейсом синхронизации,
контролем доступа и кэшем «горячих» файлов.

## Что демонстрирует

1. Выбор провайдера (AWS-like / Azure-like) — без изменения кода клиента.
2. Загрузка/скачивание файла через единый `ICloudProvider`.
3. Повторное обращение к файлу идёт из кэша (быстро).
4. Отказ в доступе при нехватке прав.

## Паттерны

| Паттерн | Роль | Где |
|---|---|---|
| **Adapter** | Несовместимые SDK двух провайдеров ? единый `ICloudProvider` | `Patterns/Providers/*Adapter.cs` |
| **Bridge** | Логика синхронизации (`CloudStorage`) отделена от реализации провайдера | `Patterns/Bridge/*` |
| **Proxy** | `CachedStorageProxy` — контроль доступа + кэш «горячих» файлов | `Patterns/Proxy/CachedStorageProxy.cs` |
| **Strategy** | Сменяемая политика доступа `IAccessPolicy` | `Patterns/Strategy/*` |
| **Facade** | `SyncService` — единая точка входа для клиента | `Patterns/Facade/SyncService.cs` |

## Запуск

```bash
git clone <repo-url>
cd CloudStorageDemo
dotnet run --project CloudStorageDemo
```

Требуется .NET SDK 6.0+.

## Обоснование паттернов

- **Adapter** — два разных SDK нельзя использовать напрямую (у AWS — бакеты и byte[], у Azure — контейнеры и Stream). Без адаптеров были бы ветвления `if (provider == "aws") ... else ...` по всему коду.
- **Bridge** — иначе `AwsSyncStorage`, `AzureSyncStorage`, а при второй логике ещё `*CompressedStorage` — комбинаторный взрыв M?N. Bridge даёт M+N.
- **Proxy** — контроль доступа и кэш нужны прозрачно для клиента. Без Proxy клиент проверял бы права сам ? риск обхода.
- **Strategy** — матрица прав зашита в прокси ? каждая правка — риск сломать основное.
- **Facade** — клиенту не надо собирать цепочку Bridge+Proxy+Strategy вручную.
