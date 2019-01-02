using System;
using System.Collections.Generic;
using System.Text;

namespace UseCaseBoundary.Model
{
    public class Time
    {
        public int _hour;
        public int _minute;
        public Time(int hours, int minutes)
        {
            _hour = hours;
            _minute = minutes;
        }
    }
}
