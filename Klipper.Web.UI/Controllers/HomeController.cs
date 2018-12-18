using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Klipper.Web.UI.Models;
using Klipper.Web.UI.Services;

namespace Klipper.Web.UI.Controllers
{
    [AuthenticateSession]
    public class HomeController : Controller
    {
        private IEmployeeAttendanceRecordData _employeeAttendanceRecordData;
        public HomeController(IEmployeeAttendanceRecordData employeeAttendanceRecordData)
        {
            _employeeAttendanceRecordData = employeeAttendanceRecordData;
        }
        public IActionResult Index()
        {
            var model = new EmployeeAttendanceRecordViewModel();
            model.EmployeeAttendance_Record = _employeeAttendanceRecordData.GetEmployeeOneWeekAttendanceRecord();
            return View(model);
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
