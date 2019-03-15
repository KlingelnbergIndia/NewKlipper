using DataAccess.EntityModel.Department;
using DataAccess.Helper;
using MongoDB.Driver;

namespace DataAccess
{
    public class DepartmentDBContext
    {
        protected readonly IMongoDatabase _database = null;

        #region Instance

        private static DepartmentDBContext _instance = null;
        public static DepartmentDBContext Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DepartmentDBContext();
                }
                return _instance;
            }
        }

        #endregion

        public DepartmentDBContext()
        {
            var connectionString = DBConfigurator.GetConnectionString("EmployeeDB");
            var mongoClient = new MongoClient(connectionString);
            _database = mongoClient.GetDatabase("OperationalsDB");
        }

        public IMongoCollection<DepartmentEntityModel> Departments => 
            _database.GetCollection<DepartmentEntityModel>("Departments"); 
    }
}
