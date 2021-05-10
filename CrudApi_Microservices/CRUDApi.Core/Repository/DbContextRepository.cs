using CRUD.Transaction.CRUDApi.Core.Context;
using CRUD.Transaction.CRUDApi.Core.Entities;
using CRUD.Transaction.CRUDApi.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUD.Transaction.CRUDApi.Core.Repository
{
    public class DbContextRepository<TEntity> : IRepositoryAsync<TEntity> where TEntity : ModelEntityBase
    {
        public IAsyncContext<TEntity> Context { get; private set; }

        public DbContextRepository(IAsyncContext<TEntity> context)
        {
            Context = context;
        }

        public Task<int> AddAsync(TEntity item)
        {
            return Context.AddAsync(item);
        }

        public Task<int> AddAsync(IEnumerable<TEntity> items)
        {
            return Context.AddAsync(items);
        }

        public async Task<int> DeleteAsync(long id)
        {
            var entity = await GetAsync(id);
            entity.Delete();
            return await Context.DeleteAsync(entity);
        }

        public Task<List<TEntity>> GetAllAsync()
        {
            return Context.GetAllAsync();
        }

        public Task<List<TEntity>> GetAllChildEntitiesAsync()
        {
            return Context.GetAllChildEntitiesAsync();
        }

        public virtual Task<TEntity> GetAsync(long id)
        {
            return Context.GetAsync(id);
        }

        public Task<int> SaveAsync(TEntity item)
        {
            return Context.SaveAsync(item);
        }
          
        public Task<int> SaveAsync(IEnumerable<TEntity> items)
        {
            return Context.SaveAsync(items);
        }
 
        public Task<int> SaveAsync(long id, object dto)
        {
            return Context.SaveAsync(id, dto);
        }

        public Task<int> ExecuteSqlCommandAsync(string functionName, params object[] parameters)
        {
            return Context.ExecuteSqlCommandAsync(functionName, parameters);
        }

        public void Dispose()
        {
            try
            {
                (Context as DbContext)?.Dispose();
            }
            catch { }
            GC.SuppressFinalize(this);
        }

    }
}
