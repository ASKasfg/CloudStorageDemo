# CloudStorageDemo

ƒемонстрационное консольное приложение (.NET 6+) Ч клиент облачного хранилища
с двум€ провайдерами и разными API, единым интерфейсом синхронизации,
контролем доступа и кэшем Ђгор€чихї файлов.

## „то демонстрирует

1. ¬ыбор провайдера (AWS-like / Azure-like) Ч без изменени€ кода клиента.
2. «агрузка/скачивание файла через единый `ICloudProvider`.
3. ѕовторное обращение к файлу идЄт из кэша (быстро).
4. ќтказ в доступе при нехватке прав.

## ѕаттерны

| ѕаттерн | –оль | √де |
|---|---|---|
| **Adapter** | Ќесовместимые SDK двух провайдеров ? единый `ICloudProvider` | `Patterns/Providers/*Adapter.cs` |
| **Bridge** | Ћогика синхронизации (`CloudStorage`) отделена от реализации провайдера | `Patterns/Bridge/*` |
| **Proxy** | `CachedStorageProxy` Ч контроль доступа + кэш Ђгор€чихї файлов | `Patterns/Proxy/CachedStorageProxy.cs` |
| **Strategy** | —мен€ема€ политика доступа `IAccessPolicy` | `Patterns/Strategy/*` |
| **Facade** | `SyncService` Ч едина€ точка входа дл€ клиента | `Patterns/Facade/SyncService.cs` |

## ƒиаграмма классов

![ƒиаграмма классов](docs/class-diagram.png)

—м. также `docs/class-diagram.puml`.

## —криншоты

![ эш-промах vs кэш-хит](docs/screenshot-2.png)
![ќтказ в доступе](docs/screenshot-3.png)

## «апуск

```bash
git clone <repo-url>
cd CloudStorageDemo
dotnet run --project CloudStorageDemo
```

“ребуетс€ .NET SDK 6.0+.

## ќбоснование паттернов

- **Adapter** Ч два разных SDK нельз€ использовать напр€мую (у AWS Ч бакеты и byte[], у Azure Ч контейнеры и Stream). Ѕез адаптеров были бы ветвлени€ `if (provider == "aws") ... else ...` по всему коду.
- **Bridge** Ч иначе `AwsSyncStorage`, `AzureSyncStorage`, а при второй логике ещЄ `*CompressedStorage` Ч комбинаторный взрыв M?N. Bridge даЄт M+N.
- **Proxy** Ч контроль доступа и кэш нужны прозрачно дл€ клиента. Ѕез Proxy клиент провер€л бы права сам ? риск обхода.
- **Strategy** Ч матрица прав зашита в прокси ? кажда€ правка Ч риск сломать основное.
- **Facade** Ч клиенту не надо собирать цепочку Bridge+Proxy+Strategy вручную.