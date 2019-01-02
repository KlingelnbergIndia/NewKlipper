using System.Collections.Generic;
using DomainModel;
using DomainModel.Model;

namespace UseCaseBoundary
{
    public interface IAccessEventsRepository
    {
        AccessEvents GetAccessEventsByEmployeeId(int id);
    }
}