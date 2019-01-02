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
        public Employee GetEmployee(int employeeId)
        {
            var filterForAuthDBContext = Builders<Users>.Filter.Eq("ID", employeeId);
            var employeeFromAuthDBContext = _authDBContext.Users.Find(filterForAuthDBContext).First();

            var filterForEmployeeDBContext = Builders<EmployeeEntityModel>.Filter.Eq("ID", employeeId);
            var employeeFromEmployeeDBContext = _employeeDBContext.Employees.Find(filterForEmployeeDBContext).First();

            Employee employeeDomain = new Employee();
            employeeDomain.EmployeeId = employeeFromAuthDBContext.ID;
            employeeDomain.UserName = employeeFromAuthDBContext.UserName;
            employeeDomain.Password = employeeFromAuthDBContext.PasswordHash;
            //employeeDomain.Role = employeeFromEmployeeDBContext.Roles;
            return employeeDomain;
        }
    }
}