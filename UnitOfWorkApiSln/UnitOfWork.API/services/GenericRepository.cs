using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using UnitOfWork.API.Data;

namespace UnitOfWork.API.services
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected DBContext context;
        internal DbSet<T> dbSet;
        protected readonly ILogger _logger;
        public GenericRepository(DBContext context, ILogger logger)
        {
            this.context = context;
            this.dbSet = context.Set<T>();
            this._logger = logger;
        }

        public virtual Task<IEnumerable<T>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public virtual async Task<T> GetByIdAsync(Guid id)
        {
            return await dbSet.FindAsync(id);
        }

        public virtual async Task<bool> Add(T entity)
        {
            await dbSet.AddAsync(entity);
            return true;
        }

        public virtual async Task<bool> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public virtual Task<bool> Update(T entity)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate)
        {
            return await dbSet.Where(predicate).ToListAsync();
        }

    }
}
