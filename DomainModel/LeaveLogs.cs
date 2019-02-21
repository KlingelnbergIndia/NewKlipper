using System;
using System.Collections.Generic;
using System.Linq;
using static DomainModel.Leave;

namespace DomainModel
{
    public class LeaveLogs
    {
        private List<Leave> _listOfLeave;

        public LeaveLogs(List<Leave> listOfLeave)
        {
            _listOfLeave = listOfLeave;
        }

        public int CalculateCasualLeaveTaken()
        {
           int noOfCasualLeave = _listOfLeave.Where(x=>x.GetLeaveType()==LeaveType.CasualLeave && x.GetStatus()!= StatusType.Cancelled).Sum(x=>x.GetLeaveDate().Count);
            return noOfCasualLeave;
        }

        public int CalculateSickLeaveTaken()
        {
            int noOfSickLeave = _listOfLeave.Where(x => x.GetLeaveType() == LeaveType.SickLeave && x.GetStatus() != StatusType.Cancelled).Sum(x => x.GetLeaveDate().Count);
            return noOfSickLeave;
        }

        public int CalculateCompOffLeaveTaken()
        {
            int noOfCompOffLeave = _listOfLeave.Where(x => x.GetLeaveType() == LeaveType.CompOff && x.GetStatus() != StatusType.Cancelled).Sum(x => x.GetLeaveDate().Count);
            return noOfCompOffLeave;
        }

        public int GetTotalLeaveTaken()
        {
            return _listOfLeave.Where(x=> x.GetStatus() != StatusType.Cancelled).Count();
        }
    }
}
