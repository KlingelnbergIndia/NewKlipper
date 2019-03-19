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


        public TimeSpan GetTimeIn()
        {
            return CalculateAbsoluteOutTimeAndInTime(
                _accessEvents
                .Where(x => x.FromMainDoor())
                .Select(x => x.EventTime.TimeOfDay).Min(), AbsoluteTime.TimeIn);
        }

        public TimeSpan GetTimeOut()
        {
            var AccessEventsOFServerRoom = _accessEvents.Where(x =>x.FromServerRoom());
            var AccessEventsOfUSBRoom = _accessEvents.Where(x => x.FromUSBDeviceDoor());
            var AccessEventForTimeOut = _accessEvents.Except(AccessEventsOFServerRoom)
                .ToList();

            AccessEventForTimeOut= AccessEventForTimeOut.Except(AccessEventsOfUSBRoom)
                .ToList();

            var minTime = AccessEventForTimeOut.Select(x => x.EventTime.TimeOfDay)
                .Min();
            var maxTime = AccessEventForTimeOut.Select(x => x.EventTime.TimeOfDay)
                .Max();

            if (minTime == maxTime)
            {
                return TimeSpan.Zero;
            }
                return CalculateAbsoluteOutTimeAndInTime(maxTime,AbsoluteTime.TimeOut);
            
        }

        public TimeSpan CalculateWorkingHours()
        {
            var accessEventsOfMainEntry = GetMainEntryPointAccessEvents();
            var accessEventsOfGymEntry = GetGymnasiumPointAccessEvents();
            if (accessEventsOfMainEntry.Count == 0)
            {
                return TimeSpan.Zero;
            }

            var minTime = CalculateAbsoluteOutTimeAndInTime(
                accessEventsOfMainEntry.Select(x => x.EventTime.TimeOfDay).Min(),
                AbsoluteTime.TimeIn);
            var maxTime = CalculateAbsoluteOutTimeAndInTime(
                accessEventsOfMainEntry.Select(x => x.EventTime.TimeOfDay).Max(),
                AbsoluteTime.TimeOut);

            var timeIn = new TimeSpan(minTime.Hours, minTime.Minutes, 00);
            var timeOut = new TimeSpan(maxTime.Hours, maxTime.Minutes, 00);

            var GymnasiumPointAccessEvents = new WorkLogs(accessEventsOfGymEntry);
            var MainEntryPointAccessEvents = new WorkLogs(accessEventsOfMainEntry);

            return GetWorkingHours(accessEventsOfMainEntry,accessEventsOfGymEntry,
                timeIn, timeOut, GymnasiumPointAccessEvents,
                MainEntryPointAccessEvents);
        }

        private static TimeSpan GetWorkingHours(List<AccessEvent> accessEventsOfMainEntry,
            List<AccessEvent> accessEventsOfGymEntry, TimeSpan timeIn, TimeSpan timeOut,
            WorkLogs GymnasiumPointAccessEvents, WorkLogs MainEntryPointAccessEvents)
        {
            var totalWorkingHours = TimeSpan.Zero;

            if (accessEventsOfMainEntry.Count % 2 == 0 
                && accessEventsOfGymEntry.Count % 2 == 0)
            {
                if (timeOut != TimeSpan.Zero)
                {
                    totalWorkingHours = 
                        (timeOut - timeIn) - GymnasiumPointAccessEvents
                            .CalculateTotalTimeSpend();
                }

                var lunchTime = TimeSpan.Parse("00:45:00");
                var RefreshmentTime = MainEntryPointAccessEvents
                    .CalculateOutsidePremisesTime();

                if (RefreshmentTime > lunchTime)
                {
                    totalWorkingHours -= (RefreshmentTime - lunchTime);
                }
            }

            return totalWorkingHours;
        }

        public List<AccessEvent> GetRecreationPointAccessEvents()
        {
            return _accessEvents.Where(x => x.FromRecreationDoor())
                .OrderBy(x => x.EventTime).ToList();
        }
        public List<AccessEvent> GetGymnasiumPointAccessEvents()
        {
            return _accessEvents.Where(x => x.FromGymnasiumDoor())
                .OrderBy(x => x.EventTime).ToList();
        }
        public List<AccessEvent> GetMainEntryPointAccessEvents()
        {
            return _accessEvents.Where(x => x.FromMainDoor())
                .OrderBy(x => x.EventTime).ToList();
        }

        private enum AbsoluteTime
        {
            TimeIn,
            TimeOut
        }
        private TimeSpan CalculateAbsoluteOutTimeAndInTime(
            TimeSpan timeSpan, AbsoluteTime time)
        {
            if (time == AbsoluteTime.TimeOut && timeSpan.Seconds > 0)
            {
                return new TimeSpan(timeSpan.Hours, timeSpan.Minutes + 1, 00);
            }
            return new TimeSpan(timeSpan.Hours, timeSpan.Minutes, 00);
        }

    }
}
