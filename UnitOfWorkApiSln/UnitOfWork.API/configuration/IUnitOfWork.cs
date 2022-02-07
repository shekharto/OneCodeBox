using UnitOfWork.API.services;

namespace UnitOfWork.API.configuration
{
    public interface IUnitOfWork
    {
        IEmployeeRepository Employee { get; }
        Task CompleteAsync();
        void Dispose();
    }
}
