using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DomainModel
{
    public class Leave
    {
        private readonly int _employeeId;
        private readonly List<DateTime> _leaveDates;
        private readonly LeaveType _leaveType;
        private readonly bool _isHalfDayLeave;
        private readonly string _remark;
        private readonly StatusType _status;
        private readonly string _leaveId;

        public enum LeaveType
        {
            [Display(Name = "Privilege Leave")]
            CasualLeave,
            [Display(Name = "Sick Leave")]
            SickLeave,
            [Display(Name = "Comp-Off")]
            CompOff,
            [Display(Name = "On Site Service Call")]
            OnServiceCall
        }
        public enum StatusType
        {
            Approved,
            Updated,
            Cancelled,
            [Display(Name = "Comp-Off Added")]
            CompOffAdded,
            [Display(Name = "Comp-Off Updated")]
            CompOffUpdated,
            [Display(Name = "Comp-Off Cancelled")]
            CompOffCancelled
        }

        public Leave(int employeeId, List<DateTime> leaveDates, 
            LeaveType leaveType,bool isHalfDayLeave, string remark, 
            StatusType status, string leaveId = null)
        {
            _employeeId = employeeId;
            _leaveDates = leaveDates;
            _leaveType = leaveType;
            _isHalfDayLeave = isHalfDayLeave;
            _remark = remark;
            _status = status;
            _leaveId = leaveId;
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

        public string GetLeaveId()
        {
            return _leaveId;
        }
        public bool IsHalfDayLeave()
        {
            return _isHalfDayLeave;
        }

    }
}
