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
        public AttendanceService(IAccessEventsRepository accessEventsRepository, IEmployeeRepository employeeRepository)
        {
            _accessEventsRepository = accessEventsRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task<AttendanceRecordsDTO> GetAttendanceRecord(int employeeId, int noOfDays)
        {
            AccessEvents accessEvents = _accessEventsRepository.GetAccessEvents(employeeId);
            var workRecordByDate = accessEvents.WorkRecord(noOfDays);
            AttendanceRecordsDTO listOfAttendanceRecord = await CreateAttendanceRecordAsync(workRecordByDate, employeeId);

            var fromDate = listOfAttendanceRecord.ListOfAttendanceRecordDTO.Select(x => x.Date).Min();
            var toDate = listOfAttendanceRecord.ListOfAttendanceRecordDTO.Select(x => x.Date).Max();

            listOfAttendanceRecord = IncludeHolidays(listOfAttendanceRecord, fromDate, toDate);

            return await Task.Run(() =>
            {
                return listOfAttendanceRecord;
            });
        }

       

        public async Task<AttendanceRecordsDTO> GetAccessEventsForDateRange(int employeeId, DateTime fromDate, DateTime toDate)
        {
            AccessEvents accessEvents = _accessEventsRepository.GetAccessEventsForDateRange(employeeId, fromDate, toDate);
            var datewiseAccessEvents = accessEvents.GetAllAccessEvents();
            AttendanceRecordsDTO listOfAttendanceRecord = await CreateAttendanceRecordAsync(datewiseAccessEvents, employeeId);
            listOfAttendanceRecord = IncludeHolidays(listOfAttendanceRecord, fromDate,toDate);

            return await Task.Run(() =>
            {
                return listOfAttendanceRecord;
            });
        }
        public async Task<List<AccessPointRecord>> GetAccessPointDetails(int employeeId, DateTime date)
        {
            PerDayWorkRecord perDayWorkRecord = _accessEventsRepository.GetAccessEventsForADay(employeeId, date);

            var RecreationPointAccessEvents = perDayWorkRecord.GetRecreationPointAccessEvents();
            var GymnasiumPointAccessEvents = perDayWorkRecord.GetGymnasiumPointAccessEvents();
            var MainEntryPointAccessEvents = perDayWorkRecord.GetMainEntryPointAccessEvents();

            List<AccessPointRecord> RecreationAccessPointRecord = GetAccessPointRecord(RecreationPointAccessEvents, "Recreation");
            List<AccessPointRecord> GymnasiumAccessPointRecord = GetAccessPointRecord(GymnasiumPointAccessEvents, "Gymnasium");
            List<AccessPointRecord> MainEntryPointAccessPointRecord = GetAccessPointRecord(MainEntryPointAccessEvents, "Main Entry");

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

        private List<AccessPointRecord> GetAccessPointRecord(List<AccessEvent> listOfAccessEvent, string AccessPoint)
        {
            List<AccessPointRecord> listOfaccessPointRecords = new List<AccessPointRecord>();
            for (int i = 0; i < listOfAccessEvent.Count; i += 2)
            {
                var timeIn = CalculateAbsoluteOutTimeAndInTime(listOfAccessEvent[i].EventTime.TimeOfDay, AbsoluteTime.TimeIn);
                var timeOut = TimeSpan.Zero;
                if (i != listOfAccessEvent.Count - 1)
                {
                    timeOut = CalculateAbsoluteOutTimeAndInTime(listOfAccessEvent[i + 1].EventTime.TimeOfDay, AbsoluteTime.TimeOut); ;
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
                    AccessPoint = AccessPoint
                };
                listOfaccessPointRecords.Add(accessPointRecord);
            }
            return listOfaccessPointRecords;
        }
       

        private AttendanceRecordsDTO IncludeHolidays(AttendanceRecordsDTO listOfAttendanceRecord, DateTime fromDate, DateTime toDate)
        {
            var availableDates = listOfAttendanceRecord.ListOfAttendanceRecordDTO.Select(x => x.Date).Distinct().ToList();
            var listOfAttendanceRecordDTO = listOfAttendanceRecord.ListOfAttendanceRecordDTO;
            for (var i = fromDate; i <= toDate; i = i.AddDays(1))
            {
                if (!availableDates.Any(x => x.Date.Date == i.Date.Date))
                {
                    listOfAttendanceRecordDTO.Add(new PerDayAttendanceRecordDTO()
                    {
                        Date = i,
                        LateBy = new Time(0, 0),
                        OverTime = new Time(0, 0),
                        TimeIn = new Time(0, 0),
                        TimeOut = new Time(0, 0),
                        WorkingHours = new Time(0, 0),
                    });
                }
            }
            listOfAttendanceRecord.ListOfAttendanceRecordDTO = listOfAttendanceRecordDTO.OrderByDescending(x => x.Date).ToList();

            return listOfAttendanceRecord;
        }

        private async Task<AttendanceRecordsDTO> CreateAttendanceRecordAsync(IList<PerDayWorkRecord> workRecordByDate, int employeeId)
        {
            AttendanceRecordsDTO listOfAttendanceRecordDTO = new AttendanceRecordsDTO();
            Employee employeeData = _employeeRepository.GetEmployee(employeeId);

            foreach (var perDayWorkRecord in workRecordByDate)
            {
                var timeIn = perDayWorkRecord.GetTimeIn();
                var timeOut = perDayWorkRecord.GetTimeOut();
                var workingHours = perDayWorkRecord.CalculateWorkingHours();

                PerDayAttendanceRecordDTO attendanceRecord = new PerDayAttendanceRecordDTO()
                {
                    Date = perDayWorkRecord.Date,
                    TimeIn = new Time(timeIn.Hours, timeIn.Minutes),
                    TimeOut = new Time(timeOut.Hours, timeOut.Minutes),
                    WorkingHours = new Time(workingHours.Hours, workingHours.Minutes),
                    OverTime = GetOverTime(workingHours, GetNoOfHoursToBeWorked(employeeData.Department())),
                    LateBy = GetLateByTime(workingHours, GetNoOfHoursToBeWorked(employeeData.Department()))
                };
                listOfAttendanceRecordDTO.ListOfAttendanceRecordDTO.Add(attendanceRecord);
            }

            return await Task.Run(() =>
            {
                var perDayAttendanceRecords = listOfAttendanceRecordDTO.ListOfAttendanceRecordDTO;
                return new AttendanceRecordsDTO()
                {
                    ListOfAttendanceRecordDTO = perDayAttendanceRecords,
                    TotalWorkingHours = CalculateTotalWorkingHours(perDayAttendanceRecords),
                    TotalDeficitOrExtraHours = CalculateDeficiateOrExtraTime(perDayAttendanceRecords, GetNoOfHoursToBeWorked(employeeData.Department())),
                };
            });
        }


        private Time GetOverTime(TimeSpan workingHours, double noOfHoursToBeWorked)
        {
            var extraHour = GetExtraHours(workingHours, noOfHoursToBeWorked);
            if (extraHour.Hour > 0 || extraHour.Minute > 0)
            {
                return extraHour;
            }
            else
            {
                return new Time(0, 0);
            }
        }
        private Time GetLateByTime(TimeSpan workingHours, double noOfHoursToBeWorked)
        {
            var extraHour = GetExtraHours(workingHours, noOfHoursToBeWorked);
            if (extraHour.Hour < 0 || extraHour.Minute < 0)
            {
                int latebyHours = Math.Abs(extraHour.Hour);
                int latebyMinutes = Math.Abs(extraHour.Minute);
                return new Time(latebyHours, latebyMinutes);
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

        private Time CalculateDeficiateOrExtraTime(List<PerDayAttendanceRecordDTO> listOfAttendanceRecordDTO, double noOfHoursToBeWorked)
        {
            if (listOfAttendanceRecordDTO.Count == 0)
            {
                return new Time(00, 00);
            }

            double totalRequiredHoursToBeWorked = listOfAttendanceRecordDTO.Count * noOfHoursToBeWorked;
            Time totalWorkedTime = CalculateTotalWorkingHours(listOfAttendanceRecordDTO);
            var totalWorkedSpan = new TimeSpan(totalWorkedTime.Hour, totalWorkedTime.Minute, 00);

            double totalDefiateOrOverTimeHrs = totalRequiredHoursToBeWorked - totalWorkedSpan.TotalHours;

            var extraTime = TimeSpan.FromHours(totalDefiateOrOverTimeHrs);

            return new Time(
                (int)extraTime.TotalHours,
                (int)extraTime.Minutes);

        }

        private Time CalculateTotalWorkingHours(List<PerDayAttendanceRecordDTO> listOfAttendanceRecordDTO)
        {
            if (listOfAttendanceRecordDTO.Count == 0)
            {
                return new Time(00, 00);
            }

            var sumOfTotalWorkingHours = listOfAttendanceRecordDTO
                .Select(x => new TimeSpan(x.WorkingHours.Hour, x.WorkingHours.Minute, 00))
                .Aggregate((t1, t2) => t1 + t2);

            return new Time(
                (int)sumOfTotalWorkingHours.TotalHours,
                (int)sumOfTotalWorkingHours.Minutes);
        }

        private double GetNoOfHoursToBeWorked(Departments department)
        {
            return department == Departments.Design ? 10.0 : 9.0;
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
