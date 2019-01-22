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
                var timeIn = listOfAccessEvent[i].EventTime.TimeOfDay;
                timeIn = new TimeSpan(timeIn.Hours, timeIn.Minutes, 00);

                var timeOut = TimeSpan.Zero;
                if (i != listOfAccessEvent.Count - 1)
                {
                    timeOut = listOfAccessEvent[i + 1].EventTime.TimeOfDay;
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
            var listOfAccessEvent = _accessEvents;
            for (int i = 1; i < listOfAccessEvent.Count - 1; i += 2)
            {
                var timeOut = listOfAccessEvent[i].EventTime.TimeOfDay;
                timeOut = new TimeSpan(timeOut.Hours, timeOut.Minutes, 00);

                var timeIn = listOfAccessEvent[i + 1].EventTime.TimeOfDay;
                timeIn = new TimeSpan(timeIn.Hours, timeIn.Minutes, 00);
                totalTime += timeIn - timeOut;
            }
            return totalTime;
        }
    }

}
