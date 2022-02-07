


using System.Linq.Expressions;

namespace UnitOfWork.API.services
{
    public interface  IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(Guid id);
        Task<bool> Add(T entity);
        Task<bool> Delete(Guid id);
        Task<bool> Update(T entity);
        Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate);

    }
}
