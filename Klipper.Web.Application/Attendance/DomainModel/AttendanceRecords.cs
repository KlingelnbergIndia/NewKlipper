using Models.Core.HR.Attendance;
using System;
using System.Collections.Generic;
using System.Text;

namespace Klipper.Web.Application.Attendance.DomainModel
{
    public class AttendanceRecords
    {
        private Time _totalWorkingHours;
        private DateTime _date;
        private Time _timeIn;
        private Time _timeOut;
        private Time _lateBy;
        private Time _overTime;
        public AttendanceRecords(DateTime date, Time timeIn, Time timeOut, Time totalWorkingHours, Time lateBy, Time overTime)
        {
            _date = date;
            _timeIn = timeIn;
            _timeOut = timeOut;
            _totalWorkingHours = totalWorkingHours;
            _lateBy = lateBy;
            _overTime = overTime;
        }
        public Time WorkingHours()
        {
            return _totalWorkingHours;
        }
        public DateTime Date()
        {
            return _date;
        }
        public Time TimeIn()
        {
            return _timeIn;
        }
        public Time TimeOut()
        {
            return _timeOut;
        }
        public Time LateBy()
        {
            return _lateBy;
        }
        public Time OverTime()
        {
            return _overTime;
        }
    }
}
