using DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseCaseBoundary;
using UseCaseBoundary.DTO;
using UseCaseBoundary.Model;

namespace UseCases
{
    public class AccessPointService
    {
        private IAccessEventsRepository _accessEventsRepository;
        public AccessPointService(IAccessEventsRepository accessEventsRepository)
        {
            _accessEventsRepository = accessEventsRepository;
        }
        public async Task<List<AccessPointRecord>> GetAccessPointDetails(int employeeId, DateTime date)
        {
            PerDayWorkRecord perDayWorkRecord = _accessEventsRepository.GetAccessEventsForADay(employeeId, date);

            var RecreationPointAccessEvents = perDayWorkRecord.GetRecreationPointAccessEvents();
            var GymnasiumPointAccessEvents = perDayWorkRecord.GetGymnasiumPointAccessEvents();
            var MainEntryPointAccessEvents = perDayWorkRecord.GetMainEntryPointAccessEvents();

            List<AccessPointRecord> RecreationAccessPointRecord = GetAccessPointRecord(RecreationPointAccessEvents, "Recreation");
            List<AccessPointRecord> GymnasiumAccessPointRecord = GetAccessPointRecord(GymnasiumPointAccessEvents, "Gymnasium");
            List<AccessPointRecord> MainEntryPointAccessPointRecord = GetAccessPointRecord(MainEntryPointAccessEvents, "MainEntry");

            List<AccessPointRecord> listOfAccessPointRecord = RecreationAccessPointRecord
                .Concat(GymnasiumAccessPointRecord)
                .Concat(MainEntryPointAccessPointRecord)
                .ToList();

            listOfAccessPointRecord.Sort((x, y) => 
                x.TimeIn.Hour.CompareTo(y.TimeIn.Hour) == 0 ? 
                x.TimeIn.Minute.CompareTo(y.TimeIn.Minute) : x.TimeIn.Hour.CompareTo(y.TimeIn.Hour));

            return await Task.Run(() =>
            {
                return listOfAccessPointRecord;
            });
        }

        private List<AccessPointRecord> GetAccessPointRecord(List<AccessEvent> listOfAccessEvent, string AccessPoint)
        {
            List<AccessPointRecord> listOfaccessPointRecords = new List<AccessPointRecord>();
            for (int i = 0; i < listOfAccessEvent.Count; i += 2)
            {
                var timeIn = CalculateAbsoluteOutTimeAndInTime(listOfAccessEvent[i].EventTime.TimeOfDay, AbsoluteTime.TimeIn);
                var timeOut = TimeSpan.Zero;
                if (i!=listOfAccessEvent.Count-1)
                {
                    timeOut = CalculateAbsoluteOutTimeAndInTime(listOfAccessEvent[i + 1].EventTime.TimeOfDay, AbsoluteTime.TimeOut); ;
                }
                var timeSpend = TimeSpan.Zero;
                if ((listOfAccessEvent.Count % 2) == 0)
                {
                   timeSpend = (timeOut - timeIn);
                }
                AccessPointRecord accessPointRecord = new AccessPointRecord()
                {
                    TimeIn = new Time(timeIn.Hours, timeIn.Minutes),
                    TimeOut = new Time(timeOut.Hours, timeOut.Minutes),
                    TimeSpend = new Time(timeSpend.Hours, timeSpend.Minutes),
                    AccessPoint = AccessPoint
                };
                listOfaccessPointRecords.Add(accessPointRecord);
            }
            return listOfaccessPointRecords;
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
