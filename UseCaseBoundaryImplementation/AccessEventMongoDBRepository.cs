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

        public List<DomainModel.Model.AccessEvent> ConvertEntityAccessEventToDomainModelAccessEvent(List<AccessEventEntityModel> listOfEntityAccessEvent)
        {
            List<DomainModel.Model.AccessEvent> listOfDomainModelAccessEvent = new List<DomainModel.Model.AccessEvent>();
            foreach (var domainModelAccessEvent in listOfEntityAccessEvent)
            {
                DomainModel.Model.AccessEvent accessEvent = new DomainModel.Model.AccessEvent();

                accessEvent.AccessPointID = domainModelAccessEvent.AccessPointID;
                accessEvent.AccessPointName = domainModelAccessEvent.AccessPointName;
                accessEvent.EmployeeID = domainModelAccessEvent.EmployeeID;
                accessEvent.EventTime = domainModelAccessEvent.EventTime;
                listOfDomainModelAccessEvent.Add(accessEvent);
            }

            return listOfDomainModelAccessEvent;
        }

    }
}
