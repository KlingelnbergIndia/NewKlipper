using Models.Core.Employment;
using System;
using System.Collections.Generic;
using System.Text;

namespace Klipper.Web.Application.Reportee.Service
{
    public interface IReportee
    {
        List<Employee> GetReporteesByEmployeeID(int imployeeId);
    }
}
