using System.Collections.Generic;

public interface IRepository<T> where T : class, IEntity
{
    T? GetById(int id);
    IEnumerable<T> GetAll();
    void Add(T item);
    void Update(T item);
    void Delete(int id);
}