using CloudStorageDemo.Models;
using CloudStorageDemo.Patterns.Bridge;
using CloudStorageDemo.Patterns.Facade;
using CloudStorageDemo.Patterns.Providers;
using CloudStorageDemo.Patterns.Proxy;
using CloudStorageDemo.Patterns.Strategy;

namespace CloudStorageDemo.Program;

public static class DemoRunner
{
    public static async Task RunAsync()
    {
        Header();

        // ---- Сборка компонентов ----
        // Adapter: SDK → ICloudProvider.
        var awsProvider = new AwsProviderAdapter(new AwsLikeProvider());
        var azureProvider = new AzureProviderAdapter(new AzureLikeProvider());

        // Bridge: логика синхронизации поверх ICloudProvider.
        var awsStorage = new SyncCloudStorage(awsProvider);
        var azureStorage = new SyncCloudStorage(azureProvider);

        // Strategy: одна политика.
        var policy = new RoleBasedAccessPolicy();

        // Proxy + Facade.
        var proxyAws = new CachedStorageProxy(new RealStorageService(awsStorage), policy, cacheCapacity: 3);
        var proxyAzure = new CachedStorageProxy(new RealStorageService(azureStorage), policy, cacheCapacity: 3);

        var syncAws = new SyncService(awsStorage, proxyAws);
        var syncAzure = new SyncService(azureStorage, proxyAzure);

        // Пользователи.
        var admin = new User("Alice", Role.Admin);
        var bob = new User("Bob", Role.User);
        var guest = new User("Guest", Role.Guest);

        // ---------- 1. Adapter + Bridge ----------
        Section("1. Adapter + Bridge: два провайдера за одним интерфейсом");
        Console.WriteLine($"Провайдер A: {syncAws.ProviderName}");
        Console.WriteLine($"Провайдер B: {syncAzure.ProviderName}");

        // ---------- 2. Загрузка ----------
        Section("2. Загрузка файла через единый интерфейс");
        await syncAws.UploadAsync(admin, "public/readme.txt", "Hello from AWS-like");
        await syncAzure.UploadAsync(admin, "public/readme.txt", "Hello from Azure-like");

        // ---------- 3. Первое скачивание ----------
        Section("3. Первое скачивание — КЭШ-ПРОМАХ");
        var t1 = await syncAws.DownloadAsync(admin, "public/readme.txt");
        Console.WriteLine($"Получено: \"{t1}\"");

        // ---------- 4. Повторное ----------
        Section("4. Повторное чтение — КЭШ-ХИТ");
        var t2 = await syncAws.DownloadAsync(admin, "public/readme.txt");
        Console.WriteLine($"Получено: \"{t2}\"");

        // ---------- 5. Guest не читает private ----------
        Section("5. Контроль доступа: Guest читает private/*");
        await syncAws.UploadAsync(admin, "private/secret.txt", "TOP-SECRET");
        try
        {
            await syncAws.DownloadAsync(guest, "private/secret.txt");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"!!! Отказ: {ex.Message}");
        }

        // ---------- 6. User не пишет в private ----------
        Section("6. Контроль доступа: User пишет в private/*");
        try
        {
            await syncAws.UploadAsync(bob, "private/hack.txt", "nope");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"!!! Отказ: {ex.Message}");
        }

        // ---------- 7. User читает public ----------
        Section("7. User читает public/* — разрешено");
        var t3 = await syncAws.DownloadAsync(bob, "public/readme.txt");
        Console.WriteLine($"Bob получил: \"{t3}\"");

        // ---------- 8. Независимые кэши ----------
        Section("8. Кэши разных прокси независимы");
        Console.WriteLine($"Кэш AWS:   [{string.Join(", ", proxyAws.CachedKeys)}]");
        Console.WriteLine($"Кэш Azure: [{string.Join(", ", proxyAzure.CachedKeys)}]");

        // ---------- 9. Инвалидация ----------
        Section("9. Инвалидация кэша");
        proxyAws.Invalidate("public/readme.txt");
        Console.WriteLine($"Кэш AWS после: [{string.Join(", ", proxyAws.CachedKeys)}]");
        await syncAws.DownloadAsync(admin, "public/readme.txt");

        // ---------- 10. LRU-вытеснение ----------
        Section("10. LRU-вытеснение (capacity = 3)");
        await syncAws.UploadAsync(admin, "public/a.txt", "A");
        await syncAws.UploadAsync(admin, "public/b.txt", "B");
        await syncAws.UploadAsync(admin, "public/c.txt", "C");
        Console.WriteLine($"Кэш до: [{string.Join(", ", proxyAws.CachedKeys)}]");
        await syncAws.UploadAsync(admin, "public/d.txt", "D");
        Console.WriteLine($"Кэш после: [{string.Join(", ", proxyAws.CachedKeys)}]");

        // ---------- 11. Azure ----------
        Section("11. Azure-провайдер: те же сценарии, другой SDK");
        Console.WriteLine($"Azure: \"{await syncAzure.DownloadAsync(admin, "public/readme.txt")}\"");
        Console.WriteLine($"Azure: \"{await syncAzure.DownloadAsync(admin, "public/readme.txt")}\"");

        Footer();
    }

    private static void Header()
    {
        Console.WriteLine("========================================================");
        Console.WriteLine(" ДЕМО: CloudStorageDemo (Adapter + Bridge + Proxy + ...)");
        Console.WriteLine("========================================================");
    }

    private static void Section(string title)
        => Console.WriteLine($"\n=== {title} ===");

    private static void Footer()
    {
        Console.WriteLine("\n========================================================");
        Console.WriteLine(" ДЕМО ЗАВЕРШЕНО");
        Console.WriteLine("========================================================");
    }
}