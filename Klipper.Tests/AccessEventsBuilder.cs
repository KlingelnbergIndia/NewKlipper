using DomainModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Klipper.Tests
{
    public class AccessEventsBuilder
    {
        private string accessEventsFilePath;
        private List<AccessEvent> dummyAccessEvent = new List<AccessEvent>();

        public AccessEventsBuilder()
        {
            string currentDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            accessEventsFilePath = currentDirectory.Remove(currentDirectory.Length - 3) + "AccessEventsDummyData.json";
        }

        public AccessEvents Build()
        {
            var jsonData = File.ReadAllText(accessEventsFilePath);
            dummyAccessEvent = JsonConvert.DeserializeObject<List<AccessEvent>>(jsonData);
            return new AccessEvents(dummyAccessEvent);
        }

        public AccessEvents BuildBetweenDate(DateTime fromDate, DateTime toDate)
        {
            fromDate = toDate.Date + DateTime.MinValue.TimeOfDay;
            toDate = toDate.Date + DateTime.MaxValue.TimeOfDay;
            var jsonData = File.ReadAllText(accessEventsFilePath);
            dummyAccessEvent = JsonConvert.DeserializeObject<List<AccessEvent>>(jsonData)
                                .Where(x=>x.EventTime >= fromDate && x.EventTime <= toDate).ToList();
            return new AccessEvents(dummyAccessEvent);
        }
    }
}
