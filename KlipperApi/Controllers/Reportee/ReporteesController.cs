using Microsoft.AspNetCore.Mvc;
using Models.Core.Employment;
using Models.Core.HR.Attendance;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KlipperApi.Controllers.Reportee
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReporteesController : ControllerBase
    {
        private IReporteesAccessor _reporteesAccessor;


        public ReporteesController(IReporteesAccessor reporteesAccessor)
        {
            _reporteesAccessor = reporteesAccessor;
        }




        [HttpGet("{id}")]
        public async Task<IActionResult> GetByEmployeeId(int employeeId)
        {
            if (employeeId < 0)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var reprteed = await _reporteesAccessor.GetReporteesByEmployeeIDAsync(employeeId) as List<Employee>;

            if (reprteed == null)
            {
                return NotFound();
            }
            return Ok(reprteed);
        }
    }
}
