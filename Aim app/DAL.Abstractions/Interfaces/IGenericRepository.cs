using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DAL.Abstractions.Interfaces
{
    public interface IGenericRepository<TEnity>
    {
        Task<IEnumerable<TEnity>> FindAllAsync();
        Task<IEnumerable<TEnity>> FindByConditionAsync(Expression<Func<TEnity, bool>> expression);
        Task CreateAsync(TEnity entity);
        Task UpdateAsync(TEnity entity);
        Task DeleteAsync(TEnity entity);
    }
}