using user_service.Interfaces;

namespace user_service.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly DatabaseContext context;
    public IUserRepository Users { get; }

    public UnitOfWork(DatabaseContext context)
    {
        this.context = context;

        Users = new UserRepository(context);
    }

    public int Commit()
    {
        return context.SaveChanges();
    }

    public void Dispose()
    {
        context.Dispose();
    }
}
