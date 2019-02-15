using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModel
{
    public class CarryForwardLeaves
    {
        public int EmployeeId { get; set; }
        public DateTime LeaveBalanceTillDate { get; set; }

        public int TakenCasualLeaves { get; set; }
        public int TakenSickLeaves { get; set; }
        public int TakenCompoffLeaves { get; set; }

        public int MaxCasualLeaves { get; set; }
        public int MaxSickLeaves { get; set; }
        public int MaxCompoffLeaves { get; set; }
    }
}
