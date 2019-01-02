using System;
using System.Collections.Generic;
using DomainModel.Model;

namespace DomainModel
{
    public class AccessEvents
    {
        private List<AccessEvent> _listOfAccessEvent;
        public AccessEvents(List<AccessEvent> listOfAccessEvent)
        {
            listOfAccessEvent = _listOfAccessEvent;
        }
        public void CalculateWorkingHours() { }
        public void CalculateDeficitHours() { }
        public void CalculateLateByHours() { }
        public void CalculateExtraHours() { }

    }
}
