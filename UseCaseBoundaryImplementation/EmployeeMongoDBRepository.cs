using System.Collections.Generic;
using System.Linq;
using DataAccess;
using DataAccess.EntityModel.Authentication;
using DataAccess.EntityModel.Employment;
using DomainModel;
using MongoDB.Driver;
using UseCaseBoundary;

namespace UseCaseBoundaryImplementation
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
            var employeeFromAuthDBContext = _authDBContext.Users.AsQueryable()
                .Where(x => x.UserName.ToLower() == userName)
                .FirstOrDefault();

            if (employeeFromAuthDBContext == null)
            {
                return null;
            }

            var filterForEmployeeDBContext = Builders<EmployeeEntityModel>.Filter.Eq("ID", employeeFromAuthDBContext.ID);
            var employeeFromEmployeeDBContext = _employeeDBContext.Employees.Find(filterForEmployeeDBContext).FirstOrDefault();

            List<int> reportees = new List<int>();

            int _id = employeeFromAuthDBContext.ID;
            string _userName = employeeFromAuthDBContext.UserName;
            string _password = employeeFromAuthDBContext.PasswordHash;
            string firstName = employeeFromEmployeeDBContext.FirstName;
            string lastName = employeeFromEmployeeDBContext.LastName;
            string title = employeeFromEmployeeDBContext.Title;
            reportees = employeeFromEmployeeDBContext.Reportees;
            Departments department = (Departments)employeeFromEmployeeDBContext.DepartmentId;

            List<EmployeeRoles> _roles = ConvertStringRolesToEnumRoles(employeeFromEmployeeDBContext.Roles);
            Employee domainEmployee = new Employee(_id, _userName, _password, firstName, lastName, title, _roles, reportees, department);

            return domainEmployee;
        }

        public Employee GetEmployee(int employeeId)
        {
            var employeeFromAuthDBContext = _authDBContext.Users.AsQueryable()
                .Where(x => x.ID == employeeId)
                .FirstOrDefault();

            if (employeeFromAuthDBContext == null)
            {
                return null;
            }

            var filterForEmployeeDBContext = Builders<EmployeeEntityModel>.Filter.Eq("ID", employeeFromAuthDBContext.ID);
            var employeeFromEmployeeDBContext = _employeeDBContext.Employees.Find(filterForEmployeeDBContext).FirstOrDefault();

            List<int> reportees = new List<int>();

            int _id = employeeFromAuthDBContext.ID;
            string _userName = employeeFromAuthDBContext.UserName;
            string _password = employeeFromAuthDBContext.PasswordHash;
            string firstName = employeeFromEmployeeDBContext.FirstName;
            string lastName = employeeFromEmployeeDBContext.LastName;
            string title = employeeFromEmployeeDBContext.Title;
            reportees = employeeFromEmployeeDBContext.Reportees;
            Departments department = (Departments)employeeFromEmployeeDBContext.DepartmentId;

            List<EmployeeRoles> _roles = ConvertStringRolesToEnumRoles(employeeFromEmployeeDBContext.Roles);
            Employee domainEmployee = new Employee(_id, _userName, _password, firstName, lastName, title, _roles,reportees, department);

            return domainEmployee;

        }

        public List<Employee> GetAllEmployeeExceptAdmin(int employeeId)
        {
            var AllEmployeeFromAuthDBContext = _authDBContext.Users.AsQueryable()
                .Where(x => x.ID != employeeId)
                .ToList();

            if (AllEmployeeFromAuthDBContext == null)
            {
                return null;
            }

            var allemployeesExceptAdmin = _employeeDBContext.Employees.Find(x=>x.ID != employeeId).ToList();

            var listOfEmployee = new List<Employee>();
            foreach (var employee in allemployeesExceptAdmin)
            {
                List<int> reportees = new List<int>();

                var employeeFromAuthDBContext = AllEmployeeFromAuthDBContext.Where(x=>x.ID == employee.ID).FirstOrDefault();
                int id = employeeFromAuthDBContext.ID;
                string userName = employeeFromAuthDBContext.UserName;
                string password = employeeFromAuthDBContext.PasswordHash;
                string firstName = employee.FirstName;
                string lastName = employee.LastName;
                string title = employee.Title;
                reportees = employee.Reportees;
                Departments department = (Departments)employee.DepartmentId;
                List<EmployeeRoles> _roles = ConvertStringRolesToEnumRoles(employee.Roles);
                Employee domainEmployee = new Employee(id, userName, password, firstName, lastName, title, _roles, reportees, department);
                listOfEmployee.Add(domainEmployee);
            }

            return listOfEmployee;
        }

        private List<EmployeeRoles> ConvertStringRolesToEnumRoles(List<string> roles)
        {
            List<EmployeeRoles> empRoles = new List<EmployeeRoles>();
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
    }
}