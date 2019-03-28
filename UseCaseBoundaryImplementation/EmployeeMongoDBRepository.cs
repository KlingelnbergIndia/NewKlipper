using System.Collections.Generic;
using System.Linq;
using DataAccess;
using DataAccess.EntityModel.Authentication;
using DataAccess.EntityModel.Employment;
using DomainModel;
using MongoDB.Driver;
using UseCaseBoundary;

namespace RepositoryImplementation
{
    public class EmployeeMongoDBRepository : IEmployeeRepository
    {
        private readonly AuthDBContext _authDBContext = null;
        private readonly EmployeeDBContext _employeeDBContext = null;

        public EmployeeMongoDBRepository()
        {
            _authDBContext = AuthDBContext.Instance;
            _employeeDBContext = EmployeeDBContext.Instance;
        }

        public Employee GetEmployee(string userName)
        {
            var employeeFromAuthDBContext = _authDBContext.Users
                .AsQueryable()
                .Where(x => x.UserName.ToLower() == userName)
                .FirstOrDefault();

            if (employeeFromAuthDBContext == null)
                return null;

            var filterForEmployeeDBContext = Builders<EmployeeEntityModel>
                .Filter
                .Eq("ID", employeeFromAuthDBContext.ID);

            var employeeFromEmployeeDBContext = _employeeDBContext
                .Employees
                .Find(filterForEmployeeDBContext)
                .FirstOrDefault();

            return Employee(employeeFromAuthDBContext,
                employeeFromEmployeeDBContext);
        }

        public Employee GetEmployee(int employeeId)
        {
            var employeeFromAuthDBContext = _authDBContext.Users.AsQueryable()
                .Where(x => x.ID == employeeId)
                .FirstOrDefault();

            if (employeeFromAuthDBContext == null)
                return null;

            var filterForEmployeeDBContext = Builders<EmployeeEntityModel>
                .Filter
                .Eq("ID", employeeFromAuthDBContext.ID);

            var employeeFromEmployeeDBContext = _employeeDBContext
                .Employees
                .Find(filterForEmployeeDBContext)
                .FirstOrDefault();

            return Employee(employeeFromAuthDBContext,
                employeeFromEmployeeDBContext);
        }

        public List<Employee> GetAllEmployeeExceptAdmin(int employeeId)
        {
            var AllEmployeeFromAuthDBContext = _authDBContext.Users
                .AsQueryable()
                .Where(x => x.ID != employeeId)
                .ToList();

            if (AllEmployeeFromAuthDBContext == null)
                return null;

            var allemployeesExceptAdmin = _employeeDBContext.Employees
                .Find(x=>x.ID != employeeId)
                .ToList();

            var listOfEmployee = new List<Employee>();
            foreach (var employee in allemployeesExceptAdmin)
            {
                var employeeFromAuthDBContext = AllEmployeeFromAuthDBContext
                    .Where(x=>x.ID == employee.ID)
                    .FirstOrDefault();

                listOfEmployee.Add(
                    Employee(employeeFromAuthDBContext,employee));
            }
            return listOfEmployee;
        }

        private List<EmployeeRoles> ConvertStringRolesToEnumRoles(List<string> roles)
        {
            var empRoles = new List<EmployeeRoles>();
            foreach (var role in roles)
            {
                switch (role)
                {
                    case "Admin":
                        empRoles.Add(EmployeeRoles.Admin);
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

        private Employee Employee(UsersEntityModel employeeFromAuthDBContext,
            EmployeeEntityModel employeeFromEmployeeDBContext)
        {
            var _id = employeeFromAuthDBContext.ID;
            var _userName = employeeFromAuthDBContext.UserName;
            var _password = employeeFromAuthDBContext.PasswordHash;
            var firstName = employeeFromEmployeeDBContext.FirstName;
            var lastName = employeeFromEmployeeDBContext.LastName;
            var title = employeeFromEmployeeDBContext.Title;
            var reportees = new List<int>();

            reportees = employeeFromEmployeeDBContext.Reportees;
            var department = (Departments)employeeFromEmployeeDBContext
                .DepartmentId;

            var _roles = ConvertStringRolesToEnumRoles
                (employeeFromEmployeeDBContext.Roles);

            var domainEmployee = new Employee
            (_id, _userName, _password, firstName, lastName,
                title, _roles, reportees, department);

            return domainEmployee;
        }

    }
}