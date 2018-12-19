using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Klipper.Web.Application.Reportee.Data_Access;
using Models.Core.Employment;

namespace Klipper.Web.Application.Reportee.Service
{
    public class Reportee : IReportee
    {
        private IReporteeAccessor _reporteeAccessor;

        public Reportee(IReporteeAccessor reporteeAccessor)
        {
            _reporteeAccessor = reporteeAccessor;
        }

        public Task<List<Employee>> GetReporteesByEmployeeID(int employeeId)
        {
            var reporteeData = _reporteeAccessor.GetReporteesByEmployeeId(employeeId);
            return reporteeData;
        }
    }
}
