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
    }
}
