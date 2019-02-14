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
        private IEmployeeRepository _employeeRepository;
        private IDepartmentRepository _departmentRepository;
        public LeaveService(
            ILeavesRepository leavesRepository,
            IEmployeeRepository employeeRepository,
            IDepartmentRepository departmentRepository)
        {
            _leavesRepository = leavesRepository;
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
        }

        public ServiceResponseDTO ApplyLeave(int employeeId, DateTime fromDate, DateTime toDate, LeaveType leaveType, string remark)
        {
            List<DateTime> takenLeaveDates = new List<DateTime>();
            LeaveRecordDTO leaveRecord = new LeaveRecordDTO();

            Employee employeeData = _employeeRepository.GetEmployee(employeeId);
            Department department = _departmentRepository.GetDepartment(employeeData.Department());

            var allAppliedLeaves = _leavesRepository.GetAllLeavesInfo(employeeId);
            int invalidDays = 0;
            int totalAppliedDays = 0;

            for (DateTime eachLeaveDay = fromDate.Date; eachLeaveDay <= toDate; eachLeaveDay = eachLeaveDay.AddDays(1).Date)
            {
                bool isLeaveExist = allAppliedLeaves.Any(x => x.GetEmployeeId() == employeeId && x.GetLeaveDate().Contains(eachLeaveDay.Date));
                if (!isLeaveExist && department.IsValidWorkingDay(eachLeaveDay))
                {
                    takenLeaveDates.Add(eachLeaveDay);
                }
                if (!department.IsValidWorkingDay(eachLeaveDay))
                {
                    invalidDays++;
                }
                totalAppliedDays++;
            }

            if (takenLeaveDates.Any())
            {
                var takenLeave = new Leave(employeeId, takenLeaveDates, leaveType, remark);
                _leavesRepository.AddNewLeave(takenLeave);
                return ServiceResponseDTO.Saved;
            }
            else
            {
                if(invalidDays == totalAppliedDays)
                {
                    return ServiceResponseDTO.InvalidDays;
                }
                else
                {
                    return ServiceResponseDTO.RecordExists;
                }
            }
        }

        public List<LeaveRecordDTO> GetAppliedLeaves(int employeeId)
        {
            List<Leave> leavesInfo = _leavesRepository.GetAllLeavesInfo(employeeId);
            var leaveDTO = leavesInfo.Select(x => new LeaveRecordDTO()
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

        public LeaveSummaryDTO GetTotalSummary (int employeeId)
        {
            int totalCasualLeaveAvailable = 21;
            int totalSickLeaveAvailable = 6;
            int totalCompOffLeaveAvailable = 0;

            var listOfAppliedLeaves = new LeaveLogs(_leavesRepository.GetAllLeavesInfo(employeeId));

            int CasualLeaveTaken = listOfAppliedLeaves.CalculateCasualLeaveTaken();
            int SickLeaveTaken = listOfAppliedLeaves.CalculateSickLeaveTaken();
            int CompOffLeaveTaken = listOfAppliedLeaves.CalculateCompOffLeaveTaken();

            return new LeaveSummaryDTO()
            {
                TotalCasualLeaveTaken = CasualLeaveTaken,
                TotalSickLeaveTaken = SickLeaveTaken,
                TotalCompOffLeaveTaken = CompOffLeaveTaken,
                BalanceOfCasualLeave = totalCasualLeaveAvailable - CasualLeaveTaken,
                BalanceOfSickLeave = totalSickLeaveAvailable - SickLeaveTaken,
                BalanceOfCompOffLeave = totalCompOffLeaveAvailable - CompOffLeaveTaken
            };
        }
    }
}
