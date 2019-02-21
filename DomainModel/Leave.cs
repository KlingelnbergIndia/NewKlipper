using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DomainModel
{
    public class Leave
    {
        private readonly int _employeeId;
        private readonly List<DateTime> _leaveDates;
        private readonly LeaveType _leaveType;
        private readonly string _remark;
        private readonly StatusType _status;

        public enum LeaveType
        {
            [Display(Name = "Comp-Off")]
            CompOff,
            [Display(Name = "Casual Leave")]
            CasualLeave,
            [Display(Name = "Sick Leave")]
            SickLeave
        }
        public enum StatusType
        {
            Approved,
            Updated,
            Cancelled,
            LeaveAdded
        }

        public Leave(int employeeId, List<DateTime> leaveDates, LeaveType leaveType, string remark, StatusType status)
        {
            _employeeId = employeeId;
            _leaveDates = leaveDates;
            _leaveType = leaveType;
            _remark = remark;
            _status = status;
        }

        public LeaveType GetLeaveType()
        {
            return _leaveType;
        }

        public List<DateTime> GetLeaveDate()
        {
            return _leaveDates;
        }

        public string GetRemark()
        {
            return _remark;
        }

        public int GetEmployeeId()
        {
            return _employeeId;
        }
        public StatusType GetStatus()
        {
            return _status;
        }
    }
}
