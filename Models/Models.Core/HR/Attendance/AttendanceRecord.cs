using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Core.HR.Attendance
{
    public class AttendanceRecord
    {
        public TimeSpan TotalWorkingHours;
        public String Date;
        public TimeSpan TimeIn;
        public TimeSpan TimeOut;
        public TimeSpan LateBy;
        public TimeSpan OverTime;
       
    }
}