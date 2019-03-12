using System.Linq;
using DataAccess;
using DomainModel;
using MongoDB.Driver;
using UseCaseBoundary;

namespace RepositoryImplementation
{
    public class DepartmentMongoDBRepository : IDepartmentRepository
    {
        private readonly DepartmentDBContext _departmentDBContext = null;

        public DepartmentMongoDBRepository()
        {
            _departmentDBContext = DepartmentDBContext.Instance;
        }

        public Department GetDepartment(Departments department)
        {
            var departmentDetails =_departmentDBContext
                .Departments
                .AsQueryable()
                .Where( x => x.ID == (int)department)
                .FirstOrDefault();

            Departments dept = (Departments)departmentDetails.ID;
            return new Department(dept);
        }
    }
}
