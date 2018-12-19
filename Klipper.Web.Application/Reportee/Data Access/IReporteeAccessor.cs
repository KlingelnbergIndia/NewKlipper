using Models.Core.Employment;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Klipper.Web.Application.Reportee.Data_Access
{
    public interface IReporteeAccessor
    {
        Task<List<Employee>> GetReporteesByEmployeeId(int employeeId);
    }
}
