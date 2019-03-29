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
        private ICompanyHolidayRepository _companyHolidayRepository;

        public AttendanceService(
            IAccessEventsRepository accessEventsRepository,
            IEmployeeRepository employeeRepository,
            IDepartmentRepository departmentRepository,
            IAttendanceRegularizationRepository attendanceRegularizationRepository,
            ILeavesRepository leavesRepository,
            ICompanyHolidayRepository companyHolidayRepository)
        {
            _accessEventsRepository = accessEventsRepository;
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _attendanceRegularizationRepository = attendanceRegularizationRepository;
            _leavesRepository = leavesRepository;
            _companyHolidayRepository = companyHolidayRepository;
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

            listOfPerDayAttendanceRecord = IncludeMissingEntry(
                listOfPerDayAttendanceRecord, listOfLeave, 
                fromDate, toDate, employeeId);

            var employeeData = _employeeRepository
                .GetEmployee(employeeId);

            var department = _departmentRepository
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
            return GetRegularizationEntry(reguraliozationDTO.EmployeeID).Any()
                ? _attendanceRegularizationRepository
                    .OverrideRegularizationRecord(reguraliozationDTO)
                : _attendanceRegularizationRepository
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
            var listOfRegularizedData = _attendanceRegularizationRepository
                .GetRegularizedRecords(employeeId).ToList();

            return listOfRegularizedData != null
                ? listOfRegularizedData
                .Where(x => x.RegularizedDate().Date == date.Date)
                .FirstOrDefault()
                : null;
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

        private List<PerDayAttendanceRecordDTO> IncludeMissingEntry
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

            AddMissingEntryRecord(listOfPerDayAttendanceRecordDTOs, listOfLeave, 
                fromDate, toDate, employeeId, accessEventAvailableDates, 
                department);

            listOfPerDayAttendanceRecordDTOs = Enumerable
                .OrderByDescending<PerDayAttendanceRecordDTO, DateTime>
                (listOfPerDayAttendanceRecordDTOs,
                (Func<PerDayAttendanceRecordDTO, DateTime>)(x => (DateTime)x.Date))
                .ToList();

            return listOfPerDayAttendanceRecordDTOs;
        }

        private void AddMissingEntryRecord(List<PerDayAttendanceRecordDTO> 
                listOfPerDayAttendanceRecordDTOs, List<Leave> listOfLeave,
            DateTime fromDate, DateTime toDate, int employeeId, 
            List<DateTime> accessEventAvailableDates, Department department)
        {
            var listOfCompanyHoliday = _companyHolidayRepository.Holidays();

            for (var i = fromDate; i <= toDate; i = i.AddDays(1))
            {
                if (!accessEventAvailableDates.Any(x => x.Date.Date == i.Date.Date)
                && department.IsValidWorkingDay(i.Date.Date) == true)
                {
                        AddPerDayAttendanceRecord(listOfPerDayAttendanceRecordDTOs,
                            listOfLeave, listOfCompanyHoliday, employeeId, department, i);
                }
            }
        }

        private void AddPerDayAttendanceRecord(List<PerDayAttendanceRecordDTO> 
            listOfPerDayAttendanceRecordDTOs, List<Leave> listOfLeave,
            List<Holiday> listOfCompanyHoliday,
            int employeeId, Department department, DateTime i)
        {
            var leaveOfParticularDay = LeaveOfParticularDay(listOfLeave, i);
            var companyHolidayOfParticularDay = 
                CompanyHolidayOfParticularDay(listOfCompanyHoliday,i);

            var reguralizedEntry = GetRegularizationEntryByDate
                (employeeId, i.Date.Date);

            string remark = GetRemark(leaveOfParticularDay, 
                reguralizedEntry,
                companyHolidayOfParticularDay);

            bool flag = IsRegularizedEntry(leaveOfParticularDay, reguralizedEntry);
            var dayStatus = GetDayStatus
                (leaveOfParticularDay, department
                .IsValidWorkingDay(i.Date.Date), companyHolidayOfParticularDay);
            var regularizedHours = GetRegularizedHours
                (reguralizedEntry, leaveOfParticularDay, department,null);
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

        private Leave LeaveOfParticularDay(List<Leave> listOfLeave, DateTime date)
        {
            return listOfLeave != null
                ? listOfLeave
                    .Where(x => x.GetLeaveDate().Contains(date.Date.Date)
                                && (x.GetStatus() == Leave.StatusType.Approved
                                    || x.GetStatus() == Leave.StatusType.Updated))
                    .FirstOrDefault()
                : null;
         }

        private Holiday CompanyHolidayOfParticularDay(
            List<Holiday> listOfCompanyHoliday,DateTime date)
        {
            var holidayOfParticularDate = listOfCompanyHoliday
                    .Where(x => x.Date() == date)
                    .FirstOrDefault();

            return holidayOfParticularDate != null 
                ? holidayOfParticularDate 
                : null;
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

                var leaveOfParticularDate = 
                    LeaveOfParticularDate(listOfLeave, perDayWorkRecord);

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

        private Leave LeaveOfParticularDate(List<Leave> listOfLeave,
            PerDayWorkRecord perDayWorkRecord)
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

        private PerDayAttendanceRecordDTO AttendanceRecord(
            PerDayWorkRecord perDayWorkRecord, TimeSpan timeIn,
            TimeSpan timeOut, TimeSpan workingHours, TimeSpan regularizedHours,
            bool flag, bool haveLeave, Time overTime, Time lateBy,
            string remark, DayStatus dayStatus)
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
                (reguralizedEntry, leaveOfParticularDate, department,null);

            flag = IsRegularizedEntry(leaveOfParticularDate, reguralizedEntry);
            overTime = GetOverTime(isValidWorkingDay, workingHours, regularizedHours,
                   flag, department.GetNoOfHoursToBeWorked());

            lateBy = GetLateByTime(isValidWorkingDay, workingHours, regularizedHours,
                   flag, department.GetNoOfHoursToBeWorked());

            haveLeave = HaveLeave(leaveOfParticularDate);
            remark = GetRemark(leaveOfParticularDate,reguralizedEntry, null);
            dayStatus = GetDayStatus(leaveOfParticularDate, isValidWorkingDay,null);
        }

        private Time GetOverTime(bool isValidWorkingDay, TimeSpan workingHours,
            TimeSpan regularizedHours, bool flag, double noOfHoursToBeWorked)
        {
            if (isValidWorkingDay == true)
            {
               var extraHour = flag == false
                    ? GetExtraHours(workingHours + regularizedHours,
                        noOfHoursToBeWorked)
                    : GetExtraHours(workingHours, noOfHoursToBeWorked);

                return 
                    extraHour.Hour > 0 || extraHour.Minute > 0
                    ? extraHour
                    : new Time(0, 0);
            }
                return new Time(workingHours.Hours, workingHours.Minutes);
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
            return 
                leaveOfParticularDate != null ? true : false;
        }

        private DayStatus GetDayStatus(Leave leaveOfParticularDay,
            bool isValidWorkingDay,Holiday companyHoliday)
        {
            return companyHoliday != null
                ? DayStatus.Holiday
                : leaveOfParticularDay != null
                    ? leaveOfParticularDay.IsHalfDayLeave() == true
                       ? DayStatus.HalfDayLeave
                       : DayStatus.Leave
                    : isValidWorkingDay == true
                        ? DayStatus.WorkingDay
                        : DayStatus.NonWorkingDay;
        }

        private string GetRemark(Leave leaveOfParticularDate,
            Regularization reguralizedEntry,
            Holiday companyHolidayOfParticularDay)
        {
            if (companyHolidayOfParticularDay != null)
                return companyHolidayOfParticularDay.Name();
            if (leaveOfParticularDate != null)
            {
                var leaveType = leaveOfParticularDate.GetLeaveType();

                return leaveOfParticularDate.IsHalfDayLeave() == true
                    ? string
                        .Concat(EnumHelperMethod.EnumDisplayNameFor(leaveType)
                                .ToString(),
                            " - Half Day")
                    : EnumHelperMethod.EnumDisplayNameFor(leaveType).ToString();
            }

            return reguralizedEntry != null 
                ? reguralizedEntry.GetRemark() 
                : null;
        }

        private TimeSpan GetRegularizedHours(Regularization reguralizedEntry,
            Leave leaveOfParticularDate, Department department, Holiday companyHoliday)
        {
            return (leaveOfParticularDate != null && companyHoliday == null)
                ? leaveOfParticularDate.IsHalfDayLeave() == true
                    ? TimeSpan.FromHours(department.GetNoOfHoursToBeWorked() / 2)
                    : TimeSpan.FromHours(department.GetNoOfHoursToBeWorked())
                : reguralizedEntry != null
                ? (reguralizedEntry.GetRegularizedHours())
                : TimeSpan.Zero;
        }

        private bool IsRegularizedEntry(Leave leaveOfParticularDate,
            Regularization reguralizedEntry)
        {
            return leaveOfParticularDate == null && reguralizedEntry != null 
                ? true 
                : false;
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
                .Count(x => x.DayStatus != DayStatus.NonWorkingDay
                            && x.DayStatus!= DayStatus.Holiday) * noOfHoursToBeWorked;

            var totalWorkedTime = CalculateTotalWorkingHours
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
        private Time CalculateEstimatedHours(
            List<PerDayAttendanceRecordDTO> listOfPerDayAttendanceRecord,
            double noOfHoursToBeWorked)
        {
            if (listOfPerDayAttendanceRecord.Count == 0)
                return new Time(00, 00);
           
            double totalRequiredHoursToBeWorked = listOfPerDayAttendanceRecord
              .Count(x => x.DayStatus != DayStatus.NonWorkingDay && 
                          x.DayStatus!= DayStatus.Holiday) 
                                                  * noOfHoursToBeWorked;

            return new Time((int)totalRequiredHoursToBeWorked, 00);
        }

        private Time CalculateTotalWorkingHours(
            List<PerDayAttendanceRecordDTO> listOfAttendanceRecordDTO)
        {
            if (listOfAttendanceRecordDTO.Count == 0)
                return new Time(00, 00);

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

        private TimeSpan SumOfTotalWorkingHours(TimeSpan sumOfTotalWorkingHours,
            PerDayAttendanceRecordDTO AttendanceRecordDTO)
        {
            if (AttendanceRecordDTO.HaveLeave == true)
               return sumOfTotalWorkingHours = WorkingHoursForLeave(
                    sumOfTotalWorkingHours, AttendanceRecordDTO);
           
            switch (AttendanceRecordDTO.IsHoursRegularized)
            {
                case true:
                    {
                        var regularizedHours = AttendanceRecordDTO
                            .RegularizedHours;
                        return sumOfTotalWorkingHours +=
                            new TimeSpan(regularizedHours.Hour,
                            regularizedHours.Minute,
                            00);
                    }

                default:
                    {
                        var workingHours = AttendanceRecordDTO.WorkingHours;
                        return sumOfTotalWorkingHours += new TimeSpan
                            (workingHours.Hour, workingHours.Minute, 00);
                    }
            }
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

        private TimeSpan CalculateAbsoluteOutTimeAndInTime(
            TimeSpan timeSpan, AbsoluteTime time)
        {
            return time == AbsoluteTime.TimeOut && timeSpan.Seconds > 0
                ? new TimeSpan(timeSpan.Hours, timeSpan.Minutes + 1, 00)
                : new TimeSpan(timeSpan.Hours, timeSpan.Minutes, 00);
        }
    }
}
