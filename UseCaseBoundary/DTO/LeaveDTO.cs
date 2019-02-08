using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public DateTime Date;
        public LeaveType Leave;
        public string Remark;
        public IEnumerable<SelectListItem> LeaveTypeList;

        //public LeaveDTO()
        //{
        //    LeaveTypeList = Enum.GetNames(typeof(LeaveType)).Select(name => new SelectListItem()
        //    {
        //        Text = name,
        //        Value = name
        //    });
        //}
    }
}
