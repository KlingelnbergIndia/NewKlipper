using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UseCaseBoundary.DTO;
using UseCaseBoundary.Model;

namespace Application.Web.Models
{
    public class EmployeeViewModel
    {
        public AttendanceRecordsDTO employeeAttendaceRecords = new AttendanceRecordsDTO();

        public DateTime fromDate;

        public DateTime toDate;

        public int EmployeeId;

        public string EmployeeName;

        public LeaveViewModel LeaveViewModel = new LeaveViewModel();
    }
}
