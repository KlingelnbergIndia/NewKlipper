using System.Collections.Generic;
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

        public EmployeeDTO LoginUser(string userName, string password)
        {
            Employee employee = _employeeRepository.GetEmployee(userName);
            bool result  = employee.Authenticate(userName, password);
            if (result)
            {
                int id = employee.Id();
                string username = employee.UserName();
                List<EmployeeRoles> roles = employee.Roles();

                EmployeeDTO employeeDto = new EmployeeDTO(id, username, roles);

                return employeeDto;
            }
            else
            {
                return null;
            }
        }
    }
}