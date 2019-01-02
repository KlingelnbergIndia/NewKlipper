using System;

namespace DomainModel.Model
{
    public class AccessEvent
    {
        public int AccessPointID { get; set; }

        public string AccessPointName { get; set; }

        public int EmployeeID { get; set; }

        public DateTime EventTime { get; set; }
    }
}