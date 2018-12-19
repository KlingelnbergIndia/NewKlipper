using System;
using System.Collections.Generic;
using System.Text;
using Models.Core.Employment;

namespace Klipper.Web.Application.Reportee.Service
{
    public class Reportee : IReportee
    {
        private IReportee _reportee;

        public Reportee(IReportee reportee)
        {
            _reportee = reportee;
        }

        public List<Employee> GetReporteesByEmployeeID(int employeeId)
        {
            var reporteeData = _reportee.GetReporteesByEmployeeID(employeeId);
            return reporteeData;
        }
    }
}
