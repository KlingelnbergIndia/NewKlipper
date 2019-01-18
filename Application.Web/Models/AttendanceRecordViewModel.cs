using System.Collections.Generic;
using UseCaseBoundary.DTO;
using UseCaseBoundary.Model;

namespace Application.Web.Models
{
    public class AttendanceRecordViewModel : LayoutViewModel
    {
        public List<PerDayAttendanceRecordDTO> attendanceRecordDtos = new List<PerDayAttendanceRecordDTO>();
    }
}