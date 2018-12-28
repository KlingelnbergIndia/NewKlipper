using Models.Core.HR.Attendance;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Core.HR.Attendance
{
    public class AttendanceRecord
    {
        public Time TotalWorkingHours;
        public DateTime Date;
        public Time TimeIn;
        public Time TimeOut;
        public Time LateBy;
        public Time OverTime;
        
    }
}
