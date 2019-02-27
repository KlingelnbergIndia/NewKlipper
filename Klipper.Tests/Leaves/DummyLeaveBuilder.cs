using System;
using System.Collections.Generic;
using DomainModel;
using static DomainModel.Leave;

namespace Klipper.Tests.Leaves
{
    public class DummyLeaveBuilder
    {
        private int _EmployeeId;
        private LeaveType _LeaveType;
        private StatusType _StatusType;
        private string _leaveId;

        public List<DateTime> AppliedLeaveDates { get; private set; }

        public DummyLeaveBuilder WithEmployeeId(int employeeId)
        {
            this._EmployeeId = employeeId;
            return this;
        }

        public DummyLeaveBuilder WithLeaveType(LeaveType leaveType)
        {
            this._LeaveType = leaveType;
            return this;
        }

        public DummyLeaveBuilder WithLeaveStatusType(StatusType statusType)
        {
            this._StatusType = statusType;
            return this;
        }

        public DummyLeaveBuilder WithLeaveDates(List<DateTime> appliedLeaveDates)
        {
            this.AppliedLeaveDates = appliedLeaveDates;
            return this;
        }

        public DummyLeaveBuilder WithLeaveId(string leaveId)
        {
            this._leaveId = leaveId;
            return this;
        }

        public Leave Build()
        {
            return new Leave(
                this._EmployeeId,
                this.AppliedLeaveDates,
                this._LeaveType = LeaveType.CasualLeave,
                false,
                "",
                this._StatusType,
                this._leaveId);
        }

    }
}