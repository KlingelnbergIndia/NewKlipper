using Models.Core.HR.Attendance;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Klipper.Web.Application.Attendance.Service
{
    public interface IAttendanceService
    {
        IEnumerable<AccessEvent> GetAttendanceByDate(int employeeId, DateTime startDate, DateTime endDate);

    }
}
