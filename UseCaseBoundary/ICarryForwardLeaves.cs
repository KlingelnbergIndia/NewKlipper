using DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace UseCaseBoundary
{
    public interface ICarryForwardLeaves
    {
        CarryForwardLeaves GetCarryForwardLeave(int employeeId);
       
    }
}
