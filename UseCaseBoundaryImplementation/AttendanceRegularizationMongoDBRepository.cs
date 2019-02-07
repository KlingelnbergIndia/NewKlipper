using DataAccess;
using DataAccess.EntityModel.Attendance;
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

        public List<RegularizationDTO> GetRegularizedRecords(int employeeId)
        {
            var allRecords = _regularizationDBContext.AttendanceRegularization
                .AsQueryable()
                .Where(x => x.EmployeeID == employeeId)
                .Select(x => new RegularizationDTO() {
                    EmployeeID = x.EmployeeID,
                    RegularizedDate = x.RegularizedDate,
                    ReguralizedHours = x.RegularizedHours,
                    Remark = x.Remark
                })
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
                RegularizedHours = reguraliozationDTO.ReguralizedHours
            };
            _regularizationDBContext.AttendanceRegularization
                .InsertOneAsync(data)
                .GetAwaiter()
                .GetResult();
            return true;
        }

    }
}
