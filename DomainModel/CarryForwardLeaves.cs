using System;

namespace DomainModel
{
    public class CarryForwardLeaves
    {
        public CarryForwardLeaves(int employeeId, DateTime leaveBalanceTillDate,
           float takenCasualLeaves, float takenSickLeaves, float takenCompoffLeaves,
           float maxCasualLeaves, float maxSickLeaves, float maxCompoffLeaves)
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
        private float _takenCasualLeaves;
        private float _takenSickLeaves;
        private float _takenCompoffLeaves;
        private float _maxCasualLeaves;
        private float _maxSickLeaves;
        private float _maxCompoffLeaves;

        public float TakenCasualLeaves()
        {
            return _takenCasualLeaves;
        }
        public float TakenSickLeaves()
        {
            return _takenSickLeaves;
        }
        public float TakenCompoffLeaves()
        {
            return _takenCompoffLeaves;
        }
        public float MaxCasualLeaves()
        {
            return _maxCasualLeaves;
        }
        public float MaxSickLeaves()
        {
            return _maxSickLeaves;
        }
        public float MaxCompoffLeaves()
        {
            return _maxCompoffLeaves;
        }
    }
}
