using System;
using System.Collections.Generic;
using System.Linq;

public class InMemoryRepository<T> : IRepository<T> where T : class, IEntity
{
    private readonly List<T> _items = new List<T>();
    private int _nextId = 1;

    public T? GetById(int id)
    {
        return _items.FirstOrDefault(item => item.Id == id);
    }

    public IEnumerable<T> GetAll()
    {
        return _items.ToList();
    }

    public void Add(T item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        if (item.Id == 0)
        {
            item.Id = _nextId;
        }
        else if (_items.Any(existing => existing.Id == item.Id))
        {
            throw new InvalidOperationException($"Елемент з Id = {item.Id} вже існує");
        }

        _items.Add(item);
        _nextId = Math.Max(_nextId, item.Id) + 1;
    }

    public void Update(T item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        int index = _items.FindIndex(existing => existing.Id == item.Id);
        if (index < 0)
        {
            throw new KeyNotFoundException($"Елемент з Id = {item.Id} не знайдено");
        }

        _items[index] = item;
    }

    public void Delete(int id)
    {
        int index = _items.FindIndex(existing => existing.Id == id);
        if (index < 0)
        {
            throw new KeyNotFoundException($"Елемент з Id = {id} не знайдено");
        }

        _items.RemoveAt(index);
    }
}