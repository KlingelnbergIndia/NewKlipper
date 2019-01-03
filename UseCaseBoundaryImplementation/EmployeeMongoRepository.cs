using System.Collections.Generic;
using DataAccess;
using DataAccess.EntityModel.Authentication;
using DataAccess.EntityModel.Employment;
using DomainModel;
using MongoDB.Driver;
using UseCaseBoundary;

namespace UseCaseBoundaryImplementation
{
    public class EmployeeMongoRepository : IEmployeeRepository
    {
        private readonly AuthDBContext _authDBContext = null;
        private readonly EmployeeDBContext _employeeDBContext = null;

        public EmployeeMongoRepository()
        {
            _authDBContext = AuthDBContext.Instance;
            _employeeDBContext = EmployeeDBContext.Instance;
        }
        public Employee GetEmployee(string userName)
        {
            var filterForAuthDBContext = Builders<Users>.Filter.Eq("UserName", userName);
            var employeeFromAuthDBContext = _authDBContext.Users.Find(filterForAuthDBContext).First();

            var filterForEmployeeDBContext = Builders<EmployeeEntityModel>.Filter.Eq("ID", employeeFromAuthDBContext.ID);
            var employeeFromEmployeeDBContext = _employeeDBContext.Employees.Find(filterForEmployeeDBContext).First();

            Employee employeeDomain = new Employee();
            employeeDomain.EmployeeId = employeeFromAuthDBContext.ID;
            employeeDomain.UserName = employeeFromAuthDBContext.UserName;
            employeeDomain.Password = employeeFromAuthDBContext.PasswordHash;
            employeeDomain.Role = ConvertRoleStringToEnum(employeeFromEmployeeDBContext.Roles);
            return employeeDomain;
        }

        List<EmployeeRoles> ConvertRoleStringToEnum(List<string> roles)
        {
            List<EmployeeRoles> empRoles = null;
            foreach (var role in roles)
            {
                switch (role)
                {
                    case "Ädmin":
                        empRoles.Add(EmployeeRoles.Ädmin);
                        break;
                    case "TeamLeader":
                        empRoles.Add(EmployeeRoles.TeamLeader);
                        break;
                    case "Employee":
                        empRoles.Add(EmployeeRoles.Employee);
                        break;
                } 
            }

            return empRoles;
        }
    }
}