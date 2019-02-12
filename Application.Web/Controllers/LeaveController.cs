using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UseCaseBoundary;
using UseCaseBoundary.DTO;
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
            //leaveViewModel.GetAppliedLeaves = leaveService.GetAppliedLeaves(loggedInEmpId);  
            leaveViewModel.GetAppliedLeaves = mockData(); //delete it when UI is done
            return View(leaveViewModel);
        }

        private List<LeaveDTO> mockData()
        {
            List<LeaveDTO> lst = new List<LeaveDTO>();
            lst.Add(new LeaveDTO { FromDate = DateTime.Now, ToDate = DateTime.Now, Remark = "rkjhf kjdh kdjfh kjd", TypeOfLeave = LeaveDTO.LeaveType.CasualLeave });
            lst.Add(new LeaveDTO { FromDate = DateTime.Now, ToDate = DateTime.Now, Remark = "rkjhf kjdh kdjfh kjd", TypeOfLeave = LeaveDTO.LeaveType.CasualLeave });
            lst.Add(new LeaveDTO { FromDate = DateTime.Now, ToDate = DateTime.Now, Remark = "rkjhf kjdh kdjfh kjd", TypeOfLeave = LeaveDTO.LeaveType.CasualLeave });
            lst.Add(new LeaveDTO { FromDate = DateTime.Now, ToDate = DateTime.Now, Remark = "rkjhf kjdh kdjfh kjd", TypeOfLeave = LeaveDTO.LeaveType.CasualLeave });
            lst.Add(new LeaveDTO { FromDate = DateTime.Now, ToDate = DateTime.Now, Remark = "rkjhf kjdh kdjfh kjd", TypeOfLeave = LeaveDTO.LeaveType.CasualLeave });
            lst.Add(new LeaveDTO { FromDate = DateTime.Now, ToDate = DateTime.Now, Remark = "rkjhf kjdh kdjfh kjd", TypeOfLeave = LeaveDTO.LeaveType.CasualLeave });
            return lst;
        }
    }
}