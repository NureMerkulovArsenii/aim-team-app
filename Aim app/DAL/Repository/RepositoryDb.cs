using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DAL.Abstractions.Interfaces;

namespace DAL.Repository
{
    public class RepositoryDb<TEntity> : IGenericRepository<TEntity>
    {
        public Task<IEnumerable<TEntity>> FindAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> FindByConditionAsync(Expression<Func<TEntity, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> GetEntityById(string id)
        {
            throw new NotImplementedException();
        }

        public Task CreateAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
