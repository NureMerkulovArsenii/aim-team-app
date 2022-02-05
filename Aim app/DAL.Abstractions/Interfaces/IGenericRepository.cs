using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DAL.Abstractions.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> FindByConditionAsync(Expression<Func<TEntity, bool>> expression);
        
        Task<TEntity> GetEntityById(int id);
        
        Task CreateAsync(TEntity entity);
        
        Task UpdateAsync(TEntity entity);
        
        Task DeleteAsync(TEntity entity);
    }
}
