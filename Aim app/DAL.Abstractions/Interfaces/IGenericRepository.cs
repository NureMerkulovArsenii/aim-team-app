using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core;

namespace DAL.Abstractions.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        Task<IList<TEntity>> FindByConditionAsync(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, TEntity>> selector = null);

        //Task<IList<TEntity>> Get(Expression<Func<TEntity, TEntity>> selector,
        //Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> GetEntityById(int id);

        Task CreateAsync(TEntity entity);

        Task UpdateAsync(TEntity entity);

        Task DeleteAsync(TEntity entity);
    }
}
