using System;
using System.Collections.Generic;
using DomainModel;

namespace UseCaseBoundary
{
    public interface IAccessEventsRepository
    {
        AccessEvents GetAccessEvents(int employeeid);
        AccessEvents GetAccessEventsForDateRange(int employeeId, DateTime fromDate, DateTime toDate);
        PerDayWorkRecord GetAccessEventsForADay(int employeeId, DateTime date);
    }
}