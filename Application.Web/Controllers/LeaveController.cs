using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Application.Web.Models;
using Application.Web.PageAccessAuthentication;
using Klipper.Web.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using UseCaseBoundary;
using UseCaseBoundary.DTO;
using UseCases;
using static DomainModel.Leave;

namespace Application.Web.Controllers
{

    public class LeaveController : Controller
    {
        private readonly ILeavesRepository _leavesRepository;
        private IEmployeeRepository _employeeRepository;
        private IDepartmentRepository _departmentRepository;
        private ICarryForwardLeaves _carryForwardLeaves;

        public LeaveController(
            ILeavesRepository leavesRepository, 
            IEmployeeRepository employeeRepository,
            IDepartmentRepository departmentRepository, 
            ICarryForwardLeaves carryForwardLeaves)
        {
            _leavesRepository = leavesRepository;
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _carryForwardLeaves = carryForwardLeaves;

        }
        public IActionResult Index()
        {
            var loggedInEmpId = HttpContext.Session.GetInt32("ID") ?? 0;
            var leaveService = new UseCases.LeaveService(_leavesRepository, _employeeRepository, _departmentRepository, _carryForwardLeaves);

            var leaveViewModel = new LeaveViewModel();
            leaveViewModel.GetAppliedLeaves = leaveService.GetAppliedLeaves(loggedInEmpId);

            var leaveSummary = leaveService.GetTotalSummary(loggedInEmpId);
            leaveViewModel.LeaveSummary = new ReporteeViewModel()
                .ConvertToLeaveSummaryViewModel(leaveSummary);
           

            return View(leaveViewModel);
        }
        [HttpPost]
        public IActionResult ApplyLeave(DateTime FromDate, DateTime ToDate, LeaveType LeaveType,bool isHalfDay, string Remark)
        {
            if (FromDate > ToDate)
            {
                TempData["errorMessage"] = "Please select valid date range !";
                return RedirectToAction("Index");
            }
            string leaveType = HttpContext.Request.Form["leaveList"].ToString();
            if (int.Parse(leaveType) == 0)
                LeaveType = LeaveType.CompOff;
            else if (int.Parse(leaveType) == 1)
                LeaveType = LeaveType.CasualLeave;
            else if (int.Parse(leaveType) == 2)
                LeaveType = LeaveType.SickLeave;

            var loggedInEmpId = HttpContext.Session.GetInt32("ID") ?? 0;
            var leaveService = new UseCases.LeaveService(_leavesRepository, _employeeRepository, _departmentRepository, _carryForwardLeaves);
            var response = leaveService.ApplyLeave(loggedInEmpId, FromDate, ToDate, LeaveType, isHalfDay, Remark);

            if (response == ServiceResponseDTO.Saved)
                TempData["responseMessage"] = "Leave applied sucessfully !";
            else if (response == ServiceResponseDTO.RecordExists)
                TempData["errorMessage"] = "Leave record is available for selected date !";
            else if (response == ServiceResponseDTO.InvalidDays)
                TempData["errorMessage"] = "Please select valid date range !";
            else if(response == ServiceResponseDTO.CanNotApplied)
                TempData["errorMessage"] = "Selected leave is not available !";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateLeave(string leaveId,DateTime FromDate, DateTime ToDate, LeaveType LeaveType, bool isHalfDay, string Remark)
        {
            string ExistingLeaveId = HttpContext.Request.Form["leaveId"].ToString();
            if (leaveId != null)
            {
                if (FromDate > ToDate)
                {
                    TempData["errorMessage"] = "Please select valid date range !";
                    return RedirectToAction("Index");
                }
                string leaveType = HttpContext.Request.Form["leaveList"].ToString();
                if (int.Parse(leaveType) == 0)
                    LeaveType = LeaveType.CompOff;
                else if (int.Parse(leaveType) == 1)
                    LeaveType = LeaveType.CasualLeave;
                else if (int.Parse(leaveType) == 2)
                    LeaveType = LeaveType.SickLeave;

                var loggedInEmpId = HttpContext.Session.GetInt32("ID") ?? 0;
                var leaveService = new UseCases.LeaveService(_leavesRepository, _employeeRepository, _departmentRepository, _carryForwardLeaves);
                var response = leaveService.UpdateLeave(ExistingLeaveId, loggedInEmpId, FromDate,ToDate,LeaveType, isHalfDay, Remark);

                if (response == ServiceResponseDTO.Updated)
                    TempData["responseMessage"] = "Leave updated sucessfully !";
                else if (response == ServiceResponseDTO.RecordExists)
                    TempData["errorMessage"] = "Leave record is available for selected date !";
                else if (response == ServiceResponseDTO.InvalidDays)
                    TempData["errorMessage"] = "Please select valid date range !";
                else if (response == ServiceResponseDTO.CanNotApplied)
                    TempData["errorMessage"] = "Selected leave is not available!";
                else if (response == ServiceResponseDTO.Deleted)
                    TempData["errorMessage"] = "Cancelled leaves can not be updated !";
                else if (response == ServiceResponseDTO.RealizedLeave)
                    TempData["errorMessage"] = "Passed leaves can not be updated !";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult CancelLeave(string LeaveId)
        {
            var leaveService = new UseCases.LeaveService(_leavesRepository, _employeeRepository, _departmentRepository, _carryForwardLeaves);
            var loggedInEmpId = HttpContext.Session.GetInt32("ID") ?? 0;
            var response = leaveService.CancelLeave(LeaveId, loggedInEmpId);

            if (response == ServiceResponseDTO.Deleted)
                TempData["responseMessage"] = "Leave deleted sucessfully !";
            else if (response == ServiceResponseDTO.RealizedLeave)
                TempData["errorMessage"] = "Passed leaves can not be cancelled !";
            else
            TempData["errorMessage"] = "Cancelled leaves can not be updated !";

            return RedirectToAction("Index");
        }


        //    [HttpPost]
        //    public FileResult ExportLeaveReport()
        //    {
        //        var stream = new System.IO.MemoryStream();
        //        using (ExcelPackage package = new ExcelPackage(stream))
        //        {
        //            IList<EmployeeViewModel> empList = GetLeaveDataOfAllReporteesAndTeamLead();

        //            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("LeaveReport");
        //            worksheet.TabColor = Color.Gold;
        //            worksheet.DefaultColWidth = 20;
        //            worksheet.Cells[1, 1].Style.Font.Color.SetColor(Color.Blue);
        //            worksheet.Cells[1, 2].Style.Font.Color.SetColor(Color.Blue);
        //            worksheet.Cells[1, 3].Style.Font.Color.SetColor(Color.Blue);
        //            worksheet.Cells[1, 4].Style.Font.Color.SetColor(Color.Blue);

        //            int totalRows = empList.Count();

        //            for (int j = 1; j <= empList.Count();)
        //            {
        //                worksheet.Cells[j, 1].Value = "Employee ID";
        //                worksheet.Cells[j + 1, 1].Value = "Employee Name";
        //                worksheet.Cells[j, 2].Value = empList[j - 1].EmployeeId;
        //                worksheet.Cells[j + 1, 2].Value = empList[j - 1].EmployeeName;

        //                worksheet.Cells[j + 4, 1].Value = "Date";
        //                worksheet.Cells[j + 4, 2].Value = "Day";
        //                worksheet.Cells[j + 4, 3].Value = "Time In";
        //                worksheet.Cells[j + 4, 4].Value = "Time Out";
        //                worksheet.Cells[j + 4, 5].Value = "Deficit Time";
        //                worksheet.Cells[j + 4, 6].Value = "Over Time";
        //                worksheet.Cells[j + 4, 7].Value = "Actual Hours";
        //                worksheet.Cells[j + 4, 8].Value = "Regularized Hours";
        //                worksheet.Cells[j + 5, 9].Value = "Remark";

        //                int k = 5;
        //                foreach (var perdayRecord in empList[j - 1].employeeAttendaceRecords.ListOfAttendanceRecordDTO)
        //                {
        //                    worksheet.Cells[k, 1].Value = perdayRecord.Date;
        //                    worksheet.Cells[k, 2].Value = perdayRecord.Date.DayOfWeek;
        //                    worksheet.Cells[k, 3].Value = perdayRecord.TimeIn;
        //                    worksheet.Cells[k, 4].Value = perdayRecord.TimeOut;
        //                    worksheet.Cells[k, 5].Value = perdayRecord.LateBy;
        //                    worksheet.Cells[k, 6].Value = perdayRecord.OverTime;
        //                    worksheet.Cells[k, 7].Value = perdayRecord.WorkingHours;
        //                    worksheet.Cells[k, 8].Value = perdayRecord.RegularizedHours;
        //                    worksheet.Cells[k, 9].Value = perdayRecord.Remark;
        //                    k++;
        //                }
        //                j += k + 2;
        //            }
        //            package.Save();
        //        }
        //        string fileName = @"Report_" + DateTime.Now.ToString("dd_MM") + ".xlsx";
        //        string fileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //        stream.Position = 0;
        //        return File(stream, fileType, fileName);
        //    }

        //    private GetLeaveDataOfAllReporteesAndTeamLead()
        //    {
        //        var employeeId = HttpContext.Session.GetInt32("ID") ?? 0;
        //        ReporteeService reporteeService = new ReporteeService(_employeeRepository);
        //        LeaveService leaveService = new LeaveService(_leavesRepository, _employeeRepository,
        //          _departmentRepository, _carryForwardLeaves);

        //        var reportees = reporteeService.GetReporteesData(employeeId);
        //        var listOfReporteesAttendanceRecord = new List<EmployeeViewModel>();
        //        if (reportees.Count() != 0)
        //        {
        //            reportees.Add(reporteeService.GetTeamLeadData(employeeId));
        //            foreach (var reportee in reportees)
        //            {
        //                EmployeeViewModel employeeViewModel = new EmployeeViewModel();
        //                var listOfLeaveRecord = new List<LeaveRecordDTO>();
        //                employeeViewModel.fromDate = DateTime.Parse(fromDate);
        //                employeeViewModel.toDate = DateTime.Parse(toDate);
        //                employeeViewModel.EmployeeId = reportee.ID;
        //                employeeViewModel.EmployeeName = string.Concat(reportee.FirstName, " ", reportee.LastName);

        //                employeeViewModel.employeeAttendaceRecords = attendanceService.AttendanceReportForDateRange
        //                    (reportee.ID, employeeViewModel.fromDate, employeeViewModel.toDate);

        //                employeeViewModel.employeeAttendaceRecords.ListOfAttendanceRecordDTO =
        //                    ConvertAttendanceRecordsTimeToIST(employeeViewModel.employeeAttendaceRecords.ListOfAttendanceRecordDTO);

        //                listOfReporteesAttendanceRecord.Add(employeeViewModel);
        //            }
        //            return listOfReporteesAttendanceRecord;
        //        }
        //        return null;
        //    }
        //}
    }
}