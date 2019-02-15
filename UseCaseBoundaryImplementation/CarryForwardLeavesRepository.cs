using DataAccess;
using DomainModel;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UseCaseBoundary;

namespace RepositoryImplementation
{
    public class CarryForwardLeavesRepository : ICarryForwardLeaves
    {
        private LeaveManagementDBContext _dbContext;
        public CarryForwardLeavesRepository()
        {
            _dbContext = LeaveManagementDBContext.Instance;
        }

        public CarryForwardLeaves GetCarryForwardLeave(int employeeId)
        {
            return
                _dbContext.CarryForwardLeaves
                .AsQueryable()
                .Where(x => x.EmployeeId == employeeId)
                .Select(x => new CarryForwardLeaves()
                {
                    //needs to refractor domain model
                })
                .FirstOrDefault();
        }
    }
}
