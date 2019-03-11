using DataAccess.EntityModel.Authentication;
using DataAccess.Helper;
using MongoDB.Driver;

namespace DataAccess
{
    public class AuthDBContext
    {
        protected readonly IMongoDatabase _database = null;

        #region Instance

        private static AuthDBContext _instance = null;

        public static AuthDBContext Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AuthDBContext();
                }
                return _instance;
            }
        }

        #endregion

        public AuthDBContext()
        {
            var connectionString = DBConfigurator.GetConnectionString("AuthDB");
            var mongoClient = new MongoClient(connectionString);
            _database = mongoClient.GetDatabase("AuthDB");
        }

        public IMongoCollection<UsersEntityModel> Users => 
            _database.GetCollection<UsersEntityModel>("Users");

    }
}