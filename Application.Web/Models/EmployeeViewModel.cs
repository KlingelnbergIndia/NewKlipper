using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UseCaseBoundary.Model;

namespace Application.Web.Models
{
    public class EmployeeViewModel
    {
        public List<AttendanceRecordDTO> employeeAttendaceRecords = new List<AttendanceRecordDTO>();

        public DateTime fromDate;

        public DateTime toDate;
    }
}
