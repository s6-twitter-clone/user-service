namespace user_service.Interfaces;

public interface IUnitOfWork
{
    public IUserRepository Users { get; }
    public int Commit();
}
