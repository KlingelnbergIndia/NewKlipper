using Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Models.Core.HR.Attendance
{
    public class DayEvents
    {
        private DateTime _distinctDayEvent;

        public DayEvents(DateTime distinctDayEvent)
        {
            _distinctDayEvent = distinctDayEvent;
        }
        public AttendanceRecord CalculateWorkingHours()
        {
            throw new NotImplementedException();

            //foreach (var entry in lastTenEntry)
            //{
            //    AttendanceRecord timeRecord = new AttendanceRecord();

            //    var filterByDay = accessEvents.Where(K => K.EventTime.Date == entry && K.AccessPointID == 16).ToList();
            //    var minTime = filterByDay.Select(x => x.EventTime.TimeOfDay).Min();
            //    var maxTime = filterByDay.Select(x => x.EventTime.TimeOfDay).Max();
            //    timeRecord.TimeIn = minTime;
            //    if (minTime == maxTime)
            //    {
            //        timeRecord.TimeOut = TimeSpan.Zero;
            //    }
            //    else
            //    {
            //        timeRecord.TimeOut = maxTime;
            //    }

            //    TimeSpan calculationData = TimeSpan.Parse("12:00:00");
            //    TimeSpan totalHour = TimeSpan.Parse("9:00:00");

            //    TimeSpan workingHours = (calculationData - minTime) + (maxTime - calculationData);
            //    timeRecord.TotalWorkingHours = workingHours;
            //    timeRecord.Date = entry.ToShortDateString();
            //    var extrahour = workingHours - totalHour;
            //    if (extrahour > TimeSpan.Zero)
            //    {
            //        timeRecord.OverTime = extrahour;
            //    }
            //    else
            //    {
            //        timeRecord.LateBy = totalHour - workingHours;
            //    }


            //    //    listOfTimeRecord.Add(timeRecord);
            //    //}

            }
    }

    public class AccessEvents
    {
        private List<AccessEvent> accessEvents;

        public AccessEvents(List<AccessEvent> events)
        {
            accessEvents = events;
        }
        public List<DayEvents> DistinctDays(string timeZoneStr, int noOfDays)
        {
            var distinctDay = accessEvents.Select(x => x.EventTime.Date).Distinct();
            List<DayEvents> dayEvents = new List<DayEvents>();
            foreach (var dayEvent in dayEvents)
            {
                dayEvents.Add(dayEvent);
            }
            return dayEvents;
        }

    }

    public class AccessEvent
    {
        [BsonId]
        [JsonConverter(typeof(ObjectIdConverter))]
        public ObjectId _objectId { get; set; }

        public int ID { get; set; }

        public int AccessPointID { get; set; }

        public string AccessPointName { get; set; }

        public string AccessPointIPAddress { get; set; }

        public int EmployeeID { get; set; }

        public string EmployeeFirstName { get; set; }

        public string EmployeeLastName { get; set; }

        [BsonDateTimeOptions]
        public DateTime EventTime { get; set; }

        public bool IsManualEntry { get; set; } = false;

    }
}


