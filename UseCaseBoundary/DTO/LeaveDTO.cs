using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using static DomainModel.Leave;

namespace UseCaseBoundary.DTO
{
    public class LeaveDTO
    {
        public DateTime Date;
        public LeaveType TypeOfLeave;
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
