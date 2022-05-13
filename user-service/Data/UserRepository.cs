using user_service.Interfaces;
using user_service.Models;

namespace user_service.Data;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(DatabaseContext context) : base(context)
    {
    }
}
