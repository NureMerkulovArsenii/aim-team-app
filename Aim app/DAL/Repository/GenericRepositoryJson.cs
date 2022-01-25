using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core;
using DAL.Abstractions.Interfaces;
using DAL.Worker;
using Microsoft.Extensions.Options;

namespace DAL.Repository
{
    public class GenericRepositoryJson<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity

    {
        private readonly AppSettings _appSettings;
        private readonly IJsonWorker _jsonWorker;
        private List<TEntity> _allData;
        
        // public GenericRepositoryJson(IJsonWorker jsonWorker, IOptions<AppSettings> appSettings)
        public GenericRepositoryJson(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));
            _jsonWorker = new JsonWorker();
            _allData = new List<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> FindAllAsync()
        {
            return await _jsonWorker.LoadFromFileAsync<IEnumerable<TEntity>>(_appSettings.TempDir);
        }

        public async Task<IEnumerable<TEntity>> FindByConditionAsync(Expression<Func<TEntity, bool>> expression)
        {
            var m = await FindAllAsync();
            return m.Where(expression.Compile());
        }

        public async Task CreateAsync(TEntity entity)
        {
            _allData = (await FindAllAsync()).ToList();
            _allData.Add(entity);
            await _jsonWorker.SaveToFileAsync(_allData, _appSettings.TempDir);
        }

        public async Task UpdateAsync(TEntity entity)
        {
            await DeleteAsync(entity);
            await CreateAsync(entity);
        }

        public async Task DeleteAsync(TEntity entity)
        {
            _allData = (await FindAllAsync()).ToList();
            var k = _allData.FirstOrDefault(o => o.Id == entity.Id);
            _allData.Remove(k);
            await _jsonWorker.SaveToFileAsync(_allData, _appSettings.TempDir);
        }
    }
}
