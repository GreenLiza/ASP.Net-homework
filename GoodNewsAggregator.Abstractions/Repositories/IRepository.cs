using GoodNewsAggregator.Data;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GoodNewsAggregator.Abstractions.Repositories
{
    public interface IRepository<TEntity> : IDisposable 
        where TEntity : class, IBaseEntity
    {
        Task<TEntity?> GetByIdAsync(int id);
        IQueryable<TEntity> GetAsQueryable();
        IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes);
        Task<EntityEntry<TEntity>> AddAsync(TEntity entity);
        Task AddRangeAsync(IEnumerable<TEntity> entities);

        //Task PatchAsync(int id, List<PatchDto> patchDtos);
        //Task Update(TEntity entity);

        Task Remove(int id);
        Task RemoveRange(IEnumerable<TEntity> entities);

        Task<int> CountAsync();

    }
}
