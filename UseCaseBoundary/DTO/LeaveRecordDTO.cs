using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using static DomainModel.Leave;

namespace UseCaseBoundary.DTO
{
   
    public class LeaveRecordDTO
    {
        public string LeaveId;
        public List<DateTime> Date;
        public LeaveType TypeOfLeave;
        public bool isHalfDayLeave;
        public HtmlString GetLeaveDisplayName()
        {
            return EnumHelperMethod.EnumDisplayNameFor(TypeOfLeave);
        }
        public DateTime FromDate;
        public DateTime ToDate;
        public string Remark;
        public StatusType Status;
        public HtmlString GetStatusDisplayName()
        {
            return EnumHelperMethod.EnumDisplayNameFor(Status);
        }
        public int NoOfDays;
        public bool IsRealizedLeave;
        public bool IsRecordSaved;
        public ServiceResponseDTO ServiceResponse;
    }

    public enum ServiceResponseDTO
    {
        Saved,
        Updated,
        Deleted,
        RecordExists,
        InvalidDays,
        CanNotApplied,
        RealizedLeave
    }
}
