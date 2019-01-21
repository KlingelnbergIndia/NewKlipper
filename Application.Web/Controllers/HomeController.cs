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
    public class HomeController : Controller// : ApplicationController
    {
        private IAccessEventsRepository _accessEventRepository;
        private IEmployeeRepository _employeeRepository;

        public HomeController(IAccessEventsRepository accessEventRepository,IEmployeeRepository employeeRepository)
        {
            _accessEventRepository = accessEventRepository;
            _employeeRepository = employeeRepository;
            LoginViewModel loginViewModel = new LoginViewModel();
        }

        public async Task<IActionResult> Index(string searchFilter)
        {
            var employeeId = HttpContext.Session.GetInt32("ID") ?? 0;
            AttendanceService attendanceService = new AttendanceService(_accessEventRepository, _employeeRepository);

            EmployeeViewModel employeeViewModel = new EmployeeViewModel();

            if (searchFilter == SearchFilter.AccessEventsByDateRange.ToString())
            {
                string fromDate = HttpContext.Request.Form["fromDate"].ToString();
                string toDate = HttpContext.Request.Form["toDate"].ToString();

                employeeViewModel.fromDate = DateTime.Parse(fromDate);
                employeeViewModel.toDate= DateTime.Parse(toDate);

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
                employeeViewModel.employeeAttendaceRecords = 
                    await attendanceService.GetAttendanceRecord(employeeId, 7);
            }

            employeeViewModel
                .employeeAttendaceRecords
                .ListOfAttendanceRecordDTO = ConvertRecordsTimeToIST(employeeViewModel.employeeAttendaceRecords.ListOfAttendanceRecordDTO);
            return View(employeeViewModel);
        }

        [AuthenticateTeamLeaderRole]
        public IActionResult Reportees()
        {           
            var employeeId = HttpContext.Session.GetInt32("ID") ?? 0;
            ReporteeService reporteeService = new ReporteeService(_employeeRepository);

            var reportees =  reporteeService.GetReporteesData(employeeId);

            ReporteeViewModel reporteeViewModel = new ReporteeViewModel();
            AttendanceService attendanceService = new AttendanceService(_accessEventRepository, _employeeRepository);
            List<AttendanceRecordsDTO> listOfAttendanceRecord = new List<AttendanceRecordsDTO>();

            if (reportees.Count != 0)
            {
                foreach (var reportee in reportees)
                {
                    string reporteeNameWithId = reportee.FirstName + " " + reportee.LastName + " - " + reportee.ID;
                    reporteeViewModel.reportees.Add(reporteeNameWithId);
                }
            }

            reporteeViewModel.Name = string.Empty;

            return View(reporteeViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> GetSelectedreportee()
        {
            var employeeId = HttpContext.Session.GetInt32("ID") ?? 0;
            ReporteeService reporteeService = new ReporteeService(_employeeRepository);

            var reportees = reporteeService.GetReporteesData(employeeId);
            ReporteeViewModel reporteeViewModel = new ReporteeViewModel();

            if (reportees.Count != 0)
            {
                foreach (var reportee in reportees)
                {
                    string reporteeNameWithId = reportee.FirstName + " " + reportee.LastName + " - " + reportee.ID;
                    reporteeViewModel.reportees.Add(reporteeNameWithId);
                }
            }

            string selectedReportee = Request.Form["selectMenu"].ToString();

            string fromDate = Request.Form["fromDate"].ToString();
            string toDate = Request.Form["toDate"].ToString();

            reporteeViewModel.fromDate = DateTime.Parse(fromDate);
            reporteeViewModel.toDate = DateTime.Parse(toDate);
            string idFromSelectedReportee = Regex.Match(selectedReportee, @"\d+").Value;

            int reporteeId = int.Parse(string.IsNullOrEmpty(idFromSelectedReportee) ? "0" : idFromSelectedReportee);

            AttendanceService attendanceService = new AttendanceService(_accessEventRepository, _employeeRepository);
            AttendanceRecordsDTO listOfAttendanceRecord = new AttendanceRecordsDTO();

            if (reporteeId!=0)
            {
                reporteeViewModel.Name = Request.Form["selectMenu"].ToString();
                if(!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
                {
                    listOfAttendanceRecord = await attendanceService.GetAccessEventsForDateRange(reporteeId, 
                        reporteeViewModel.fromDate, reporteeViewModel.toDate);
                }
                else
                {
                    listOfAttendanceRecord = await attendanceService.GetAttendanceRecord(reporteeId, 7);
                }

                reporteeViewModel
                    .reporteesAttendaceRecords = listOfAttendanceRecord;

                reporteeViewModel
                    .reporteesAttendaceRecords
                    .ListOfAttendanceRecordDTO = ConvertRecordsTimeToIST(listOfAttendanceRecord.ListOfAttendanceRecordDTO);
                
            }
            
            return View("Reportees", reporteeViewModel);
        }
        
        private List<PerDayAttendanceRecordDTO> ConvertRecordsTimeToIST(List<PerDayAttendanceRecordDTO> listOfAttendanceRecord)
        {
            foreach (var attendanceRecord in listOfAttendanceRecord)
            {
                if (attendanceRecord.TimeIn.Hour != 0 && attendanceRecord.TimeIn.Minute != 0)
                {
                    attendanceRecord.TimeIn = ConvertTimeZone(attendanceRecord.Date, attendanceRecord.TimeIn);
                }
                if (attendanceRecord.TimeOut.Hour != 0 && attendanceRecord.TimeIn.Minute != 0)
                {
                    attendanceRecord.TimeOut = ConvertTimeZone(attendanceRecord.Date, attendanceRecord.TimeOut);
                }
            }

            return listOfAttendanceRecord;
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
