using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainModel;
using UseCaseBoundary;
using UseCaseBoundary.DTO;
using UseCaseBoundary.Model;
using Microsoft.AspNetCore.Html;

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
            IAttendanceRegularizationRepository attendanceRegularizationRepository,ILeavesRepository leavesRepository)
        {
            _accessEventsRepository = accessEventsRepository;
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _attendanceRegularizationRepository = attendanceRegularizationRepository;
            _leavesRepository = leavesRepository;
        }

        public AttendanceRecordsDTO AttendanceReportForDateRange(int employeeId, DateTime fromDate, DateTime toDate)
        {
            WorkLogs accessEvents = _accessEventsRepository.GetAccessEventsForDateRange(employeeId, fromDate, toDate);
            var datewiseAccessEvents = accessEvents.GetAllAccessEvents();

            var listOfLeave = _leavesRepository.GetAllLeavesInfo(employeeId);

            List<PerDayAttendanceRecordDTO> listOfPerDayAttendanceRecord =
                CreatePerDayAttendanceRecord(employeeId,datewiseAccessEvents,  listOfLeave);

            listOfPerDayAttendanceRecord = IncludeHolidays
                (listOfPerDayAttendanceRecord, listOfLeave, fromDate, toDate, employeeId);

            Employee employeeData = _employeeRepository.GetEmployee(employeeId);
            Department department = _departmentRepository.GetDepartment(employeeData.Department());

            return new AttendanceRecordsDTO
            {
                ListOfAttendanceRecordDTO = listOfPerDayAttendanceRecord,
                TotalWorkingHours = CalculateTotalWorkingHours(listOfPerDayAttendanceRecord),
                EstimatedHours = CalculateEstimatedHours
                 (listOfPerDayAttendanceRecord, department.GetNoOfHoursToBeWorked()),
                TotalDeficitOrExtraHours = CalculateDeficiateOrExtraTime
                 (listOfPerDayAttendanceRecord, department.GetNoOfHoursToBeWorked())
            };
        }

        public async Task<List<AccessPointRecord>> GetAccessPointDetails(int employeeId, DateTime date)
        {
            PerDayWorkRecord perDayWorkRecord = _accessEventsRepository.GetAccessEventsForADay(employeeId, date);

            var RecreationPointAccessEvents = perDayWorkRecord.GetRecreationPointAccessEvents();
            var GymnasiumPointAccessEvents = perDayWorkRecord.GetGymnasiumPointAccessEvents();
            var MainEntryPointAccessEvents = perDayWorkRecord.GetMainEntryPointAccessEvents();

            List<AccessPointRecord> RecreationAccessPointRecord = GetAccessPointRecord
                (RecreationPointAccessEvents, AccessPoint.Recreation);
            List<AccessPointRecord> GymnasiumAccessPointRecord = GetAccessPointRecord
                (GymnasiumPointAccessEvents, AccessPoint.Gymnasium);
            List<AccessPointRecord> MainEntryPointAccessPointRecord = GetAccessPointRecord
                (MainEntryPointAccessEvents, AccessPoint.MainEntry);

            List<AccessPointRecord> listOfAccessPointRecord = RecreationAccessPointRecord
                .Concat(GymnasiumAccessPointRecord)
                .Concat(MainEntryPointAccessPointRecord)
                .ToList();

            listOfAccessPointRecord.Sort((x, y) =>
                x.TimeIn.Hour.CompareTo(y.TimeIn.Hour) == 0 ?
                x.TimeIn.Minute.CompareTo(y.TimeIn.Minute) : x.TimeIn.Hour.CompareTo(y.TimeIn.Hour));

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
        private Regularization GetRegularizationEntryByDate(int employeeId, DateTime date)
        {
            Regularization regularizedDataOfADay = null;
            var listOfRegularizedData = _attendanceRegularizationRepository
                .GetRegularizedRecords(employeeId).ToList();
            
            if (listOfRegularizedData!=null)
            {
                regularizedDataOfADay = listOfRegularizedData
                .Where(x => x.RegularizedDate().Date == date.Date)
                .FirstOrDefault();
            }
            
            return regularizedDataOfADay;
        }

        private List<AccessPointRecord> GetAccessPointRecord(List<AccessEvent> listOfAccessEvent, AccessPoint accessPoint)
        {
            List<AccessPointRecord> listOfaccessPointRecords = new List<AccessPointRecord>();
            for (int i = 0; i < listOfAccessEvent.Count; i += 2)
            {
                var timeIn = CalculateAbsoluteOutTimeAndInTime
                    (listOfAccessEvent[i].EventTime.TimeOfDay, AbsoluteTime.TimeIn);
                var timeOut = TimeSpan.Zero;
                if (i != listOfAccessEvent.Count - 1)
                {
                    timeOut = CalculateAbsoluteOutTimeAndInTime
                        (listOfAccessEvent[i + 1].EventTime.TimeOfDay, AbsoluteTime.TimeOut); 
                }
                var timeSpend = TimeSpan.Zero;
                if ((listOfAccessEvent.Count % 2) == 0)
                {
                    timeSpend = (timeOut - timeIn);
                }
                AccessPointRecord accessPointRecord = new AccessPointRecord()
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

        private List<PerDayAttendanceRecordDTO> IncludeHolidays(List<PerDayAttendanceRecordDTO>
            listOfPerDayAttendanceRecordDTOs,List<Leave> listOfLeave, DateTime fromDate, DateTime toDate, int employeeId)
        {
            var accessEventAvailableDates = Enumerable
                .Select<PerDayAttendanceRecordDTO, DateTime>
                (listOfPerDayAttendanceRecordDTOs, (Func<PerDayAttendanceRecordDTO, DateTime>)(x => (DateTime)x.Date))
                .Distinct()
                .ToList();

            Employee employeeData = _employeeRepository.GetEmployee(employeeId);
            Department department = _departmentRepository.GetDepartment(employeeData.Department());

            for (var i = fromDate; i <= toDate; i = i.AddDays(1))
            {
                if (!accessEventAvailableDates.Any(x => x.Date.Date == i.Date.Date))
                {
                    if (department.IsValidWorkingDay(i.Date.Date) == true)
                    {
                        Leave leaveOfParticularDay = null;
                        if (listOfLeave != null)
                        {
                            leaveOfParticularDay = listOfLeave
                            .Where(x => x.GetLeaveDate().Contains(i.Date.Date)
                            && x.GetStatus() != Leave.StatusType.Cancelled)
                            .FirstOrDefault();
                        }
                        var reguralizedEntry = GetRegularizationEntryByDate(employeeId, i.Date.Date);
                        string remark = GetRemark(leaveOfParticularDay, reguralizedEntry);
                        bool flag = IsRegularizedEntry(leaveOfParticularDay, reguralizedEntry);
                        var dayStatus = GetDayStatus(leaveOfParticularDay, department.IsValidWorkingDay(i.Date.Date));
                        var regularizedHours = GetRegularizedHours(reguralizedEntry, leaveOfParticularDay, department);
                        var haveLeave = HaveLeave(leaveOfParticularDay);
                        listOfPerDayAttendanceRecordDTOs.Add(new PerDayAttendanceRecordDTO()
                        {
                            Date = i,
                            LateBy = new Time(0, 0),
                            OverTime = new Time(0, 0),
                            TimeIn = new Time(0, 0),
                            TimeOut = new Time(0, 0),
                            WorkingHours = new Time(0, 0),
                            RegularizedHours = new Time(regularizedHours.Hours, regularizedHours.Minutes),
                            DayStatus = dayStatus,
                            Remark = remark,
                            IsHoursRegularized = flag,
                            HaveLeave = haveLeave
                        });
                    }
                }
            }
            listOfPerDayAttendanceRecordDTOs = Enumerable
                .OrderByDescending<PerDayAttendanceRecordDTO, DateTime>
                (listOfPerDayAttendanceRecordDTOs, 
                (Func<PerDayAttendanceRecordDTO, DateTime>)(x => (DateTime)x.Date))
                .ToList();

            return listOfPerDayAttendanceRecordDTOs;
        }

        private List<PerDayAttendanceRecordDTO> CreatePerDayAttendanceRecord
            (int employeeId,IList<PerDayWorkRecord> workRecordByDate,List<Leave> listOfLeave)
        {
            List<PerDayAttendanceRecordDTO> listOfPerDayAttendanceRecordDTO = new List<PerDayAttendanceRecordDTO>();
            Employee employeeData = _employeeRepository.GetEmployee(employeeId);
            Department department = _departmentRepository.GetDepartment(employeeData.Department());

            foreach (var perDayWorkRecord in workRecordByDate)
            {
                Leave leaveOfParticularDate = null;
                if (listOfLeave != null)
                {
                    leaveOfParticularDate = listOfLeave
                    .Where(x => x.GetLeaveDate().Contains(perDayWorkRecord.Date)
                    && x.GetStatus() != Leave.StatusType.Cancelled)
                    .FirstOrDefault();
                }
                
                var timeIn = perDayWorkRecord.GetTimeIn();
                var timeOut = perDayWorkRecord.GetTimeOut();
                var workingHours = perDayWorkRecord.CalculateWorkingHours();
                var isValidWorkingDay = department.IsValidWorkingDay(perDayWorkRecord.Date);
                var reguralizedEntry = GetRegularizationEntryByDate(employeeId, perDayWorkRecord.Date);
                var regularizedHours = GetRegularizedHours(reguralizedEntry, leaveOfParticularDate, department);
                bool flag = IsRegularizedEntry(leaveOfParticularDate, reguralizedEntry);
                var overTime = GetOverTime
                    (isValidWorkingDay,workingHours, regularizedHours,flag, department.GetNoOfHoursToBeWorked());
                var lateBy = GetLateByTime
                    (isValidWorkingDay,workingHours, regularizedHours, flag, department.GetNoOfHoursToBeWorked());
                var haveLeave = HaveLeave(leaveOfParticularDate);
                string remark = GetRemark(leaveOfParticularDate, reguralizedEntry);
                var dayStatus = GetDayStatus(leaveOfParticularDate, isValidWorkingDay);
               
                PerDayAttendanceRecordDTO attendanceRecord = new PerDayAttendanceRecordDTO()
                {
                    Date = perDayWorkRecord.Date,
                    TimeIn = new Time(timeIn.Hours, timeIn.Minutes),
                    TimeOut = new Time(timeOut.Hours, timeOut.Minutes),
                    WorkingHours = new Time(workingHours.Hours, workingHours.Minutes),
                    RegularizedHours = new Time(regularizedHours.Hours, regularizedHours.Minutes),
                    OverTime = overTime,
                    LateBy = lateBy,
                    DayStatus = dayStatus,
                    Remark = remark,
                    IsHoursRegularized = flag,
                    HaveLeave = haveLeave
                };
                listOfPerDayAttendanceRecordDTO.Add(attendanceRecord);
            }
            return listOfPerDayAttendanceRecordDTO;
        }

        private Time GetOverTime(bool isValidWorkingDay,TimeSpan workingHours, 
            TimeSpan regularizedHours,bool flag, double noOfHoursToBeWorked)
        {
            Time extraHour ;
            if (isValidWorkingDay == true)
            {
                if (flag == false)
                {
                    extraHour = GetExtraHours(workingHours + regularizedHours, noOfHoursToBeWorked);
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
        private Time GetLateByTime(bool isValidWorkingDay,TimeSpan workingHours,
            TimeSpan regularizedHours,bool flag, double noOfHoursToBeWorked)
        {
            Time extraHour;
            if (isValidWorkingDay == true)
            {
                if (flag == false)
                {
                    extraHour = GetExtraHours(workingHours + regularizedHours, noOfHoursToBeWorked);
                }
                else
                {
                    extraHour = GetExtraHours(workingHours, noOfHoursToBeWorked);
                }

                if (extraHour.Hour < 0 || extraHour.Minute < 0)
                {
                    int latebyHours = Math.Abs(extraHour.Hour);
                    int latebyMinutes = Math.Abs(extraHour.Minute);
                    return new Time(latebyHours, latebyMinutes);
                }
                return new Time(0, 0);
            }
            else
            {
                return new Time(0, 0);
            }
            
        }
        private Time GetExtraHours(TimeSpan workingHours, double noOfHoursToBeWorked)
        {
            TimeSpan TotalWorkingHours = TimeSpan.FromHours(noOfHoursToBeWorked);
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
        private DayStatus GetDayStatus(Leave leaveOfParticularDay,bool isValidWorkingDay)
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
        private string GetRemark(Leave leaveOfParticularDate,Regularization reguralizedEntry)
        {
            string remark=null;
            if (leaveOfParticularDate != null)
            {
                var leaveType = leaveOfParticularDate.GetLeaveType();
                if(leaveOfParticularDate.IsHalfDayLeave()==true)
                remark = string.Concat(EnumHelperMethod.EnumDisplayNameFor(leaveType).ToString(), " - Half Day");
                else
                    remark = EnumHelperMethod.EnumDisplayNameFor(leaveType).ToString();
            }
            else
            {
                if (reguralizedEntry != null)
                {
                    remark = reguralizedEntry.GetRemark();
                }
            }
            return remark;
        }

        private TimeSpan GetRegularizedHours(Regularization reguralizedEntry,
            Leave leaveOfParticularDate,Department department)
        {
            TimeSpan regularizedHours=TimeSpan.Zero;
            if (leaveOfParticularDate != null)
            {
                if(leaveOfParticularDate.IsHalfDayLeave()==true)
                 regularizedHours = TimeSpan.FromHours(department.GetNoOfHoursToBeWorked() / 2);
                else
                    regularizedHours = TimeSpan.FromHours(department.GetNoOfHoursToBeWorked());
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

        private bool IsRegularizedEntry(Leave leaveOfParticularDate,Regularization reguralizedEntry)
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
            Time totalWorkedTime = CalculateTotalWorkingHours(listOfAttendanceRecordDTO);
            var totalWorkedSpan = new TimeSpan(totalWorkedTime.Hour, totalWorkedTime.Minute, 00);

            double totalDefiateOrOverTimeHrs = totalWorkedSpan.TotalHours - totalRequiredHoursToBeWorked;

            var extraTime = TimeSpan.FromHours(totalDefiateOrOverTimeHrs);

            return new Time(
                (int)extraTime.TotalHours,
                Math.Abs((int)extraTime.Minutes));

        }
        private Time CalculateEstimatedHours
                 (List<PerDayAttendanceRecordDTO> listOfPerDayAttendanceRecord,double noOfHoursToBeWorked)
        {
            if (listOfPerDayAttendanceRecord.Count == 0)
            {
                return new Time(00, 00);
            }
             double totalRequiredHoursToBeWorked = listOfPerDayAttendanceRecord
               .Count(x => x.DayStatus != DayStatus.NonWorkingDay) * noOfHoursToBeWorked;
            return new Time ((int)totalRequiredHoursToBeWorked,00);
        }

        private Time CalculateTotalWorkingHours(List<PerDayAttendanceRecordDTO> listOfAttendanceRecordDTO)
        {
            if (listOfAttendanceRecordDTO.Count == 0)
            {
                return new Time(00, 00);
            }

            var sumOfTotalWorkingHours = TimeSpan.Zero;

            foreach (var AttendanceRecordDTO in listOfAttendanceRecordDTO)
            {
                    if (AttendanceRecordDTO.HaveLeave==true)
                    {
                        var workingHours = new TimeSpan
                            (AttendanceRecordDTO.WorkingHours.Hour, AttendanceRecordDTO.WorkingHours.Minute, 00); 
                        var regularizedHours = new TimeSpan
                            (AttendanceRecordDTO.RegularizedHours.Hour, AttendanceRecordDTO.RegularizedHours.Minute, 00); 
                        sumOfTotalWorkingHours += workingHours + regularizedHours;
                    }
                    else
                    {
                        if (AttendanceRecordDTO.IsHoursRegularized == true)
                        {
                            var regularizedHours = AttendanceRecordDTO.RegularizedHours;
                            sumOfTotalWorkingHours += new TimeSpan(regularizedHours.Hour, regularizedHours.Minute, 00);
                        }
                    else
                    {
                        var workingHours = AttendanceRecordDTO.WorkingHours;
                        sumOfTotalWorkingHours += new TimeSpan(workingHours.Hour, workingHours.Minute, 00);
                    }
                    }
            }
           
            return new Time(
                (int)sumOfTotalWorkingHours.TotalHours,
                (int)sumOfTotalWorkingHours.Minutes);
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
