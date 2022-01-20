using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DAL.Abstractions.Interfaces;

namespace DAL.Repository
{
    public class RepositoryDb<TEnity> : IGenericRepository<TEnity>
    {
        public Task<IEnumerable<TEnity>> FindAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEnity>> FindByConditionAsync(Expression<Func<TEnity, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public Task CreateAsync(TEnity entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(TEnity entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(TEnity entity)
        {
            throw new NotImplementedException();
        }
    }
}