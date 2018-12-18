using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Klipper.Web.UI.Models;

namespace Klipper.Web.UI.Services
{
    public interface IEmployeeAttendanceRecordData
    {
        IEnumerable<EmployeeAttendance_Record> GetEmployeeOneWeekAttendanceRecord();
    }

    public class EmployeeAttendanceRecordData : IEmployeeAttendanceRecordData
    {
        public List<EmployeeAttendance_Record> _employeeAttendance_Record;
        public EmployeeAttendanceRecordData()
        {
            _employeeAttendance_Record = new List<EmployeeAttendance_Record>
            {
                new EmployeeAttendance_Record
                {
                    Date = DateTime.UtcNow.Date.ToString("dd/MM/yyyy"),
                    TimeIn = DateTime.Now,
                    TimeOut = DateTime.Now,
                    DeficitHours = TimeSpan.Zero,
                    WorkingHours = TimeSpan.Zero
                },
                new EmployeeAttendance_Record
                {
                    Date = DateTime.UtcNow.Date.ToString("dd/MM/yyyy"),
                    TimeIn = DateTime.Now,
                    TimeOut = DateTime.Now,
                    DeficitHours = TimeSpan.Zero,
                    WorkingHours = TimeSpan.Zero
                },
                new EmployeeAttendance_Record
                {
                    Date = DateTime.UtcNow.Date.ToString("dd/MM/yyyy"),
                    TimeIn = DateTime.Now,
                    TimeOut = DateTime.Now,
                    DeficitHours = TimeSpan.Zero,
                    WorkingHours = TimeSpan.Zero
                },
                new EmployeeAttendance_Record
                {
                    Date = DateTime.UtcNow.Date.ToString("dd/MM/yyyy"),
                    TimeIn = DateTime.Now,
                    TimeOut = DateTime.Now,
                    DeficitHours = TimeSpan.Zero,
                    WorkingHours = TimeSpan.Zero
                },
                new EmployeeAttendance_Record
                {
                    Date = DateTime.UtcNow.Date.ToString("dd/MM/yyyy"),
                    TimeIn = DateTime.Now,
                    TimeOut = DateTime.Now,
                    DeficitHours = TimeSpan.Zero,
                    WorkingHours = TimeSpan.Zero
                },
                new EmployeeAttendance_Record
                {
                    Date = DateTime.UtcNow.Date.ToString("dd/MM/yyyy"),
                    TimeIn = DateTime.Now,
                    TimeOut = DateTime.Now,
                    DeficitHours = TimeSpan.Zero,
                    WorkingHours = TimeSpan.Zero
                },
                new EmployeeAttendance_Record
                {
                    Date = DateTime.UtcNow.Date.ToString("dd/MM/yyyy"),
                    TimeIn = DateTime.Now,
                    TimeOut = DateTime.Now,
                    DeficitHours = TimeSpan.Zero,
                    WorkingHours = TimeSpan.Zero
                },
            };
        }
        public IEnumerable<EmployeeAttendance_Record> GetEmployeeOneWeekAttendanceRecord()
        {
            return _employeeAttendance_Record;
        }
    }
}
