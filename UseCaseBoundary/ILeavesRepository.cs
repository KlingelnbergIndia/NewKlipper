using DomainModel;
using System;
using System.Collections.Generic;
using System.Text;
using UseCaseBoundary.DTO;

namespace UseCaseBoundary
{
    public interface ILeavesRepository
    {
        bool AddNewLeave(Leave leaveDTO);
        List<Leave> GetAllLeavesInfo(int employeeId);
        bool IsLeaveExist(int employeeId, DateTime leaveDate);
        bool OverrideLeave(Leave leaveDto);
    }
}
