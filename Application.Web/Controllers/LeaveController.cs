using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Application.Web.Models;
using Application.Web.PageAccessAuthentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UseCaseBoundary;
using UseCaseBoundary.DTO;
using UseCases;
using static DomainModel.Leave;

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
        [HttpPost]
        public IActionResult ApplyLeave(DateTime FromDate, DateTime ToDate, LeaveType LeaveType, string Remark)
        {
            if (FromDate > ToDate)
            {
                TempData["errorMessage"] = "From-date should not be greater than To-Date !";
                return RedirectToAction("Index");
            }

            var loggedInEmpId = HttpContext.Session.GetInt32("ID") ?? 0;
            var leaveService = new LeaveService(_leavesRepository);
            var response = leaveService.ApplyLeave(loggedInEmpId, FromDate, ToDate, LeaveType, Remark);

            if (response == ServiceResponseDTO.Saved)
                TempData["responseMessage"] = "Your Leave is submitted !";
            else if (response == ServiceResponseDTO.RecordExists)
                TempData["responseMessage"] = "Your Leave is already submitted !";
            
            return RedirectToAction("Index");
        }
    }
}