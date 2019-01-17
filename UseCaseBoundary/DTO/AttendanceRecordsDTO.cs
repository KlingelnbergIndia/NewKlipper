using System;
using System.Collections.Generic;
using UseCaseBoundary.DTO;

namespace UseCaseBoundary.Model
{
    public class AttendanceRecordsDTO
    {
        public List<PerDayAttendanceRecordDTO> ListOfAttendanceRecordDTO = new List<PerDayAttendanceRecordDTO>();
        public Time TotalWorkingHours;
    }
}