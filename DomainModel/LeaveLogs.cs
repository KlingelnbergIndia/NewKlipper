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

        public float CalculateCasualLeaveTaken()
        {
           float noOfCasualLeaveTaken = _listOfLeave.Where(x=>x.GetLeaveType()==LeaveType.CasualLeave
           && x.GetStatus()!= StatusType.Cancelled && x.IsHalfDayLeave()==false)
           .Sum(x=>x.GetLeaveDate().Count);
           float noOfHalfDayCasualLeaveTaken = _listOfLeave.Where(x => x.GetLeaveType() == LeaveType.CasualLeave
            && x.GetStatus() != StatusType.Cancelled && x.IsHalfDayLeave() == true)
            .Sum(x => x.GetLeaveDate().Count);

            return noOfCasualLeaveTaken + noOfHalfDayCasualLeaveTaken/2;
        }

        public float CalculateSickLeaveTaken()
        {
            float noOfSickLeaveTaken = _listOfLeave.Where(x => x.GetLeaveType() == LeaveType.SickLeave
            && x.GetStatus() != StatusType.Cancelled && x.IsHalfDayLeave() == false)
            .Sum(x => x.GetLeaveDate().Count);
            float noOfHalfDaySickLeaveTaken = _listOfLeave.Where(x => x.GetLeaveType() == LeaveType.SickLeave
            && x.GetStatus() != StatusType.Cancelled && x.IsHalfDayLeave() == true)
            .Sum(x => x.GetLeaveDate().Count);

            return noOfSickLeaveTaken + noOfHalfDaySickLeaveTaken/2;
        }

        public float CalculateCompOffLeaveTaken()
        {
            float noOfCompOffLeaveTaken = _listOfLeave.Where(x => x.GetLeaveType() == LeaveType.CompOff
            && x.GetStatus() != StatusType.Cancelled && x.IsHalfDayLeave() == false)
            .Sum(x => x.GetLeaveDate().Count);
            float noOfHalfDayCompOffLeaveTaken = _listOfLeave.Where(x => x.GetLeaveType() == LeaveType.CompOff
           && x.GetStatus() != StatusType.Cancelled && x.IsHalfDayLeave() == true)
           .Sum(x => x.GetLeaveDate().Count) ;

            return noOfCompOffLeaveTaken + noOfHalfDayCompOffLeaveTaken/2;
        }

    }
}
