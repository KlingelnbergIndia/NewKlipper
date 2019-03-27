using System.Collections.Generic;
using DomainModel;
using UseCaseBoundary;
using UseCaseBoundary.Model;
using System.Security.Cryptography;
using System;

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
            var encryptedPassword = ToSha256(password);
            Employee employee = _employeeRepository
                .GetEmployee(userName.ToLower());
            if (employee == null)
            {
                return null;
            }
            bool result  = employee.Authenticate(userName, encryptedPassword);
            if (result)
            {
                int id = employee.Id();
                string username = employee.UserName();
                string firstName = employee.FirstName();
                string lastName = employee.LastName();
                string title = employee.Title();
                List<EmployeeRoles> roles = employee.Roles();

                EmployeeDTO employeeDto = new EmployeeDTO(
                    id, 
                    username, 
                    firstName,
                    lastName,
                    title,
                    roles);

                return employeeDto;
            }
            else
            {
                return null;
            }
        }
        private string ToSha256(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            using (var sha = SHA256.Create())
            {
                var bytes = System.Text.Encoding.ASCII.GetBytes(input);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}