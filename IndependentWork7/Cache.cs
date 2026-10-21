using System.Collections.Generic;

namespace IndependentWork7;

public class Cache<TKey, TValue> where TKey : notnull
{
    private int _capacity;
    private Dictionary<TKey, TValue> _cache;
    private Queue<TKey> _keysOrder;

    public int Count
    {
        get { return _cache.Count; }
    }

    public int Capacity
    {
        get { return _capacity; }
    }

    public Cache(int capacity)
    {
        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), "Ємність кешу має бути більшою за 0");
        }

        _capacity = capacity;
        _cache = new Dictionary<TKey, TValue>(capacity);
        _keysOrder = new Queue<TKey>(capacity);
    }

    public void Add(TKey key, TValue value)
    {
        if (_cache.ContainsKey(key))
        {
            _cache[key] = value;
            return;
        }

        if (_cache.Count >= _capacity)
        {
            TKey oldestKey = _keysOrder.Dequeue();
            _cache.Remove(oldestKey);
        }

        _cache.Add(key, value);
        _keysOrder.Enqueue(key);
    }

    public TValue Get(TKey key)
    {
        if (_cache.TryGetValue(key, out TValue? value))
        {
            return value;
        }

        throw new KeyNotFoundException($"Ключ '{key}' не знайдено в кеші");
    }

    public bool ContainsKey(TKey key)
    {
        return _cache.ContainsKey(key);
    }
}
