using System;
using DataAccess.EntityModel;
using DataAccess.Helper;
using MongoDB.Driver;

namespace DataAccess
{
    public class AttendanceDBContext
    {
        protected readonly IMongoDatabase _database = null;

        #region Instance

        private static AttendanceDBContext _instance = null;

        public static AttendanceDBContext Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AttendanceDBContext();
                }
                return _instance;
            }
        }

        #endregion

        public AttendanceDBContext()
        {
            var connectionString = DBConfigurator.GetConnectionString("AttendanceDB");
            var mongoClient = new MongoClient(connectionString);
            _database = mongoClient.GetDatabase("AttendanceDB");
        }

        public IMongoCollection<AccessEventEntityModel> AccessEvents => _database.GetCollection<AccessEventEntityModel>("AccessEvents");
        //public IMongoCollection<AccessPoint> AccessPoints => _database.GetCollection<AccessPoint>("AccessPoints");
    }
}
