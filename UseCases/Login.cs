using DomainModel;
using UseCaseBoundary;
using UseCaseBoundary.Model;

namespace UseCases
{
    public class Login
    {
        private IEmployeeRepository _employeeRepository;
        public Login(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        EmployeeDTO LoginUser(string userName, string password)
        {
            Employee employee = _employeeRepository.GetEmployee(userName);
            bool result  = employee.Authenticate(userName, password);
            if (result)
            {
                EmployeeDTO employeeDto = new EmployeeDTO();
                employeeDto.EmployeeId = employee.EmployeeId;
                employeeDto.UserName = employee.UserName;
                employeeDto.Role = employee.Role;
                return employeeDto;
            }
            else
            {
                return null;
            }
        }
    }
}