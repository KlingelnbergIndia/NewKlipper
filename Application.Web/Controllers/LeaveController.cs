using Application.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UseCaseBoundary;
using UseCases;

namespace Application.Web.Controllers
{
    public class LeaveController : Controller
    {
        private IEmployeeRepository _employeeRepository;

        public LeaveController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public IActionResult Index()
        {
            var leaveViewModel = new LeaveViewModel();
            var employeeId = HttpContext.Session.GetInt32("ID") ?? 0;

            ReporteeService reporteeService = new ReporteeService(_employeeRepository);

            var reportees = reporteeService.GetReporteesData(employeeId);

            if (reportees.Count != 0)
            {
                foreach (var reportee in reportees)
                {
                    string reporteeNameWithId = reportee.FirstName + " " + reportee.LastName + " - " + reportee.ID;
                    leaveViewModel.reportees.Add(reporteeNameWithId);
                }
            }

          
            //leaveViewModel.ShowReporteesPanel();
            return View(leaveViewModel);
        }

        
    }
}