using Microsoft.Extensions.Logging;
using UnitOfWork.API.Data;
using UnitOfWork.API.services;

namespace UnitOfWork.API.configuration
{
    public class UnitofWork : IUnitOfWork, IDisposable
    {
        private readonly DBContext _context;
        private readonly ILogger _logger;

        public IEmployeeRepository Employee { get; private set; }

        public UnitofWork(DBContext context, ILoggerFactory loggerFactory )
        {
            _context = context;
            _logger = loggerFactory.CreateLogger("logs");

            Employee = new EmployeeRepository(_context, _logger);
        }

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }


    }
}
