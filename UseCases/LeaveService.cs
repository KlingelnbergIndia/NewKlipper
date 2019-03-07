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

        public ServiceResponseDTO ApplyLeave(int employeeId, DateTime fromDate, DateTime toDate, LeaveType leaveType,bool isHalfDay, string remark)
        {
            if (IsMultipleDateForHalfDayLeave(fromDate, toDate, isHalfDay))
            {
                return ServiceResponseDTO.InvalidDays;
            }
            List<DateTime> takenLeaveDates = new List<DateTime>();

            Employee employeeData = _employeeRepository.GetEmployee(employeeId);
            Department department = _departmentRepository.GetDepartment(employeeData.Department());

            var allAppliedLeaves = _leavesRepository.GetAllLeavesInfo(employeeId);
            int invalidDays = 0;
            int totalAppliedDays = 0;

            for (DateTime eachLeaveDay = fromDate.Date; eachLeaveDay <= toDate; eachLeaveDay = eachLeaveDay.AddDays(1).Date)
            {
                bool isLeaveExist = allAppliedLeaves.Any(x => x.GetEmployeeId() == employeeId 
                && x.GetLeaveDate().Contains(eachLeaveDay.Date) && x.GetStatus() != StatusType.Cancelled);
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
                    float CountOfTakenLeaveDates = takenLeaveDates.Count;
                    if (leaveSummmary == null)
                        return ServiceResponseDTO.CanNotApplied;
                    if (isHalfDay == true)
                    {
                        CountOfTakenLeaveDates = CountOfTakenLeaveDates / 2;
                    }
                    if ((leaveType == LeaveType.SickLeave && leaveSummmary.RemainingSickLeave - CountOfTakenLeaveDates < 0) ||
                        (leaveType == LeaveType.CompOff && leaveSummmary.RemainingCompOffLeave - CountOfTakenLeaveDates < 0))
                    {
                        return ServiceResponseDTO.CanNotApplied;
                    }

                }
                var status = StatusType.Approved;
                var takenLeave = new Leave(employeeId, takenLeaveDates, leaveType, isHalfDay, remark, status, null);

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
            List<LeaveRecordDTO> listOfLeaveDTO = new List<LeaveRecordDTO>();
            foreach (var eachLeave in leavesInfo)
            {
                var leaveDTO = new LeaveRecordDTO()
                {
                    Date = eachLeave.GetLeaveDate(),
                    TypeOfLeave = eachLeave.GetLeaveType(),
                    isHalfDayLeave= eachLeave.IsHalfDayLeave(),
                    Remark = eachLeave.GetRemark(),
                    FromDate = eachLeave.GetLeaveDate().Min(),
                    ToDate = eachLeave.GetLeaveDate().Max(),
                    NoOfDays = CalculateNoOfDays(eachLeave),
                    Status = eachLeave.GetStatus(),
                    IsRealizedLeave = IsRealizedLeave(eachLeave.GetLeaveId()),
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

            if (carryForwardLeave==null)
            {
                return null;
            }
            float totalCasualLeaveAvailable = carryForwardLeave.MaxCasualLeaves();
            float totalSickLeaveAvailable = carryForwardLeave.MaxSickLeaves();
            float totalCompOffLeaveAvailable = carryForwardLeave.MaxCompoffLeaves();
            float TotalAvailableLeave = totalCasualLeaveAvailable + totalSickLeaveAvailable + totalCompOffLeaveAvailable;

            var listOfAppliedLeaves = new LeaveLogs(_leavesRepository.GetAllLeavesInfo(employeeId));

            float casualLeaveTaken = listOfAppliedLeaves.CalculateCasualLeaveTaken() + carryForwardLeave.TakenCasualLeaves();
            float sickLeaveTaken = listOfAppliedLeaves.CalculateSickLeaveTaken() + carryForwardLeave.TakenSickLeaves();
            float compOffLeaveTaken = listOfAppliedLeaves.CalculateCompOffLeaveTaken() + carryForwardLeave.TakenCompoffLeaves();

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

            };
        }

        public ServiceResponseDTO UpdateLeave(string leaveId, int employeeId,
            DateTime fromDate, DateTime toDate, LeaveType leaveType,bool isHalfDayLeave, string remark)
        {
            if (IsMultipleDateForHalfDayLeave(fromDate, toDate, isHalfDayLeave))
            {
                return ServiceResponseDTO.InvalidDays;
            }
            List<DateTime> takenLeaveDates = new List<DateTime>();

            Employee employeeData = _employeeRepository.GetEmployee(employeeId);
            Department department = _departmentRepository.GetDepartment(employeeData.Department());

            var allAppliedLeaves = _leavesRepository.GetAllLeavesInfo(employeeId);
            int invalidDays = 0;
            int totalAppliedDays = 0;

            var existingLeaveIsCancelledLeave = allAppliedLeaves.Any(x =>
            x.GetLeaveId() == leaveId && x.GetStatus() == StatusType.Cancelled);
            var empIdOfLeaveToBeUpdate = _leavesRepository.GetLeaveByLeaveId(leaveId).GetEmployeeId();

            if (existingLeaveIsCancelledLeave)
            {
                return ServiceResponseDTO.Deleted;
            }

            if (empIdOfLeaveToBeUpdate == employeeId)
            {
                if (IsRealizedLeave(leaveId))
                {
                    return ServiceResponseDTO.RealizedLeave;
                }
            }
        
                for (DateTime eachLeaveDay = fromDate.Date; eachLeaveDay <= toDate; 
                    eachLeaveDay = eachLeaveDay.AddDays(1).Date)
                {
                  
                    bool isLeaveExist = allAppliedLeaves.Any(x=> x.GetLeaveDate().Contains(eachLeaveDay.Date)
                    && x.GetLeaveId() != leaveId && x.GetStatus() != StatusType.Cancelled);

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
                    var takenLeave = new Leave(employeeId, takenLeaveDates, leaveType, isHalfDayLeave, remark, StatusType.Updated, null);
                    _leavesRepository.OverrideLeave(leaveId, takenLeave);
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

        public ServiceResponseDTO CancelLeave(string leaveId, int employeeId)
        {
            var empIdOfLeaveToBeUpdate = _leavesRepository.GetLeaveByLeaveId(leaveId).GetEmployeeId();
            if (empIdOfLeaveToBeUpdate == employeeId)
            {
                if (IsRealizedLeave(leaveId))
                {
                    return ServiceResponseDTO.RealizedLeave;
                }
            }
            if (_leavesRepository.CancelLeave(leaveId))
            {
                return ServiceResponseDTO.Deleted;
            }

            return ServiceResponseDTO.InvalidDays;
        }

        private bool IsRealizedLeave(string LeaveId)
        {
            var leave = _leavesRepository.GetLeaveByLeaveId(LeaveId);
            if (leave != null)
            {
                if (leave.GetLeaveDate().Min() <= DateTime.Now.Date)
                {
                    return true;
                }
            }

            return false;
        }
        private bool IsMultipleDateForHalfDayLeave(DateTime fromDate,DateTime toDate,bool isHalfDayLeave)
        {
            if(isHalfDayLeave==true && fromDate.Date!= toDate.Date)
            {
                return true ;
            }
            return false;
        }
        private float CalculateNoOfDays(Leave leave)
        {
            if (leave.IsHalfDayLeave() == true)
            {
               return  (float)leave.GetLeaveDate().Count() / 2;
            }
            return leave.GetLeaveDate().Count();
        }
    }
}
