using DataAccess;
using DataAccess.EntityModel.Attendance;
using DomainModel;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UseCaseBoundary;
using UseCaseBoundary.DTO;
using UseCaseBoundary.Model;

namespace RepositoryImplementation
{
    public class AttendanceRegularizationMongoDBRepository : IAttendanceRegularizationRepository
    {
        private readonly AttendanceRegularizationDBContext _regularizationDBContext;
        public AttendanceRegularizationMongoDBRepository()
        {
            _regularizationDBContext = AttendanceRegularizationDBContext.Instance;
        }

        public List<Regularization> GetRegularizedRecords(int employeeId)
        {
            var allRecords = _regularizationDBContext.AttendanceRegularization
                .AsQueryable()
                .Where(x => x.EmployeeID == employeeId)
                .Select(x => new Regularization(x.EmployeeID, x.RegularizedDate, x.RegularizedHours, x.Remark))
                .ToList();
            return allRecords;
        }

        public bool SaveRegularizationRecord(RegularizationDTO reguraliozationDTO)
        {

            AttendanceRegularizationEntityModel data = new AttendanceRegularizationEntityModel()
            {
                EmployeeID = reguraliozationDTO.EmployeeID,
                RegularizedDate = reguraliozationDTO.RegularizedDate,
                Remark = reguraliozationDTO.Remark,
                RegularizedHours = new TimeSpan(reguraliozationDTO.ReguralizedHours.Hour, reguraliozationDTO.ReguralizedHours.Minute,00)
            };
            _regularizationDBContext.AttendanceRegularization
                .InsertOneAsync(data)
                .GetAwaiter()
                .GetResult();
            return true;
        }

    }
}
