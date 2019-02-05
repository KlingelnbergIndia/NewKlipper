﻿using System;
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
        private IEmployeeRepository _employeeRepository;
        private IDepartmentRepository _departmentRepository;
        public AttendanceService(IAccessEventsRepository accessEventsRepository, IEmployeeRepository employeeRepository, IDepartmentRepository departmentRepository)
        {
            _accessEventsRepository = accessEventsRepository;
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
        }

        public async Task<AttendanceRecordsDTO> GetAttendanceRecord(int employeeId, int noOfDays)
        {
            AccessEvents accessEvents = _accessEventsRepository.GetAccessEvents(employeeId);
            var workRecordByDate = accessEvents.WorkRecord(noOfDays);
            AttendanceRecordsDTO listOfAttendanceRecord = await CreateAttendanceRecordAsync(workRecordByDate, employeeId);

            return await Task.Run(() =>
            {
                return listOfAttendanceRecord;
            });
        }

        public async Task<AttendanceRecordsDTO> GetAccessEventsForDateRange(int employeeId, DateTime fromDate, DateTime toDate)
        {
            AccessEvents accessEvents = _accessEventsRepository.GetAccessEventsForDateRange(employeeId, fromDate, toDate);
            var datewiseAccessEvents = accessEvents.GetAllAccessEvents();
            AttendanceRecordsDTO listOfAttendanceRecord = await CreateAttendanceRecordAsync(datewiseAccessEvents, employeeId);
            listOfAttendanceRecord = IncludeHolidays(listOfAttendanceRecord, fromDate, toDate, employeeId);

            return await Task.Run(() =>
            {
                return listOfAttendanceRecord;
            });
        }
        public async Task<List<AccessPointRecord>> GetAccessPointDetails(int employeeId, DateTime date)
        {
            PerDayWorkRecord perDayWorkRecord = _accessEventsRepository.GetAccessEventsForADay(employeeId, date);

            var RecreationPointAccessEvents = perDayWorkRecord.GetRecreationPointAccessEvents();
            var GymnasiumPointAccessEvents = perDayWorkRecord.GetGymnasiumPointAccessEvents();
            var MainEntryPointAccessEvents = perDayWorkRecord.GetMainEntryPointAccessEvents();

            List<AccessPointRecord> RecreationAccessPointRecord = GetAccessPointRecord(RecreationPointAccessEvents, AccessPoint.Recreation);
            List<AccessPointRecord> GymnasiumAccessPointRecord = GetAccessPointRecord(GymnasiumPointAccessEvents, AccessPoint.Gymnasium);
            List<AccessPointRecord> MainEntryPointAccessPointRecord = GetAccessPointRecord(MainEntryPointAccessEvents, AccessPoint.MainEntry);

            List<AccessPointRecord> listOfAccessPointRecord = RecreationAccessPointRecord
                .Concat(GymnasiumAccessPointRecord)
                .Concat(MainEntryPointAccessPointRecord)
                .ToList();

            listOfAccessPointRecord.Sort((x, y) =>
                x.TimeIn.Hour.CompareTo(y.TimeIn.Hour) == 0 ?
                x.TimeIn.Minute.CompareTo(y.TimeIn.Minute) : x.TimeIn.Hour.CompareTo(y.TimeIn.Hour));

            return await Task.Run(() =>
            {
                return listOfAccessPointRecord;
            });
        }

        private List<AccessPointRecord> GetAccessPointRecord(List<AccessEvent> listOfAccessEvent,AccessPoint accessPoint)
        {
            List<AccessPointRecord> listOfaccessPointRecords = new List<AccessPointRecord>();
            for (int i = 0; i < listOfAccessEvent.Count; i += 2)
            {
                var timeIn = CalculateAbsoluteOutTimeAndInTime(listOfAccessEvent[i].EventTime.TimeOfDay, AbsoluteTime.TimeIn);
                var timeOut = TimeSpan.Zero;
                if (i != listOfAccessEvent.Count - 1)
                {
                    timeOut = CalculateAbsoluteOutTimeAndInTime(listOfAccessEvent[i + 1].EventTime.TimeOfDay, AbsoluteTime.TimeOut); ;
                }
                var timeSpend = TimeSpan.Zero;
                if ((listOfAccessEvent.Count % 2) == 0)
                {
                    timeSpend = (timeOut - timeIn);
                }
                AccessPointRecord accessPointRecord = new AccessPointRecord()
                {
                    TimeIn = new Time(timeIn.Hours, timeIn.Minutes),
                    TimeOut = new Time(timeOut.Hours, timeOut.Minutes),
                    TimeSpend = new Time(timeSpend.Hours, timeSpend.Minutes),
                    AccessPoint = accessPoint
                };
                listOfaccessPointRecords.Add(accessPointRecord);
            }
            return listOfaccessPointRecords;
        }

        private AttendanceRecordsDTO IncludeHolidays(AttendanceRecordsDTO listOfAttendanceRecord, DateTime fromDate, DateTime toDate,int employeeId)
        {
            var availableDates = listOfAttendanceRecord.ListOfAttendanceRecordDTO.Select(x => x.Date).Distinct().ToList();
            var listOfAttendanceRecordDTO = listOfAttendanceRecord.ListOfAttendanceRecordDTO;
            
            Employee employeeData = _employeeRepository.GetEmployee(employeeId);
            Department department = _departmentRepository.GetDepartment(employeeData.Department());

            for (var i = fromDate; i <= toDate; i = i.AddDays(1))
            {
                if (!availableDates.Any(x => x.Date.Date == i.Date.Date))
                {
                    if (department.IsValidWorkingDay(i.Date.Date) == true)
                    {
                        listOfAttendanceRecordDTO.Add(new PerDayAttendanceRecordDTO()
                        {
                            Date = i,
                            LateBy = new Time(0, 0),
                            OverTime = new Time(0, 0),
                            TimeIn = new Time(0, 0),
                            TimeOut = new Time(0, 0),
                            WorkingHours = new Time(0, 0),
                            DayStatus =DayStatus.Leave
                        });
                    }
                }
            }
            listOfAttendanceRecord.ListOfAttendanceRecordDTO = listOfAttendanceRecordDTO.OrderByDescending(x => x.Date).ToList();

            return listOfAttendanceRecord;
        }

        private async Task<AttendanceRecordsDTO> CreateAttendanceRecordAsync(IList<PerDayWorkRecord> workRecordByDate, int employeeId)
        {
            AttendanceRecordsDTO listOfAttendanceRecordDTO = new AttendanceRecordsDTO();
            Employee employeeData = _employeeRepository.GetEmployee(employeeId);
            Department department = _departmentRepository.GetDepartment(employeeData.Department());

            foreach (var perDayWorkRecord in workRecordByDate)
            {
                var timeIn = perDayWorkRecord.GetTimeIn();
                var timeOut = perDayWorkRecord.GetTimeOut();
                var workingHours = perDayWorkRecord.CalculateWorkingHours();
                var overTime = new Time(0,0);
                var lateBy = new Time(0,0);
                var isValidWorkingDay = department.IsValidWorkingDay(perDayWorkRecord.Date);

                if (isValidWorkingDay == true)
                {
                    overTime = GetOverTime(workingHours, department.GetNoOfHoursToBeWorked());
                    lateBy = GetLateByTime(workingHours, department.GetNoOfHoursToBeWorked());
                }
                else
                {
                    overTime = new Time(workingHours.Hours, workingHours.Minutes);
                }
               
                PerDayAttendanceRecordDTO attendanceRecord = new PerDayAttendanceRecordDTO()
                {
                    Date = perDayWorkRecord.Date,
                    TimeIn = new Time(timeIn.Hours, timeIn.Minutes),
                    TimeOut = new Time(timeOut.Hours, timeOut.Minutes),
                    WorkingHours = new Time(workingHours.Hours, workingHours.Minutes),
                    OverTime = overTime,
                    LateBy = lateBy,
                    DayStatus = isValidWorkingDay ? DayStatus.WorkingDay : DayStatus.NonWorkingDay
                };
                listOfAttendanceRecordDTO.ListOfAttendanceRecordDTO.Add(attendanceRecord);
            }

            return await Task.Run(() =>
            {
                var perDayAttendanceRecords = listOfAttendanceRecordDTO.ListOfAttendanceRecordDTO;
                return new AttendanceRecordsDTO()
                {
                    ListOfAttendanceRecordDTO = perDayAttendanceRecords,
                    TotalWorkingHours = CalculateTotalWorkingHours(perDayAttendanceRecords),
                    TotalDeficitOrExtraHours = CalculateDeficiateOrExtraTime(perDayAttendanceRecords, department.GetNoOfHoursToBeWorked()),
                };
            });
        }

        private Time GetOverTime(TimeSpan workingHours, double noOfHoursToBeWorked)
        {
            var extraHour = GetExtraHours(workingHours, noOfHoursToBeWorked);
            if (extraHour.Hour > 0 || extraHour.Minute > 0)
            {
                return extraHour;
            }
            else
            {
                return new Time(0, 0);
            }
        }
        private Time GetLateByTime(TimeSpan workingHours, double noOfHoursToBeWorked)
        {
            var extraHour = GetExtraHours(workingHours, noOfHoursToBeWorked);
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
        private Time GetExtraHours(TimeSpan workingHours, double noOfHoursToBeWorked)
        {
            TimeSpan TotalWorkingHours = TimeSpan.FromHours(noOfHoursToBeWorked);
            var extraHour = workingHours - TotalWorkingHours;
            return new Time(extraHour.Hours, extraHour.Minutes);
        }

        private Time CalculateDeficiateOrExtraTime(List<PerDayAttendanceRecordDTO> listOfAttendanceRecordDTO, double noOfHoursToBeWorked)
        {
            if (listOfAttendanceRecordDTO.Count == 0)
            {
                return new Time(00, 00);
            }

            double totalRequiredHoursToBeWorked = listOfAttendanceRecordDTO.Count(x => x.DayStatus == DayStatus.WorkingDay) * noOfHoursToBeWorked;
            Time totalWorkedTime = CalculateTotalWorkingHours(listOfAttendanceRecordDTO);
            var totalWorkedSpan = new TimeSpan(totalWorkedTime.Hour, totalWorkedTime.Minute, 00);

            double totalDefiateOrOverTimeHrs = totalWorkedSpan.TotalHours - totalRequiredHoursToBeWorked;

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

        private enum AbsoluteTime
        {
            TimeIn,
            TimeOut
        }
        private TimeSpan CalculateAbsoluteOutTimeAndInTime(TimeSpan timeSpan, AbsoluteTime time)
        {
            if (time == AbsoluteTime.TimeOut && timeSpan.Seconds > 0)
            {
                return new TimeSpan(timeSpan.Hours, timeSpan.Minutes + 1, 00);
            }

            return new TimeSpan(timeSpan.Hours, timeSpan.Minutes, 00);
        }

    }
}
