using Klipper.Web.Application.Attendance.DataAccess;
using Models.Core.HR.Attendance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klipper.Web.Application.Attendance.Service
{
    public class AttendanceService : IAttendanceService
    {
        private IAttendanceAccessor _attendanceAccessor;

        public AttendanceService(IAttendanceAccessor attendanceAccessor)
        {
            _attendanceAccessor = attendanceAccessor;
        }

        public IEnumerable<AccessEvent> GetAttendanceByDate(int employeeId, DateTime startDate, DateTime endDate)
        {
            var accessEvents = _attendanceAccessor.GetAttendanceByDateIDAsync(employeeId, startDate, endDate) as List<AccessEvent>;
            var distinctDay = accessEvents.Select(x => x.EventTime.Date).Distinct();
            return accessEvents;
        }

        public async Task<List<AttendanceRecord>> GetAttendance(int employeeId)
        {

            var accessEvents = await _attendanceAccessor.GetAttendanceByEmployeeIdAsync(employeeId) as List<AccessEvent>;

            var distinctDay = accessEvents.Select(x => x.EventTime.Date).Distinct();

            var lastTenEntry = distinctDay.OrderByDescending(i => i.Date).Take(10);
            List<AttendanceRecord> listOfTimeRecord = new List<AttendanceRecord>();
            foreach (var entry in lastTenEntry)
            {
                AttendanceRecord timeRecord = new AttendanceRecord();

                var filterByDay = accessEvents.Where(K => K.EventTime.Date == entry && K.AccessPointID == 16).ToList();
                var minTime = filterByDay.Select(x => x.EventTime.TimeOfDay).Min();
                var maxTime = filterByDay.Select(x => x.EventTime.TimeOfDay).Max();

                timeRecord.TimeIn = minTime;
                timeRecord.TimeOut = maxTime;


                TimeSpan calculationData = TimeSpan.Parse("12:00:00");
                TimeSpan totalHour = TimeSpan.Parse("9:00:00");

                TimeSpan workingHours = (calculationData - minTime) + (maxTime - calculationData);
                timeRecord.TotalWorkingHours = workingHours;
                timeRecord.Date = entry.ToShortDateString();

                timeRecord.DeficitHours = workingHours - totalHour;

                listOfTimeRecord.Add(timeRecord);
            }

            return listOfTimeRecord;
        }
    }
}
