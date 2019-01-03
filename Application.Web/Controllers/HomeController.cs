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
            //var employeeId = HttpContext.Session.GetInt32("ID");
            var employeeId = HttpContext.Session.GetInt32("ID") ?? 0;
            AttendanceRecordForEmployeeID attendanceService = new AttendanceRecordForEmployeeID(_accessEventRepository);
            var listOfAttendanceRecord=await attendanceService.GetAttendanceRecord(employeeId, 7);
            foreach(var attendanceRecord in listOfAttendanceRecord)
            {
                if (attendanceRecord.TimeIn.Hour != 0)
                {
                    DateTime timeInByUTC = new DateTime(attendanceRecord.Date.Year, attendanceRecord.Date.Month,
                        attendanceRecord.Date.Day, attendanceRecord.TimeIn.Hour, attendanceRecord.TimeIn._minute, 00);
                    DateTime timeInByIST = TimeZoneInfo.ConvertTimeFromUtc(timeInByUTC, 
                        TimeZoneInfo.FindSystemTimeZoneById(TimeZone.CurrentTimeZone.StandardName));
                    Time timeIn = new Time(timeInByIST.Hour, timeInByIST.Minute);
                    attendanceRecord.TimeIn = timeIn;
                }
                if (attendanceRecord.TimeOut.Hour != 0)
                {
                    DateTime timeOutByUTC = new DateTime(attendanceRecord.Date.Year, attendanceRecord.Date.Month,
                        attendanceRecord.Date.Day, attendanceRecord.TimeOut.Hour, attendanceRecord.TimeOut._minute, 00);
                    DateTime timeOutByIST = TimeZoneInfo.ConvertTimeFromUtc(timeOutByUTC, 
                        TimeZoneInfo.FindSystemTimeZoneById(TimeZone.CurrentTimeZone.StandardName));
                    Time timeOut = new Time(timeOutByIST.Hour, timeOutByIST.Minute);
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
