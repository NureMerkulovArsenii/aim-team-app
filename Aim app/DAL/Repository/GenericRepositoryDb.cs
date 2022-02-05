using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DAL.Abstractions.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository
{
    public class GenericRepositoryDb<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly AppContext _appContext;
        private readonly DbSet<TEntity> _dbSet;
        public GenericRepositoryDb(AppContext appContext)
        {
            this._appContext = appContext;
            this._dbSet = appContext.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> FindByConditionAsync(Expression<Func<TEntity, bool>> expression)
        {
            IQueryable<TEntity> query = _dbSet;

            if (expression != null)
            {
                query = query.Where(expression);
            }

            IEnumerable<TEntity> result = query;

            return result;
        }
        
        public async Task<TEntity> GetEntityById(int id)
        {
            var queryResult = await _dbSet.FindAsync(id);
            return queryResult;
        }
        
        public async Task CreateAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await _appContext.SaveChangesAsync();
        }
        
        public async Task UpdateAsync(TEntity entityToUpdate)
        {
            _dbSet.Update(entityToUpdate);
            await _appContext.SaveChangesAsync();
        }

        // public void Update(TEntity entityToUpdate)
        // {
        //     _dbSet.Attach(entityToUpdate);
        //     _appContext.Entry(entityToUpdate).State = EntityState.Modified;
        // }
        
        public async Task DeleteAsync(TEntity entityToDelete)
        {    
            _dbSet.Remove(entityToDelete);
            await _appContext.SaveChangesAsync();
        }

        // public async Task Delete(TEntity entityToDelete)
        // {
        //     if (_appContext.Entry(entityToDelete).State == EntityState.Detached)
        //     {
        //         _dbSet.Attach(entityToDelete);
        //     }
        //     _dbSet.Remove(entityToDelete);
        // }
    }
}
