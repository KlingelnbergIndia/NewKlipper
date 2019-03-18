using DomainModel;
using System.Collections.Generic;

namespace UseCaseBoundary
{
    public interface IEmployeeRepository
    {
        Employee GetEmployee(string UserName);
        Employee GetEmployee(int employeeId);
        List<Employee> GetAllEmployeeExceptAdmin(int employeeId);
    }
}