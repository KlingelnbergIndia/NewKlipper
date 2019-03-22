using DomainModel;
using System;
using System.Collections.Generic;
using System.Text;
using UseCaseBoundary.DTO;

namespace UseCaseBoundary
{
    public interface ILeavesRepository
    {
        bool AddNewLeave(Leave leave);
        List<Leave> GetAllLeavesInfo(int employeeId);
        bool IsLeaveExist(int employeeId, DateTime leaveDate);
        bool OverrideLeave(string leaveId, Leave leaveData);
        bool CancelLeave(string LeaveId);
        Leave GetLeaveByLeaveId(string LeaveId);
        bool CancelCompOff(string LeaveId);
    }
}
