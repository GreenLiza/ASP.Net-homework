using GoodNewsAggregator.Abstractions.Repositories;
using GoodNewsAggregator.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodNewsAggregator.Abstractions
{
    public interface IUnitOfWork : IDisposable
    {
        public IRepository<Comment> Comments { get; }
        public INewsRepository News { get; }
        public IRepository<RatingWord> RatingWords { get; }
        public IRepository<Role> Roles { get; }
        public IRepository<Source> Sources { get; }
        public IUserRepository Users { get; }

        public Task<int> SaveChangesAsync();
    }
}
