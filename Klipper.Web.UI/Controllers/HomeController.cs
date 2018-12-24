using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Klipper.Web.Application.Attendance.Service;
using Microsoft.AspNetCore.Mvc;
using Klipper.Web.UI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Models.Core.HR.Attendance;

namespace Klipper.Web.UI.Controllers
{
    [AuthenticateSession]
    public class HomeController : Controller
    {
        private IAttendanceService _attendanceService;

        public HomeController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }
        public async Task<IActionResult> Index()
        {
            var employeeId = HttpContext.Session.GetInt32("ID");
            int id = employeeId ?? 0;
            var model = await _attendanceService.GetAttendance(id,7,"Indian standard format");
            return View((IEnumerable<AttendanceRecord>) model);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
