using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application.Web.Models;
using UseCaseBoundary;
using UseCaseBoundary.Model;
using UseCases;
using UseCaseBoundaryImplementation;
using Microsoft.AspNetCore.Http;
using Klipper.Web.UI;
using System.Dynamic;
using System.Text.RegularExpressions;
using Application.Web.PageAccessAuthentication;
using UseCaseBoundary.DTO;
using DomainModel;
using FizzWare.NBuilder;
using OfficeOpenXml;
using System.Drawing;
using System.IO;

namespace Application.Web.Controllers
{
    [AuthenticateSession]
    public class HomeController : Controller
    {
        private IAccessEventsRepository _accessEventRepository;
        private IEmployeeRepository _employeeRepository;
        private IDepartmentRepository _departmentRepository;
        private IAttendanceRegularizationRepository _attendanceRegularizationRepository;
        private ILeavesRepository _leavesRepository;
        private ICarryForwardLeaves _carryForwardLeaves;

        public HomeController(IAccessEventsRepository accessEventRepository, IEmployeeRepository employeeRepository,
            IDepartmentRepository departmentRepository, IAttendanceRegularizationRepository attendanceRegularizationRepository,
            ILeavesRepository leavesRepository, ICarryForwardLeaves carryForwardLeaves)
        {
            _accessEventRepository = accessEventRepository;
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _attendanceRegularizationRepository = attendanceRegularizationRepository;
            _leavesRepository = leavesRepository;
            _carryForwardLeaves = carryForwardLeaves;
        }

        public IActionResult Index(string searchFilter)
        {
            var employeeId = HttpContext.Session.GetInt32("ID") ?? 0;
            AttendanceService attendanceService = new AttendanceService(_accessEventRepository, _employeeRepository,
                _departmentRepository, _attendanceRegularizationRepository,_leavesRepository);

            EmployeeViewModel employeeViewModel = new EmployeeViewModel();

            if (searchFilter == SearchFilter.AccessEventsByDateRange.ToString())
            {
                string fromDate = HttpContext.Request.Form["fromDate"].ToString();
                string toDate = HttpContext.Request.Form["toDate"].ToString();
                Export(fromDate, toDate);
                employeeViewModel.fromDate = DateTime.Parse(fromDate);
                employeeViewModel.toDate = DateTime.Parse(toDate);

                employeeViewModel.employeeAttendaceRecords =
                    attendanceService.AttendanceReportForDateRange(employeeId, employeeViewModel.fromDate, employeeViewModel.toDate);

                ViewData["resultMessage"] = String.Format(
                    "Attendance from {0} to {1}. Total days:{2}",
                    employeeViewModel.fromDate.ToShortDateString(),
                    employeeViewModel.toDate.ToShortDateString(),
                    employeeViewModel.employeeAttendaceRecords.ListOfAttendanceRecordDTO.Count());
            }
            else
            {
                var toDate = DateTime.Now.Date;
                var fromDate = toDate.AddDays(DayOfWeek.Monday - toDate.DayOfWeek);

                employeeViewModel.toDate = toDate;
                employeeViewModel.fromDate = fromDate;

                employeeViewModel.employeeAttendaceRecords =
                    attendanceService.AttendanceReportForDateRange(employeeId, fromDate, toDate);
            }

            employeeViewModel.EmployeeId = employeeId;

            employeeViewModel
                .employeeAttendaceRecords
                .ListOfAttendanceRecordDTO = ConvertAttendanceRecordsTimeToIST
                (employeeViewModel.employeeAttendaceRecords.ListOfAttendanceRecordDTO);

            ViewData["VisibilityReporteesTab"] = HttpContext.Session.GetString("VisibilityOfReporteesTab");

            return View(employeeViewModel);
        }

        [AuthenticateTeamLeaderRole]
        public IActionResult Reportees(string selectedViewTabs)
        {
            var employeeId = HttpContext.Session.GetInt32("ID") ?? 0;
            ReporteeService reporteeService = new ReporteeService(_employeeRepository);

            var reportees = reporteeService.GetReporteesData(employeeId);

            ReporteeViewModel reporteeViewModel = new ReporteeViewModel();
           
            if (reportees.Count != 0)
            {
                foreach (var reportee in reportees)
                {
                    string reporteeName = reportee.FirstName + " " + reportee.LastName;
                    int reporteeId = reportee.ID;
                    reporteeViewModel.reportees.Add(reporteeId, reporteeName);
                }
            }

            reporteeViewModel.AttendanceFormName = string.Empty;
            reporteeViewModel.LeaveFormName = string.Empty;
            reporteeViewModel.toDate = DateTime.Now.Date;
            reporteeViewModel.fromDate = DateTime.Now.AddDays(DayOfWeek.Monday - DateTime.Now.DayOfWeek);
            ViewBag.SelectedTab = selectedViewTabs;
            return View(reporteeViewModel);
        }

        [HttpPost]
        [AuthenticateTeamLeaderRole]
        public IActionResult GetSelectedreportee(string selectedViewTabs)
        {
            var employeeId = HttpContext.Session.GetInt32("ID") ?? 0;
            ReporteeService reporteeService = new ReporteeService(_employeeRepository);

            var reportees = reporteeService.GetReporteesData(employeeId);
            ReporteeViewModel reporteeViewModel = new ReporteeViewModel();

            foreach (var reportee in reportees)
            {
                string reporteeName = reportee.FirstName + " " + reportee.LastName;
                int reporteeId = reportee.ID;
                reporteeViewModel.reportees.Add(reporteeId, reporteeName);
            }

            if (!string.IsNullOrEmpty(Request.Form["selectMenu"]))
            {
                int selectedReporteeId = int.Parse(Request.Form["selectMenu"]);
                if (selectedViewTabs == ViewTabs.attendanceReportMenu.ToString())
                {
                    string fromDate = Request.Form["fromDate"].ToString();
                    string toDate = Request.Form["toDate"].ToString();

                    if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
                    {
                        reporteeViewModel.fromDate = DateTime.Parse(fromDate);
                        reporteeViewModel.toDate = DateTime.Parse(toDate);

                        AttendanceService attendanceService = new AttendanceService(_accessEventRepository, _employeeRepository,
                        _departmentRepository, _attendanceRegularizationRepository,_leavesRepository);

                        AttendanceRecordsDTO listOfAttendanceRecord = attendanceService.AttendanceReportForDateRange(selectedReporteeId,
                            reporteeViewModel.fromDate, reporteeViewModel.toDate);
                        reporteeViewModel.AttendaceRecordsOfSelectedReportee = listOfAttendanceRecord;

                        reporteeViewModel.AttendaceRecordsOfSelectedReportee.ListOfAttendanceRecordDTO = ConvertAttendanceRecordsTimeToIST(listOfAttendanceRecord.ListOfAttendanceRecordDTO);
                        reporteeViewModel.AttendanceFormName = reporteeViewModel.reportees[selectedReporteeId];
                        reporteeViewModel.SelectedEmpIdForAttendanceTab = selectedReporteeId;

                    }
                }
                else
                {
                    reporteeViewModel.toDate = DateTime.Now.Date;
                    reporteeViewModel.fromDate = DateTime.Now.AddDays(DayOfWeek.Monday - DateTime.Now.DayOfWeek);

                    UseCases.LeaveService leaveService = new UseCases.LeaveService(_leavesRepository, _employeeRepository, _departmentRepository, _carryForwardLeaves);
                    reporteeViewModel.leaveRecordsOfSelectedReportee = leaveService.GetAppliedLeaves(selectedReporteeId);

                    var leaveSummary = leaveService.GetTotalSummary(selectedReporteeId);
                    reporteeViewModel.LeaveSummary = reporteeViewModel.ConvertToLeaveSummaryViewModel(leaveSummary);

                    selectedViewTabs = ViewTabs.leaveReportMenu.ToString();
                    reporteeViewModel.LeaveFormName = reporteeViewModel.reportees[selectedReporteeId];
                    reporteeViewModel.SelectedEmpIdForLeaveTab = selectedReporteeId;
                }
                reporteeViewModel.EmployeeId = selectedReporteeId;
            }
            else
            {
                reporteeViewModel.toDate = DateTime.Now.Date;
                reporteeViewModel.fromDate = DateTime.Now.AddDays(DayOfWeek.Monday - DateTime.Now.DayOfWeek);
            }
            reporteeViewModel.leaveViewModel = GetLeave();
            ViewBag.SelectedTab = selectedViewTabs;
            return View("Reportees", reporteeViewModel);
        }

        private LeaveViewModel GetLeave()
        {
            var loggedInEmpId = HttpContext.Session.GetInt32("ID") ?? 0;
            var leaveService = new UseCases.LeaveService(_leavesRepository, _employeeRepository, _departmentRepository, _carryForwardLeaves);

            var leaveViewModel = new LeaveViewModel();
            leaveViewModel.GetAppliedLeaves = leaveService.GetAppliedLeaves(loggedInEmpId);

            var leaveSummary = leaveService.GetTotalSummary(loggedInEmpId);
            leaveViewModel.LeaveSummary = new ReporteeViewModel()
                .ConvertToLeaveSummaryViewModel(leaveSummary);
            return leaveViewModel;
        }

        [HttpGet]
        public async Task<IActionResult> AccessPointDetail(DateTime date, int employeeId)
        {
            AttendanceService attendanceService = new AttendanceService(_accessEventRepository, _employeeRepository,
                _departmentRepository, _attendanceRegularizationRepository, _leavesRepository);
            List<AccessPointRecord> listofaccesspointdetail = await attendanceService.GetAccessPointDetails(employeeId, date);
            listofaccesspointdetail = ConvertAccessPointRecordsTimeToIST(date, listofaccesspointdetail);
            return View(listofaccesspointdetail);
        }

        [HttpPost]
        [AuthenticateTeamLeaderRole]
        public IActionResult SaveRegularizedHours(DateTime date, int employeeId, DateTime timeToBeRegularize, string remark)
        {
            AttendanceService attendanceService = new AttendanceService(_accessEventRepository, _employeeRepository,
                _departmentRepository, _attendanceRegularizationRepository, _leavesRepository);
            var redularizationData = new RegularizationDTO()
            {
                EmployeeID = employeeId,
                RegularizedDate = date,
                ReguralizedHours = new Time(timeToBeRegularize.Hour, timeToBeRegularize.Minute),
                Remark = remark,
            };
            var response = attendanceService.AddRegularization(redularizationData);
            if (response)
                return Ok(Json("Record regularized !"));

            return null;
        }

        [HttpPost]
        public FileResult Export(string fromDate,string toDate)
        {
                var stream = new System.IO.MemoryStream();
                using (ExcelPackage package = new ExcelPackage(stream))
                {
                    IList<EmployeeViewModel> empList = GetAttendanceDataOfAllReporteesAndTeamLead(fromDate, toDate);

                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Employee");
                    worksheet.TabColor = Color.Gold;
                    worksheet.DefaultColWidth = 20;
                    worksheet.Cells[1, 1].Style.Font.Color.SetColor(Color.Blue);
                    worksheet.Cells[1, 2].Style.Font.Color.SetColor(Color.Blue);
                    worksheet.Cells[1, 3].Style.Font.Color.SetColor(Color.Blue);
                    worksheet.Cells[1, 4].Style.Font.Color.SetColor(Color.Blue);

                    int totalRows = empList.Count();

                    for (int j = 1; j <= empList.Count();)
                    {
                        worksheet.Cells[j, 1].Value = "Employee ID";
                        worksheet.Cells[j + 1, 1].Value = "Employee Name";
                        worksheet.Cells[j, 2].Value = empList[j - 1].EmployeeId;
                        worksheet.Cells[j + 1, 2].Value = empList[j - 1].EmployeeName;

                        worksheet.Cells[j + 4, 1].Value = "Date";
                        worksheet.Cells[j + 4, 2].Value = "Day";
                        worksheet.Cells[j + 4, 3].Value = "Time In";
                        worksheet.Cells[j + 4, 4].Value = "Time Out";
                        worksheet.Cells[j + 4, 5].Value = "Deficit Time";
                        worksheet.Cells[j + 4, 6].Value = "Over Time";
                        worksheet.Cells[j + 4, 7].Value = "Actual Hours";
                        worksheet.Cells[j + 4, 8].Value = "Regularized Hours";
                        worksheet.Cells[j + 5, 9].Value = "Remark";

                        int k = 5;
                        foreach (var perdayRecord in empList[j - 1].employeeAttendaceRecords.ListOfAttendanceRecordDTO)
                        {
                            worksheet.Cells[k, 1].Value = perdayRecord.Date;
                            worksheet.Cells[k, 2].Value = perdayRecord.Date.DayOfWeek;
                            worksheet.Cells[k, 3].Value = perdayRecord.TimeIn;
                            worksheet.Cells[k, 4].Value = perdayRecord.TimeOut;
                            worksheet.Cells[k, 5].Value = perdayRecord.LateBy;
                            worksheet.Cells[k, 6].Value = perdayRecord.OverTime;
                            worksheet.Cells[k, 7].Value = perdayRecord.WorkingHours;
                            worksheet.Cells[k, 8].Value = perdayRecord.RegularizedHours;
                            worksheet.Cells[k, 9].Value = perdayRecord.Remark;
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

    private List<EmployeeViewModel> GetAttendanceDataOfAllReporteesAndTeamLead(string fromDate, string toDate)
    {
        var employeeId = HttpContext.Session.GetInt32("ID") ?? 0;
        ReporteeService reporteeService = new ReporteeService(_employeeRepository);
        AttendanceService attendanceService = new AttendanceService(_accessEventRepository, _employeeRepository,
          _departmentRepository, _attendanceRegularizationRepository, _leavesRepository);

        var reportees = reporteeService.GetReporteesData(employeeId);
        var listOfReporteesAttendanceRecord = new List<EmployeeViewModel>();
        if (reportees.Count() != 0)
        {
            reportees.Add(reporteeService.GetTeamLeadData(employeeId));
            foreach (var reportee in reportees)
            {
                EmployeeViewModel employeeViewModel = new EmployeeViewModel();
                var listOfAttendanceRecord = new List<AttendanceRecordsDTO>();
                employeeViewModel.fromDate = DateTime.Parse(fromDate);
                employeeViewModel.toDate = DateTime.Parse(toDate);
                employeeViewModel.EmployeeId = reportee.ID;
                employeeViewModel.EmployeeName = string.Concat(reportee.FirstName, " ", reportee.LastName);

                employeeViewModel.employeeAttendaceRecords = attendanceService.AttendanceReportForDateRange
                    (reportee.ID, employeeViewModel.fromDate, employeeViewModel.toDate);

                employeeViewModel.employeeAttendaceRecords.ListOfAttendanceRecordDTO =
                    ConvertAttendanceRecordsTimeToIST(employeeViewModel.employeeAttendaceRecords.ListOfAttendanceRecordDTO);

                listOfReporteesAttendanceRecord.Add(employeeViewModel);
            }
            return listOfReporteesAttendanceRecord;
        }
        return null;
    }

        private List<PerDayAttendanceRecordDTO> ConvertAttendanceRecordsTimeToIST(List<PerDayAttendanceRecordDTO> listOfAttendanceRecord)
        {
            foreach (var attendanceRecord in listOfAttendanceRecord)
            {
                if (attendanceRecord.TimeIn.Hour != 0 || attendanceRecord.TimeIn.Minute != 0)
                {
                    attendanceRecord.TimeIn = ConvertTimeZone(attendanceRecord.Date, attendanceRecord.TimeIn);
                }
                if (attendanceRecord.TimeOut.Hour != 0 || attendanceRecord.TimeOut.Minute != 0)
                {
                    attendanceRecord.TimeOut = ConvertTimeZone(attendanceRecord.Date, attendanceRecord.TimeOut);
                }
            }

            return listOfAttendanceRecord;
        }
        private List<AccessPointRecord> ConvertAccessPointRecordsTimeToIST(DateTime date, List<AccessPointRecord> listOfAccessPointRecord)
        {
            foreach (var accessPointRecord in listOfAccessPointRecord)
            {
                if (accessPointRecord.TimeIn.Hour != 0 || accessPointRecord.TimeIn.Minute != 0)
                {
                    accessPointRecord.TimeIn = ConvertTimeZone(date, accessPointRecord.TimeIn);
                }
                if (accessPointRecord.TimeOut.Hour != 0 || accessPointRecord.TimeOut.Minute != 0)
                {
                    accessPointRecord.TimeOut = ConvertTimeZone(date, accessPointRecord.TimeOut);
                }
            }

            return listOfAccessPointRecord;
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private Time ConvertTimeZone(DateTime date, Time time)
        {
            DateTime TimeZone_UTC = new DateTime(date.Year, date.Month,
                      date.Day, time.Hour, time.Minute, 00);
            DateTime TimeZone_IST = TimeZoneInfo.ConvertTimeFromUtc(TimeZone_UTC,
                TimeZoneInfo.FindSystemTimeZoneById(TimeZoneInfo.Local.Id));
            Time convertedTime = new Time(TimeZone_IST.Hour, TimeZone_IST.Minute);
            return convertedTime;
        }
    }
}
