using Models.Core.Employment;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Klipper.Web.Application.Reportee.Service
{
    public interface IReportee
    {
        Task<List<Employee>> GetReporteesByEmployeeID(int imployeeId);
    }
}
