using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess;
using DataAccess.EntityModel.Employment;
using DataAccess.EntityModel.Leave;
using DomainModel;
using MongoDB.Bson;
using MongoDB.Driver;
using UseCaseBoundary;
using static DataAccess.EntityModel.Leave.LeaveEntityModel;
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
                Status = leaveDetails.GetStatus()
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
                    leave.Remark,
                    leave.Status,
                    leave._objectId.ToString()
                    ));
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

        public bool OverrideLeave(string leaveId,Leave leaveData)
        {
            var isLeaveExist = __leaveDBContext.AppliedLeaves
                .AsQueryable()
                .Where(x => x._objectId == ObjectId.Parse(leaveId))
                .Any();

            if (isLeaveExist)
            {
                __leaveDBContext.AppliedLeaves.DeleteOneAsync(x => x._objectId == ObjectId.Parse(leaveId));

                var leaveEntity = new LeaveEntityModel()
                {
                    AppliedLeaveDates = leaveData.GetLeaveDate(),
                    TypeOfLeave = leaveData.GetLeaveType(),
                    Remark = leaveData.GetRemark(),
                    EmployeeId = leaveData.GetEmployeeId(),
                    Status = leaveData.GetStatus()
                };
                __leaveDBContext.AppliedLeaves
                    .InsertOneAsync(leaveEntity)
                    .GetAwaiter()
                    .GetResult();
                return true;
                
            }
            else
            {
                return false;
            }

        }

        public bool CancelLeave(string LeaveId)
        {
            FilterDefinition<LeaveEntityModel> filter = Builders<LeaveEntityModel>.Filter.Eq("_id", ObjectId.Parse(LeaveId));
            UpdateDefinition<LeaveEntityModel> update = Builders<LeaveEntityModel>.Update.Set(x => x.Status, StatusType.Cancelled);

            var opts = new FindOneAndUpdateOptions<LeaveEntityModel>()
            {
                IsUpsert = true,
            };

            var model = __leaveDBContext.AppliedLeaves.FindOneAndUpdate(filter, update, opts);

            if (model != null)
            {

                return true;
            }
            else
            {
                return false;
            }
        }

        public Leave GetLeaveByLeaveId(string leaveId)
        {
            var leave = __leaveDBContext.AppliedLeaves.AsQueryable()
                .Where(x => x._objectId == ObjectId.Parse(leaveId)).FirstOrDefault();

            var leaveToLeaveObject = new Leave(leave.EmployeeId, leave.AppliedLeaveDates, leave.TypeOfLeave,leave.Remark, leave.Status, leave._objectId.ToString());

            return leaveToLeaveObject;
        }
    }
}
