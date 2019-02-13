using DataAccess;
using System;
using System.Collections.Generic;
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


    }
}
