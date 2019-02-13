using DataAccess.EntityModel.Department;
using DataAccess.EntityModel.Leave;
using DataAccess.Helper;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

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

        public IMongoCollection<DepartmentEntityModel> Departments => _database.GetCollection<DepartmentEntityModel>("Departments"); 
        public IMongoCollection<CarryForwardLeavesEntityModel> CarryForwardLeaves => _database.GetCollection<CarryForwardLeavesEntityModel>("CarryForwardLeaves"); 

    }
}
