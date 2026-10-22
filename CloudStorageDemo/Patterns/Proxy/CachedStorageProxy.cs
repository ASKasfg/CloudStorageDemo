using CloudStorageDemo.Models;
using CloudStorageDemo.Patterns.Strategy;

namespace CloudStorageDemo.Patterns.Proxy;

public sealed class CachedStorageProxy : IStorageService
{
    private readonly IStorageService _real;
    private readonly IAccessPolicy _policy;
    private readonly int _cacheCapacity;

    private readonly Dictionary<string, byte[]> _cache = new();
    private readonly LinkedList<string> _lruOrder = new();

    public CachedStorageProxy(IStorageService real, IAccessPolicy policy, int cacheCapacity = 3)
    {
        if (cacheCapacity <= 0) throw new ArgumentOutOfRangeException(nameof(cacheCapacity));
        _real = real ?? throw new ArgumentNullException(nameof(real));
        _policy = policy ?? throw new ArgumentNullException(nameof(policy));
        _cacheCapacity = cacheCapacity;
    }

    public IReadOnlyCollection<string> CachedKeys => _cache.Keys.ToArray();

    public async Task<byte[]> DownloadAsync(User user, string path)
    {
        var access = _policy.Check(user, path, Operation.Read);
        if (!access.Allowed)
        {
            Console.WriteLine($"[Proxy] ОТКАЗ в чтении: {user} → '{path}' | причина: {access.Reason}");
            throw new UnauthorizedAccessException(access.Reason);
        }

        if (_cache.TryGetValue(path, out var cached))
        {
            TouchLru(path);
            Console.WriteLine($"[Proxy] КЭШ-ХИТ '{path}' — возврат из временного хранилища ({cached.Length} байт)");
            return cached;
        }

        Console.WriteLine($"[Proxy] КЭШ-ПРОМАХ '{path}' — обращаемся к провайдеру");
        var data = await _real.DownloadAsync(user, path);
        PutCache(path, data);
        return data;
    }

    public async Task UploadAsync(User user, string path, byte[] data)
    {
        var access = _policy.Check(user, path, Operation.Write);
        if (!access.Allowed)
        {
            Console.WriteLine($"[Proxy] ОТКАЗ в записи: {user} → '{path}' | причина: {access.Reason}");
            throw new UnauthorizedAccessException(access.Reason);
        }

        Console.WriteLine($"[Proxy] Запись разрешена для {user} → '{path}'");
        await _real.UploadAsync(user, path, data);
        PutCache(path, data); 
    }

    public void Invalidate(string path)
    {
        if (_cache.Remove(path))
        {
            _lruOrder.Remove(path);
            Console.WriteLine($"[Proxy] Кэш очищен: '{path}'");
        }
    }


    private void PutCache(string path, byte[] data)
    {
        if (_cache.ContainsKey(path))
        {
            _cache[path] = data;
            TouchLru(path);
            return;
        }

        if (_cache.Count >= _cacheCapacity)
        {
            var oldest = _lruOrder.First!.Value;
            _cache.Remove(oldest);
            _lruOrder.RemoveFirst();
            Console.WriteLine($"[Proxy] Кэш переполнен — вытеснен '{oldest}'");
        }

        _cache[path] = data;
        _lruOrder.AddLast(path);
    }

    private void TouchLru(string path)
    {
        _lruOrder.Remove(path);
        _lruOrder.AddLast(path);
    }
}