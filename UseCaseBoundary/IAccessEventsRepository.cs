using System.Collections.Generic;
using DomainModel;
using DomainModel.Model;

namespace UseCaseBoundary
{
    public interface IAccessEventsRepository
    {
        AccessEvents GetAccessEvents(int employeeid);
    }
}