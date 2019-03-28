using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
    [AuthenticateSession]
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
            var leaveService = new LeaveService
                (_leavesRepository, _employeeRepository, 
                _departmentRepository, _carryForwardLeaves);
            var leaveViewModel = new LeaveViewModel();
            var appliedLeaves = leaveService.AppliedLeaves(loggedInEmpId);
            var leaveSummary = leaveService.TotalSummary(loggedInEmpId);

            if (appliedLeaves != null)
                leaveViewModel.GetAppliedLeaves = appliedLeaves;
           
            if (leaveSummary != null)
                leaveViewModel.LeaveSummary = new ReporteeViewModel()
                    .ConvertToLeaveSummaryViewModel(leaveSummary);

            return View(leaveViewModel);
        }

        public IActionResult ApplyLeave(DateTime fromDate, DateTime toDate,
            string leaveType,bool isHalfDay, string remark)
        {
            if (fromDate > toDate)
            {
                TempData["errorMessage"] = "Please select valid date range !";
                return RedirectToAction("Index");
            }
            var LeaveType = GetLeaveType(leaveType);
            var loggedInEmpId = HttpContext.Session.GetInt32("ID") ?? 0;
            var leaveService = new LeaveService
                (_leavesRepository, _employeeRepository,
                _departmentRepository, _carryForwardLeaves);
            var response = leaveService
                .ApplyLeave(loggedInEmpId, fromDate, toDate,
                    LeaveType, isHalfDay, remark);

            GetResponseMessageForLeaveRecord(response);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult CancelLeave(string LeaveId)
        {
            var leaveService = new LeaveService
                (_leavesRepository, _employeeRepository, 
                _departmentRepository, _carryForwardLeaves);

            var loggedInEmpId = HttpContext.Session.GetInt32("ID") ?? 0;
            var response = leaveService.CancelLeave(LeaveId, loggedInEmpId);

            if (response == ServiceResponseDTO.Deleted)
                TempData["responseMessage"] = "Leave cancelled sucessfully !";
            else if (response == ServiceResponseDTO.RealizedLeave)
                TempData["errorMessage"] = "Passed leaves can not be cancelled !";
            else
            TempData["errorMessage"] = "Cancelled leaves can not be updated !";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult CancelAddedCompOff(string LeaveId)
        {
            var leaveService = new LeaveService
            (_leavesRepository, _employeeRepository,
                _departmentRepository, _carryForwardLeaves);
            var loggedInEmpId = HttpContext.Session.GetInt32("ID") ?? 0;
            var response = leaveService.CancelCompOff(LeaveId, loggedInEmpId);

            if (response == ServiceResponseDTO.Deleted)
                TempData["responseMessage"] = "CompOff cancelled sucessfully !";
            else if (response == ServiceResponseDTO.RealizedLeave)
                TempData["errorMessage"] = "Passed CompOff can not be cancelled !";
            else
                TempData["errorMessage"] = "Cancelled CompOff can not be updated !";

            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult ApplyCompOff(DateTime fromDate, DateTime toDate,
            string remark)
        {
            if (fromDate > toDate)
            {
                TempData["errorMessage"] = "Please select valid date range !";
                return RedirectToAction("Index");
            }

            var loggedInEmpId = HttpContext.Session.GetInt32("ID") ?? 0;
            var leaveService = new LeaveService
            (_leavesRepository, _employeeRepository,
                _departmentRepository, _carryForwardLeaves);

            var response = leaveService
                .ApplyCompOff(loggedInEmpId, fromDate, toDate,
                    remark);

            GetResponseMessageForApplyCompOff(response);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateAddedCompOff(string leaveId, 
            DateTime fromDate, DateTime toDate, string remark)
        {
            if (leaveId != null)
            {
                if (fromDate > toDate)
                {
                    TempData["errorMessage"] = "Please select valid date range !";
                    return RedirectToAction("Index");
                }

                var loggedInEmpId = HttpContext.Session.GetInt32("ID") ?? 0;
                var leaveService = new LeaveService
                (_leavesRepository, _employeeRepository,
                    _departmentRepository, _carryForwardLeaves);

                var response = leaveService
                    .UpdateAddedCompOff
                        (leaveId, loggedInEmpId, fromDate, toDate, remark);

                GetResponseMessageOfUpdateAddedCompOff(response);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [AuthenticateTeamLeaderRole]
        public FileResult ExportLeaveReport()
        {
            var stream = new System.IO.MemoryStream();
            CreateExcelSheet(stream);
            string fileName = @"LeaveReport_" +
                              DateTime.Now.ToString("dd_MM") + ".xlsx";
            string fileType =
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            stream.Position = 0;
            return File(stream, fileType, fileName);
        }

        private void CreateExcelSheet(System.IO.MemoryStream stream)
        {
            using (ExcelPackage package = new ExcelPackage(stream))
            {
                var empList = GetLeaveDataOfAllReporteesAndTeamLead();

                var worksheet = package.Workbook.Worksheets.Add("LeaveReport");
                worksheet.TabColor = Color.Gold;
                worksheet.DefaultColWidth = 20;

                int totalRows = empList.Count();
                int j = 1;
                j = FormatExcelSheetRecord(empList, worksheet, j);
                package.Save();
            }
        }

        private static int FormatExcelSheetRecord
            (List<EmployeeViewModel> empList, ExcelWorksheet worksheet, int j)
        {
            foreach (var perEmployeeRecord in empList)
            {
                FormatEmployeeDetail(worksheet, j, perEmployeeRecord);

                int i = FormatLeaveSummary(worksheet, j, perEmployeeRecord);

                int k = j + i + 1;
                k = FormatLeaveRecord(worksheet, perEmployeeRecord, k);
                j = k + 2;
            }
            return j;
        }

        private static LeaveType GetLeaveType(string leaveType)
        {
            LeaveType LeaveType = LeaveType.CasualLeave;
            if (leaveType == "Comp-Off")
                LeaveType = LeaveType.CompOff;
            if (leaveType == "Casual Leave")
                LeaveType = LeaveType.CasualLeave;
            if (leaveType == "Sick Leave")
                LeaveType = LeaveType.SickLeave;
            if (leaveType == "On Service Call")
                LeaveType = LeaveType.OnServiceCall;

            return LeaveType;
        }

        private void GetResponseMessageForLeaveRecord(
            ServiceResponseDTO response)
        {
            if (response == ServiceResponseDTO.Saved)
                TempData["responseMessage"] = "Leave applied sucessfully !";
            else if (response == ServiceResponseDTO.RecordExists)
                TempData["errorMessage"] = "Record is available for selected date !";
            else if (response == ServiceResponseDTO.InvalidDays)
                TempData["errorMessage"] = "Please select valid days!";
            else if (response == ServiceResponseDTO.CanNotApplied)
                TempData["errorMessage"] = "Selected leave is not available !";
        }

        private void GetResponseMessageForApplyCompOff(
            ServiceResponseDTO response)
        {
            if (response == ServiceResponseDTO.Saved)
                TempData["responseMessage"] = "Comp-Off added sucessfully !";
            else if (response == ServiceResponseDTO.RecordExists)
                TempData["errorMessage"] = "Record is available for selected date !";
            else if (response == ServiceResponseDTO.InvalidDays)
                TempData["errorMessage"] = "Please select valid days !";
        }

        public IActionResult UpdateLeave
            (string leaveId, DateTime fromDate, DateTime toDate,
            string leaveType, bool isHalfDay, string remark)
        {
            if (leaveId != null)
            {
                if (fromDate > toDate)
                {
                    TempData["errorMessage"] = "Please select valid date range !";
                    return RedirectToAction("Index");
                }
                var LeaveType = GetLeaveType(leaveType);
                var loggedInEmpId = HttpContext.Session.GetInt32("ID") ?? 0;
                var leaveService = new LeaveService
                    (_leavesRepository, _employeeRepository,
                    _departmentRepository, _carryForwardLeaves);

                var response = leaveService
                    .UpdateLeave(leaveId, loggedInEmpId, fromDate, 
                        toDate, LeaveType, isHalfDay, remark);

                GetResponseMessageOfUpdateLeave(response);
            }
            return RedirectToAction("Index");
        }

        private void GetResponseMessageOfUpdateLeave(
            ServiceResponseDTO response)
        {
            if (response == ServiceResponseDTO.Updated)
                TempData["responseMessage"] = "Leave updated sucessfully !";
            else if (response == ServiceResponseDTO.RecordExists)
                TempData["errorMessage"] = "Record is available for selected date !";
            else if (response == ServiceResponseDTO.InvalidDays)
                TempData["errorMessage"] = "Please select valid days !";
            else if (response == ServiceResponseDTO.CanNotApplied)
                TempData["errorMessage"] = "Selected leave is not available !";
            else if (response == ServiceResponseDTO.Deleted)
                TempData["errorMessage"] = "Cancelled leaves can not be updated !";
            else if (response == ServiceResponseDTO.RealizedLeave)
                TempData["errorMessage"] = "Passed leaves can not be updated !";
        }

        private void GetResponseMessageOfUpdateAddedCompOff(
            ServiceResponseDTO response)
        {
            if (response == ServiceResponseDTO.Updated)
                TempData["responseMessage"] = "CompOff updated sucessfully !";
            else if (response == ServiceResponseDTO.RecordExists)
                TempData["errorMessage"] = "Record is available for selected date !";
            else if (response == ServiceResponseDTO.InvalidDays)
                TempData["errorMessage"] = "Please select valid days !";
            else if (response == ServiceResponseDTO.Deleted)
                TempData["errorMessage"] = "Cancelled CompOff can not be updated !";
            else if (response == ServiceResponseDTO.RealizedLeave)
                TempData["errorMessage"] = "Passed CompOff can not be updated !";
        }

        private static void FormatEmployeeDetail(ExcelWorksheet worksheet,
            int j, EmployeeViewModel perEmployeeRecord)
        {
            worksheet.Cells[j, 1].Value = "Employee ID";
            worksheet.Cells[j + 1, 1].Value = "Employee Name";
            worksheet.Cells[j, 2].Value = perEmployeeRecord.EmployeeId;
            worksheet.Cells[j + 1, 2].Value = perEmployeeRecord.EmployeeName;
        }

        private static int FormatLeaveSummary(ExcelWorksheet worksheet, 
            int j, EmployeeViewModel perEmployeeRecord)
        {
            worksheet.Cells[j, 4].Value = "Leave Type";
            worksheet.Cells[j, 5].Value = "Total Leave Available";
            worksheet.Cells[j, 6].Value = "Leave Taken";
            worksheet.Cells[j, 7].Value = "Remaining Leave";

            worksheet.Cells[j, 4].Style.Font.Color.SetColor(Color.Blue);
            worksheet.Cells[j, 5].Style.Font.Color.SetColor(Color.Blue);
            worksheet.Cells[j, 6].Style.Font.Color.SetColor(Color.Blue);
            worksheet.Cells[j, 7].Style.Font.Color.SetColor(Color.Blue);
            int i = 1;
            foreach (var leaveSummary in perEmployeeRecord.LeaveViewModel.LeaveSummary)
            {
                worksheet.Cells[j + i, 4].Value = leaveSummary.LeaveType;
                worksheet.Cells[j + i, 5].Value = leaveSummary.TotalAvailableLeave;
                worksheet.Cells[j + i, 6].Value = leaveSummary.LeaveTaken;
                worksheet.Cells[j + i, 7].Value = leaveSummary.RemainingLeave;
                i++;
            }

            return i;
        }

        private static int FormatLeaveRecord(ExcelWorksheet worksheet,
            EmployeeViewModel perEmployeeRecord, int k)
        {
            worksheet.Cells[k, 1].Value = "From Date";
            worksheet.Cells[k, 2].Value = "To Date";
            worksheet.Cells[k, 3].Value = "No Of Days";
            worksheet.Cells[k, 4].Value = "Leave Type";
            worksheet.Cells[k, 5].Value = "Remark";
            worksheet.Cells[k, 6].Value = "Status";
            k++;
            foreach (var perLeaveRecord in perEmployeeRecord
                .LeaveViewModel
                .GetAppliedLeaves)
            {
                worksheet.Cells[k, 1].Value =
                    perLeaveRecord.FromDate.ToString("yyyy-MM-dd");
                worksheet.Cells[k, 2].Value =
                    perLeaveRecord.ToDate.ToString("yyyy-MM-dd");
                worksheet.Cells[k, 3].Value =
                    perLeaveRecord.NoOfDays;
                worksheet.Cells[k, 4].Value =
                    perLeaveRecord.GetLeaveDisplayName();
                worksheet.Cells[k, 5].Value =
                    perLeaveRecord.Remark;
                worksheet.Cells[k, 6].Value =
                    perLeaveRecord.GetStatusDisplayName();

                k++;
            }

            return k;
        }

        private List<EmployeeViewModel> GetLeaveDataOfAllReporteesAndTeamLead()
        {
            var employeeId = HttpContext.Session.GetInt32("ID") ?? 0;
            var reporteeService = new ReporteeService(_employeeRepository);
            var leaveService = new LeaveService(_leavesRepository, _employeeRepository,
              _departmentRepository, _carryForwardLeaves);
            var reportees = reporteeService.ReporteesData(employeeId);
            var listOfReporteesLeaveRecord = new List<EmployeeViewModel>();

            if (reportees.Count() != 0)
            {
                reportees.Add(reporteeService.TeamLeadData(employeeId));
                GetReporteeAndTeamLeadData(
                    leaveService, reportees.OrderBy(i=>i.ID).ToList(), 
                    listOfReporteesLeaveRecord);

                return listOfReporteesLeaveRecord;
            }
            return null;
        }

        private static void GetReporteeAndTeamLeadData
            (LeaveService leaveService, List<ReporteeDTO> reportees,
            List<EmployeeViewModel> listOfReporteesLeaveRecord)
        {
            foreach (var reportee in reportees)
            {
                var employeeViewModel = new EmployeeViewModel();

                employeeViewModel.EmployeeId = reportee.ID;
                employeeViewModel.EmployeeName = string
                    .Concat(reportee.FirstName, " ", reportee.LastName);
                employeeViewModel.LeaveViewModel.GetAppliedLeaves =
                    leaveService.AppliedLeaves(reportee.ID);

                var leaveSummary = leaveService.TotalSummary(reportee.ID);

                if (leaveSummary != null)
                    employeeViewModel.LeaveViewModel.LeaveSummary = 
                        new ReporteeViewModel()
                    .ConvertToLeaveSummaryViewModel(leaveSummary);
                listOfReporteesLeaveRecord.Add(employeeViewModel);
            }
        }
    }
}
