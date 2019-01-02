using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainModel;
using UseCaseBoundary;
using UseCaseBoundary.Model;

namespace UseCases
{
    public class AttendanceRecordForEmployeeID
    {
        private IAccessEventsRepository _accessEventsRepository;
        public AttendanceRecordForEmployeeID(IAccessEventsRepository accessEventsRepository)
        {
            _accessEventsRepository = accessEventsRepository;
        }

        public AttendanceRecordForEmployeeID()
        {
        }

        public async Task<List<AttendanceRecord>> GetAttendanceRecord(int employeeId, int noOfDays)
        {
            AccessEvents accessEvents = _accessEventsRepository.GetAccessEventsByEmployeeId(employeeId);
            var listOfAccessEventByDate = accessEvents.GetNoOfDaysAccessEventsByDate(noOfDays);
            List<AttendanceRecord> listOfAttendanceRecord = new List<AttendanceRecord>();
            foreach (var perDayAccessEvents in listOfAccessEventByDate)
            {
                var listOfAccessEventByDay = perDayAccessEvents.Select(x => x).ToList();
                AccessEvents accessEventsPerDay = new AccessEvents(listOfAccessEventByDay);
                var timeIn = perDayAccessEvents.Select(x => x.EventTime.TimeOfDay).Min();
                var timeOut = perDayAccessEvents.Select(x => x.EventTime.TimeOfDay).Max();
                var workingHours = accessEventsPerDay.CalculateWorkingHours();
                var extraHour = workingHours - TimeSpan.Parse("9:00:00");
                AttendanceRecord attendanceRecord = new AttendanceRecord()
                {
                    Date = perDayAccessEvents.Key.Date,
                    TimeIn = new Time(timeIn.Hours, timeIn.Minutes),
                    TimeOut = GetTimeOut(timeIn, timeOut),
                    WorkingHours = new Time(workingHours.Hours, workingHours.Minutes),
                    OverTime = GetOverTime(extraHour),
                    LateBy = GetLateByTime(extraHour)
                };
                listOfAttendanceRecord.Add(attendanceRecord);
            }
            return listOfAttendanceRecord;
        }

        private Time GetOverTime(TimeSpan extrahour)
        {
            if (extrahour > TimeSpan.Zero)
            {
                return new Time(extrahour.Hours, extrahour.Minutes);
            }
            else
            {
                return new Time(0, 0);
            }
        }
        private Time GetLateByTime(TimeSpan extrahour)
        {
            if (extrahour < TimeSpan.Zero)
            {
                return new Time(extrahour.Hours, extrahour.Minutes);
            }
            else
            {
                return new Time(0, 0);
            }
        }
        private Time GetTimeOut(TimeSpan minTime, TimeSpan maxTime)
        {
            if (minTime == maxTime)
            {
                return new Time(0, 0);
            }
            else
            {
                return new Time(maxTime.Hours, maxTime.Minutes);
            }
        }
    }
}
