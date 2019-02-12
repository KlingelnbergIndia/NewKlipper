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

        public bool AddNewLeave(Leave leaveDetails, DateTime fromDate, DateTime toDate)
        {
            LeaveEntityModel takenLeaves = new LeaveEntityModel()
            {
                EmployeeId = leaveDetails.GetEmployeeId(),
                FromDate = fromDate,
                ToDate = toDate,
                Remark = leaveDetails.GetRemark(),
                TypeOfLeave = leaveDetails.GetLeaveType(),
                AppliedLeaveDates = leaveDetails.GetLeaveDate(),
            };

            _employeeDBContext.EmployeeLeaves
                .InsertOneAsync(takenLeaves)
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
                    leave.AppliedLeaveDates,
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
                .Where(x => x.EmployeeId == employeeId && x.AppliedLeaveDates.Contains(leaveDate.Date))
                .Any();
        }

        public bool OverrideLeave(Leave leaveData)
        {
            var isLeaveExist = _employeeDBContext.EmployeeLeaves
                .AsQueryable()
                .Where(x => x.EmployeeId == leaveData.GetEmployeeId() && x.AppliedLeaveDates == leaveData.GetLeaveDate())
                .Any();

            if (isLeaveExist)
            {
                _employeeDBContext.EmployeeLeaves
                .DeleteOneAsync(x => x.EmployeeId == leaveData.GetEmployeeId() && x.AppliedLeaveDates == leaveData.GetLeaveDate());
            }

            var leaveEntity = new LeaveEntityModel()
            {
                AppliedLeaveDates = leaveData.GetLeaveDate(),
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
