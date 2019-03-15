using System;

namespace UseCaseBoundary.DTO
{
    public class RegularizationData
    {
        public int EmployeeID { get; set; }
        public DateTime RegularizedDate { get; set; }
        public string Remark { get; set; }
        public int ReguralizedHours { get; set; }
    }
}
