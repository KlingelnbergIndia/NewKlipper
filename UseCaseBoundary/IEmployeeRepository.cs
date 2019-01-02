using DomainModel;

namespace UseCaseBoundary
{
    public interface IEmployeeRepository
    {
        Employee GetEmployee(int EmployeeId);
    }
}