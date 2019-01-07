using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainModel;
using UseCaseBoundary;
using UseCaseBoundary.Model;

namespace UseCases
{
    public class AttendanceService
    {
        private IAccessEventsRepository _accessEventsRepository;
        public AttendanceService(IAccessEventsRepository accessEventsRepository)
        {
            _accessEventsRepository = accessEventsRepository;
        }

        public async Task<List<AttendanceRecordDTO>> GetAttendanceRecord(int employeeId, int noOfDays)
        {
            AccessEvents accessEvents = _accessEventsRepository.GetAccessEvents(employeeId);
            var listOfAccessEventByDate = accessEvents.GetNoOfDaysAccessEventsByDate(noOfDays);
            List<AttendanceRecordDTO> listOfAttendanceRecord = new List<AttendanceRecordDTO>();
            foreach (var perDayAccessEvents in listOfAccessEventByDate)
            {
                var listOfMainEntryPointAccessEventOfADay = perDayAccessEvents.Select(x => x).Where(K => K.AccessPointName=="Main Entry").ToList();
                AccessEvents accessEventsPerDay = new AccessEvents(listOfMainEntryPointAccessEventOfADay);
                var timeIn = accessEventsPerDay.GetTimeIn();
                var timeOut = accessEventsPerDay.GetTimeOut();
                var workingHours = accessEventsPerDay.CalculateWorkingHours();
              
                AttendanceRecordDTO attendanceRecord = new AttendanceRecordDTO()
                {
                    Date = perDayAccessEvents.Key.Date,
                    TimeIn = new Time(timeIn.Hours, timeIn.Minutes),
                    TimeOut = new Time(timeOut.Hours, timeOut.Minutes),
                    WorkingHours = new Time(workingHours.Hours, workingHours.Minutes),
                    OverTime = GetOverTime(workingHours),
                    LateBy = GetLateByTime(workingHours)
                };
                listOfAttendanceRecord.Add(attendanceRecord);
            }
            return await Task.Run(()=> 
            {
                return listOfAttendanceRecord;
            });
        }

        public async Task<List<AttendanceRecordDTO>> GetAccessEventsForDateRange(int employeeId, DateTime fromDate, DateTime toDate)
        {
            var accessEvents = _accessEventsRepository.GetAccessEventsForDateRange(employeeId, fromDate, toDate);
            var datewiseAccessEvents = accessEvents.GetAllAccessEvents().GroupBy(x => DateTime.Parse(x.EventTime.ToShortDateString()));

            List<AttendanceRecordDTO> listOfAttendanceRecord = new List<AttendanceRecordDTO>();
            foreach (var perDayAccessEvents in datewiseAccessEvents)
            {
                var listOfMainEntryPointAccessEventOfADay = perDayAccessEvents.Select(x => x).Where(K => K.AccessPointName == "Main Entry").ToList();
                AccessEvents accessEventsPerDay = new AccessEvents(listOfMainEntryPointAccessEventOfADay);
                var timeIn = accessEventsPerDay.GetTimeIn();
                var timeOut = accessEventsPerDay.GetTimeOut();
                var workingHours = accessEventsPerDay.CalculateWorkingHours();

                AttendanceRecordDTO attendanceRecord = new AttendanceRecordDTO()
                {
                    Date = perDayAccessEvents.Key.Date,
                    TimeIn = new Time(timeIn.Hours, timeIn.Minutes),
                    TimeOut = new Time(timeOut.Hours, timeOut.Minutes),
                    WorkingHours = new Time(workingHours.Hours, workingHours.Minutes),
                    OverTime = GetOverTime(workingHours),
                    LateBy = GetLateByTime(workingHours)
                };
                listOfAttendanceRecord.Add(attendanceRecord);
            }

            return await Task.Run(() =>
            {
                return listOfAttendanceRecord
                    .OrderByDescending(x => x.Date)
                    .ToList();
            });
        }

        private Time GetOverTime(TimeSpan workingHours)
        {
            var extraHour = GetExtraHours(workingHours);
            if (extraHour.Hour > 0 || extraHour.Minute > 0)
            {
                return extraHour;
            }
            else
            {
                return new Time(0, 0);
            }
        }
        private Time GetLateByTime(TimeSpan workingHours)
        {
            var extraHour = GetExtraHours(workingHours);
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

        private Time GetExtraHours(TimeSpan workingHours)
        {
            TimeSpan TotalWorkingHours = TimeSpan.Parse("9:00:00");
            var extraHour = workingHours - TotalWorkingHours;
            return new Time(extraHour.Hours, extraHour.Minutes);
        }
    }
}
