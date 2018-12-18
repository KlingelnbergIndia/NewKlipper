using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Klipper.Web.UI.Services;

namespace Klipper.Web.UI.Models
{
    public class EmployeeAttendanceRecordViewModel
    {
        public IEnumerable<EmployeeAttendance_Record> EmployeeAttendance_Record { get; set; }

    }
}
