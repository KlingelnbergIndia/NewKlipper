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
    }

}
