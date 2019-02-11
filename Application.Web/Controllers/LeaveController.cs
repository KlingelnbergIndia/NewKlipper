using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Klipper.Web.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UseCaseBoundary;
using UseCases;

namespace Application.Web.Controllers
{
    [AuthenticateSession]
    public class LeaveController : Controller
    {
        private readonly ILeavesRepository _leavesRepository;
        public LeaveController(ILeavesRepository leavesRepository)
        {
            _leavesRepository = leavesRepository;
            
        }
        public IActionResult Index()
        {
            var loggedInEmpId = HttpContext.Session.GetInt32("ID") ?? 0;
            var leaveService = new LeaveService(_leavesRepository);

            ViewBag.AllLeavesList = leaveService.GetLeaves(loggedInEmpId);
            return View();
        }
    }
}