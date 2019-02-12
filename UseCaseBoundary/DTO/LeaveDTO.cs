using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using static DomainModel.Leave;

namespace UseCaseBoundary.DTO
{
   
    public class LeaveDTO
    {
        public enum LeaveType
        {
            EarnedLeave,
            CasualLeave,
            SickLeave
        }

        public DateTime FromDate;
        public DateTime ToDate;
        public int NoOfDays;
        public LeaveType TypeOfLeave;
        public string Remark;
    }
}
