using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Klipper.Web.Application.Attendance.Service;
using Microsoft.AspNetCore.Mvc;

namespace Klipper.Web.UI.Controllers
{
    [Route("api/[controller]")]
    public class AttendanceController : Controller
    {
        private IAttendanceService _attendanceService;
        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        public IActionResult Index()
        {
            return View();
        }

        // api/Attendance/employeeId?employeeId=45
        [HttpGet]
        [Route("employeeId")]
        public async Task<IActionResult> Get(int employeeId)
        {
            var result= await _attendanceService.GetAttendance(employeeId, 7, "India Standard Time");
            return Ok(result);
        }

        //// api/Attendance/date?date=16-10-2018
        //[HttpGet]
        //[Route("date")]
        //public async Task<IActionResult> Get(String date)
        //{


        //    return Ok(await _attendanceService.GetAttendanceDetailByDate(date));
        //}
    }
}