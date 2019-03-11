using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess;
using DataAccess.EntityModel;
using DomainModel;
using MongoDB.Driver;
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

        public WorkLogs GetAccessEvents(int employeeid)
        {
            var filter = Builders<AccessEventEntityModel>.Filter.Eq("EmployeeID", employeeid);
            var listOfEntityAccessEvent = _context.AccessEvents
                .Find(filter)
                .ToList();

            var listOfDomainModelAccessEvent = ConvertEntityAccessEventToDomainModelAccessEvent(listOfEntityAccessEvent);
            WorkLogs accessEvents = new WorkLogs(listOfDomainModelAccessEvent);
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

        public WorkLogs GetAccessEventsForDateRange(int employeeId, DateTime fromDate, DateTime toDate)
        {
            DateTime toDateWithMaxTimeOfTheDay = toDate.Date + DateTime.MaxValue.TimeOfDay;
            var accessEvents = _context.AccessEvents.AsQueryable()
                .Where(x => x.EmployeeID == employeeId && x.EventTime <= toDateWithMaxTimeOfTheDay && x.EventTime >= fromDate)
                .ToList();

            var listOfDomainModelAccessEvent = ConvertEntityAccessEventToDomainModelAccessEvent(accessEvents);
            return new WorkLogs(listOfDomainModelAccessEvent);
        }

        public PerDayWorkRecord GetAccessEventsForADay(int employeeId, DateTime date)
        {
            DateTime dateWithMaxTimeOfTheDay = date.Date + DateTime.MaxValue.TimeOfDay;
            var accessEvents = _context.AccessEvents.AsQueryable()
                .Where(x => x.EmployeeID == employeeId && x.EventTime <= dateWithMaxTimeOfTheDay && x.EventTime >= date)
                .ToList();
            var listOfAccessEvent = ConvertEntityAccessEventToDomainModelAccessEvent(accessEvents);
            return new PerDayWorkRecord(date, listOfAccessEvent);
        }
    }
}
