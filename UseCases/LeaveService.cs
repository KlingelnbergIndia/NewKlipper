using DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UseCaseBoundary;
using UseCaseBoundary.DTO;
using static DomainModel.Leave;

namespace UseCases
{
    public class LeaveService
    {
        private ILeavesRepository _leavesRepository;
        public LeaveService(ILeavesRepository leavesRepository)
        {
            _leavesRepository = leavesRepository;
        }

        public bool ApplyLeave(int employeeId, DateTime leaveDate, LeaveType leaveType, string remark)
        {
            var leaveDto =  new Leave(
                employeeId,
                leaveDate, 
                leaveType,
                remark);

            bool isLeaveExist = _leavesRepository.IsLeaveExist(employeeId, leaveDate);
            if (isLeaveExist)
            {
                return _leavesRepository.OverrideLeave(leaveDto);
            }

            return _leavesRepository.AddNewLeave(leaveDto);
        }

        public List<LeaveDTO> GetLeaves(int employeeId)
        {
            List<Leave> leavesInfo = _leavesRepository.GetAllLeavesInfo(employeeId);
            var leaveDTO = leavesInfo.Select(x=> new LeaveDTO() {
                Date = x.GetLeaveDate(),
                TypeOfLeave = x.GetLeaveType(),
                Remark = x.GetRemark()
            })
            .ToList();
            return leaveDTO;
        }
    }
}
