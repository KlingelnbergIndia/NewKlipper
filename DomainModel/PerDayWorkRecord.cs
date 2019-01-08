using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainModel
{
    public class PerDayWorkRecord
    {
        private IList<AccessEvent> _accessEvents;
        public readonly DateTime Date;
        public PerDayWorkRecord(DateTime date, IList<AccessEvent> events)
        {
            Date = date;
            _accessEvents = events;
        }

        public TimeSpan CalculateWorkingHours()
        {
            var accessEventsOfMainEntry = _accessEvents.Where(accessEvent => accessEvent.FromMainDoor()).ToList();
            var minTime = accessEventsOfMainEntry.Select(x => x.EventTime.TimeOfDay).Min();
            var maxTime = accessEventsOfMainEntry.Select(x => x.EventTime.TimeOfDay).Max();
            TimeSpan workingHours = (maxTime - minTime);
            return workingHours;
        }

        public TimeSpan GetTimeIn()
        {
            return _accessEvents.Select(x => x.EventTime.TimeOfDay).Min();
        }

        public TimeSpan GetTimeOut()
        {
            var minTime = _accessEvents.Select(x => x.EventTime.TimeOfDay).Min();
            var maxTime = _accessEvents.Select(x => x.EventTime.TimeOfDay).Max();
            if (minTime == maxTime)
            {
                return TimeSpan.Zero;
            }
            else
            {
                return maxTime;
            }
        }
    }
}
