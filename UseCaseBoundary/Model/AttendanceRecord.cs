using System;

namespace UseCaseBoundary.Model
{
    public class AttendanceRecord
    {
        public TimeSpan TimeIn { get; set; }
        public TimeSpan TimeOut { get; set; }
        public TimeSpan LateBy { get; set; }
        public TimeSpan ExtraHours { get; set; }
        public TimeSpan TotalWorkingHours { get; set; }

    }
}