using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModel
{
    public class Leave
    {
        private readonly int _employeeId;
        private readonly DateTime _leaveDate;
        private readonly LeaveType _leaveType;
        private readonly string _remark;

        public enum LeaveType
        {
            EarnedLeave,
            CasualLeave,
            SickLeave
        }

        public Leave(int employeeId, DateTime leaveDate, LeaveType leaveType, string remark)
        {
            _employeeId = employeeId;
            _leaveDate = leaveDate;
            _leaveType = leaveType;
            _remark = remark;
        }

        public LeaveType GetLeaveType()
        {
            return _leaveType;
        }

        public DateTime GetLeaveDate()
        {
            return _leaveDate;
        }

        public string GetRemark()
        {
            return _remark;
        }
    }
}
