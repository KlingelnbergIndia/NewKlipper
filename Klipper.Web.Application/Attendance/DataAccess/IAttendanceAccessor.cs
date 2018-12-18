using Models.Core.HR.Attendance;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Klipper.Web.Application.Attendance.DataAccess
{
    public interface IAttendanceAccessor
    {
        IEnumerable<AccessEvent> GetAttendanceByDateIDAsync(int employeeId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<AccessEvent>> GetAttendanceByEmployeeIdAsync(int employeeId);
    }
}
