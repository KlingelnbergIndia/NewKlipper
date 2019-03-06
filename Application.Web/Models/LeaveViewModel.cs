
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using UseCaseBoundary.DTO;
using static DomainModel.Leave;

namespace Application.Web.Models
{
    public class LeaveViewModel
    {
        public LeaveRecordDTO leaveDTO;

        public List<LeaveRecordDTO> GetAppliedLeaves = new List<LeaveRecordDTO>();
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FromDate;
        public DateTime ToDate;
        public string Remark;
        public string IsHalfDay;
        public LeaveType LeaveType;
        public List<string> GetAllLeaveTypes()
        {
            var leaveTypes = new List<string>();
            var enumerationType = typeof(LeaveType);
            foreach (int value in Enum.GetValues(enumerationType))
            {
                if (value == (int)LeaveType.CompOff)
                    leaveTypes.Add(EnumHelperMethod.EnumDisplayNameFor(LeaveType.CompOff).ToString());
                else if (value == (int)LeaveType.CasualLeave)
                    leaveTypes.Add(EnumHelperMethod.EnumDisplayNameFor(LeaveType.CasualLeave).ToString());
                else if (value == (int)LeaveType.SickLeave)
                    leaveTypes.Add(EnumHelperMethod.EnumDisplayNameFor(LeaveType.SickLeave).ToString());
            }

            return leaveTypes;
        }
        public List<LeaveSummaryViewModel> LeaveSummary = new List<LeaveSummaryViewModel>();
    }
}
