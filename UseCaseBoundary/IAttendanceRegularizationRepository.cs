using DomainModel;
using System;
using System.Collections.Generic;
using System.Text;
using UseCaseBoundary.DTO;

namespace UseCaseBoundary
{
    public interface IAttendanceRegularizationRepository
    {
        bool SaveRegularizationRecord(RegularizationDTO reguraliozationDTO);
        List<Regularization> GetRegularizedRecords(int employeeId);
    }
}
