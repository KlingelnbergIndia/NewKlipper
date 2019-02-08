using System;
using System.Collections.Generic;
using System.Text;
using UseCaseBoundary.Model;

namespace UseCaseBoundary.DTO
{
    public class RegularizationDTO
    {
        public int EmployeeID { get; set; }
        public DateTime RegularizedDate { get; set; }
        public string Remark { get; set; }
        public Time ReguralizedHours { get; set; }
    }
}
