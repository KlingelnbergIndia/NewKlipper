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
            var filterForAuthDBContext = Builders<UsersEntityModel>.Filter.Eq("UserName", userName);
            var employeeFromAuthDBContext = _authDBContext.Users.Find(filterForAuthDBContext).First();

            var filterForEmployeeDBContext = Builders<EmployeeEntityModel>.Filter.Eq("ID", employeeFromAuthDBContext.ID);
            var employeeFromEmployeeDBContext = _employeeDBContext.Employees.Find(filterForEmployeeDBContext).First();

            int _id = employeeFromAuthDBContext.ID;
            string _userName = employeeFromAuthDBContext.UserName;
            string _password = employeeFromAuthDBContext.PasswordHash;
            List<EmployeeRoles> _roles = ConvertStringRolesToEnumRoles(employeeFromEmployeeDBContext.Roles);
            Employee domainEmployee = new Employee(_id, _userName, _password, _roles);

            return domainEmployee;
        }

        private List<EmployeeRoles> ConvertStringRolesToEnumRoles(List<string> roles)
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