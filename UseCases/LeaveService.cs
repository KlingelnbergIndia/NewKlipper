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

        public bool ApplyLeave(int employeeId, DateTime fromDate, DateTime toDate, LeaveType leaveType, string remark)
        {
            List<DateTime> takenLeaveDates = new List<DateTime>();
            var allAppliedLeaves = _leavesRepository.GetAllLeavesInfo(employeeId);
            for (DateTime eachLeaveDay = fromDate.Date; eachLeaveDay <= toDate; eachLeaveDay = eachLeaveDay.AddDays(1).Date)
            {
                bool isLeaveExist = allAppliedLeaves.Any(x => x.GetEmployeeId() == employeeId && x.GetLeaveDate().Contains(eachLeaveDay.Date));
                if (!isLeaveExist)
                {
                    takenLeaveDates.Add(eachLeaveDay);
                }
            }
            if (takenLeaveDates.Any())
            {
                var takenLeave = new Leave(employeeId, takenLeaveDates, leaveType, remark);
                _leavesRepository.AddNewLeave(takenLeave);
            }

            return true;
        }

        public List<LeaveDTO> GetAppliedLeaves(int employeeId)
        {
            List<Leave> leavesInfo = _leavesRepository.GetAllLeavesInfo(employeeId);
            var leaveDTO = leavesInfo.Select(x => new LeaveDTO()
            {
                Date = x.GetLeaveDate(),
                TypeOfLeave = x.GetLeaveType(),
                Remark = x.GetRemark(),
                FromDate = x.GetLeaveDate().Min(),
                ToDate = x.GetLeaveDate().Max(),
                NoOfDays = x.GetLeaveDate().Count()
            })
            .ToList();
            return leaveDTO;
        }
    }
}
