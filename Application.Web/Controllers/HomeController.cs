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

namespace Application.Web.Controllers
{
    [AuthenticateSession]
    public class HomeController : Controller
    {
        private IAccessEventsRepository _accessEventRepository;
        private IEmployeeRepository _employeeRepository;

        public HomeController(IAccessEventsRepository accessEventRepository,IEmployeeRepository employeeRepository)
        {
            _accessEventRepository = accessEventRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task<IActionResult> Index(string searchFilter)
        {
            var employeeId = HttpContext.Session.GetInt32("ID") ?? 0;
            AttendanceService attendanceService = new AttendanceService(_accessEventRepository);
            List<AttendanceRecordDTO> listOfAttendanceRecord = new List<AttendanceRecordDTO>();

            if (searchFilter == SearchFilter.AccessEventsByDateRange.ToString())
            {
                var fromDate = DateTime.Parse(HttpContext.Request.Form["fromDate"].ToString());
                var toDate = DateTime.Parse(HttpContext.Request.Form["toDate"].ToString());

                listOfAttendanceRecord = await attendanceService.GetAccessEventsForDateRange(employeeId, fromDate, toDate);

                ViewData["resultMessage"] = String.Format(
                    "Attendance from {0} to {1}. Total days:{2}", 
                    fromDate.ToShortDateString(), 
                    toDate.ToShortDateString(),
                    listOfAttendanceRecord.Count());
            }
            else
            {
                listOfAttendanceRecord = await attendanceService.GetAttendanceRecord(employeeId, 7);
            }

            listOfAttendanceRecord = ConvertRecordsTimeToIST(listOfAttendanceRecord);
            return View(listOfAttendanceRecord);
        }

        public async Task<IActionResult> Reportees()
        {
            
            var employeeId = HttpContext.Session.GetInt32("ID") ?? 0;
            ReporteeService reporteeService = new ReporteeService(_employeeRepository);

            var reportees =  reporteeService.GetReporteesData(employeeId);

            ReporteeViewModel reporteeViewModel = new ReporteeViewModel();
            AttendanceService attendanceService = new AttendanceService(_accessEventRepository);
            List<AttendanceRecordDTO> listOfAttendanceRecord = new List<AttendanceRecordDTO>();

            foreach (var reportee in reportees)
            {
                string reporteeNameWithId = reportee.FirstName + " " + reportee.LastName + " - " + reportee.ID;
                reporteeViewModel.reportees.Add(reporteeNameWithId);
            }

            //listOfAttendanceRecord =  await attendanceService.GetAttendanceRecord(reportees[0].ID, 7);
            //reporteeViewModel.attendaceRecords = ConvertRecordsTimeToIST(listOfAttendanceRecord);

            reporteeViewModel.Name = string.Empty;

            return View(reporteeViewModel);

        }

        [HttpPost]
        public async Task<IActionResult> GetSelectedreportee(string searchString)
        {

            var employeeId = HttpContext.Session.GetInt32("ID") ?? 0;
            ReporteeService reporteeService = new ReporteeService(_employeeRepository);

            var reportees = reporteeService.GetReporteesData(employeeId);
            ReporteeViewModel reporteeViewModel = new ReporteeViewModel();

            foreach (var reportee in reportees)
            {
                string reporteeNameWithId = reportee.FirstName + " " + reportee.LastName + " - " + reportee.ID;
                reporteeViewModel.reportees.Add(reporteeNameWithId);
            }

            string selectedReportee = Request.Form["selectMenu"].ToString();
            string idFromSelectedReportee = Regex.Match(selectedReportee, @"\d+").Value;

            reporteeViewModel.Name = Request.Form["selectMenu"].ToString();

            int reporteeId = int.Parse(idFromSelectedReportee);

            AttendanceService attendanceService = new AttendanceService(_accessEventRepository);

            List<AttendanceRecordDTO> listOfAttendanceRecord = new List<AttendanceRecordDTO>();
            listOfAttendanceRecord = await attendanceService.GetAttendanceRecord(reporteeId, 7);
            reporteeViewModel.attendaceRecords = ConvertRecordsTimeToIST(listOfAttendanceRecord);

            return View("Reportees", reporteeViewModel);
        }
        
        private List<AttendanceRecordDTO> ConvertRecordsTimeToIST(List<AttendanceRecordDTO> listOfAttendanceRecord)
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
