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

        public async Task<IActionResult> Index()
        {
            var employeeId = HttpContext.Session.GetInt32("ID") ?? 0;           
            
            AttendanceRecordForEmployeeID attendanceService = new AttendanceRecordForEmployeeID(_accessEventRepository);
            var listOfAttendanceRecord = await attendanceService.GetAttendanceRecord(employeeId, 7);
            foreach (var attendanceRecord in listOfAttendanceRecord)
            {
                if (attendanceRecord.TimeIn._hour != 0)
                {
                    DateTime TimeInByUTC = new DateTime(2018, 12, 10, attendanceRecord.TimeIn._hour, attendanceRecord.TimeIn._minute, 00);
                    DateTime TimeInByIST = TimeZoneInfo.ConvertTimeFromUtc(TimeInByUTC, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                    Time timeIn = new Time(TimeInByIST.Hour, TimeInByIST.Minute);
                    attendanceRecord.TimeIn = timeIn;
                }
                if (attendanceRecord.TimeOut._hour != 0)
                {
                    DateTime TimeOutByUTC = new DateTime(2018, 12, 10, attendanceRecord.TimeOut._hour, attendanceRecord.TimeOut._minute, 00);
                    DateTime TimeOutByIST = TimeZoneInfo.ConvertTimeFromUtc(TimeOutByUTC, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                    Time timeOut = new Time(TimeOutByIST.Hour, TimeOutByIST.Minute);
                    attendanceRecord.TimeOut = timeOut;
                }
            }
            return View(listOfAttendanceRecord);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
