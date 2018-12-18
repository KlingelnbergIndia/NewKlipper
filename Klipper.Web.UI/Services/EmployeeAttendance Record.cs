using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Klipper.Web.UI.Models;

namespace Klipper.Web.UI.Services
{
    public class EmployeeAttendance_Record
    {
        public string Date { get; set; }
        public DateTime TimeIn { get; set; }
        public DateTime TimeOut { get; set; }
        public TimeSpan DeficitHours { get; set; }
        public TimeSpan WorkingHours { get; set; }

    }
}
