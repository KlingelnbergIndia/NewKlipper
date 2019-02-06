using RepositoryImplementation;
using System;
using System.Collections.Generic;
using System.Text;
using UseCaseBoundary.DTO;

namespace UseCases
{
    public class RegularizationService
    {
        private AttendanceRegularizationMongoDBRepository _regularizatioRepository;

        public RegularizationService(AttendanceRegularizationMongoDBRepository regularizatioRepository)
        {
            _regularizatioRepository = regularizatioRepository;
        }

        public void SaveReguralizationData(ReguralizationDTO reguraliozationDTO)
        {
            _regularizatioRepository.SaveReguralizationData(reguraliozationDTO);
        }

        public ReguralizationDTO GetReguralizationData(int employeeId,DateTime date)
        {
            var regularizedData =_regularizatioRepository.GetReguralizationData(int employeeId, DateTime date);
            return regularizedData;
        }

    }
}
