
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

        public List<LeaveRecordDTO> GetAppliedLeaves { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FromDate;
        public DateTime ToDate;
        public string Remark;
        public string IsHalfDay;
        public LeaveType LeaveType;
        public IDictionary<int, string> GetAllLeaveTypes()
        {
            var dictionary = new Dictionary<int, string>();

            var enumerationType = typeof(LeaveType);
            foreach (int value in Enum.GetValues(enumerationType))
            {
                if (value == (int)LeaveType.CompOff)
                    dictionary.Add(value, "Comp-Off");
                else if (value == (int)LeaveType.CasualLeave)
                    dictionary.Add(value, "Casual Leave");
                else if (value == (int)LeaveType.SickLeave)
                    dictionary.Add(value, "Sick Leave");
                else
                    dictionary.Add(value, ((LeaveType)value).ToString());
            }

            return dictionary;
        }

        public List<LeaveSummaryViewModel> LeaveSummary;
    }
}
