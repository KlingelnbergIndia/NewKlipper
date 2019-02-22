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
        private ICarryForwardLeaves _carryForwardLeaves;

        public LeaveService(
            ILeavesRepository leavesRepository,
            IEmployeeRepository employeeRepository,
            IDepartmentRepository departmentRepository, ICarryForwardLeaves carryForwardLeavesRepository)
        {
            _leavesRepository = leavesRepository;
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _carryForwardLeaves = carryForwardLeavesRepository;
        }

        public ServiceResponseDTO ApplyLeave(int employeeId, DateTime fromDate, DateTime toDate, LeaveType leaveType, string remark)
        {
           
            List<DateTime> takenLeaveDates = new List<DateTime>();

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
                if (leaveType == LeaveType.SickLeave || leaveType == LeaveType.CompOff)
                {
                    var leaveSummmary = GetTotalSummary(employeeId);
                    if ((leaveType == LeaveType.SickLeave && leaveSummmary.RemainingSickLeave - takenLeaveDates.Count < 0) ||
                        (leaveType == LeaveType.CompOff && leaveSummmary.RemainingCompOffLeave - takenLeaveDates.Count < 0))
                    {
                        return ServiceResponseDTO.CanNotApplied;
                    }

                }
                var status = StatusType.Approved;
                var takenLeave = new Leave(employeeId, takenLeaveDates, leaveType, remark, status,null);
                _leavesRepository.AddNewLeave(takenLeave);
                return ServiceResponseDTO.Saved;
            }
            else
            {
                if (invalidDays == totalAppliedDays)
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
            var isRealizedLeave = false;
            List<LeaveRecordDTO> listOfLeaveDTO = new List<LeaveRecordDTO>();
            foreach (var eachLeave in leavesInfo)
            {
                if (eachLeave.GetLeaveDate().Min() < DateTime.Today.Date)
                {
                    isRealizedLeave = true;
                }
                var leaveDTO = new LeaveRecordDTO()
                {
                    Date = eachLeave.GetLeaveDate(),
                    TypeOfLeave = eachLeave.GetLeaveType(),
                    Remark = eachLeave.GetRemark(),
                    FromDate = eachLeave.GetLeaveDate().Min(),
                    ToDate = eachLeave.GetLeaveDate().Max(),
                    NoOfDays = eachLeave.GetLeaveDate().Count(),
                    Status = eachLeave.GetStatus(),
                    IsRealizedLeave = isRealizedLeave,
                    LeaveId = eachLeave.GetLeaveId(),
                };
                listOfLeaveDTO.Add(leaveDTO);
            }
            return listOfLeaveDTO;
        }

        public LeaveSummaryDTO GetTotalSummary(int employeeId)
        {
            var carryForwardLeave = _carryForwardLeaves.GetCarryForwardLeaveAsync(employeeId)
                .GetAwaiter()
                .GetResult();

            float totalCasualLeaveAvailable = carryForwardLeave.MaxCasualLeaves();
            float totalSickLeaveAvailable = carryForwardLeave.MaxSickLeaves();
            float totalCompOffLeaveAvailable = carryForwardLeave.MaxCompoffLeaves();
            float TotalAvailableLeave = totalCasualLeaveAvailable + totalSickLeaveAvailable + totalCompOffLeaveAvailable;

            var listOfAppliedLeaves = new LeaveLogs(_leavesRepository.GetAllLeavesInfo(employeeId));

            float casualLeaveTaken = listOfAppliedLeaves.CalculateCasualLeaveTaken() + carryForwardLeave.TakenCasualLeaves();
            float sickLeaveTaken = listOfAppliedLeaves.CalculateSickLeaveTaken() + carryForwardLeave.TakenSickLeaves();
            float compOffLeaveTaken = listOfAppliedLeaves.CalculateCompOffLeaveTaken() + carryForwardLeave.TakenCompoffLeaves();
            float leaveBalance = TotalAvailableLeave - listOfAppliedLeaves.GetTotalLeaveTaken();

            return new LeaveSummaryDTO()
            {
                TotalCasualLeaveTaken = casualLeaveTaken,
                TotalSickLeaveTaken = sickLeaveTaken,
                TotalCompOffLeaveTaken = compOffLeaveTaken,

                RemainingCasualLeave = totalCasualLeaveAvailable - casualLeaveTaken,
                RemainingSickLeave = totalSickLeaveAvailable - sickLeaveTaken,
                RemainingCompOffLeave = totalCompOffLeaveAvailable - compOffLeaveTaken,

                MaximumCasualLeave = totalCasualLeaveAvailable,
                MaximumSickLeave = totalSickLeaveAvailable,
                MaximumCompOffLeave = totalCompOffLeaveAvailable,

                LeaveBalance = leaveBalance
            };
        }

        public ServiceResponseDTO UpdateLeave(int employeeId, DateTime fromDate, DateTime toDate, LeaveType LeaveType,
            string Remark, List<DateTime> datesToBeChanged)
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
                bool isLeaveExist = false;
                if (!datesToBeChanged.Contains(eachLeaveDay))
                {
                    isLeaveExist = allAppliedLeaves.Any(x => x.GetEmployeeId() == employeeId && x.GetLeaveDate().Contains(eachLeaveDay.Date));
                }
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
                var status = StatusType.Updated;
                var takenLeave = new Leave(employeeId, takenLeaveDates, LeaveType, Remark, status,null);
                _leavesRepository.OverrideLeave(takenLeave, datesToBeChanged);
                return ServiceResponseDTO.Updated;
            }
            else
            {
                if (invalidDays == totalAppliedDays)
                {
                    return ServiceResponseDTO.InvalidDays;
                }
                else
                {
                    return ServiceResponseDTO.RecordExists;
                }
            }

        }

        public ServiceResponseDTO CancelLeave(string LeaveId)
        {
            if (_leavesRepository.CancelLeave(LeaveId))
            {
                return ServiceResponseDTO.Deleted;
            }

            return ServiceResponseDTO.InvalidDays;
        }
    }
}
