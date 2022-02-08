using CRUD.Transaction.CRUDApi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUD.Transaction.CRUDApi.Core.Context
{
    public interface IAsyncContext<TEntity> where TEntity : ModelEntityBase
    {
        Task<int> AddAsync(TEntity entity);
        Task<int> AddAsync(IEnumerable<TEntity> entities);
        Task<int> DeleteAsync(TEntity entity);
        Task<TEntity> GetAsync(long id);
        Task<List<TEntity>> GetAllChildEntitiesAsync();
        Task<List<TEntity>> GetAllAsync();
        Task<int> SaveAsync(TEntity entity);
        Task<int> SaveAsync(IEnumerable<TEntity> entities);
        Task<int> SaveAsync(long id, object dto);
        IQueryable<TEntity> GetIncludeQuery();
        Task<int> ExecuteSqlCommandAsync(string functionName, params object[] parameters);
    }

}
