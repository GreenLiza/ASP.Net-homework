using GoodNewsAggregator.Abstractions.Repositories;
using GoodNewsAggregator.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace GoodNewsAggregator.Repositories.Implementations
{
    public class Repository<TEntity> : IRepository<TEntity>
    where TEntity : class, IBaseEntity
    {
        protected readonly NewsAggregatorContext DbContext;
        protected readonly DbSet<TEntity> DbSet;

        public Repository(NewsAggregatorContext dbContext)
        {
            DbContext = dbContext;
            DbSet = DbContext.Set<TEntity>();
        }
        public virtual async Task<EntityEntry<TEntity>> AddAsync(TEntity entity)
        {
            return await DbSet.AddAsync(entity);
        }
        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await DbSet.AddRangeAsync(entities);
        }
        public virtual void Dispose()
        {
            DbContext.Dispose();
            GC.SuppressFinalize(this);
        }

        public IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
        {
            var result = DbSet.Where(predicate);
            if (includes.Any())
            {
                result = includes.Aggregate(result, (current, include) => current.Include(include));
            }
            return result;
        }

        public virtual IQueryable<TEntity> GetAsQueryable()
        {
            return DbSet;
        }

        public virtual async Task<TEntity?> GetByIdAsync(int id)
        {
            return await DbSet.AsNoTracking().FirstOrDefaultAsync(entity => entity.Id == id);
        }
        public virtual async Task Remove(int id)
        {            
            DbSet.Remove(await DbSet.FirstOrDefaultAsync(entity => entity.Id == id));
        }
        public virtual async Task RemoveRange(IEnumerable<TEntity> entities)
        {
            DbSet.RemoveRange(entities);
        }

        public virtual async Task<int> CountAsync()
        {
            return await DbSet.CountAsync();
        }
    }
}
