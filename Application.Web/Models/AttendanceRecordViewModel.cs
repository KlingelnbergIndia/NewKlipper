using System.Collections.Generic;
using UseCaseBoundary.Model;

namespace Application.Web.Models
{
    public class AttendanceRecordViewModel : LayoutViewModel
    {
        public List<AttendanceRecordDTO> attendanceRecordDtos = new List<AttendanceRecordDTO>();
    }
}