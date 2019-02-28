﻿using System;
using System.Collections.Generic;
using System.Text;
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
    }

    public enum DayStatus
    {
        WorkingDay,
        NonWorkingDay,
        Holiday,
        Leave
    }

   
}
