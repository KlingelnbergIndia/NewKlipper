using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UseCaseBoundary.DTO;
using static DomainModel.Leave;

namespace Application.Web.Models
{
    public class LeaveViewModel
    {
        public LeaveDTO leaveDTO;

        public List<LeaveDTO> GetAppliedLeaves { get; set; }

        public IDictionary<int, string> GetAllLeaveTypes()
        {
            var dictionary = new Dictionary<int, string>();

            var enumerationType = typeof(LeaveType);
            foreach (int value in Enum.GetValues(enumerationType))
            {
                if (value == (int)LeaveType.EarnedLeave)
                    dictionary.Add(value, "Earned Leave");
                else if (value == (int)LeaveType.CasualLeave)
                    dictionary.Add(value, "Casual Leave");
                else if (value == (int)LeaveType.SickLeave)
                    dictionary.Add(value, "Sick Leave");
                else
                    dictionary.Add(value, ((LeaveType)value).ToString());
            }

            return dictionary;
        }

    }
}
