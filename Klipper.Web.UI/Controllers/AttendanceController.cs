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

        //api/Attendance/41/2018-09-01/2018-10-01
        [HttpGet]
        [Route("{employeeId}/{startDate}/{endDate}")]
        public IActionResult Get(int employeeId, DateTime startDate, DateTime endDate)
        {


            return Ok(_attendanceService.GetAttendanceByDate(employeeId, startDate, endDate));
        }

        // api/Attendance/employeeId?employeeId=45
        [HttpGet]
        [Route("employeeId")]
        public async Task<IActionResult> Get(int employeeId)
        {


            return Ok(await _attendanceService.GetAttendance(employeeId));
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