using Models.Core.Employment;
using Models.Core.HR.Attendance;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KlipperApi.Controllers.Reportee
{
    public interface IReporteesAccessor
    {
        Task<List<Employee>> GetReporteesByEmployeeIDAsync(int employeeId);
    }
}
