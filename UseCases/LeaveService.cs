using System;
using System.Collections.Generic;
using System.Text;
using UseCaseBoundary;
using UseCaseBoundary.DTO;

namespace UseCases
{
    public class LeaveService
    {
        private ILeavesRepository _leavesRepository;
        public LeaveService(ILeavesRepository leavesRepository)
        {
            _leavesRepository = leavesRepository;
        }

        public LeaveDTO ApplyLeave(int employeeId, DateTime leaveDate, LeaveType leaveType)
        {
            var leave = _leavesRepository.AddNewLeave(new LeaveDTO());
        }
    }
}
