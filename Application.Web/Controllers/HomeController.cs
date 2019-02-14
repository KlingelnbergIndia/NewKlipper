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

        public HomeController(IAccessEventsRepository accessEventRepository, IEmployeeRepository employeeRepository,
            IDepartmentRepository departmentRepository, IAttendanceRegularizationRepository attendanceRegularizationRepository, ILeavesRepository leavesRepository)
        {
            _accessEventRepository = accessEventRepository;
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _attendanceRegularizationRepository = attendanceRegularizationRepository;
            _leavesRepository = leavesRepository;
        }

        public async Task<IActionResult> Index(string searchFilter)
        {
            var employeeId = HttpContext.Session.GetInt32("ID") ?? 0;
            AttendanceService attendanceService = new AttendanceService(_accessEventRepository, _employeeRepository,
                _departmentRepository, _attendanceRegularizationRepository);

            EmployeeViewModel employeeViewModel = new EmployeeViewModel();

            if (searchFilter == SearchFilter.AccessEventsByDateRange.ToString())
            {
                string fromDate = HttpContext.Request.Form["fromDate"].ToString();
                string toDate = HttpContext.Request.Form["toDate"].ToString();

                employeeViewModel.fromDate = DateTime.Parse(fromDate);
                employeeViewModel.toDate = DateTime.Parse(toDate);

                employeeViewModel.employeeAttendaceRecords =
                    await attendanceService.GetAccessEventsForDateRange(employeeId, employeeViewModel.fromDate, employeeViewModel.toDate);

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
                    await attendanceService.GetAccessEventsForDateRange(employeeId, fromDate, toDate);
            }

            employeeViewModel.EmployeeId = employeeId;

            employeeViewModel
                .employeeAttendaceRecords
                .ListOfAttendanceRecordDTO = ConvertAttendanceRecordsTimeToIST(employeeViewModel.employeeAttendaceRecords.ListOfAttendanceRecordDTO);

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
            AttendanceService attendanceService = new AttendanceService(_accessEventRepository, _employeeRepository,
                _departmentRepository, _attendanceRegularizationRepository);
            List<AttendanceRecordsDTO> listOfAttendanceRecord = new List<AttendanceRecordsDTO>();

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
        public async Task<IActionResult> GetSelectedreportee(string selectedViewTabs)
        {
            var employeeId = HttpContext.Session.GetInt32("ID") ?? 0;
            ReporteeService reporteeService = new ReporteeService(_employeeRepository);

            var reportees = reporteeService.GetReporteesData(employeeId);
            ReporteeViewModel reporteeViewModel = new ReporteeViewModel();

                foreach (var reportee in reportees)
                {
                    string reporteeName = reportee.FirstName + " " + reportee.LastName ;
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
                        _departmentRepository, _attendanceRegularizationRepository);

                        AttendanceRecordsDTO listOfAttendanceRecord = await attendanceService.GetAccessEventsForDateRange(selectedReporteeId,
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
                    LeaveService leaveService = new LeaveService(_leavesRepository, _employeeRepository,_departmentRepository);
                    List<LeaveRecordDTO> listOfLeaveRecord = leaveService.GetAppliedLeaves(selectedReporteeId);
                    reporteeViewModel.leaveRecordsOfSelectedReportee = listOfLeaveRecord;
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

            ViewBag.SelectedTab = selectedViewTabs;
            return View("Reportees", reporteeViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> AccessPointDetail(DateTime date, int employeeId)
        {
            AttendanceService attendanceService = new AttendanceService(_accessEventRepository, _employeeRepository,
                _departmentRepository, _attendanceRegularizationRepository);
            List<AccessPointRecord> listofaccesspointdetail = await attendanceService.GetAccessPointDetails(employeeId, date);
            listofaccesspointdetail = ConvertAccessPointRecordsTimeToIST(date, listofaccesspointdetail);
            return View(listofaccesspointdetail);
        }

        [HttpPost]
        [AuthenticateTeamLeaderRole]
        public IActionResult SaveRegularizedHours(DateTime date, int employeeId, DateTime timeToBeRegularize, string remark)
        {
            AttendanceService attendanceService = new AttendanceService(_accessEventRepository, _employeeRepository,
                _departmentRepository, _attendanceRegularizationRepository);
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
