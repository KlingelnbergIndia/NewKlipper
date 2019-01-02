using DataAccess.EntityModel.Employment;
using DataAccess.Helper;
using MongoDB.Driver;

namespace DataAccess
{
    public class EmployeeDBContext
    {
        protected readonly IMongoDatabase _database = null;

        #region Instance

        private static EmployeeDBContext _instance = null;

        public static EmployeeDBContext Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EmployeeDBContext();
                }
                return _instance;
            }
        }

        #endregion

        public EmployeeDBContext()
        {
            var connectionString = DBConfigurator.GetConnectionString("EmployeeDB");
            var mongoClient = new MongoClient(connectionString);
            _database = mongoClient.GetDatabase("EmployeeDB");
        }

        public IMongoCollection<EmployeeEntityModel> Employees => _database.GetCollection<EmployeeEntityModel>("Employees");

    }
}