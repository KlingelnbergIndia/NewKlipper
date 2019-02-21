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
        public bool isCanceled;

        public enum LeaveType
        {
            [Display(Name = "Comp-Off")]
            CompOff,
            [Display(Name = "Casual Leave")]
            CasualLeave,
            [Display(Name = "Sick Leave")]
            SickLeave
        }

        public Leave(int employeeId, List<DateTime> leaveDates, LeaveType leaveType, string remark)
        {
            _employeeId = employeeId;
            _leaveDates = leaveDates;
            _leaveType = leaveType;
            _remark = remark;
            isCanceled = false;
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

    }
}
