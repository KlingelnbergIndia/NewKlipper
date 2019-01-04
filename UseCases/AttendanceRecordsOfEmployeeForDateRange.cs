﻿using DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseCaseBoundary;
using UseCaseBoundary.Model;

namespace UseCases
{
    public class AttendanceRecordsOfEmployeeForDateRange
    {
        private IAccessEventsRepository _accessEventsRepository;
        public AttendanceRecordsOfEmployeeForDateRange(IAccessEventsRepository accessEventsRepository)
        {
            _accessEventsRepository = accessEventsRepository;
        }

        public async Task<List<AttendanceRecordDTO>> GetAttendanceRecord(int employeeId, DateTime fromDate, DateTime toDate)
        {
            var accessEvents = _accessEventsRepository.GetAccessEventsForDateRange(employeeId, fromDate, toDate);

            var datewiseAccessEvents = accessEvents.GetAllAccessEvents().GroupBy(x=> DateTime.Parse(x.EventTime.ToShortDateString()));
            List<AttendanceRecordDTO> listOfAttendanceRecord = new List<AttendanceRecordDTO>();

            foreach (var perDayAccessEvents in datewiseAccessEvents)
            {
                var listOfMainEntryPointAccessEventOfADay = perDayAccessEvents.Select(x => x).Where(K => K.AccessPointName == "Main Entry").ToList();
                AccessEvents accessEventsPerDay = new AccessEvents(listOfMainEntryPointAccessEventOfADay);
                var timeIn = accessEventsPerDay.GetTimeIn();
                var timeOut = accessEventsPerDay.GetTimeOut();
                var workingHours = accessEventsPerDay.CalculateWorkingHours();

                AttendanceRecordDTO attendanceRecord = new AttendanceRecordDTO()
                {
                    Date = perDayAccessEvents.Key.Date,
                    TimeIn = new Time(timeIn.Hours, timeIn.Minutes),
                    TimeOut = new Time(timeOut.Hours, timeOut.Minutes),
                    WorkingHours = new Time(workingHours.Hours, workingHours.Minutes),
                    OverTime = GetOverTime(workingHours),
                    LateBy = GetLateByTime(workingHours)
                };
                listOfAttendanceRecord.Add(attendanceRecord);
            }

            return await Task.Run(() =>
            {
                return listOfAttendanceRecord
                    .OrderByDescending(x => x.Date)
                    .ToList();
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
    }
}
