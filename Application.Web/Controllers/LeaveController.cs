using Application.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq;
using UseCaseBoundary;
using UseCases;

namespace Application.Web.Controllers
{
    public class LeaveController : Controller
    {
        private readonly ILeavesRepository _leavesRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public LeaveController(ILeavesRepository leavesRepository,IEmployeeRepository employeeRepository)
        {
            _leavesRepository = leavesRepository;
            _employeeRepository = employeeRepository;
        }
        public IActionResult Index()
        {
            var loggedInEmpId = HttpContext.Session.GetInt32("ID") ?? 0;
            var leaveService = new LeaveService(_leavesRepository);

            ReporteeService reporteeService = new ReporteeService(_employeeRepository);

            var reportees = reporteeService.GetReporteesData(loggedInEmpId);

  

            var leaveViewModel = new LeaveViewModel();
            leaveViewModel.GetAppliedLeaves = leaveService.GetAppliedLeaves(loggedInEmpId);
            leaveViewModel.ReporteesList = reportees;
            leaveViewModel.IsTeamLead = IsTeamLeadRole();
            return View(leaveViewModel);
        }

        private bool IsTeamLeadRole()
        {
            var user = HttpContext.Session.GetString("EmployeeRoles");
            var rolesJson = string.IsNullOrEmpty(user) ? "" : user;
            var EmployeeRolesList = JsonConvert.DeserializeObject<string[]>(rolesJson);
            if (EmployeeRolesList.Contains("TeamLeader"))
                return true;

            return false;
        }


    }
}