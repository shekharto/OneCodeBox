using Microsoft.EntityFrameworkCore;
using UnitOfWork.API.Data;
using UnitOfWork.API.Model;

namespace UnitOfWork.API.services
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository 
    {
        public EmployeeRepository(DBContext context, ILogger logger) : base(context, logger)
        {     
        }

        public override async Task<IEnumerable<Employee>> GetAllAsync()
        {
            try
            {
                return await dbSet.ToListAsync();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} All function error", typeof(EmployeeRepository));
                return new List<Employee>();
            }
        }

        public override async Task<bool> Update(Employee entity)
        {
            try
            {
                var existingUser = await dbSet.Where(x => x.Id == entity.Id).FirstOrDefaultAsync();

                if (existingUser == null)
                    throw new Exception();
                  //  return await Add(entity);

                existingUser.FirstName = entity.FirstName;
                existingUser.LastName = entity.LastName;
                existingUser.Email = entity.Email;
                existingUser.PhoneNumber = entity.PhoneNumber;

                return true;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} Upsert function error", typeof(EmployeeRepository));
                return false;
            }

        }


        public override async Task<bool> Delete(Guid id)
        {
            try
            {
                var exist = await dbSet.Where(x => x.Id == id).FirstOrDefaultAsync();
                if (exist == null) return false;

                dbSet.Remove(exist);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} Delete function error", typeof(EmployeeRepository));
                return false;
            }
        }




    }
}
