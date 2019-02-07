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
            var data = new List<Regularization>();
            var allRecords = _regularizationDBContext.AttendanceRegularization
                .AsQueryable()
                .Where(x => x.EmployeeID == employeeId)
                .ToList();

            foreach (var record in allRecords)
            {
                data.Add(new Regularization(
                    record.EmployeeID,
                    record.RegularizedDate,
                    record.RegularizedHours,
                    record.Remark
                    ));
            }

            return data;
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
