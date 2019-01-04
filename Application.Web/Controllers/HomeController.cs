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

namespace Application.Web.Controllers
{
    [AuthenticateSession]
    public class HomeController : Controller
    {
        private IAccessEventsRepository _accessEventRepository;
        public HomeController(IAccessEventsRepository accessEventRepository)
        {
            _accessEventRepository = accessEventRepository;
        }

        public async Task<IActionResult> Index(string searchFilter)
        {
            var employeeId = HttpContext.Session.GetInt32("ID") ?? 0;
            List<AttendanceRecordDTO> listOfAttendanceRecord = new List<AttendanceRecordDTO>();

            if (searchFilter == SearchFilter.AccessEventsByDateRange.ToString())
            {
                AttendanceRecordsOfEmployeeForDateRange attendanceRecordForEmployee = new AttendanceRecordsOfEmployeeForDateRange(_accessEventRepository);
                listOfAttendanceRecord = await attendanceRecordForEmployee.GetAttendanceRecord(employeeId,DateTime.Now.AddDays(-50), DateTime.Now);
            }
            else
            {
                AttendanceRecordForEmployee attendanceRecordForEmployee = new AttendanceRecordForEmployee(_accessEventRepository);
                listOfAttendanceRecord = await attendanceRecordForEmployee.GetAttendanceRecord(employeeId, 7);
            }

            listOfAttendanceRecord = ConvertRecordsTimeToIST(listOfAttendanceRecord);
            return View(listOfAttendanceRecord);
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
