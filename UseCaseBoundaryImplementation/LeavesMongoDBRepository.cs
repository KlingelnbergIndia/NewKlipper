using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess;
using DataAccess.EntityModel.Employment;
using DataAccess.EntityModel.Leave;
using DomainModel;
using MongoDB.Driver;
using UseCaseBoundary;
using static DomainModel.Leave;

namespace RepositoryImplementation
{
    public class LeavesMongoDBRepository : ILeavesRepository
    {
        private readonly LeaveManagementDBContext __leaveDBContext = null;

        public LeavesMongoDBRepository()
        {
            __leaveDBContext = LeaveManagementDBContext.Instance;
        }

        public bool AddNewLeave(Leave leaveDetails)
        {
            LeaveEntityModel takenLeaves = new LeaveEntityModel()
            {
                EmployeeId = leaveDetails.GetEmployeeId(),
                FromDate = leaveDetails.GetLeaveDate().Min(),
                ToDate = leaveDetails.GetLeaveDate().Max(),
                Remark = leaveDetails.GetRemark(),
                TypeOfLeave = leaveDetails.GetLeaveType(),
                AppliedLeaveDates = leaveDetails.GetLeaveDate(),
            };

            __leaveDBContext.AppliedLeaves
                .InsertOneAsync(takenLeaves)
                .GetAwaiter()
                .GetResult();

            return true;
        }

        public List<Leave> GetAllLeavesInfo(int employeeId)
        {
            List<Leave> leaves = new List<Leave>();
            var empLeaves = __leaveDBContext.AppliedLeaves
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
                __leaveDBContext.AppliedLeaves
                .AsQueryable()
                .Where(x => x.EmployeeId == employeeId && x.AppliedLeaveDates.Contains(leaveDate.Date))
                .Any();
        }

        public bool OverrideLeave(Leave leaveData,List<DateTime> datesToBeChanged)
        {
            var isLeaveExist = __leaveDBContext.AppliedLeaves
                .AsQueryable()
                .Where(x => x.EmployeeId == leaveData.GetEmployeeId() && x.AppliedLeaveDates== datesToBeChanged)
                .Any();

            if (isLeaveExist)
            {
                __leaveDBContext.AppliedLeaves
                .DeleteOneAsync(x => x.EmployeeId == leaveData.GetEmployeeId() && x.AppliedLeaveDates == datesToBeChanged);
            }
            else
            {
                return false;
            }

            var leaveEntity = new LeaveEntityModel()
            {
                AppliedLeaveDates = leaveData.GetLeaveDate(),
                TypeOfLeave = leaveData.GetLeaveType(),
                Remark = leaveData.GetRemark(),
                EmployeeId = leaveData.GetEmployeeId()
            };
            __leaveDBContext.AppliedLeaves
                .InsertOneAsync(leaveEntity)
                .GetAwaiter()
                .GetResult();
            return true;
        }
        
    }
}
