using DataAccess;
using DomainModel;
using MongoDB.Driver;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<CarryForwardLeaves> GetCarryForwardLeaveAsync(int employeeId)
        {
            //var leaves = _dbContext.CarryForwardLeaves
            //    .AsQueryable()
            //    .Where(x => x.EmployeeId == employeeId)
            //    .FirstOrDefault();

            var leaves = (await _dbContext.CarryForwardLeaves
                .FindAsync(x => x.EmployeeId == employeeId)).FirstOrDefault();

            if (leaves != null)
            {
                return new CarryForwardLeaves(
                   leaves.EmployeeId,
                   leaves.LeaveBalanceTillDate,
                   leaves.TakenCasualLeaves,
                   leaves.TakenSickLeaves,
                   leaves.TakenCompoffLeaves,
                   leaves.MaxCasualLeaves,
                   leaves.MaxSickLeaves,
                   leaves.MaxCompoffLeaves);

            }
            return null;
               
        }
    }
}
