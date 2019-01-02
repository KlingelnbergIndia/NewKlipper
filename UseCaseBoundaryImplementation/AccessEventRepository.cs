using System;
using System.Collections.Generic;
using DomainModel;
using DomainModel.Model;
using UseCaseBoundary;

namespace UseCaseBoundaryImplementation
{
    public class AccessEventRepository : IAccessEventsRepository
    {
        public AccessEvents GetAccessEventsByEmployeeId(int id)
        {
            throw new NotImplementedException();
        }
    }
}
