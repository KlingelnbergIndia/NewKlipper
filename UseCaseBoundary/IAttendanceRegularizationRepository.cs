using System;
using System.Collections.Generic;
using System.Text;
using UseCaseBoundary.DTO;

namespace UseCaseBoundary
{
    public interface IAttendanceRegularizationRepository
    {
        RegularizationDTO SaveRegularizationData(RegularizationDTO regularizationData);
        bool SaveRegularizationRecord(RegularizationDTO reguraliozationDTO);
        List<RegularizationDTO> GetRegularizedRecords(int employeeId);
    }
}
