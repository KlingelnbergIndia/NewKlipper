using System;
using System.Collections.Generic;
using DomainModel;

namespace UseCaseBoundary
{
    public interface IAccessEventsRepository
    {
        WorkLogs GetAccessEvents(int employeeid);
        WorkLogs GetAccessEventsForDateRange(int employeeId, DateTime fromDate, DateTime toDate);
        PerDayWorkRecord GetAccessEventsForADay(int employeeId, DateTime date);
    }
}