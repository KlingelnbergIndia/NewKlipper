using DataAccess.EntityModel.Attendance;
using DataAccess.Helper;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess
{
    public class AttendanceRegularizationDBContext
    {
        protected readonly IMongoDatabase _database = null;

        #region Instance

        private static AttendanceRegularizationDBContext _instance = null;

        public static AttendanceRegularizationDBContext Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AttendanceRegularizationDBContext();
                }
                return _instance;
            }
        }

        #endregion

        public AttendanceRegularizationDBContext()
        {
            var connectionString = DBConfigurator.GetConnectionString("AttendanceDB");
            var mongoClient = new MongoClient(connectionString);
            _database = mongoClient.GetDatabase("AttendanceDB");
        }

        public IMongoCollection<AttendanceRegularizationEntityModel> AttendanceRegularization => 
            _database.GetCollection<AttendanceRegularizationEntityModel>("AttendanceRegularization");
    }
}
