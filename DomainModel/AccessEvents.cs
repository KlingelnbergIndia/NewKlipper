using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainModel
{
    public class AccessEvents
    {
        private List<AccessEvent> _accessEvents;

        public AccessEvents(List<AccessEvent> events)
        {
            _accessEvents = events;
        }

        public IList<PerDayWorkRecord> WorkRecord(int noOfDays)
        {
            return 
                _accessEvents
                .GroupBy(x => x.EventTime.Date)
                .OrderByDescending(i => i.Key.Date)
                .Take(noOfDays)
                .Select(x => new PerDayWorkRecord(x.Key, x.Select(y => y).ToList()))
                .ToList();
        }

        public IList<PerDayWorkRecord> GetAllAccessEvents()
        {
            return
                _accessEvents
                .GroupBy(x => x.EventTime.Date)
                .OrderByDescending(i => i.Key.Date)
                .Select(x => new PerDayWorkRecord(x.Key, x.Select(y => y).ToList()))
                .ToList();
        }

        public TimeSpan CalculateTotalTimeSpend()
        {
            TimeSpan totalTime = TimeSpan.Zero;
            var listOfAccessEvent = _accessEvents;
            for (int i = 0; i < listOfAccessEvent.Count; i += 2)
            {
                var timeIn = CalculateAbsoluteOutTimeAndInTime(listOfAccessEvent[i].EventTime.TimeOfDay, AbsoluteTime.TimeIn);
                timeIn = new TimeSpan(timeIn.Hours, timeIn.Minutes, 00);

                var timeOut = TimeSpan.Zero;
                if (i != listOfAccessEvent.Count - 1)
                {
                    timeOut = CalculateAbsoluteOutTimeAndInTime(listOfAccessEvent[i + 1].EventTime.TimeOfDay, AbsoluteTime.TimeOut); ;
                    timeOut = new TimeSpan(timeOut.Hours, timeOut.Minutes, 00);
                }
                if ((listOfAccessEvent.Count % 2) == 0)
                {
                    totalTime += (timeOut - timeIn);
                }
            }
            return totalTime;
        }

        public TimeSpan CalculateOutsidePremisesTime()
        {
            TimeSpan totalTime = TimeSpan.Zero;
            var listOfMainEntryAccessEvent = _accessEvents.Where(x=>x.FromMainDoor()).ToList();
            for (int i = 1; i < listOfMainEntryAccessEvent.Count - 1; i += 2)
            {
                var timeOut = CalculateAbsoluteOutTimeAndInTime(listOfMainEntryAccessEvent[i].EventTime.TimeOfDay, AbsoluteTime.TimeOut);
                timeOut = new TimeSpan(timeOut.Hours, timeOut.Minutes, 00);

                var timeIn = CalculateAbsoluteOutTimeAndInTime(listOfMainEntryAccessEvent[i + 1].EventTime.TimeOfDay, AbsoluteTime.TimeIn);
                timeIn = new TimeSpan(timeIn.Hours, timeIn.Minutes, 00);
                totalTime += timeIn - timeOut;
            }
            return totalTime;
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
