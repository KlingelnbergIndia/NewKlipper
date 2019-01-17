using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainModel;
using UseCaseBoundary;
using UseCaseBoundary.DTO;
using UseCaseBoundary.Model;

namespace UseCases
{
    public class AttendanceService
    {
        private IAccessEventsRepository _accessEventsRepository;
        public AttendanceService(IAccessEventsRepository accessEventsRepository)
        {
            _accessEventsRepository = accessEventsRepository;
        }

        public async Task<AttendanceRecordsDTO> GetAttendanceRecord(int employeeId, int noOfDays)
        {
            AccessEvents accessEvents = _accessEventsRepository.GetAccessEvents(employeeId);
            List<PerDayAttendanceRecordDTO> listOfAttendanceRecordDTO = new List<PerDayAttendanceRecordDTO>();

            var workRecordByDate = accessEvents.WorkRecord(noOfDays);
            foreach (var perDayWorkRecord in workRecordByDate)
            {
                var timeIn = perDayWorkRecord.GetTimeIn();
                var timeOut = perDayWorkRecord.GetTimeOut();
                var workingHours = perDayWorkRecord.CalculateWorkingHours();

                PerDayAttendanceRecordDTO attendanceRecord = new PerDayAttendanceRecordDTO()
                {
                    Date = perDayWorkRecord.Date,
                    TimeIn = new Time(timeIn.Hours, timeIn.Minutes),
                    TimeOut = new Time(timeOut.Hours, timeOut.Minutes),
                    WorkingHours = new Time(workingHours.Hours, workingHours.Minutes),
                    OverTime = GetOverTime(workingHours),
                    LateBy = GetLateByTime(workingHours)
                };
                listOfAttendanceRecordDTO.Add(attendanceRecord);
            }

            return await Task.Run(() =>
            {
                return new AttendanceRecordsDTO()
                {
                    ListOfAttendanceRecordDTO = listOfAttendanceRecordDTO,
                    TotalWorkingHours = CalculateTotalWorkingHours(listOfAttendanceRecordDTO),
                    TotalDeficitHours = CalculateTotalDeficitHours(listOfAttendanceRecordDTO)
                };
            });
        }



        public async Task<AttendanceRecordsDTO> GetAccessEventsForDateRange(int employeeId, DateTime fromDate, DateTime toDate)
        {
            var accessEvents = _accessEventsRepository.GetAccessEventsForDateRange(employeeId, fromDate, toDate);
            var datewiseAccessEvents = accessEvents.GetAllAccessEvents();

            AttendanceRecordsDTO listOfAttendanceRecord = new AttendanceRecordsDTO();
            foreach (var perDayAccessEvents in datewiseAccessEvents)
            {
                var timeIn = perDayAccessEvents.GetTimeIn();
                var timeOut = perDayAccessEvents.GetTimeOut();
                var workingHours = perDayAccessEvents.CalculateWorkingHours();

                PerDayAttendanceRecordDTO attendanceRecord = new PerDayAttendanceRecordDTO()
                {
                    Date = perDayAccessEvents.Date,
                    TimeIn = new Time(timeIn.Hours, timeIn.Minutes),
                    TimeOut = new Time(timeOut.Hours, timeOut.Minutes),
                    WorkingHours = new Time(workingHours.Hours, workingHours.Minutes),
                    OverTime = GetOverTime(workingHours),
                    LateBy = GetLateByTime(workingHours)
                };
                listOfAttendanceRecord
                    .ListOfAttendanceRecordDTO
                    .Add(attendanceRecord);
            }

            return await Task.Run(() =>
            {
                listOfAttendanceRecord.ListOfAttendanceRecordDTO = listOfAttendanceRecord.ListOfAttendanceRecordDTO
                    .OrderByDescending(x => x.Date)
                    .ToList();
                return listOfAttendanceRecord;
            });
        }

        private Time GetOverTime(TimeSpan workingHours)
        {
            var extraHour = GetExtraHours(workingHours);
            if (extraHour.Hour > 0 || extraHour.Minute > 0)
            {
                return extraHour;
            }
            else
            {
                return new Time(0, 0);
            }
        }
        private Time GetLateByTime(TimeSpan workingHours)
        {
            var extraHour = GetExtraHours(workingHours);
            if (extraHour.Hour < 0 || extraHour.Minute < 0)
            {
                int latebyHours = Math.Abs(extraHour.Hour);
                int latebyMinutes = Math.Abs(extraHour.Minute);
                return new Time(latebyHours, latebyMinutes);
            }
            else
            {
                return new Time(0, 0);
            }
        }
        private Time GetExtraHours(TimeSpan workingHours)
        {
            TimeSpan TotalWorkingHours = TimeSpan.Parse("9:00:00");
            var extraHour = workingHours - TotalWorkingHours;
            return new Time(extraHour.Hours, extraHour.Minutes);
        }

        private Time CalculateTotalDeficitHours(List<PerDayAttendanceRecordDTO> listOfAttendanceRecordDTO)
        {
            int totalRequiredHours = listOfAttendanceRecordDTO.Count * 9;
            int totalWorkedHours = CalculateTotalWorkingHours(listOfAttendanceRecordDTO).Hour;
            int totalDefiateHours = totalRequiredHours - totalWorkedHours;

            return new Time(totalDefiateHours, 00);
        }

        private Time CalculateTotalWorkingHours(List<PerDayAttendanceRecordDTO> listOfAttendanceRecordDTO)
        {
            var sumOfTotalWorkingHours = listOfAttendanceRecordDTO
                .Select(x => new TimeSpan(x.WorkingHours.Hour, x.WorkingHours.Minute, 00))
                .Aggregate((t1, t2) => t1 + t2);
            int totalWorkingHours = (int)Math.Round(sumOfTotalWorkingHours.TotalHours);

            return new Time(totalWorkingHours, 00);
        }
    }
}
