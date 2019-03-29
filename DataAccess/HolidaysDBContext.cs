using DataAccess.Helper;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using DataAccess.EntityModel.Holiday;

namespace DataAccess
{
    public class HolidaysDBContext
    {

        protected readonly IMongoDatabase _database = null;

        #region Instance

        private static HolidaysDBContext _instance = null;
        public static HolidaysDBContext Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new HolidaysDBContext();
                }
                return _instance;
            }
        }

        #endregion

        public HolidaysDBContext()
        {
            var connectionString = DBConfigurator.GetConnectionString("OperationalsDB");
            var mongoClient = new MongoClient(connectionString);
            _database = mongoClient.GetDatabase("OperationalsDB");
        }

        public IMongoCollection<HolidayEntityModel> holidays =>
            _database.GetCollection<HolidayEntityModel>("Holidays");
    }
}

