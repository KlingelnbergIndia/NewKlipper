using Models.Core.HR.Attendance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Klipper.Web.Application.Attendance.DomainModel
{
    public class AccessEvents
    {
        private List<AccessEvent> accessEvents;

        public AccessEvents(List<AccessEvent> events)
        {
            accessEvents = events;
        }
        public DayEvents DistinctDays(int noOfDays)
        {
            var distinctDay = accessEvents.Select(x => x.EventTime.Date).Distinct().ToList();
            var specificDistinctDays = distinctDay.OrderByDescending(i => i.Date).Take(noOfDays).ToList();
            DayEvents dayEvents = new DayEvents(specificDistinctDays);
            return dayEvents;
        }
        public AccessEvents GetNoOfDaysRecord(DayEvents dayEvents)
        {
            var lisiOfDistinctDayEvent = dayEvents.ListOfDistinctDayEvent();
            List<AccessEvent> listOfAccessEvents = new List<AccessEvent>();
            foreach (var eachDay in lisiOfDistinctDayEvent)
            {
                var accessEvent = accessEvents.Where(x => x.EventTime.Date == eachDay).ToList();
                foreach (var eachEntry in accessEvent)
                {
                    listOfAccessEvents.Add(eachEntry);
                }
            }
            AccessEvents specificDayAccessEvents = new AccessEvents(listOfAccessEvents);
            return specificDayAccessEvents;
        }
        public List<AttendanceRecords> CalculateWorkingHours(DayEvents dayEvents)
        {
            var ListOfDate = dayEvents.ListOfDistinctDayEvent();
            List<AttendanceRecords> listOfTimeRecord = new List<AttendanceRecords>();
            foreach (var Date in ListOfDate)
            {
                var filterByDay = accessEvents.Where(K => K.EventTime.Date == Date && K.AccessPointID == 16).ToList();
                var minTime = filterByDay.Select(x => x.EventTime.TimeOfDay).Min();
                var timeInOfAccessEvent = GetTime(minTime);
                var maxTime = filterByDay.Select(x => x.EventTime.TimeOfDay).Max();
                var timeOutOfAccessEvent = GetTimeOutOfAccessEvent(minTime, maxTime);
                TimeSpan workingHours = (TimeSpan.Parse("12:00:00") - minTime) + (maxTime - TimeSpan.Parse("12:00:00"));
                 var totalWorkingHours = GetTime(workingHours);
                var extrahour = workingHours - TimeSpan.Parse("9:00:00");
                var overTime=GetOverTime(workingHours, extrahour);
                var lateByTime = GetLateByTime(workingHours, extrahour);
                AttendanceRecords timeRecord = new AttendanceRecords(Date.Date, timeInOfAccessEvent, timeOutOfAccessEvent, totalWorkingHours, lateByTime, overTime);
                listOfTimeRecord.Add(timeRecord);
            }
            return listOfTimeRecord;
        }

        private Time GetOverTime(TimeSpan workingHours, TimeSpan extrahour)
        {
            if (extrahour > TimeSpan.Zero)
            {
                return GetTime(extrahour);

            }
            else
            {
                return GetTime(TimeSpan.Zero);
            }
        }
        private Time GetLateByTime(TimeSpan workingHours, TimeSpan extrahour)
        {
            if (extrahour < TimeSpan.Zero)
            {
                var lateBy = TimeSpan.Parse("9:00:00") - workingHours;
                return GetTime(lateBy);
            }
            else
            {
                return GetTime(TimeSpan.Zero);
            }
        }

        private Time GetTimeOutOfAccessEvent(TimeSpan minTime, TimeSpan maxTime)
        {
            if (minTime == maxTime)
            {
                return GetTime(TimeSpan.Zero);
            }
            else
            {
                return GetTime(maxTime);
            }
        }

        public AccessEvents ConvertTimeZone(string timeZoneStr)
        {
            foreach (var eachDay in accessEvents)
            {
                eachDay.EventTime = TimeZoneInfo.ConvertTimeFromUtc(eachDay.EventTime, TimeZoneInfo.FindSystemTimeZoneById(timeZoneStr));
            }
            AccessEvents listOfAccessEvents = new AccessEvents(accessEvents);
            return listOfAccessEvents;
        }
        private Time GetTime(TimeSpan time)
        {
            Time timeOfAccessEvent=new Time();
            timeOfAccessEvent._hours = time.Hours;
            timeOfAccessEvent._minute=time.Minutes;
            return timeOfAccessEvent;
        }
    }
}
