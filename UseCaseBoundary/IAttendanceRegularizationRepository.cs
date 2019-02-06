using System;
using System.Collections.Generic;
using System.Text;
using UseCaseBoundary.DTO;

namespace UseCaseBoundary
{
    public interface IAttendanceRegularizationRepository
    {
        ReguralizationDTO SaveRegularizationData(ReguralizationDTO regularizationData);
    }
}
