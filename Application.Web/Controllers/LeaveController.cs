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


        [HttpPost]
        public FileResult ExportLeaveReport()
        {
            var stream = new System.IO.MemoryStream();
            using (ExcelPackage package = new ExcelPackage(stream))
            {
                IList<EmployeeViewModel> empList = GetLeaveDataOfAllReporteesAndTeamLead();

                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("LeaveReport");
                worksheet.TabColor = Color.Gold;
                worksheet.DefaultColWidth = 20;
                worksheet.Cells[1, 1].Style.Font.Color.SetColor(Color.Blue);
                worksheet.Cells[1, 2].Style.Font.Color.SetColor(Color.Blue);
                worksheet.Cells[1, 3].Style.Font.Color.SetColor(Color.Blue);
                worksheet.Cells[1, 4].Style.Font.Color.SetColor(Color.Blue);

                int totalRows = empList.Count();

                for (int j = 1; j <= empList.Count();)
                {
                    worksheet.Cells[j, 1].Value = "Leave Type";
                    worksheet.Cells[j, 2].Value = "Total Leave Available";
                    worksheet.Cells[j, 3].Value = "Leave Taken";
                    worksheet.Cells[j, 4].Value = "Remaining Leave";
                    int i = 1;
                    foreach (var leaveSummary in empList[j-1].LeaveViewModel.LeaveSummary)
                    {
                        worksheet.Cells[j + i, 1].Value = leaveSummary.LeaveType;
                        worksheet.Cells[j + i, 2].Value = leaveSummary.TotalAvailableLeave;
                        worksheet.Cells[j + i, 3].Value = leaveSummary.LeaveTaken;
                        worksheet.Cells[j + i, 4].Value = leaveSummary.RemainingLeave;
                        i++;
                    }

                    int k = j + i + 1;
                    worksheet.Cells[k, 1].Value = "From Date";
                    worksheet.Cells[k, 2].Value = "To Date";
                    worksheet.Cells[k, 3].Value = "No Of Days";
                    worksheet.Cells[k, 4].Value = "Leave Type";
                    worksheet.Cells[k, 5].Value = "Remark";
                    worksheet.Cells[k, 6].Value = "Status";
                    k++;
                    foreach (var perLeaveRecord in empList[j - 1].LeaveViewModel.GetAppliedLeaves)
                    {
                        worksheet.Cells[k, 1].Value = perLeaveRecord.FromDate;
                        worksheet.Cells[k, 2].Value = perLeaveRecord.ToDate;
                        worksheet.Cells[k, 3].Value = perLeaveRecord.NoOfDays;
                        worksheet.Cells[k, 4].Value = perLeaveRecord.GetLeaveDisplayName();
                        worksheet.Cells[k, 5].Value = perLeaveRecord.Remark;
                        worksheet.Cells[k, 6].Value = perLeaveRecord.GetStatusDisplayName();
                        
                        k++;
                    }
                    j += k + 2;
                }
                package.Save();
            }
            string fileName = @"Report_" + DateTime.Now.ToString("dd_MM") + ".xlsx";
            string fileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            stream.Position = 0;
            return File(stream, fileType, fileName);
        }

        private List<EmployeeViewModel> GetLeaveDataOfAllReporteesAndTeamLead()
        {
            var employeeId = HttpContext.Session.GetInt32("ID") ?? 0;
            ReporteeService reporteeService = new ReporteeService(_employeeRepository);
            LeaveService leaveService = new LeaveService(_leavesRepository, _employeeRepository,
              _departmentRepository, _carryForwardLeaves);

            var reportees = reporteeService.GetReporteesData(employeeId);
            var listOfReporteesLeaveRecord = new List<EmployeeViewModel>();
            if (reportees.Count() != 0)
            {
                reportees.Add(reporteeService.GetTeamLeadData(employeeId));
                foreach (var reportee in reportees)
                {
                    EmployeeViewModel employeeViewModel = new EmployeeViewModel();

                    employeeViewModel.EmployeeId = reportee.ID;
                    employeeViewModel.EmployeeName = string.Concat(reportee.FirstName, " ", reportee.LastName);
                    var leaveViewModel = new LeaveViewModel()
                    {
                        GetAppliedLeaves = leaveService.GetAppliedLeaves(reportee.ID),
                        LeaveSummary = new ReporteeViewModel()
                 .ConvertToLeaveSummaryViewModel(leaveService.GetTotalSummary(reportee.ID))
                };
                    employeeViewModel.LeaveViewModel = leaveViewModel;
                    listOfReporteesLeaveRecord.Add(employeeViewModel);
                }
                return listOfReporteesLeaveRecord;
            }
            return null;
        }
    }
}
