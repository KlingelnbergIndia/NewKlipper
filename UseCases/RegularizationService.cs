using RepositoryImplementation;
using System;
using System.Collections.Generic;
using System.Text;
using UseCaseBoundary;
using UseCaseBoundary.DTO;

namespace UseCases
{
    public class RegularizationService
    {
        private IAttendanceRegularizationRepository _regularizatioRepository;

        public RegularizationService(IAttendanceRegularizationRepository regularizatioRepository)
        {
            _regularizatioRepository = regularizatioRepository;
        }

        public void AddRegularization(RegularizationDTO reguraliozationDTO)
        {
            _regularizatioRepository.SaveRegularizationRecord(reguraliozationDTO);
        }

        public List<RegularizationDTO> GetRegularization(int employeeId)
        {
            var regularizedData =_regularizatioRepository.GetRegularizedRecords(employeeId);
            return regularizedData;
        }

    }
}
