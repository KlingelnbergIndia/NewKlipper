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
        public int BalanceOfCasualLeave;
        public int BalanceOfSickLeave;
        public int BalanceOfCompOffLeave;
    }
}
