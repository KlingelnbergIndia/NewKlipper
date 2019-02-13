using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UseCaseBoundary.DTO;
using UseCaseBoundary.Model;

namespace Application.Web.Models
{
    public class ReporteeViewModel
    {
        public List<string> reportees = new List<string>();

        public AttendanceRecordsDTO AttendaceRecordsOfSelectedReportee = new AttendanceRecordsDTO();

        public List<LeaveRecordDTO> leaveRecordsOfSelectedReportee = new List<LeaveRecordDTO>();

        public string Name;

        public DateTime fromDate;

        public DateTime toDate;

        public int EmployeeId;
    }
}
