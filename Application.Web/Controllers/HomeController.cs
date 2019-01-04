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
        //private int _employeeIdFromSession;
        public HomeController(IAccessEventsRepository accessEventRepository)
        {
            _accessEventRepository = accessEventRepository;
            //_employeeIdFromSession = HttpContext.Session.GetInt32("ID") ?? 0;
        }

        public async Task<IActionResult> Index(string searchFilter)
        {
            int employeeId = HttpContext.Session.GetInt32("ID") ?? 0;
            List<AttendanceRecordDTO> model;

            if (searchFilter == SearchFilter.AccessEventsByDateRange.ToString())
            {
                AttendanceRecordsOfEmployeeForDateRange attendanceRecords = new AttendanceRecordsOfEmployeeForDateRange(_accessEventRepository);
                model = await attendanceRecords.GetAttendanceRecord(employeeId, DateTime.Now.AddDays(-50), DateTime.Now);
            }
            else
            {
                AttendanceRecordForEmployeeID attendanceService = new AttendanceRecordForEmployeeID(_accessEventRepository);
                model = await attendanceService.GetAttendanceRecord(employeeId, 7);
            }

            return View(model);
        }

        //public async Task<IActionResult> Index(string id)
        //{
        //    AttendanceRecordsOfEmployeeForDateRange attendanceService = new AttendanceRecordsOfEmployeeForDateRange(_accessEventRepository);
        //    var model = await attendanceService.GetAttendanceRecord(_employeeIdFromSession, DateTime.Now.AddDays(-50), DateTime.Now);
        //    return View(model);
        //}


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
