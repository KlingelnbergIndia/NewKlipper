using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess;
using DataAccess.EntityModel.Leave;
using DomainModel;
using MongoDB.Bson;
using MongoDB.Driver;
using UseCaseBoundary;
using static DomainModel.Leave;

namespace RepositoryImplementation
{
    public class LeavesMongoDBRepository : ILeavesRepository
    {
        private readonly LeaveManagementDBContext _leaveDBContext = null;

        public LeavesMongoDBRepository()
        {
            _leaveDBContext = LeaveManagementDBContext.Instance;
        }

        public bool AddNewLeave(Leave leaveDetails)
        {
            var takenLeaves = new LeaveEntityModel()
            {
                EmployeeId = leaveDetails.GetEmployeeId(),
                FromDate = leaveDetails.GetLeaveDate().Min(),
                ToDate = leaveDetails.GetLeaveDate().Max(),
                Remark = leaveDetails.GetRemark(),
                TypeOfLeave = leaveDetails.GetLeaveType(),
                IsHalfDayLeave = leaveDetails.IsHalfDayLeave(),
                AppliedLeaveDates = leaveDetails.GetLeaveDate(),
                Status = leaveDetails.GetStatus()
            };
            _leaveDBContext.AppliedLeaves
                .InsertOneAsync(takenLeaves)
                .GetAwaiter()
                .GetResult();

            return true;
        }

        public List<Leave> GetAllLeavesInfo(int employeeId)
        {
            var leaves = new List<Leave>();
            var empLeaves = _leaveDBContext.AppliedLeaves
                .AsQueryable()
                .Where(x => x.EmployeeId == employeeId)
                .ToList();
            
            foreach (var leave in empLeaves)
            {
                leaves.Add(new Leave(
                    leave.EmployeeId,
                    leave.AppliedLeaveDates,
                    leave.TypeOfLeave,
                     leave.IsHalfDayLeave,
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
                _leaveDBContext.AppliedLeaves
                .AsQueryable()
                .Where(x => x.EmployeeId == employeeId && 
                            x.AppliedLeaveDates.Contains(leaveDate.Date))
                .Any();
        }

        public bool OverrideLeave(string leaveId,Leave leaveData)
        {
            var isLeaveExist = _leaveDBContext.AppliedLeaves
                .AsQueryable()
                .Where(x => x._objectId == ObjectId.Parse(leaveId))
                .Any();

            if (isLeaveExist)
            {
                _leaveDBContext.AppliedLeaves
                    .DeleteOneAsync(
                        x => x._objectId == ObjectId.Parse(leaveId));

                var leaveEntity = new LeaveEntityModel()
                {
                    AppliedLeaveDates = leaveData.GetLeaveDate(),
                    TypeOfLeave = leaveData.GetLeaveType(),
                    IsHalfDayLeave = leaveData.IsHalfDayLeave(),
                    Remark = leaveData.GetRemark(),
                    EmployeeId = leaveData.GetEmployeeId(),
                    Status = leaveData.GetStatus()
                };
                _leaveDBContext.AppliedLeaves
                    .InsertOneAsync(leaveEntity)
                    .GetAwaiter()
                    .GetResult();

                return true;
            }
                return false;
        }

        public bool CancelLeave(string LeaveId)
        {
            var filter = Builders<LeaveEntityModel>.Filter
                .Eq("_id", ObjectId.Parse(LeaveId));
            var update = Builders<LeaveEntityModel>.Update
                .Set(x => x.Status, StatusType.Cancelled);

            var opts = new FindOneAndUpdateOptions<LeaveEntityModel>()
            {
                IsUpsert = true,
            };

            var model = _leaveDBContext.AppliedLeaves
                .FindOneAndUpdate(filter, update, opts);

            return model != null ? true : false;
        }

        public bool CancelCompOff(string LeaveId)
        {
            var filter = Builders<LeaveEntityModel>.Filter
                .Eq("_id", ObjectId.Parse(LeaveId));
            var update = Builders<LeaveEntityModel>.Update
                .Set(x => x.Status, StatusType.CompOffCancelled);

            var opts = new FindOneAndUpdateOptions<LeaveEntityModel>()
            {
                IsUpsert = true,
            };

            var model = _leaveDBContext.AppliedLeaves
                .FindOneAndUpdate(filter, update, opts);

            return model != null ? true : false;
        }

        public Leave GetLeaveByLeaveId(string leaveId)
        {
            var leave = _leaveDBContext.AppliedLeaves.AsQueryable()
                .Where(x => x._objectId == ObjectId.Parse(leaveId))
                .FirstOrDefault();

            var leaveToLeaveObject = new Leave(leave.EmployeeId,
                leave.AppliedLeaveDates,leave.TypeOfLeave,
                leave.IsHalfDayLeave,leave.Remark, leave.Status,
                leave._objectId.ToString());

            return leaveToLeaveObject;
        }
    }
}
