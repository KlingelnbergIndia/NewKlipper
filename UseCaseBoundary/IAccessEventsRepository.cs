using System;
using System.Collections.Generic;
using DomainModel;
using DomainModel.Model;

namespace UseCaseBoundary
{
    public interface IAccessEventsRepository
    {
        AccessEvents GetAccessEvents(int employeeid);
        AccessEvents GetAccessEventsForDateRange(int employeeId, DateTime fromDate, DateTime toDate);
    }
}