using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UseCaseBoundary.Model;

namespace Application.Web.Models
{
    public class ReporteeViewModel : LayoutViewModel
    {
        public List<string> reportees = new List<string>();

        public AttendanceRecordsDTO reporteesAttendaceRecords = new AttendanceRecordsDTO();

        public string Name;

        public DateTime fromDate;

        public DateTime toDate;

        public int EmployeeId;
    }
}
