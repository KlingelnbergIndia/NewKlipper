using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UseCaseBoundary;
using UseCases;

namespace Application.Web.Controllers
{
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

            var leaveViewModel = new LeaveViewModel();
            leaveViewModel.GetAppliedLeaves = leaveService.GetAppliedLeaves(loggedInEmpId);
            return View(leaveViewModel);
        }
    }
}