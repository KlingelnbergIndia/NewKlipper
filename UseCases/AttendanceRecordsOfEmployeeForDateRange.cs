using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseCaseBoundary;
using UseCaseBoundary.Model;

namespace UseCases
{
    public class AttendanceRecordsOfEmployeeForDateRange
    {
        private IAccessEventsRepository _accessEventsRepository;
        public AttendanceRecordsOfEmployeeForDateRange(IAccessEventsRepository accessEventsRepository)
        {
            _accessEventsRepository = accessEventsRepository;
        }

        public async Task<List<AttendanceRecordDTO>> GetAttendanceRecord(int employeeId, DateTime fromDate, DateTime toDate)
        {
            var accessEvents = _accessEventsRepository.GetAccessEventsForDateRange(employeeId, fromDate, toDate);
            var attendanceRecordDTO = accessEvents.GetAllAccessEvents()
                .Select(x=> new AttendanceRecordDTO {
                    Date = x.EventTime,
                    TimeIn = new Time(0, 0),
                    //TimeOut = GetTimeOut(0, 0),
                    //WorkingHours = new Time(0, 0),
                    //OverTime = GetOverTime(extraHour),
                    //LateBy = GetLateByTime(extraHour)
                });
            return await Task.Run(()=> 
            {
                return attendanceRecordDTO.ToList();
            });
        }


        private Time GetOverTime(TimeSpan extrahour)
        {
            if (extrahour > TimeSpan.Zero)
            {
                return new Time(extrahour.Hours, extrahour.Minutes);
            }
            else
            {
                return new Time(0, 0);
            }
        }
        private Time GetLateByTime(TimeSpan extrahour)
        {
            if (extrahour < TimeSpan.Zero)
            {
                return new Time(extrahour.Hours, extrahour.Minutes);
            }
            else
            {
                return new Time(0, 0);
            }
        }

        private Time GetTimeOut(TimeSpan minTime, TimeSpan maxTime)
        {
            if (minTime == maxTime)
            {
                return new Time(0, 0);
            }
            else
            {
                return new Time(maxTime.Hours, maxTime.Minutes);
            }
        }

    }
}
