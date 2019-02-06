using System;
using System.Collections.Generic;
using System.Text;

namespace UseCaseBoundary.DTO
{
    public class ReguralizationDTO
    {
        public int EmployeeID { get; set; }
        public DateTime RegularizedDate { get; set; }
        public string Remark { get; set; }
        public int ReguralizedHours { get; set; }
    }
}
