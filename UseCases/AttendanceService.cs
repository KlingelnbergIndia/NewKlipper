using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainModel;
using UseCaseBoundary;
using UseCaseBoundary.DTO;
using UseCaseBoundary.Model;

namespace UseCases
{
    public class AttendanceService
    {
        private IAccessEventsRepository _accessEventsRepository;
        private IEmployeeRepository _employeeRepository;
        private IDepartmentRepository _departmentRepository;
        private IAttendanceRegularizationRepository _attendanceRegularizationRepository;
        private ILeavesRepository _leavesRepository;

        public AttendanceService(
            IAccessEventsRepository accessEventsRepository,
            IEmployeeRepository employeeRepository,
            IDepartmentRepository departmentRepository,
            IAttendanceRegularizationRepository attendanceRegularizationRepository,
            ILeavesRepository leavesRepository)
        {
            _accessEventsRepository = accessEventsRepository;
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _attendanceRegularizationRepository = attendanceRegularizationRepository;
            _leavesRepository = leavesRepository;
        }

        public AttendanceRecordsDTO AttendanceReportForDateRange(
            int employeeId, DateTime fromDate, DateTime toDate)
        {
            var accessEvents = _accessEventsRepository
                .GetAccessEventsForDateRange(employeeId, fromDate, toDate);
            var datewiseAccessEvents = accessEvents.GetAllAccessEvents();
            var listOfLeave = _leavesRepository.GetAllLeavesInfo(employeeId);
            var listOfPerDayAttendanceRecord = CreatePerDayAttendanceRecord(
                employeeId, datewiseAccessEvents, listOfLeave);

            listOfPerDayAttendanceRecord = IncludeLeaves
                (listOfPerDayAttendanceRecord, listOfLeave, fromDate, toDate, employeeId);

            Employee employeeData = _employeeRepository.GetEmployee(employeeId);
            Department department = _departmentRepository
                .GetDepartment(employeeData.Department());

            return new AttendanceRecordsDTO
            {
                ListOfAttendanceRecordDTO = listOfPerDayAttendanceRecord,

                TotalWorkingHours = CalculateTotalWorkingHours(
                    listOfPerDayAttendanceRecord),

                EstimatedHours = CalculateEstimatedHours(
                    listOfPerDayAttendanceRecord, department.GetNoOfHoursToBeWorked()),

                TotalDeficitOrExtraHours = CalculateDeficiateOrExtraTime(
                    listOfPerDayAttendanceRecord, department.GetNoOfHoursToBeWorked())
            };
        }

        public async Task<List<AccessPointRecord>> AccessPointDetails(
            int employeeId, DateTime date)
        {
            var perDayWorkRecord = _accessEventsRepository
                .GetAccessEventsForADay(employeeId, date);

            var RecreationPointAccessEvents = perDayWorkRecord
                .GetRecreationPointAccessEvents();
            var GymnasiumPointAccessEvents = perDayWorkRecord
                .GetGymnasiumPointAccessEvents();
            var MainEntryPointAccessEvents = perDayWorkRecord
                .GetMainEntryPointAccessEvents();

            var RecreationAccessPointRecord = GetAccessPointRecord
                (RecreationPointAccessEvents, AccessPoint.Recreation);
            var GymnasiumAccessPointRecord = GetAccessPointRecord
                (GymnasiumPointAccessEvents, AccessPoint.Gymnasium);
            var MainEntryPointAccessPointRecord = GetAccessPointRecord
                (MainEntryPointAccessEvents, AccessPoint.MainEntry);

            var listOfAccessPointRecord = RecreationAccessPointRecord
                .Concat(GymnasiumAccessPointRecord)
                .Concat(MainEntryPointAccessPointRecord)
                .ToList();

            listOfAccessPointRecord.Sort((x, y) =>
                x.TimeIn.Hour.CompareTo(y.TimeIn.Hour) == 0 ?
                x.TimeIn.Minute.CompareTo(y.TimeIn.Minute)
                : x.TimeIn.Hour.CompareTo(y.TimeIn.Hour));

            return await Task.Run(() =>
            {
                return listOfAccessPointRecord;
            });
        }

        public bool AddRegularization(RegularizationDTO reguraliozationDTO)
        {
            if (GetRegularizationEntry(reguraliozationDTO.EmployeeID).Any())
            {
                return
                    _attendanceRegularizationRepository
                    .OverrideRegularizationRecord(reguraliozationDTO);
            }
            return
                _attendanceRegularizationRepository
                .SaveRegularizationRecord(reguraliozationDTO);
        }

        private List<Regularization> GetRegularizationEntry(int employeeId)
        {
            var regularizedData = _attendanceRegularizationRepository
                .GetRegularizedRecords(employeeId);
            return regularizedData;
        }
        private Regularization GetRegularizationEntryByDate(
            int employeeId, DateTime date)
        {
            Regularization regularizedDataOfADay = null;
            var listOfRegularizedData = _attendanceRegularizationRepository
                .GetRegularizedRecords(employeeId).ToList();

            if (listOfRegularizedData != null)
            {
                regularizedDataOfADay = listOfRegularizedData
                .Where(x => x.RegularizedDate().Date == date.Date)
                .FirstOrDefault();
            }

            return regularizedDataOfADay;
        }

        private List<AccessPointRecord> GetAccessPointRecord(
            List<AccessEvent> listOfAccessEvent, AccessPoint accessPoint)
        {
            var listOfaccessPointRecords = new List<AccessPointRecord>();
            for (int i = 0; i < listOfAccessEvent.Count; i += 2)
            {
                var timeIn = CalculateAbsoluteOutTimeAndInTime
                    (listOfAccessEvent[i].EventTime.TimeOfDay, AbsoluteTime.TimeIn);

                var timeOut =
                    i != listOfAccessEvent.Count - 1
                    ? CalculateAbsoluteOutTimeAndInTime
                        (listOfAccessEvent[i + 1].EventTime.TimeOfDay, AbsoluteTime.TimeOut)
                    : TimeSpan.Zero;

                var timeSpend =
                    (listOfAccessEvent.Count % 2) == 0
                        ? timeOut - timeIn
                        : TimeSpan.Zero;

                var accessPointRecord = new AccessPointRecord()
                {
                    TimeIn = new Time(timeIn.Hours, timeIn.Minutes),
                    TimeOut = new Time(timeOut.Hours, timeOut.Minutes),
                    TimeSpend = new Time(timeSpend.Hours, timeSpend.Minutes),
                    AccessPoint = accessPoint
                };

                listOfaccessPointRecords.Add(accessPointRecord);
            }
            return listOfaccessPointRecords;
        }

        private List<PerDayAttendanceRecordDTO> IncludeLeaves
        (List<PerDayAttendanceRecordDTO> listOfPerDayAttendanceRecordDTOs,
            List<Leave> listOfLeave, DateTime fromDate,
            DateTime toDate, int employeeId)
        {
            var accessEventAvailableDates = Enumerable
                .Select<PerDayAttendanceRecordDTO, DateTime>
                (listOfPerDayAttendanceRecordDTOs,
                    (Func<PerDayAttendanceRecordDTO, DateTime>)
                    (x => (DateTime)x.Date))
                .Distinct()
                .ToList();

            Employee employeeData = _employeeRepository.GetEmployee(employeeId);
            Department department = _departmentRepository
                .GetDepartment(employeeData.Department());

            AddLeaveRecord(listOfPerDayAttendanceRecordDTOs, listOfLeave, 
                fromDate, toDate, employeeId, accessEventAvailableDates, 
                department);

            listOfPerDayAttendanceRecordDTOs = Enumerable
                .OrderByDescending<PerDayAttendanceRecordDTO, DateTime>
                (listOfPerDayAttendanceRecordDTOs,
                (Func<PerDayAttendanceRecordDTO, DateTime>)(x => (DateTime)x.Date))
                .ToList();

            return listOfPerDayAttendanceRecordDTOs;
        }

        private void AddLeaveRecord(List<PerDayAttendanceRecordDTO> 
                listOfPerDayAttendanceRecordDTOs, List<Leave> listOfLeave,
            DateTime fromDate, DateTime toDate, int employeeId, 
            List<DateTime> accessEventAvailableDates, Department department)
        {
            for (var i = fromDate; i <= toDate; i = i.AddDays(1))
            {
                if (!accessEventAvailableDates.Any(x => x.Date.Date == i.Date.Date))
                {
                    if (department.IsValidWorkingDay(i.Date.Date) == true)
                    {
                        AddPerDayAttendanceRecord(listOfPerDayAttendanceRecordDTOs,
                            listOfLeave, employeeId, department, i);
                    }
                }
            }
        }

        private void AddPerDayAttendanceRecord(List<PerDayAttendanceRecordDTO> 
            listOfPerDayAttendanceRecordDTOs, List<Leave> listOfLeave, 
            int employeeId, Department department, DateTime i)
        {
            var leaveOfParticularDay = LeaveOfParticularDay(listOfLeave, i);
            var reguralizedEntry = GetRegularizationEntryByDate
                (employeeId, i.Date.Date);
            string remark = GetRemark(leaveOfParticularDay, reguralizedEntry);
            bool flag = IsRegularizedEntry(leaveOfParticularDay, reguralizedEntry);
            var dayStatus = GetDayStatus
                (leaveOfParticularDay, department
                .IsValidWorkingDay(i.Date.Date));
            var regularizedHours = GetRegularizedHours
                (reguralizedEntry, leaveOfParticularDay, department);
            var haveLeave = HaveLeave(leaveOfParticularDay);
            listOfPerDayAttendanceRecordDTOs.Add(new PerDayAttendanceRecordDTO()
            {
                Date = i,
                LateBy = new Time(0, 0),
                OverTime = new Time(0, 0),
                TimeIn = new Time(0, 0),
                TimeOut = new Time(0, 0),
                WorkingHours = new Time(0, 0),
                RegularizedHours = new Time
                    (regularizedHours.Hours, regularizedHours.Minutes),
                DayStatus = dayStatus,
                Remark = remark,
                IsHoursRegularized = flag,
                HaveLeave = haveLeave
            });
        }

        private Leave LeaveOfParticularDay(List<Leave> listOfLeave, DateTime fromDate)
        {
            if (listOfLeave != null)
            {
                return listOfLeave
                    .Where(x => x.GetLeaveDate().Contains(fromDate.Date.Date)
                                && (x.GetStatus() == Leave.StatusType.Approved
                                    || x.GetStatus() == Leave.StatusType.Updated))
                    .FirstOrDefault();
            }

            return null;
        }

        private List<PerDayAttendanceRecordDTO> CreatePerDayAttendanceRecord
            (int employeeId, IList<PerDayWorkRecord> workRecordByDate, List<Leave> listOfLeave)
        {
            var listOfPerDayAttendanceRecordDTO =
                new List<PerDayAttendanceRecordDTO>();
            var employeeData = _employeeRepository.
                GetEmployee(employeeId);
            var department = _departmentRepository.
                GetDepartment(employeeData.Department());

            foreach (var perDayWorkRecord in workRecordByDate)
            {
                TimeSpan timeIn, timeOut, workingHours, regularizedHours;
                bool flag, haveLeave;
                Time overTime, lateBy;
                string remark;
                DayStatus dayStatus;

                var leaveOfParticularDate = LeaveOfParticularDate(listOfLeave, perDayWorkRecord);

                AssignAttendanceRecordField(employeeId, department, perDayWorkRecord,
                    leaveOfParticularDate, out timeIn, out timeOut, out workingHours,
                    out regularizedHours, out flag, out overTime, out lateBy,
                    out haveLeave, out remark, out dayStatus);

                var attendanceRecord = AttendanceRecord(perDayWorkRecord, timeIn,
                    timeOut, workingHours, regularizedHours, flag, haveLeave, 
                    overTime, lateBy, remark, dayStatus);
                listOfPerDayAttendanceRecordDTO.Add(attendanceRecord);
            }
            return listOfPerDayAttendanceRecordDTO;
        }

        private Leave LeaveOfParticularDate(List<Leave> listOfLeave, PerDayWorkRecord perDayWorkRecord)
        {
            var leaveOfParticularDate =
                listOfLeave != null
                    ? listOfLeave
                        .Where(x => x.GetLeaveDate().Contains(perDayWorkRecord.Date)
                                    && (x.GetStatus() == Leave.StatusType.Approved
                                        || x.GetStatus() == Leave.StatusType.Updated))
                        .FirstOrDefault()
                    : null;
            return leaveOfParticularDate;
        }

        private static PerDayAttendanceRecordDTO AttendanceRecord(PerDayWorkRecord perDayWorkRecord, TimeSpan timeIn, TimeSpan timeOut, TimeSpan workingHours, TimeSpan regularizedHours, bool flag, bool haveLeave, Time overTime, Time lateBy, string remark, DayStatus dayStatus)
        {
            return new PerDayAttendanceRecordDTO()
            {
                Date = perDayWorkRecord.Date,
                TimeIn = new Time(timeIn.Hours, timeIn.Minutes),
                TimeOut = new Time(timeOut.Hours, timeOut.Minutes),
                WorkingHours = new Time
                    (workingHours.Hours, workingHours.Minutes),
                RegularizedHours = new Time
                    (regularizedHours.Hours, regularizedHours.Minutes),
                OverTime = overTime,
                LateBy = lateBy,
                DayStatus = dayStatus,
                Remark = remark,
                IsHoursRegularized = flag,
                HaveLeave = haveLeave
            };
        }

        private void AssignAttendanceRecordField(int employeeId, Department department, 
            PerDayWorkRecord perDayWorkRecord, Leave leaveOfParticularDate, out TimeSpan timeIn, 
            out TimeSpan timeOut, out TimeSpan workingHours, out TimeSpan regularizedHours, 
            out bool flag, out Time overTime, out Time lateBy, out bool haveLeave, 
            out string remark, out DayStatus dayStatus)
        {
            timeIn = perDayWorkRecord.GetTimeIn();
            timeOut = perDayWorkRecord.GetTimeOut();
            workingHours = perDayWorkRecord.CalculateWorkingHours();
            var isValidWorkingDay = department
                .IsValidWorkingDay(perDayWorkRecord.Date);
            var reguralizedEntry = GetRegularizationEntryByDate
                (employeeId, perDayWorkRecord.Date);
            regularizedHours = GetRegularizedHours
                (reguralizedEntry, leaveOfParticularDate, department);
            flag = IsRegularizedEntry(leaveOfParticularDate, reguralizedEntry);
            overTime = GetOverTime(isValidWorkingDay, workingHours, regularizedHours,
                   flag, department.GetNoOfHoursToBeWorked());
            lateBy = GetLateByTime(isValidWorkingDay, workingHours, regularizedHours,
                   flag, department.GetNoOfHoursToBeWorked());
            haveLeave = HaveLeave(leaveOfParticularDate);
            remark = GetRemark(leaveOfParticularDate, reguralizedEntry);
            dayStatus = GetDayStatus(leaveOfParticularDate, isValidWorkingDay);
        }

        private Time GetOverTime(bool isValidWorkingDay, TimeSpan workingHours,
            TimeSpan regularizedHours, bool flag, double noOfHoursToBeWorked)
        {
            Time extraHour;
            if (isValidWorkingDay == true)
            {
                if (flag == false)
                {
                    extraHour = GetExtraHours(workingHours + regularizedHours,
                        noOfHoursToBeWorked);
                }
                else
                {
                    extraHour = GetExtraHours(workingHours, noOfHoursToBeWorked);
                }
                if (extraHour.Hour > 0 || extraHour.Minute > 0)
                {
                    return extraHour;
                }
                return new Time(0, 0);
            }
            else
            {
                return new Time(workingHours.Hours, workingHours.Minutes);
            }

        }
        private Time GetLateByTime(bool isValidWorkingDay, TimeSpan workingHours,
            TimeSpan regularizedHours, bool flag, double noOfHoursToBeWorked)
        {
            if (isValidWorkingDay == true)
            {
                var extraHour = flag == false
                    ? GetExtraHours
                        (workingHours + regularizedHours, noOfHoursToBeWorked)
                    : GetExtraHours(workingHours, noOfHoursToBeWorked);

                if (extraHour.Hour < 0 || extraHour.Minute < 0)
                {
                    int latebyHours = Math.Abs(extraHour.Hour);
                    int latebyMinutes = Math.Abs(extraHour.Minute);
                    return new Time(latebyHours, latebyMinutes);
                }
            }
            return new Time(0, 0);
        }

        private Time GetExtraHours(TimeSpan workingHours, 
            double noOfHoursToBeWorked)
        {
            var TotalWorkingHours = TimeSpan.FromHours(noOfHoursToBeWorked);
            var extraHour = workingHours - TotalWorkingHours;
            return new Time(extraHour.Hours, extraHour.Minutes);
        }

        private bool HaveLeave(Leave leaveOfParticularDate)
        {
            if (leaveOfParticularDate != null)
            {
                return true;
            }
            return false;
        }
        private DayStatus GetDayStatus(Leave leaveOfParticularDay,
            bool isValidWorkingDay)
        {
            if (leaveOfParticularDay != null)
            {
                if (leaveOfParticularDay.IsHalfDayLeave() == true)
                    return DayStatus.HalfDayLeave;
                else
                    return DayStatus.Leave;
            }
            else
            {
                if (isValidWorkingDay == true)
                    return DayStatus.WorkingDay;
                else
                    return DayStatus.NonWorkingDay;
            }
        }
        private string GetRemark(Leave leaveOfParticularDate, Regularization reguralizedEntry)
        {
            if (leaveOfParticularDate != null)
            {
                var leaveType = leaveOfParticularDate.GetLeaveType();
                return 
                    leaveOfParticularDate.IsHalfDayLeave() == true
                    ? string
                        .Concat(EnumHelperMethod.EnumDisplayNameFor(leaveType)
                                .ToString(),
                            " - Half Day")
                    : EnumHelperMethod.EnumDisplayNameFor(leaveType).ToString();
            }
                if (reguralizedEntry != null)
                {
                    return reguralizedEntry.GetRemark();
                }

                return null;
        }

        private TimeSpan GetRegularizedHours(Regularization reguralizedEntry,
            Leave leaveOfParticularDate, Department department)
        {
            var regularizedHours = TimeSpan.Zero;
            if (leaveOfParticularDate != null)
            {
                regularizedHours = leaveOfParticularDate.IsHalfDayLeave() == true
                    ? TimeSpan.FromHours(department.GetNoOfHoursToBeWorked() / 2)
                    : TimeSpan.FromHours(department.GetNoOfHoursToBeWorked());
            }
            else
            {
                if (reguralizedEntry != null)
                {
                    regularizedHours = reguralizedEntry.GetRegularizedHours();
                }
            }
            return regularizedHours;
        }

        private bool IsRegularizedEntry(Leave leaveOfParticularDate,
            Regularization reguralizedEntry)
        {
            bool isRegularized = false;
            if (leaveOfParticularDate == null)
            {
                if (reguralizedEntry != null)
                {
                    isRegularized = true;
                }
            }
            return isRegularized;
        }

        private Time CalculateDeficiateOrExtraTime(
            List<PerDayAttendanceRecordDTO> listOfAttendanceRecordDTO,
            double noOfHoursToBeWorked)
        {
            if (listOfAttendanceRecordDTO.Count == 0)
            {
                return new Time(00, 00);
            }

            double totalRequiredHoursToBeWorked = listOfAttendanceRecordDTO
                .Count(x => x.DayStatus != DayStatus.NonWorkingDay) * noOfHoursToBeWorked;
            Time totalWorkedTime = CalculateTotalWorkingHours
                (listOfAttendanceRecordDTO);
            var totalWorkedSpan = new TimeSpan
                (totalWorkedTime.Hour, totalWorkedTime.Minute, 00);

            double totalDefiateOrOverTimeHrs =
                totalWorkedSpan.TotalHours - totalRequiredHoursToBeWorked;

            var extraTime = TimeSpan.FromHours(totalDefiateOrOverTimeHrs);

            return new Time(
                (int)extraTime.TotalHours,
                (int)extraTime.Minutes);

        }
        private Time CalculateEstimatedHours
                 (List<PerDayAttendanceRecordDTO> listOfPerDayAttendanceRecord,
            double noOfHoursToBeWorked)
        {
            if (listOfPerDayAttendanceRecord.Count == 0)
            {
                return new Time(00, 00);
            }
            double totalRequiredHoursToBeWorked = listOfPerDayAttendanceRecord
              .Count(x => x.DayStatus != DayStatus.NonWorkingDay) * noOfHoursToBeWorked;
            return new Time((int)totalRequiredHoursToBeWorked, 00);
        }

        private Time CalculateTotalWorkingHours
            (List<PerDayAttendanceRecordDTO> listOfAttendanceRecordDTO)
        {
            if (listOfAttendanceRecordDTO.Count == 0)
            {
                return new Time(00, 00);
            }

            var sumOfTotalWorkingHours = TimeSpan.Zero;

            foreach (var AttendanceRecordDTO in listOfAttendanceRecordDTO)
            {
                sumOfTotalWorkingHours = SumOfTotalWorkingHours(
                    sumOfTotalWorkingHours, AttendanceRecordDTO);
            }

            return new Time(
                (int)sumOfTotalWorkingHours.TotalHours,
                (int)sumOfTotalWorkingHours.Minutes);
        }

        private static TimeSpan SumOfTotalWorkingHours(TimeSpan sumOfTotalWorkingHours,
            PerDayAttendanceRecordDTO AttendanceRecordDTO)
        {
            if (AttendanceRecordDTO.HaveLeave == true)
            {
               return sumOfTotalWorkingHours = WorkingHoursForLeave(
                    sumOfTotalWorkingHours, AttendanceRecordDTO);
            }
            switch (AttendanceRecordDTO.IsHoursRegularized)
            {
                case true:
                    {
                        var regularizedHours = AttendanceRecordDTO
                            .RegularizedHours;
                        sumOfTotalWorkingHours +=
                            new TimeSpan(regularizedHours.Hour,
                            regularizedHours.Minute,
                            00);
                        break;
                    }

                default:
                    {
                        var workingHours = AttendanceRecordDTO.WorkingHours;
                        sumOfTotalWorkingHours += new TimeSpan
                            (workingHours.Hour, workingHours.Minute, 00);
                        break;
                    }
            }
            return sumOfTotalWorkingHours;
        }

        private static TimeSpan WorkingHoursForLeave(TimeSpan sumOfTotalWorkingHours,
            PerDayAttendanceRecordDTO AttendanceRecordDTO)
        {
            var workingHours = new TimeSpan
                (AttendanceRecordDTO.WorkingHours.Hour,
                AttendanceRecordDTO.WorkingHours.Minute,
                00);
            var regularizedHours = new TimeSpan
                (AttendanceRecordDTO.RegularizedHours.Hour,
                AttendanceRecordDTO.RegularizedHours.Minute,
                00);
            sumOfTotalWorkingHours += workingHours + regularizedHours;
            return sumOfTotalWorkingHours;
        }

        private enum AbsoluteTime
        {
            TimeIn,
            TimeOut
        }
        private TimeSpan CalculateAbsoluteOutTimeAndInTime(TimeSpan timeSpan, AbsoluteTime time)
        {
            if (time == AbsoluteTime.TimeOut && timeSpan.Seconds > 0)
            {
                return new TimeSpan(timeSpan.Hours, timeSpan.Minutes + 1, 00);
            }

            return new TimeSpan(timeSpan.Hours, timeSpan.Minutes, 00);
        }

    }
}
