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

            //leaveViewModel.GetAppliedLeaves = mockData(); //delete it when UI is done
            return View(leaveViewModel);
        }
        [HttpPost]
        public IActionResult ApplyLeave(DateTime FromDate, DateTime ToDate, LeaveType LeaveType, string Remark)
        {
            var loggedInEmpId = HttpContext.Session.GetInt32("ID") ?? 0;
            var leaveService = new LeaveService(_leavesRepository);

            var response = leaveService.ApplyLeave(loggedInEmpId, FromDate, ToDate, LeaveType, Remark);

            return RedirectToAction("Index");
        }

       
       
        private List<LeaveRecordDTO> mockData()
        {
            List<LeaveRecordDTO> lst = new List<LeaveRecordDTO>();
            lst.Add(new LeaveRecordDTO {  Remark = "rkjhf kjdh kdjfh kjd",TypeOfLeave = LeaveType.CasualLeave});
            lst.Add(new LeaveRecordDTO {  Remark = "rkjhf kjdh kdjfh kjd",TypeOfLeave = LeaveType.CasualLeave});
            lst.Add(new LeaveRecordDTO {  Remark = "rkjhf kjdh kdjfh kjd",TypeOfLeave = LeaveType.CasualLeave});
            lst.Add(new LeaveRecordDTO {  Remark = "rkjhf kjdh kdjfh kjd",TypeOfLeave = LeaveType.CasualLeave});
            lst.Add(new LeaveRecordDTO {  Remark = "rkjhf kjdh kdjfh kjd",TypeOfLeave = LeaveType.CasualLeave});
            lst.Add(new LeaveRecordDTO {  Remark = "rkjhf kjdh kdjfh kjd",TypeOfLeave = LeaveType.CasualLeave });
            return lst;
        }
    }
}