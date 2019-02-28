using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Application.Web.Models;
using Application.Web.PageAccessAuthentication;
using Klipper.Web.UI;
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
        private IEmployeeRepository _employeeRepository;
        private IDepartmentRepository _departmentRepository;
        private ICarryForwardLeaves _carryForwardLeaves;

        public LeaveController(
            ILeavesRepository leavesRepository, 
            IEmployeeRepository employeeRepository,
            IDepartmentRepository departmentRepository, 
            ICarryForwardLeaves carryForwardLeaves)
        {
            _leavesRepository = leavesRepository;
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _carryForwardLeaves = carryForwardLeaves;

        }
        public IActionResult Index()
        {
            var loggedInEmpId = HttpContext.Session.GetInt32("ID") ?? 0;
            var leaveService = new UseCases.LeaveService(_leavesRepository, _employeeRepository, _departmentRepository, _carryForwardLeaves);

            var leaveViewModel = new LeaveViewModel();
            leaveViewModel.GetAppliedLeaves = leaveService.GetAppliedLeaves(loggedInEmpId);

            var leaveSummary = leaveService.GetTotalSummary(loggedInEmpId);
            leaveViewModel.LeaveSummary = new ReporteeViewModel()
                .ConvertToLeaveSummaryViewModel(leaveSummary);
           

            return View(leaveViewModel);
        }
        [HttpPost]
        public IActionResult ApplyLeave(DateTime FromDate, DateTime ToDate, LeaveType LeaveType, string Remark)
        {
            if (FromDate > ToDate)
            {
                TempData["errorMessage"] = "Please select valid date range !";
                return RedirectToAction("Index");
            }
            string leaveType = HttpContext.Request.Form["leaveList"].ToString();
            if (int.Parse(leaveType) == 0)
                LeaveType = LeaveType.CompOff;
            else if (int.Parse(leaveType) == 1)
                LeaveType = LeaveType.CasualLeave;
            else if (int.Parse(leaveType) == 2)
                LeaveType = LeaveType.SickLeave;

            var loggedInEmpId = HttpContext.Session.GetInt32("ID") ?? 0;
            var leaveService = new UseCases.LeaveService(_leavesRepository, _employeeRepository, _departmentRepository, _carryForwardLeaves);
            var response = leaveService.ApplyLeave(loggedInEmpId, FromDate, ToDate, LeaveType, Remark);

            if (response == ServiceResponseDTO.Saved)
                TempData["responseMessage"] = "Leave applied sucessfully !";
            else if (response == ServiceResponseDTO.RecordExists)
                TempData["errorMessage"] = "Leave record is available for selected date !";
            else if (response == ServiceResponseDTO.InvalidDays)
                TempData["errorMessage"] = "Please select valid date range !";
            else if(response == ServiceResponseDTO.CanNotApplied)
                TempData["errorMessage"] = "Selected leave is not available !";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateLeave(string leaveId,DateTime FromDate, DateTime ToDate, LeaveType LeaveType, string Remark)
        {
            string ExistingLeaveId = HttpContext.Request.Form["leaveId"].ToString();
            if (leaveId != null)
            {
                if (FromDate > ToDate)
                {
                    TempData["errorMessage"] = "Please select valid date range !";
                    return RedirectToAction("Index");
                }
                string leaveType = HttpContext.Request.Form["leaveList"].ToString();
                if (int.Parse(leaveType) == 0)
                    LeaveType = LeaveType.CompOff;
                else if (int.Parse(leaveType) == 1)
                    LeaveType = LeaveType.CasualLeave;
                else if (int.Parse(leaveType) == 2)
                    LeaveType = LeaveType.SickLeave;

                var loggedInEmpId = HttpContext.Session.GetInt32("ID") ?? 0;
                var leaveService = new UseCases.LeaveService(_leavesRepository, _employeeRepository, _departmentRepository, _carryForwardLeaves);
                var response = leaveService.UpdateLeave(ExistingLeaveId, loggedInEmpId, FromDate,ToDate,LeaveType,Remark);

                if (response == ServiceResponseDTO.Updated)
                    TempData["responseMessage"] = "Leave updated sucessfully !";
                else if (response == ServiceResponseDTO.RecordExists)
                    TempData["errorMessage"] = "Leave record is available for selected date !";
                else if (response == ServiceResponseDTO.InvalidDays)
                    TempData["errorMessage"] = "Please select valid date range !";
                else if (response == ServiceResponseDTO.CanNotApplied)
                    TempData["errorMessage"] = "Selected leave is not available!";
                else if (response == ServiceResponseDTO.Deleted)
                    TempData["errorMessage"] = "Cancelled leaves can not be updated !";
                else if (response == ServiceResponseDTO.RealizedLeave)
                    TempData["errorMessage"] = "Passed leaves can not be updated !";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult CancelLeave(string LeaveId)
        {
            var leaveService = new UseCases.LeaveService(_leavesRepository, _employeeRepository, _departmentRepository, _carryForwardLeaves);
            var loggedInEmpId = HttpContext.Session.GetInt32("ID") ?? 0;
            var response = leaveService.CancelLeave(LeaveId, loggedInEmpId);

            if (response == ServiceResponseDTO.Deleted)
                TempData["responseMessage"] = "Leave deleted sucessfully !";
            else if (response == ServiceResponseDTO.RealizedLeave)
                TempData["errorMessage"] = "Passed leaves can not be cancelled !";
            else
            TempData["errorMessage"] = "Cancelled leaves can not be updated !";

            return RedirectToAction("Index");
        }
    }
}