using user_service.Interfaces;

namespace user_service.Data;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly DatabaseContext context;

    public GenericRepository(DatabaseContext context)
    {
        this.context = context;
    }

    public T Add(T entity)
    {
        return context.Set<T>().Add(entity).Entity;
    }

    public IEnumerable<T> GetAll()
    {
        return context.Set<T>().ToList();
    }

    public T? GetById(string id)
    {
        return context.Set<T>().Find(id);
    }

    public void Remove(T entity)
    {
        context.Set<T>().Remove(entity);
    }
}