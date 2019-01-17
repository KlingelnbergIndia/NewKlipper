using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess;
using DataAccess.EntityModel;
using DomainModel;
using MongoDB.Driver;
using Newtonsoft.Json;
using UseCaseBoundary;

namespace UseCaseBoundaryImplementation
{
    public class AccessEventMongoDBRepository : IAccessEventsRepository
    {
        private readonly AttendanceDBContext _context = null;

        public AccessEventMongoDBRepository()
        {
            _context = AttendanceDBContext.Instance;
        }

        public AccessEvents GetAccessEvents(int employeeid)
        {
            var filter = Builders<AccessEventEntityModel>.Filter.Eq("EmployeeID", employeeid);
            var listOfEntityAccessEvent = _context.AccessEvents
                .Find(filter)
                .ToList();

            var listOfDomainModelAccessEvent = ConvertEntityAccessEventToDomainModelAccessEvent(listOfEntityAccessEvent);
            AccessEvents accessEvents = new AccessEvents(listOfDomainModelAccessEvent);
            return accessEvents;
        }

        public List<DomainModel.AccessEvent> ConvertEntityAccessEventToDomainModelAccessEvent(List<AccessEventEntityModel> listOfEntityAccessEvent)
        {
            List<DomainModel.AccessEvent> listOfDomainModelAccessEvent = new List<DomainModel.AccessEvent>();
            foreach (var domainModelAccessEvent in listOfEntityAccessEvent)
            {
                DomainModel.AccessEvent accessEvent =
                    new DomainModel.AccessEvent(
                        domainModelAccessEvent.AccessPointID,
                        domainModelAccessEvent.AccessPointName,
                        domainModelAccessEvent.EmployeeID,
                        domainModelAccessEvent.EventTime);


                listOfDomainModelAccessEvent.Add(accessEvent);
            }

            return listOfDomainModelAccessEvent;
        }

        public AccessEvents GetAccessEventsForDateRange(int employeeId, DateTime fromDate, DateTime toDate)
        {
            DateTime toDateWithMaxTimeOfTheDay = toDate.Date + DateTime.MaxValue.TimeOfDay;
            var accessEvents = _context.AccessEvents.AsQueryable()
                .Where(x => x.EmployeeID == employeeId && x.EventTime <= toDateWithMaxTimeOfTheDay && x.EventTime >= fromDate)
                .ToList();
            var s = JsonConvert.SerializeObject(accessEvents);

            var listOfDomainModelAccessEvent = ConvertEntityAccessEventToDomainModelAccessEvent(accessEvents);
            return new AccessEvents(listOfDomainModelAccessEvent);
        }
    }
}
