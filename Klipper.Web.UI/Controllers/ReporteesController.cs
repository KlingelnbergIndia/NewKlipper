using Klipper.Web.Application.Reportee.Data_Access;
using Klipper.Web.Application.Reportee.Service;
using Microsoft.AspNetCore.Mvc;
using Models.Core.Employment;
using Models.Core.Reportees;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Klipper.Web.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReporteesController : Controller
    {
        private IReportee _reportee;


        public ReporteesController(IReportee reportee)
        {
            _reportee = reportee;
        }

        //api/Reportees/41?employeeId=41
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReporteesbyId(int employeeId)
        {
            Reportees reportees = new Reportees();
            var reporteesData = await _reportee.GetReporteesByEmployeeID(employeeId) as List<Employee>;

            foreach(var singleReportee in reporteesData)
            {
                if(singleReportee!=null)
                {
                    reportees.FullNameOfReporteeswithTheirIds.Add(singleReportee.FirstName + " " + singleReportee.LastName + " - " + singleReportee.ID);
                }
            }

            return View(reportees);
        }
    }
}
