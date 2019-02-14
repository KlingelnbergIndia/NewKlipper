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
        public Dictionary<int,string> reportees = new Dictionary<int, string>();

        public AttendanceRecordsDTO AttendaceRecordsOfSelectedReportee = new AttendanceRecordsDTO();

        public List<LeaveRecordDTO> leaveRecordsOfSelectedReportee = new List<LeaveRecordDTO>();
        public int SumOfLeaves => leaveRecordsOfSelectedReportee.Sum(x=>x.NoOfDays);

        public DateTime fromDate;

        public DateTime toDate;

        public int EmployeeId;

        public string LeaveFormName { get; set; }
        public string AttendanceFormName { get; set; }
        public int SelectedEmpIdForAttendanceTab { get; set; }
        public int SelectedEmpIdForLeaveTab { get; set; }
    }

    enum ViewTabs
    {
        attendanceReportMenu,
        leaveReportMenu
    }
}
