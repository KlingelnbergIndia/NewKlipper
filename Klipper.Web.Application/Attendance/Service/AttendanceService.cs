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

        public async Task<List<AttendanceRecord>> GetAttendance(int employeeId, int noOfDays, string timeZoneStr)
        {
            AccessEvents accessEvents = await _attendanceAccessor.GetAccessEventsAsync(employeeId);

            var distictDays = accessEvents.DistinctDays(timeZoneStr, noOfDays);

            return distictDays.Select(x => x.CalculateWorkingHours()).ToList();
        }
    }
}
