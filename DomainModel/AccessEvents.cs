using System;
using System.Collections.Generic;
using System.Linq;
using DomainModel.Model;

namespace DomainModel
{
    public class AccessEvents
    {
        private List<AccessEvent> accessEvents;

        public AccessEvents(List<AccessEvent> events)
        {
            accessEvents = events;
        }

        public List<IGrouping<DateTime, AccessEvent>> GetNoOfDaysAccessEventsByDate(int noOfDays)
        {
            var acessEventsByDate = accessEvents.GroupBy(x => x.EventTime.Date)
                                                .OrderByDescending(i => i.Key.Date)
                                                .Take(noOfDays)
                                                .ToList(); 
            return acessEventsByDate;
        }

        public TimeSpan CalculateWorkingHours()
        {
            var accessEventsOfMainEntry = accessEvents.Where(K => K.AccessPointID == 16).ToList();
            var minTime = accessEventsOfMainEntry.Select(x => x.EventTime.TimeOfDay).Min();
            var maxTime = accessEventsOfMainEntry.Select(x => x.EventTime.TimeOfDay).Max();
            TimeSpan workingHours = (maxTime - minTime);
            return workingHours;
        }

    }

}
