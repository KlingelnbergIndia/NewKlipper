using DataAccess.EntityModel.Employment;
using DataAccess.EntityModel.Leave;
using DataAccess.Helper;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess
{
    public class LeaveManagementDBContext
    {
        protected readonly IMongoDatabase _database;

        #region Instance
        private static LeaveManagementDBContext _instance = null;
        public static LeaveManagementDBContext Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LeaveManagementDBContext();
                }
                return _instance;
            }
        }
        #endregion

        public LeaveManagementDBContext()
        {
            var connectionString = DBConfigurator.GetConnectionString("LeaveManagementDB");
            var mongoClient = new MongoClient(connectionString);
            _database = mongoClient.GetDatabase("LeaveManagementDB");
        }

        public IMongoCollection<LeaveEntityModel> AppliedLeaves => _database.GetCollection<LeaveEntityModel>("AppliedLeaves");

    }
}
