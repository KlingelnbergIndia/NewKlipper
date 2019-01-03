using DomainModel;

namespace UseCaseBoundary
{
    public interface IEmployeeRepository
    {
        Employee GetEmployee(string UserName);
    }
}