using System;
using DomainModel;
using UseCaseBoundary;
using UseCaseBoundary.Model;

namespace UseCases
{
    public class AttendanceRecordForEmployeeID
    {
        private IAccessEventsRepository _accessEventsRepository;
        public AttendanceRecordForEmployeeID(IAccessEventsRepository accessEventsRepository)
        {
            _accessEventsRepository = accessEventsRepository;
        }
        AttendanceRecord GetAttendanceRecordForEmployeeID(int id)
        {
            AccessEvents accessEvents = _accessEventsRepository.GetAccessEventsByEmployeeId(id);
            return new AttendanceRecord();
        }
    }
}
