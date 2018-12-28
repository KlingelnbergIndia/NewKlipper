using Klipper.Web.Application.Attendance.DataAccess;
using Klipper.Web.Application.Attendance.DomainModel;
using Klipper.Web.Application.Attendance.Mappers;
using Models.Core.HR.Attendance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Klipper.Web.Application.Attendance.Service
{
    public class AttendanceService : IAttendanceService
    {
        private IAttendanceAccessor _attendanceAccessor;

        public AttendanceService(IAttendanceAccessor attendanceAccessor)
        {
            _attendanceAccessor = attendanceAccessor;
        }

        public async Task<List<AttendanceRecord>> GetAttendance(int employeeId, int noOfDays, string timeZoneStr)
        {
            AccessEvents accessEvents = await _attendanceAccessor.GetAccessEventsAsync(employeeId);
            var specificDistictDays = accessEvents.DistinctDays(noOfDays);
            var specificDaysAccessEvents = accessEvents.GetNoOfDaysRecord(specificDistictDays);
            accessEvents= specificDaysAccessEvents.ConvertTimeZone(timeZoneStr);
            List<AttendanceRecords> listOfAttendanceRecord=accessEvents.CalculateWorkingHours(specificDistictDays);
            DomainModelToApiModel domainModelToApiModel = new DomainModelToApiModel();
            return domainModelToApiModel.FromDomainModel(listOfAttendanceRecord);
        }
    }
}
