using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
           int noOfCasualLeave = _listOfLeave.Where(x=>x.GetLeaveType()==Leave.LeaveType.CasualLeave && x.isCanceled == false).Sum(x=>x.GetLeaveDate().Count);
            return noOfCasualLeave;
        }

        public int CalculateSickLeaveTaken()
        {
            int noOfSickLeave = _listOfLeave.Where(x => x.GetLeaveType() == Leave.LeaveType.SickLeave && x.isCanceled == false).Sum(x => x.GetLeaveDate().Count);
            return noOfSickLeave;
        }

        public int CalculateCompOffLeaveTaken()
        {
            int noOfCompOffLeave = _listOfLeave.Where(x => x.GetLeaveType() == Leave.LeaveType.CompOff && x.isCanceled == false).Sum(x => x.GetLeaveDate().Count);
            return noOfCompOffLeave;
        }

        public int GetTotalLeaveTaken()
        {
            return _listOfLeave.Where(x=>x.isCanceled==false).Count();
        }
    }
}
