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

            var minTime = CalculateAbsoluteOutTimeAndInTime(
                accessEventsOfMainEntry.Select(x => x.EventTime.TimeOfDay).Min(), Time.TimeIn);
            var maxTime = CalculateAbsoluteOutTimeAndInTime(
                accessEventsOfMainEntry.Select(x => x.EventTime.TimeOfDay).Max(), Time.TimeOut);

            TimeSpan workingHours = (maxTime - minTime);
            return workingHours;
        }

        public TimeSpan GetTimeIn()
        {
            return CalculateAbsoluteOutTimeAndInTime(
                _accessEvents.Select(x => x.EventTime.TimeOfDay).Min(), Time.TimeIn);
        }

        public TimeSpan GetTimeOut()
        {
            var minTime = CalculateAbsoluteOutTimeAndInTime(
                _accessEvents.Select(x => x.EventTime.TimeOfDay).Min(), Time.TimeIn);

            var maxTime = CalculateAbsoluteOutTimeAndInTime(
                _accessEvents.Select(x => x.EventTime.TimeOfDay).Max(), Time.TimeOut);

            if (minTime == maxTime)
            {
                return TimeSpan.Zero;
            }
            else
            {
                return maxTime;
            }
        }

        private TimeSpan CalculateAbsoluteOutTimeAndInTime(TimeSpan timeSpan, Time time)
        {
            if (time == Time.TimeOut && timeSpan.Seconds > 0)
            {
                return new TimeSpan(timeSpan.Hours, timeSpan.Minutes + 1, 00);
            }

            return new TimeSpan(timeSpan.Hours, timeSpan.Minutes, 00);
        }

        private enum Time
        {
            TimeIn,
            TimeOut
        }
    }
}
