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
                    TotalDeficitOrExtraHours = CalculateDeficiateOrExtraTime(listOfAttendanceRecordDTO),
                };
            });
        }



        public async Task<AttendanceRecordsDTO> GetAccessEventsForDateRange(int employeeId, DateTime fromDate, DateTime toDate)
        {
            var accessEvents = _accessEventsRepository.GetAccessEventsForDateRange(employeeId, fromDate, toDate);
            var datewiseAccessEvents = accessEvents.GetAllAccessEvents();

            List<PerDayAttendanceRecordDTO> listOfAttendanceRecordDTO = new List<PerDayAttendanceRecordDTO>();
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
                listOfAttendanceRecordDTO.Add(attendanceRecord);
            }

            return await Task.Run(() =>
            {
                return new AttendanceRecordsDTO()
                {
                    ListOfAttendanceRecordDTO = listOfAttendanceRecordDTO
                        .OrderByDescending(x => x.Date)
                        .ToList(),
                    TotalWorkingHours = CalculateTotalWorkingHours(listOfAttendanceRecordDTO),
                    TotalDeficitOrExtraHours = CalculateDeficiateOrExtraTime(listOfAttendanceRecordDTO),
                };
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

        private Time CalculateDeficiateOrExtraTime(List<PerDayAttendanceRecordDTO> listOfAttendanceRecordDTO)
        {
            if (listOfAttendanceRecordDTO.Count == 0)
            {
                return new Time(00, 00);
            }

            double totalRequiredHoursToBeWorked = listOfAttendanceRecordDTO.Count * 9;
            Time totalWorkedTime = CalculateTotalWorkingHours(listOfAttendanceRecordDTO);
            var totalWorkedSpan = new TimeSpan(totalWorkedTime.Hour, totalWorkedTime.Minute, 00);

            double totalDefiateOrOverTimeHrs = totalRequiredHoursToBeWorked - totalWorkedSpan.TotalHours;

            var extraTime = TimeSpan.FromHours(totalDefiateOrOverTimeHrs);

            return new Time(
                (int)extraTime.TotalHours,
                (int)extraTime.Minutes);

        }

        private Time CalculateTotalWorkingHours(List<PerDayAttendanceRecordDTO> listOfAttendanceRecordDTO)
        {
            if (listOfAttendanceRecordDTO.Count == 0)
            {
                return new Time(00, 00);
            }

            var sumOfTotalWorkingHours = listOfAttendanceRecordDTO
                .Select(x => new TimeSpan(x.WorkingHours.Hour, x.WorkingHours.Minute, 00))
                .Aggregate((t1, t2) => t1 + t2);

            return new Time(
                (int)sumOfTotalWorkingHours.TotalHours,
                (int)sumOfTotalWorkingHours.Minutes);
        }

    }
}
