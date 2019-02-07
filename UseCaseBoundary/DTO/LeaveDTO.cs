using System;
using System.Collections.Generic;
using System.Text;

namespace UseCaseBoundary.DTO
{
    public enum LeaveType
    {
        EarnedLeave,
        CasualLeave,
        SickLeave
    }
    public class LeaveDTO
    {
        public DateTime fromDate;
        public DateTime toDate;
        public LeaveType leave;
        public string remark;
    }
}
