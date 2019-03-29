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
using Microsoft.AspNetCore.Http;
using Klipper.Web.UI;
using Application.Web.PageAccessAuthentication;
using UseCaseBoundary.DTO;
using OfficeOpenXml;
using System.Drawing;

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
        private ICompanyHolidayRepository _companyHolidayRepository;

        public HomeController(IAccessEventsRepository accessEventRepository,
            IEmployeeRepository employeeRepository,
            IDepartmentRepository departmentRepository,
            IAttendanceRegularizationRepository attendanceRegularizationRepository,
            ILeavesRepository leavesRepository,
            ICarryForwardLeaves carryForwardLeaves,
            ICompanyHolidayRepository companyHolidayRepository)
        {
            _accessEventRepository = accessEventRepository;
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _attendanceRegularizationRepository = attendanceRegularizationRepository;
            _leavesRepository = leavesRepository;
            _carryForwardLeaves = carryForwardLeaves;
            _companyHolidayRepository = companyHolidayRepository;
        }

        public IActionResult Index(string searchFilter)
        {
            var employeeId = HttpContext.Session.GetInt32("ID") ?? 0;
            var attendanceService = new AttendanceService(
                _accessEventRepository,
                _employeeRepository,
                _departmentRepository,
                _attendanceRegularizationRepository,
                _leavesRepository,
                _companyHolidayRepository);
            var employeeViewModel = GetAttendanceRecord(
                searchFilter, employeeId, attendanceService);

            ViewData["VisibilityReporteesTab"] =
                HttpContext.Session.GetString("VisibilityOfReporteesTab");

            return View(employeeViewModel);
        }


        [AuthenticateTeamLeaderRole]
        public IActionResult Reportees(string selectedViewTabs)
        {
            var employeeId = HttpContext.Session.GetInt32("ID") ?? 0;
            var reporteeService = new ReporteeService(_employeeRepository);
            var reportees = reporteeService.ReporteesData(employeeId);
            var reporteeViewModel = new ReporteeViewModel();

            if (reportees.Count != 0)
            {
                reportees = reportees.OrderBy(x => x.FirstName).ThenBy(x => x.LastName).ToList();
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
            reporteeViewModel.fromDate = DateTime.Now.AddDays
                (DayOfWeek.Monday - DateTime.Now.DayOfWeek);
            ViewBag.SelectedTab = selectedViewTabs;
            return View(reporteeViewModel);
        }

        [HttpPost]
        [AuthenticateTeamLeaderRole]
        public IActionResult Selectedreportee(string selectedViewTabs)
        {
            var employeeId = HttpContext.Session.GetInt32("ID") ?? 0;
            var reporteeService = new ReporteeService(_employeeRepository);

            var reportees = reporteeService.ReporteesData(employeeId);
            var reporteeViewModel = new ReporteeViewModel();

            reportees = reportees.OrderBy(x => x.FirstName).ThenBy(x => x.LastName).ToList();
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
                    AttendanceRecordOfReportee(reporteeViewModel, selectedReporteeId);
                }
                else
                {
                    LeaveRecordOfReportee(reporteeViewModel, selectedReporteeId);
                }

                reporteeViewModel.EmployeeId = selectedReporteeId;
            }
            else
            {
                reporteeViewModel.toDate = DateTime.Now.Date;
                reporteeViewModel.fromDate = DateTime.Now.AddDays
                    (DayOfWeek.Monday - DateTime.Now.DayOfWeek);
            }

            reporteeViewModel.leaveViewModel = Leave();
            ViewBag.SelectedTab = selectedViewTabs;
            return View("Reportees", reporteeViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> AccessPointDetail(DateTime date, int employeeId)
        {
            var attendanceService = new AttendanceService(
                _accessEventRepository, 
                _employeeRepository,
                _departmentRepository,
                _attendanceRegularizationRepository,
                _leavesRepository,
                _companyHolidayRepository);
            var listofaccesspointdetail = 
                await attendanceService.AccessPointDetails(employeeId, date);
            listofaccesspointdetail = ConvertAccessPointRecordsTimeToIST(
                date, listofaccesspointdetail);
            return View(listofaccesspointdetail);
        }

        [HttpPost]
        [AuthenticateTeamLeaderRole]
        public IActionResult RegularizedHours
            (DateTime date, int employeeId, DateTime timeToBeRegularize, string remark)
        {
            var attendanceService = new AttendanceService
            (_accessEventRepository, _employeeRepository,
                _departmentRepository, _attendanceRegularizationRepository,
                _leavesRepository, _companyHolidayRepository);
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
        [AuthenticateTeamLeaderRole]
        public FileResult ExportAttendanceReport(string fromDate, string toDate)
        {
            var stream = new System.IO.MemoryStream();
            using (ExcelPackage package = new ExcelPackage(stream))
            {
                var empList = AttendanceDataOfAllReporteesAndTeamLead(fromDate, toDate);

                var worksheet = package.Workbook.Worksheets.Add("AttendanceReport");
                worksheet.TabColor = Color.Gold;
                worksheet.DefaultColWidth = 20;
                int totalRows = empList.Count();
                int j = 1;
                foreach (var empdata in empList)
                {
                    FormateAttanceRecordEmployeeDetail(worksheet, j, empdata);

                    int z = HiglLightWithBlueColor(worksheet, j);

                    int k = FormatAttendanceRecord(worksheet, j, empdata, z);

                    j = k + 1;
                }

                package.Save();
            }
            string fileName =
                @"AttendanceReport_" + DateTime.Now.ToString("dd_MM") + ".xlsx";
            string fileType =
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            stream.Position = 0;
            return File(stream, fileType, fileName);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
                { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private EmployeeViewModel GetAttendanceRecord(
            string searchFilter,
            int employeeId,
            AttendanceService attendanceService)
        {
            var employeeViewModel = new EmployeeViewModel();
            if (searchFilter == SearchFilter.GetAccessEventsByDateRange.ToString())
            {
                FilterSelectedDateRangeAttendanceRecord
                    (employeeId, attendanceService, employeeViewModel);
            }
            else
            {
                CurrentWeekAttendanceRecord
                    (employeeId, attendanceService, employeeViewModel);
            }
            employeeViewModel.EmployeeId = employeeId;
            employeeViewModel
                .employeeAttendaceRecords
                .ListOfAttendanceRecordDTO = ConvertAttendanceRecordsTimeToIST(
                employeeViewModel.employeeAttendaceRecords.ListOfAttendanceRecordDTO);
            return employeeViewModel;
        }

        private static int HiglLightWithBlueColor(ExcelWorksheet worksheet, int j)
        {
            worksheet.Cells[j, 6].Style.Font.Color.SetColor(Color.Blue);
            worksheet.Cells[j + 1, 6].Style.Font.Color.SetColor(Color.Blue);
            worksheet.Cells[j + 2, 6].Style.Font.Color.SetColor(Color.Blue);

            int z = 4;
            worksheet.Cells[j + z, 1].Style.Font.Color.SetColor(Color.Black);
            worksheet.Cells[j + z, 2].Style.Font.Color.SetColor(Color.Black);
            worksheet.Cells[j + z, 3].Style.Font.Color.SetColor(Color.Black);
            worksheet.Cells[j + z, 4].Style.Font.Color.SetColor(Color.Black);
            worksheet.Cells[j + z, 5].Style.Font.Color.SetColor(Color.Black);
            worksheet.Cells[j + z, 6].Style.Font.Color.SetColor(Color.Black);
            worksheet.Cells[j + z, 7].Style.Font.Color.SetColor(Color.Black);
            worksheet.Cells[j + z, 8].Style.Font.Color.SetColor(Color.Black);
            worksheet.Cells[j + z, 9].Style.Font.Color.SetColor(Color.Black);
            return z;
        }

        private static void FormateAttanceRecordEmployeeDetail
            (ExcelWorksheet worksheet, int j, EmployeeViewModel empdata)
        {
            worksheet.Cells[j, 1].Value = "Employee ID";
            worksheet.Cells[j + 1, 1].Value = "Employee Holiday";
            worksheet.Cells[j, 2].Value = empdata.EmployeeId;
            worksheet.Cells[j + 1, 2].Value = empdata.EmployeeName;
            worksheet.Cells[j, 6].Value = "Estimated Hours";
            worksheet.Cells[j, 7].Value = string.Concat
            (empdata.employeeAttendaceRecords.EstimatedHours.Hour.ToString("D2"),
                ":",
                empdata.employeeAttendaceRecords.EstimatedHours.Minute.ToString("D2"));
            worksheet.Cells[j + 1, 6].Value = "Actual Hours";
            worksheet.Cells[j + 1, 7].Value = string.Concat
            (empdata.employeeAttendaceRecords.TotalWorkingHours.Hour.ToString("D2"),
                ":",
                empdata.employeeAttendaceRecords.TotalWorkingHours.Minute.ToString("D2"));
            worksheet.Cells[j + 2, 6].Value = "Difference";
            var difference = new Time
            (empdata.employeeAttendaceRecords.TotalDeficitOrExtraHours.Hour,
                Math.Abs
                    (empdata.employeeAttendaceRecords.TotalDeficitOrExtraHours.Minute));
            worksheet.Cells[j + 2, 7].Value = string.Concat
            (difference.Hour.ToString("D2"),
                ":",
                difference.Minute.ToString("D2"));
        }

        private static int FormatAttendanceRecord
            (ExcelWorksheet worksheet, int j, EmployeeViewModel empdata, int z)
        {
            worksheet.Cells[j + z, 1].Value = "Date";
            worksheet.Cells[j + z, 2].Value = "Day";
            worksheet.Cells[j + z, 3].Value = "Time In";
            worksheet.Cells[j + z, 4].Value = "Time Out";
            worksheet.Cells[j + z, 5].Value = "Deficit Time";
            worksheet.Cells[j + z, 6].Value = "Over Time";
            worksheet.Cells[j + z, 7].Value = "Actual Hours";
            worksheet.Cells[j + z, 8].Value = "Regularized Hours";
            worksheet.Cells[j + z, 9].Value = "Remark";

            int k = j + z + 1;
            foreach (var perdayRecord in empdata
                .employeeAttendaceRecords
                .ListOfAttendanceRecordDTO)
            {
                worksheet.Cells[k, 1].Value =
                    perdayRecord.Date.ToString("yyyy-MM-dd");
                worksheet.Cells[k, 2].Value =
                    perdayRecord.Date.DayOfWeek;
                worksheet.Cells[k, 3].Value = string.Concat
                (perdayRecord.TimeIn.Hour.ToString("D2"), ":",
                    perdayRecord.TimeIn.Minute.ToString("D2"));
                worksheet.Cells[k, 4].Value = string.Concat
                (perdayRecord.TimeOut.Hour.ToString("D2"), ":",
                    perdayRecord.TimeOut.Minute.ToString("D2"));
                worksheet.Cells[k, 5].Value = string.Concat
                (perdayRecord.LateBy.Hour.ToString("D2"), ":",
                    perdayRecord.LateBy.Minute.ToString("D2"));
                worksheet.Cells[k, 6].Value = string.Concat
                (perdayRecord.OverTime.Hour.ToString("D2"), ":",
                    perdayRecord.OverTime.Minute.ToString("D2"));
                worksheet.Cells[k, 7].Value = string.Concat
                (perdayRecord.WorkingHours.Hour.ToString("D2"), ":",
                    perdayRecord.WorkingHours.Minute.ToString("D2"));
                worksheet.Cells[k, 8].Value = string.Concat
                (perdayRecord.RegularizedHours.Hour.ToString("D2"), ":",
                    perdayRecord.RegularizedHours.Minute.ToString("D2"));
                worksheet.Cells[k, 9].Value = perdayRecord.Remark;
                k++;
            }

            return k;
        }

        private static void CurrentWeekAttendanceRecord
        (int employeeId, AttendanceService attendanceService,
            EmployeeViewModel employeeViewModel)
        {
            var toDate = DateTime.Now.Date;
            var fromDate = toDate.AddDays(DayOfWeek.Monday - toDate.DayOfWeek);

            employeeViewModel.toDate = toDate;
            employeeViewModel.fromDate = fromDate;

            employeeViewModel.employeeAttendaceRecords =
                attendanceService
                    .AttendanceReportForDateRange(employeeId, fromDate, toDate);
        }

        private void FilterSelectedDateRangeAttendanceRecord
        (int employeeId, AttendanceService attendanceService,
            EmployeeViewModel employeeViewModel)
        {
            string fromDate = HttpContext.Request.Form["fromDate"].ToString();
            string toDate = HttpContext.Request.Form["toDate"].ToString();

            employeeViewModel.fromDate = DateTime.Parse(fromDate);
            employeeViewModel.toDate = DateTime.Parse(toDate);
            if (employeeViewModel.fromDate > employeeViewModel.toDate)
            {
                TempData["errorMessage"] = "Please select valid date range !";
            }

            employeeViewModel.employeeAttendaceRecords =
                attendanceService.AttendanceReportForDateRange
                    (employeeId, employeeViewModel.fromDate, employeeViewModel.toDate);

            ViewData["resultMessage"] = String.Format(
                "Attendance from {0} to {1}. Total days:{2}",
                employeeViewModel.fromDate.ToShortDateString(),
                employeeViewModel.toDate.ToShortDateString(),
                employeeViewModel.employeeAttendaceRecords.ListOfAttendanceRecordDTO
                    .Count());
        }

        private void AttendanceRecordOfReportee
            (ReporteeViewModel reporteeViewModel, int selectedReporteeId)
        {
            string fromDate = Request.Form["fromDate"].ToString();
            string toDate = Request.Form["toDate"].ToString();

            if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
            {
                reporteeViewModel.fromDate = DateTime.Parse(fromDate);
                reporteeViewModel.toDate = DateTime.Parse(toDate);

                var attendanceService = new AttendanceService
                (_accessEventRepository, _employeeRepository,
                    _departmentRepository, _attendanceRegularizationRepository,
                    _leavesRepository , _companyHolidayRepository);

                var listOfAttendanceRecord = attendanceService
                    .AttendanceReportForDateRange(selectedReporteeId,
                        reporteeViewModel.fromDate, reporteeViewModel.toDate);
                reporteeViewModel.AttendaceRecordsOfSelectedReportee =
                    listOfAttendanceRecord;

                reporteeViewModel.AttendaceRecordsOfSelectedReportee
                        .ListOfAttendanceRecordDTO =
                    ConvertAttendanceRecordsTimeToIST
                        (listOfAttendanceRecord.ListOfAttendanceRecordDTO);
                reporteeViewModel.AttendanceFormName =
                    reporteeViewModel.reportees[selectedReporteeId];
                reporteeViewModel.SelectedEmpIdForAttendanceTab = selectedReporteeId;

            }
        }

        private void LeaveRecordOfReportee
            (ReporteeViewModel reporteeViewModel, int selectedReporteeId)
        {
            reporteeViewModel.toDate = DateTime.Now.Date;
            reporteeViewModel.fromDate = DateTime.Now.AddDays
                (DayOfWeek.Monday - DateTime.Now.DayOfWeek);

            var leaveService = new LeaveService
            (_leavesRepository, _employeeRepository,
                _departmentRepository, _carryForwardLeaves);

            reporteeViewModel.leaveRecordsOfSelectedReportee =
                leaveService.AppliedLeaves(selectedReporteeId);

            var leaveSummary = leaveService.TotalSummary(selectedReporteeId);
            if (leaveSummary != null)
                reporteeViewModel.LeaveSummary = reporteeViewModel
                    .ConvertToLeaveSummaryViewModel(leaveSummary);

            reporteeViewModel.LeaveFormName = reporteeViewModel.reportees[selectedReporteeId];
            reporteeViewModel.SelectedEmpIdForLeaveTab = selectedReporteeId;
        }

        private LeaveViewModel Leave()
        {
            var loggedInEmpId = HttpContext.Session.GetInt32("ID") ?? 0;
            var leaveService = new LeaveService
            (_leavesRepository, _employeeRepository, _departmentRepository,
                _carryForwardLeaves);

            var leaveViewModel = new LeaveViewModel();
            leaveViewModel.GetAppliedLeaves = leaveService.AppliedLeaves(loggedInEmpId);

            var leaveSummary = leaveService.TotalSummary(loggedInEmpId);
            if (leaveSummary != null)
                leaveViewModel.LeaveSummary = new ReporteeViewModel()
                    .ConvertToLeaveSummaryViewModel(leaveSummary);
            return leaveViewModel;
        }

        private List<EmployeeViewModel> AttendanceDataOfAllReporteesAndTeamLead
            (string fromDate, string toDate)
        {
            var employeeId = HttpContext.Session.GetInt32("ID") ?? 0;
            var reporteeService = new ReporteeService(_employeeRepository);
            var attendanceService = new AttendanceService
            (_accessEventRepository, _employeeRepository,
                _departmentRepository, _attendanceRegularizationRepository,
                _leavesRepository, _companyHolidayRepository);

            var reportees = reporteeService.ReporteesData(employeeId);
            var listOfReporteesAttendanceRecord = new List<EmployeeViewModel>();
            if (reportees.Count() != 0)
            {
                reportees.Add(reporteeService.TeamLeadData(employeeId));
                foreach (var reportee in reportees.OrderBy(i=>i.ID))
                {
                    GenerateAttendanceDataInEmployeeViewModel
                        (fromDate, toDate, attendanceService, listOfReporteesAttendanceRecord, reportee);
                }

                return listOfReporteesAttendanceRecord;
            }

            return null;
        }

    private void GenerateAttendanceDataInEmployeeViewModel
        (string fromDate, string toDate, AttendanceService attendanceService, 
        List<EmployeeViewModel> listOfReporteesAttendanceRecord, ReporteeDTO reportee)
        {
            var employeeViewModel = new EmployeeViewModel();
            var listOfAttendanceRecord = new List<AttendanceRecordsDTO>();
            employeeViewModel.fromDate = DateTime.Parse(fromDate);
            employeeViewModel.toDate = DateTime.Parse(toDate);
            employeeViewModel.EmployeeId = reportee.ID;
            employeeViewModel.EmployeeName = string
                .Concat(reportee.FirstName, " ", reportee.LastName);

            employeeViewModel.employeeAttendaceRecords =
                attendanceService.
                    AttendanceReportForDateRange
                (reportee.ID, employeeViewModel.fromDate, employeeViewModel.toDate);

            employeeViewModel.
                    employeeAttendaceRecords
                    .ListOfAttendanceRecordDTO =
                ConvertAttendanceRecordsTimeToIST
                    (employeeViewModel.employeeAttendaceRecords.ListOfAttendanceRecordDTO);

            listOfReporteesAttendanceRecord.Add(employeeViewModel);
        }

        private List<PerDayAttendanceRecordDTO> ConvertAttendanceRecordsTimeToIST
            (List<PerDayAttendanceRecordDTO> listOfAttendanceRecord)
        {
            foreach (var attendanceRecord in listOfAttendanceRecord)
            {
                if (attendanceRecord.TimeIn.Hour != 0 || attendanceRecord.TimeIn.Minute != 0)
                {
                    attendanceRecord.TimeIn = 
                        ConvertTimeZone(attendanceRecord.Date, attendanceRecord.TimeIn);
                }
                if (attendanceRecord.TimeOut.Hour != 0 || attendanceRecord.TimeOut.Minute != 0)
                {
                    attendanceRecord.TimeOut = 
                        ConvertTimeZone(attendanceRecord.Date, attendanceRecord.TimeOut);
                }
            }

            return listOfAttendanceRecord;
        }
        private List<AccessPointRecord> ConvertAccessPointRecordsTimeToIST
            (DateTime date, List<AccessPointRecord> listOfAccessPointRecord)
        {
            foreach (var accessPointRecord in listOfAccessPointRecord)
            {
                if (accessPointRecord.TimeIn.Hour != 0 || accessPointRecord.TimeIn.Minute != 0)
                {
                    accessPointRecord.TimeIn = 
                        ConvertTimeZone(date, accessPointRecord.TimeIn);
                }
                if (accessPointRecord.TimeOut.Hour != 0 || accessPointRecord.TimeOut.Minute != 0)
                {
                    accessPointRecord.TimeOut = 
                        ConvertTimeZone(date, accessPointRecord.TimeOut);
                }
            }

            return listOfAccessPointRecord;
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
