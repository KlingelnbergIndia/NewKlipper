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

namespace Application.Web.Controllers
{
    public class HomeController : Controller
    {
        private IAccessEventsRepository _accessEventRepository;
        public HomeController(IAccessEventsRepository accessEventRepository)
        {
            _accessEventRepository = accessEventRepository;
        }
        public async Task<IActionResult> Index()
        {
            var employeeId = HttpContext.Session.GetInt32("ID");
            int id = employeeId ?? 0;
            AttendanceRecordForEmployeeID attendanceService = new AttendanceRecordForEmployeeID(_accessEventRepository);
            var model=await attendanceService.GetAttendanceRecord(id, 70);
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
