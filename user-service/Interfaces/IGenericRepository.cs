namespace user_service.Interfaces;

public interface IGenericRepository<T> where T : class
{
    T? GetById(string id);
    IEnumerable<T> GetAll();
    T Add(T entity);
    void Remove(T entity);
}