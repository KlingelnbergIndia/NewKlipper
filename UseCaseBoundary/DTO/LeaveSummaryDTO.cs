using System;
using System.Collections.Generic;
using System.Text;

namespace UseCaseBoundary.DTO
{
    public class LeaveSummaryDTO
    {
        public int TotalCasualLeaveTaken;
        public int TotalSickLeaveTaken;
        public int TotalCompOffLeaveTaken;
        public int RemainingCasualLeave;
        public int RemainingSickLeave;
        public int RemainingCompOffLeave;
        public int MaximumCasualLeave;
        public int MaximumSickLeave;
        public int MaximumCompOffLeave;
        public int LeaveBalance;
    }
}
