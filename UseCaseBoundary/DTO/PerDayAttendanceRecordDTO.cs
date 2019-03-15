using System;
using UseCaseBoundary.Model;

namespace UseCaseBoundary.DTO
{
    public class PerDayAttendanceRecordDTO
    {
        public Time WorkingHours;
        public DateTime Date;
        public Time TimeIn;
        public Time TimeOut;
        public Time LateBy;
        public Time OverTime;
        public DayStatus DayStatus;
        public string Remark;
        public bool IsHoursRegularized;
        public bool HaveLeave;
        public Time RegularizedHours;

        public string ToString(Time time)
        {
            return string.Concat(time.Hour, ":", time.Minute);
        }
    }

    public enum DayStatus
    {
        WorkingDay,
        NonWorkingDay,
        Holiday,
        Leave,
        HalfDayLeave
    }

    

}
