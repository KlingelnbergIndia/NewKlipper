using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess;
using DataAccess.EntityModel.Employment;
using DomainModel;
using MongoDB.Driver;
using UseCaseBoundary;
using static DomainModel.Leave;

namespace RepositoryImplementation
{
    public class LeavesMongoDBRepository : ILeavesRepository
    {
        private readonly EmployeeDBContext _employeeDBContext = null;

        public LeavesMongoDBRepository()
        {
            _employeeDBContext = EmployeeDBContext.Instance;
        }

        public bool AddNewLeave(Leave leaveDetails)
        {
            var leaveEntity = new LeaveEntityModel()
            {
                LeaveDate = leaveDetails.GetLeaveDate(),
                TypeOfLeave = leaveDetails.GetLeaveType(),
                Remark = leaveDetails.GetRemark(),
                EmployeeId = leaveDetails.GetEmployeeId()
            };

            _employeeDBContext.EmployeeLeaves
                .InsertOneAsync(leaveEntity)
                .GetAwaiter()
                .GetResult();

            return true;
        }

        public List<Leave> GetAllLeavesInfo(int employeeId)
        {
            List<Leave> leaves = new List<Leave>();
            var empLeaves = _employeeDBContext.EmployeeLeaves
                .AsQueryable()
                .Where(x => x.EmployeeId == employeeId)
                .ToList();

            foreach (var leave in empLeaves)
            {
                leaves.Add(new Leave(
                    leave.EmployeeId,
                    leave.LeaveDate,
                    leave.TypeOfLeave,
                    leave.Remark));
            }

            return leaves;
        }

        public bool IsLeaveExist(int employeeId, DateTime leaveDate)
        {
            return
                _employeeDBContext.EmployeeLeaves
                .AsQueryable()
                .Where(x => x.EmployeeId == employeeId && x.LeaveDate == leaveDate.Date)
                .Any();
        }

        public bool OverrideLeave(Leave leaveData)
        {
            var isLeaveExist = _employeeDBContext.EmployeeLeaves
                .AsQueryable()
                .Where(x => x.EmployeeId == leaveData.GetEmployeeId() && x.LeaveDate == leaveData.GetLeaveDate())
                .Any();

            if (isLeaveExist)
                return false;

            var leaveEntity = new LeaveEntityModel()
            {
                LeaveDate = leaveData.GetLeaveDate(),
                TypeOfLeave = leaveData.GetLeaveType(),
                Remark = leaveData.GetRemark(),
                EmployeeId = leaveData.GetEmployeeId()
            };
            _employeeDBContext.EmployeeLeaves
                .InsertOneAsync(leaveEntity)
                .GetAwaiter()
                .GetResult();
            return true;
        }
    }
}
