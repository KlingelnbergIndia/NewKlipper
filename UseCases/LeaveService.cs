using DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
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
            IDepartmentRepository departmentRepository,
            ICarryForwardLeaves carryForwardLeavesRepository)
        {
            _leavesRepository = leavesRepository;
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _carryForwardLeaves = carryForwardLeavesRepository;
        }

        public ServiceResponseDTO ApplyLeave
            (int employeeId, DateTime fromDate, DateTime toDate,
            LeaveType leaveType, bool isHalfDay, string remark)
        {
            if (IsMultipleDateForHalfDayLeave(fromDate, toDate, isHalfDay))
            {
                return ServiceResponseDTO.InvalidDays;
            }

            Employee employeeData = _employeeRepository.GetEmployee(employeeId);
            Department department = _departmentRepository
                .GetDepartment(employeeData.Department());

            var allAppliedLeaves = _leavesRepository
                .GetAllLeavesInfo(employeeId);
            var takenLeaveDates = TakenLeaveDates
                (fromDate, toDate, department, allAppliedLeaves, employeeId, null);

            return AddLeave(employeeId, leaveType, isHalfDay, remark, takenLeaveDates,
                InvalidDays(department, fromDate, toDate),
                TotalAppliedDays(department, fromDate, toDate));
        }

        public ServiceResponseDTO ApplyCompOff
        (int employeeId, DateTime fromDate, DateTime toDate,
            string remark)
        {
           
            Employee employeeData = _employeeRepository.GetEmployee(employeeId);
            Department department = _departmentRepository
                .GetDepartment(employeeData.Department());

            var allAppliedLeaves = _leavesRepository
                .GetAllLeavesInfo(employeeId);

            var takenCompOffDates = TakenCompOffDates
            (fromDate, toDate,
              allAppliedLeaves, employeeId, null);

            return AddCompOff(employeeId, remark,
                takenCompOffDates, InvalidDays(department, fromDate, toDate),
                TotalAppliedDays(department, fromDate, toDate));
        }

        private ServiceResponseDTO AddLeave(int employeeId, LeaveType leaveType, bool isHalfDay,
            string remark, List<DateTime> takenLeaveDates, int invalidDays, int totalAppliedDays)
        {
            if (takenLeaveDates.Any())
            {
                if (!IsSelectedLeaveIsAvailableToApply(
                    employeeId, leaveType, takenLeaveDates, isHalfDay))
                {
                    return ServiceResponseDTO.CanNotApplied;
                }
                var status = StatusType.Approved;
                var takenLeave = new Leave(employeeId, takenLeaveDates,
                    leaveType, isHalfDay, remark, status);

                _leavesRepository.AddNewLeave(takenLeave);
                return ServiceResponseDTO.Saved;
            }
            return
                invalidDays == totalAppliedDays
                ? ServiceResponseDTO.InvalidDays
                : ServiceResponseDTO.RecordExists;
        }

        private ServiceResponseDTO AddCompOff(int employeeId,
            string remark, List<DateTime> takenLeaveDates, int invalidDays,
            int totalAppliedDays)
        {
            if (takenLeaveDates.Any())
            {
                var status = StatusType.CompOffAdded;
                var takenLeave = new Leave(employeeId, takenLeaveDates,
                    LeaveType.CompOff, false, remark, status);

                _leavesRepository.AddNewLeave(takenLeave);
                return ServiceResponseDTO.Saved;
            }
            return
                invalidDays == totalAppliedDays
                    ? ServiceResponseDTO.InvalidDays
                    : ServiceResponseDTO.RecordExists;
        }

        public List<LeaveRecordDTO> AppliedLeaves(int employeeId)
        {
            List<Leave> leavesInfo = _leavesRepository.GetAllLeavesInfo(employeeId);
            List<LeaveRecordDTO> listOfLeaveDTO = new List<LeaveRecordDTO>();
            foreach (var eachLeave in leavesInfo)
            {
                var leaveDTO = new LeaveRecordDTO()
                {
                    Date = eachLeave.GetLeaveDate(),
                    TypeOfLeave = eachLeave.GetLeaveType(),
                    isHalfDayLeave = eachLeave.IsHalfDayLeave(),
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
            return listOfLeaveDTO.OrderByDescending(x => x.FromDate).ToList();
        }

        public LeaveSummaryDTO TotalSummary(int employeeId)
        {
            var carryForwardLeave = _carryForwardLeaves
                .GetCarryForwardLeaveAsync(employeeId)
                .GetAwaiter()
                .GetResult();

            if (carryForwardLeave == null)
            {
                return null;
            }

            float totalCasualLeaveAvailable, totalSickLeaveAvailable,
                totalCompOffLeaveAvailable, casualLeaveTaken,
                sickLeaveTaken, compOffLeaveTaken;

            AssignTotalLeaveSummaryField(employeeId, carryForwardLeave,
                out totalCasualLeaveAvailable, out totalSickLeaveAvailable,
                out totalCompOffLeaveAvailable, out casualLeaveTaken,
                out sickLeaveTaken, out compOffLeaveTaken);

            return GetLeaveSummaryDto(
                totalCasualLeaveAvailable, totalSickLeaveAvailable,
                totalCompOffLeaveAvailable, casualLeaveTaken,
                sickLeaveTaken, compOffLeaveTaken);
        }

        private LeaveSummaryDTO GetLeaveSummaryDto(
            float totalCasualLeaveAvailable, float totalSickLeaveAvailable,
            float totalCompOffLeaveAvailable, float casualLeaveTaken,
            float sickLeaveTaken, float compOffLeaveTaken)
        {
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

        private void AssignTotalLeaveSummaryField(
            int employeeId, CarryForwardLeaves carryForwardLeave,
            out float totalCasualLeaveAvailable, out float totalSickLeaveAvailable,
            out float totalCompOffLeaveAvailable, out float casualLeaveTaken,
            out float sickLeaveTaken, out float compOffLeaveTaken)
        {
            var listOfAppliedLeaves = new LeaveLogs
                (_leavesRepository.GetAllLeavesInfo(employeeId));

            totalCasualLeaveAvailable = carryForwardLeave
                .MaxCasualLeaves();
            totalSickLeaveAvailable = carryForwardLeave
                                       .MaxSickLeaves();
            totalCompOffLeaveAvailable =
                carryForwardLeave.MaxCompoffLeaves()
                 + listOfAppliedLeaves.CountAddedCompoffLeaves();
            
            casualLeaveTaken = listOfAppliedLeaves.CalculateCasualLeaveTaken()
                                     + carryForwardLeave.TakenCasualLeaves();
            sickLeaveTaken = listOfAppliedLeaves.CalculateSickLeaveTaken()
                                    + carryForwardLeave.TakenSickLeaves();
            compOffLeaveTaken = listOfAppliedLeaves.CalculateCompOffLeaveTaken()
                                    + carryForwardLeave.TakenCompoffLeaves();
        }

        public ServiceResponseDTO UpdateLeave(string leaveId, int employeeId,
            DateTime fromDate, DateTime toDate, LeaveType leaveType,
            bool isHalfDayLeave, string remark)
        {
            if (IsMultipleDateForHalfDayLeave(fromDate, toDate, isHalfDayLeave))
            {
                return ServiceResponseDTO.InvalidDays;
            }

            Employee employeeData = _employeeRepository.GetEmployee(employeeId);
            Department department = _departmentRepository
                .GetDepartment(employeeData.Department());

            var allAppliedLeaves = _leavesRepository
                .GetAllLeavesInfo(employeeId);

           
            bool UpdateLeaveAndAppliedLeaveTypeIsSame =
                _leavesRepository.GetLeaveByLeaveId(leaveId)
                    .GetLeaveType() == leaveType;

            var takenLeaveDates = TakenLeaveDates
            (fromDate, toDate,
                department, allAppliedLeaves, employeeId, leaveId);

            return ServiceResponseForUpdateExistingLeave(
                leaveId, employeeId, leaveType,
                isHalfDayLeave, remark, takenLeaveDates,
                InvalidDays(department, fromDate, toDate),
                TotalAppliedDays(department, fromDate, toDate),
                UpdateLeaveAndAppliedLeaveTypeIsSame, allAppliedLeaves);
        }

        public ServiceResponseDTO UpdateAddedCompOff(string leaveId, int employeeId,
            DateTime fromDate, DateTime toDate, string remark)
        {
            Employee employeeData = _employeeRepository.GetEmployee(employeeId);
            Department department = _departmentRepository
                .GetDepartment(employeeData.Department());

            var allAppliedLeaves = _leavesRepository
                .GetAllLeavesInfo(employeeId);

            var takenCompOffDates = TakenCompOffDates
            (fromDate, toDate,allAppliedLeaves, 
                employeeId, leaveId);

            return ServiceResponseForUpdateExistingAddedCompOff(
                leaveId, employeeId,
                remark, takenCompOffDates,
                InvalidDays(department, fromDate, toDate),
                TotalAppliedDays(department, fromDate, toDate),
                 allAppliedLeaves);
        }

        private ServiceResponseDTO ServiceResponseForUpdateExistingLeave(string leaveId, int employeeId,
            LeaveType leaveType, bool isHalfDayLeave, string remark,
            List<DateTime> takenLeaveDates, int invalidDays, int totalAppliedDays,
            bool UpdateLeaveAndAppliedLeaveTypeIsSame,List<Leave> allAppliedLeaves)
        {
            var empIdOfLeaveToBeUpdate = _leavesRepository
                .GetLeaveByLeaveId(leaveId).GetEmployeeId();
            var existingLeaveIsCancelledLeave = allAppliedLeaves
                .Any(x => x.GetLeaveId() == leaveId &&
                          x.GetStatus() == StatusType.Cancelled);
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

            if (takenLeaveDates.Any())
            {
                return ServiceResponseOfUpdateLeave(
                    leaveId, employeeId, leaveType, isHalfDayLeave,
                    remark, takenLeaveDates,
                    UpdateLeaveAndAppliedLeaveTypeIsSame);
            }

            return invalidDays == totalAppliedDays ?
                ServiceResponseDTO.InvalidDays
                : ServiceResponseDTO.RecordExists;
        }

        private ServiceResponseDTO ServiceResponseForUpdateExistingAddedCompOff(
            string leaveId, int employeeId,
             string remark,
            List<DateTime> takenLeaveDates, int invalidDays, int totalAppliedDays,
            List<Leave> allAppliedLeaves)
        {
            var empIdOfAddedCompOffToBeUpdate = _leavesRepository
                .GetLeaveByLeaveId(leaveId).GetEmployeeId();

            var existingAddedCompOffIsCancelled = allAppliedLeaves
                .Any(x => x.GetLeaveId() == leaveId &&
                          x.GetStatus() == StatusType.CompOffCancelled);

            if (existingAddedCompOffIsCancelled)
            {
                return ServiceResponseDTO.Deleted;
            }

            if (empIdOfAddedCompOffToBeUpdate == employeeId)
            {
                if (IsRealizedLeave(leaveId))
                {
                    return ServiceResponseDTO.RealizedLeave;
                }
            }

            if (takenLeaveDates.Any())
            {
                return ServiceResponseOfUpdateAddedCompOff(
                    leaveId, employeeId,
                    remark, takenLeaveDates);
            }

            return invalidDays == totalAppliedDays ?
                ServiceResponseDTO.InvalidDays
                : ServiceResponseDTO.RecordExists;
        }

        private ServiceResponseDTO ServiceResponseOfUpdateAddedCompOff(
            string leaveId, int employeeId,
            string remark,
            List<DateTime> takenLeaveDates)
        {
            var takenLeave = new Leave
            (employeeId, takenLeaveDates, LeaveType.CompOff,false,
                remark, StatusType.CompOffUpdated, null);
            _leavesRepository.OverrideLeave(leaveId, takenLeave);

            return ServiceResponseDTO.Updated;
        }

        private ServiceResponseDTO ServiceResponseOfUpdateLeave(
            string leaveId, int employeeId, LeaveType leaveType, 
            bool isHalfDayLeave, string remark, 
            List<DateTime> takenLeaveDates, 
            bool UpdateLeaveAndAppliedLeaveTypeIsSame)
        {
            if (IsSelectedLeaveIsAvailableToUpdate
                (employeeId, leaveType, isHalfDayLeave, 
                    UpdateLeaveAndAppliedLeaveTypeIsSame,
                    takenLeaveDates, leaveId) == false)
            {
                return ServiceResponseDTO.CanNotApplied;
            }
            var takenLeave = new Leave
                (employeeId, takenLeaveDates, leaveType, isHalfDayLeave,
                remark, StatusType.Updated, null);
            _leavesRepository.OverrideLeave(leaveId, takenLeave);
            return ServiceResponseDTO.Updated;
        }

        public ServiceResponseDTO CancelLeave(string leaveId, int employeeId)
        {
            var empIdOfLeaveToBeUpdate = _leavesRepository
                .GetLeaveByLeaveId(leaveId)
                .GetEmployeeId();

            if (empIdOfLeaveToBeUpdate == employeeId &&
                IsRealizedLeave(leaveId))
                    return ServiceResponseDTO.RealizedLeave;
           
            if (_leavesRepository.CancelLeave(leaveId))
                return ServiceResponseDTO.Deleted;

            return ServiceResponseDTO.InvalidDays;
        }

        public ServiceResponseDTO CancelCompOff(string leaveId, int employeeId)
        {
            var empIdOfLeaveToBeUpdate = _leavesRepository
                .GetLeaveByLeaveId(leaveId)
                .GetEmployeeId();

            if (empIdOfLeaveToBeUpdate == employeeId &&
                IsRealizedLeave(leaveId))
                return ServiceResponseDTO.RealizedLeave;

            if (_leavesRepository.CancelCompOff(leaveId))
                return ServiceResponseDTO.Deleted;

            return ServiceResponseDTO.InvalidDays;
        }

        private List<DateTime> TakenLeaveDates
           (DateTime fromDate, DateTime toDate,
            Department department,
           List<Leave> allAppliedLeaves, int employeeId, string leaveId)
        {
            var takenLeaveDates = new List<DateTime>();
            for (DateTime eachLeaveDay = fromDate.Date;
                            eachLeaveDay <= toDate;
                            eachLeaveDay = eachLeaveDay.AddDays(1).Date)
            {
                bool isLeaveExist = CheckIsLeaveExist(allAppliedLeaves, employeeId, leaveId, eachLeaveDay);
                if (!isLeaveExist && department.IsValidWorkingDay(eachLeaveDay))
                {
                    takenLeaveDates.Add(eachLeaveDay);
                }
            }
            return takenLeaveDates;
        }

        private List<DateTime> TakenCompOffDates
        (DateTime fromDate, DateTime toDate,
            List<Leave> allAppliedLeaves, int employeeId, string leaveId)
        {
            var takenLeaveDates = new List<DateTime>();
            for (DateTime eachLeaveDay = fromDate.Date;
                eachLeaveDay <= toDate;
                eachLeaveDay = eachLeaveDay.AddDays(1).Date)
            {
                bool isLeaveExist = CheckIsLeaveExist(allAppliedLeaves, employeeId, leaveId, eachLeaveDay);
                if (!isLeaveExist)
                {
                    takenLeaveDates.Add(eachLeaveDay);
                }
            }
            return takenLeaveDates;
        }

        private static bool CheckIsLeaveExist(List<Leave> allAppliedLeaves, int employeeId, string leaveId, DateTime eachLeaveDay)
        {
            return leaveId == null
                ? allAppliedLeaves
                    .Any(x => x.GetEmployeeId() == employeeId
                              && x.GetLeaveDate()
                                  .Contains(eachLeaveDay.Date)
                              && x.GetStatus() != StatusType.Cancelled
                              && x.GetStatus() != StatusType.CompOffCancelled)
                : allAppliedLeaves
                        .Any(x => x.GetEmployeeId() == employeeId
                                  && x.GetLeaveDate()
                                      .Contains(eachLeaveDay.Date)
                                  && x.GetLeaveId() != leaveId
                                  && x.GetStatus() != StatusType.Cancelled
                                  && x.GetStatus() != StatusType.CompOffCancelled);
        }

        private int TotalAppliedDays(Department department, DateTime fromDate, DateTime toDate)
        {
            int totalAppliedDays = 0;
            for (var eachLeaveDay = fromDate.Date;
                eachLeaveDay <= toDate;
                eachLeaveDay = eachLeaveDay.AddDays(1).Date)
            {
                totalAppliedDays++;
            }
            return totalAppliedDays;
        }

        private int InvalidDays(Department department, DateTime fromDate, DateTime toDate)
        {
            int invalidDays = 0;
            for (var eachLeaveDay = fromDate.Date;
                eachLeaveDay <= toDate;
                eachLeaveDay = eachLeaveDay.AddDays(1).Date)
            {
                if (!department.IsValidWorkingDay(eachLeaveDay))
                {
                    invalidDays++;
                }
            }
            return invalidDays;
        }

        private bool IsSelectedLeaveIsAvailableToApply
            (int employeeId, LeaveType leaveType, List<DateTime> takenLeaveDates, bool isHalfDay)
        {
            if (leaveType == LeaveType.SickLeave || leaveType == LeaveType.CompOff)
            {
                var leaveSummmary = TotalSummary(employeeId);
                float CountOfTakenLeaveDates = takenLeaveDates.Count;
                if (leaveSummmary == null)
                    return false;
                if (isHalfDay == true)
                {
                    CountOfTakenLeaveDates = CountOfTakenLeaveDates / 2;
                }
                if ((leaveType == LeaveType.SickLeave &&
                     leaveSummmary.RemainingSickLeave - CountOfTakenLeaveDates < 0) ||
                    (leaveType == LeaveType.CompOff &&
                     leaveSummmary.RemainingCompOffLeave - CountOfTakenLeaveDates < 0))
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsSelectedLeaveIsAvailableToUpdate(
            int employeeId, LeaveType leaveType, bool isHalfDayLeave,
            bool UpdateLeaveAndAppliedLeaveTypeIsSame, 
            List<DateTime> takenLeaveDates, string leaveId)
        {
            if (leaveType == LeaveType.SickLeave || leaveType == LeaveType.CompOff)
            {
                var leaveSummmary = TotalSummary(employeeId);
                if (leaveSummmary == null)
                    return false;

                float CountOfTakenLeaveDates = isHalfDayLeave == true
                    ? takenLeaveDates.Count / 2
                : takenLeaveDates.Count;

                if (isHalfDayLeave == true)
                {
                    CountOfTakenLeaveDates = CountOfTakenLeaveDates / 2;
                }

                if (!IsLeaveAvailable(leaveType, UpdateLeaveAndAppliedLeaveTypeIsSame,
                    leaveId, leaveSummmary, CountOfTakenLeaveDates))
                    return false;
            }
            return true;
        }

        private bool IsLeaveAvailable(LeaveType leaveType, bool UpdateLeaveAndAppliedLeaveTypeIsSame, string leaveId,
            LeaveSummaryDTO leaveSummmary, float CountOfTakenLeaveDates)
        {
            if (!UpdateLeaveAndAppliedLeaveTypeIsSame)
            {
                if ((leaveType == LeaveType.SickLeave &&
                     leaveSummmary.RemainingSickLeave - CountOfTakenLeaveDates < 0) ||
                    (leaveType == LeaveType.CompOff &&
                     leaveSummmary.RemainingCompOffLeave - CountOfTakenLeaveDates < 0))
                {
                    return false;
                }
            }
            else
            {
                if ((leaveType == LeaveType.SickLeave &&
                     (leaveSummmary.RemainingSickLeave +
                      CalculateNoOfDays(_leavesRepository.GetLeaveByLeaveId(leaveId)))
                     - CountOfTakenLeaveDates < 0) ||
                    (leaveType == LeaveType.CompOff &&
                     (leaveSummmary.RemainingSickLeave +
                      CalculateNoOfDays(_leavesRepository.GetLeaveByLeaveId(leaveId)))
                     - CountOfTakenLeaveDates < 0))
                {
                    return false;
                }
            }
            return true;
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

        private bool IsMultipleDateForHalfDayLeave
            (DateTime fromDate, DateTime toDate, bool isHalfDayLeave)
        {
            if (isHalfDayLeave == true && fromDate.Date != toDate.Date)
            {
                return true;
            }
            return false;
        }

        private float CalculateNoOfDays(Leave leave)
        {
            if (leave.IsHalfDayLeave() == true)
            {
                return (float)leave.GetLeaveDate().Count() / 2;
            }
            return leave.GetLeaveDate().Count();
        }
    }
}
