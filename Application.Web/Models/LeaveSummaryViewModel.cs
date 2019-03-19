using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UseCaseBoundary.DTO;
using static DomainModel.Leave;

namespace Application.Web.Models
{
    public class LeaveSummaryViewModel
    {
        public string LeaveType;
        public float TotalAvailableLeave;
        public float LeaveTaken;
        public float RemainingLeave;
    }
}
