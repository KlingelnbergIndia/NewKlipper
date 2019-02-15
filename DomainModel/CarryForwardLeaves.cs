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
       
        private int _employeeId;
        private DateTime _leaveBalanceTillDate;
        private int _takenCasualLeaves;
        private int _takenSickLeaves;
        private int _takenCompoffLeaves;
        private int _maxCasualLeaves;
        private int _maxSickLeaves;
        private int _maxCompoffLeaves;

        public int TakenCasualLeaves()
        {
            return _takenCasualLeaves;
        }
        public int TakenSickLeaves()
        {
            return _takenSickLeaves;
        }
        public int TakenCompoffLeaves()
        {
            return _takenCompoffLeaves;
        }
        public int MaxCasualLeaves()
        {
            return _maxCasualLeaves;
        }
        public int MaxSickLeaves()
        {
            return _maxSickLeaves;
        }
        public int MaxCompoffLeaves()
        {
            return _maxCompoffLeaves;
        }
    }
}
