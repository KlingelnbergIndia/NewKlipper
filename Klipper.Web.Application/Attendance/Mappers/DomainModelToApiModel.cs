using Klipper.Web.Application.Attendance.DomainModel;
using Models.Core.HR.Attendance;
using System;
using System.Collections.Generic;
using System.Text;

namespace Klipper.Web.Application.Attendance.Mappers
{
    public class DomainModelToApiModel
    {
        public List<AttendanceRecord> FromDomainModel(List<AttendanceRecords> attendanceRecordsDomainModel)
        {
            List<AttendanceRecord> listOfAttendanceRecordDto = new List<AttendanceRecord>();
            foreach (var attendanceRecordDomainModel in attendanceRecordsDomainModel)
            {
                AttendanceRecord attendanceRecordDto = new AttendanceRecord();
                attendanceRecordDto.Date = attendanceRecordDomainModel.Date();
                attendanceRecordDto.TimeIn = attendanceRecordDomainModel.TimeIn();
                attendanceRecordDto.TimeOut = attendanceRecordDomainModel.TimeOut();
                attendanceRecordDto.TotalWorkingHours = attendanceRecordDomainModel.WorkingHours();
                attendanceRecordDto.LateBy = attendanceRecordDomainModel.LateBy();
                attendanceRecordDto.OverTime = attendanceRecordDomainModel.OverTime();
                listOfAttendanceRecordDto.Add(attendanceRecordDto);
            }
            return listOfAttendanceRecordDto;
        }
    }
}
