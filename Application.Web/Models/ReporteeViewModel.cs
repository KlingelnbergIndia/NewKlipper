using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainModel;
using UseCaseBoundary.DTO;
using UseCaseBoundary.Model;

namespace Application.Web.Models
{
    public class ReporteeViewModel
    {
        public Dictionary<int,string> reportees = 
            new Dictionary<int, string>();

        public AttendanceRecordsDTO AttendaceRecordsOfSelectedReportee = 
            new AttendanceRecordsDTO();

        public List<LeaveRecordDTO> leaveRecordsOfSelectedReportee =
            new List<LeaveRecordDTO>();
        
        public DateTime fromDate;

        public DateTime toDate;

        public int EmployeeId;

        public string LeaveFormName { get; set; }
        public string AttendanceFormName { get; set; }
        public int SelectedEmpIdForAttendanceTab { get; set; }
        public int SelectedEmpIdForLeaveTab { get; set; }
        public List<LeaveSummaryViewModel> LeaveSummary = 
            new List<LeaveSummaryViewModel>();

        public LeaveViewModel leaveViewModel =
            new LeaveViewModel();

        public List<LeaveSummaryViewModel> ConvertToLeaveSummaryViewModel
            (LeaveSummaryDTO leaveSummary)
        {
            var listOfleaveSummary = 
                new List<LeaveSummaryViewModel>()
            {
                new LeaveSummaryViewModel()
                {
                    LeaveType = Leave.LeaveType.CasualLeave,
                    TotalAvailableLeave = leaveSummary.MaximumCasualLeave,
                    LeaveTaken = leaveSummary.TotalCasualLeaveTaken,
                    RemainingLeave = leaveSummary.RemainingCasualLeave
                },
                new LeaveSummaryViewModel()
                {
                    LeaveType = Leave.LeaveType.SickLeave,
                    TotalAvailableLeave = leaveSummary.MaximumSickLeave,
                    LeaveTaken = leaveSummary.TotalSickLeaveTaken,
                    RemainingLeave = leaveSummary.RemainingSickLeave
                },
                new LeaveSummaryViewModel()
                {
                    LeaveType = Leave.LeaveType.CompOff,
                    TotalAvailableLeave = leaveSummary.MaximumCompOffLeave,
                    LeaveTaken = leaveSummary.TotalCompOffLeaveTaken,
                    RemainingLeave = leaveSummary.RemainingCompOffLeave
                }
            };
            return listOfleaveSummary;
        }
    }

    enum ViewTabs
    {
        attendanceReportMenu,
        leaveReportMenu
    }
}
