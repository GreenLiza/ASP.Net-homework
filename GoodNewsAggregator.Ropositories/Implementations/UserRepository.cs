using GoodNewsAggregator.Abstractions.Repositories;
using GoodNewsAggregator.Data;
using GoodNewsAggregator.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoodNewsAggregator.Repositories.Implementations
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(NewsAggregatorContext dbContext) : base(dbContext)
        {
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            var user = await DbSet
                .FirstOrDefaultAsync(user => user.Username == username);
            return user;
        }
    }
}
