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

namespace Application.Web.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            IAccessEventsRepository accessEventRepository = new AccessEventRepository();
            //var employeeId = HttpContext.Session.GetInt32("ID");
            //int id = employeeId ?? 0;
            int id = 45;
            AttendanceRecordForEmployeeID attendanceService = new AttendanceRecordForEmployeeID(accessEventRepository);
            var model=await attendanceService.GetAttendanceRecord(id, 7);
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
