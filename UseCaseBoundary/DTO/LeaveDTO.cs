using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using static DomainModel.Leave;

namespace UseCaseBoundary.DTO
{
   
    public class LeaveDTO
    {

        public List<DateTime> Date;
        public LeaveType TypeOfLeave;
        public DateTime FromDate;
        public DateTime ToDate;
        public string Remark;
        public int NoOfDays;
    }
}
