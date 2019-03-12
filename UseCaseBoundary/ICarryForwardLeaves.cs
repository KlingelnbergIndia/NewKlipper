using DomainModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UseCaseBoundary
{
    public interface ICarryForwardLeaves
    {
        Task<CarryForwardLeaves> GetCarryForwardLeaveAsync(int employeeId);
    }
}
