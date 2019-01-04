using System;

namespace DomainModel
{
    public class AccessEvent
    {
        public AccessEvent(
            int accessPointID,
            string accessPointName,
            int employeeID,
            DateTime eventTime)
        {
            AccessPointID = accessPointID;
            AccessPointName = accessPointName;
            EmployeeID = employeeID;
            EventTime = eventTime;
        }

        private readonly int AccessPointID;

        public bool FromMainDoor()
        {
            return AccessPointID == 16;
        }

        private string AccessPointName { get; set; }

        private int EmployeeID { get; set; }

        public DateTime EventTime { get; set; }
    }
}