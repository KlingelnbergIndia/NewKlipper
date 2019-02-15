using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModel
{
    public class CarryForwardLeaves
    {
        public CarryForwardLeaves(int employeeId, DateTime leaveBalanceTillDate,
           int takenCasualLeaves, int takenSickLeaves, int takenCompoffLeaves,
           int maxCasualLeaves, int maxSickLeaves, int maxCompoffLeaves)
        {
            _employeeId = employeeId;
            _leaveBalanceTillDate = leaveBalanceTillDate;
            _takenCasualLeaves = takenCasualLeaves;
            _takenSickLeaves = takenSickLeaves;
            _takenCompoffLeaves = takenCompoffLeaves;
            _maxCasualLeaves = maxCasualLeaves;
            _maxSickLeaves = maxSickLeaves;
            _maxCompoffLeaves = maxCompoffLeaves;
        }
        private int EmployeeId;
        private DateTime LeaveBalanceTillDate;

        private int TakenCasualLeaves;
        private int TakenSickLeaves;
        private int _employeeId;
        private DateTime _leaveBalanceTillDate;
        private int _takenCasualLeaves;
        private int _takenSickLeaves;
        private int _takenCompoffLeaves;
        private int _maxCasualLeaves;
        private int _maxSickLeaves;
        private int _maxCompoffLeaves;

    }
}
