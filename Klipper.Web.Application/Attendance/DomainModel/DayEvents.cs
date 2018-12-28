using System;
using System.Collections.Generic;
using System.Text;

namespace Klipper.Web.Application.Attendance.DomainModel
{
    public class DayEvents
    {
        private List<DateTime> _lisiOfDistinctDayEvent;

        public DayEvents(List<DateTime> listOfDistinctDayEvent)
        {
            _lisiOfDistinctDayEvent = listOfDistinctDayEvent;
        }
        public List<DateTime> ListOfDistinctDayEvent()
        {
            return _lisiOfDistinctDayEvent;
        }


    }
}
