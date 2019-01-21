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
        private IEmployeeRepository _employeeRepository;
        public AttendanceService(IAccessEventsRepository accessEventsRepository, IEmployeeRepository employeeRepository)
        {
            _accessEventsRepository = accessEventsRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task<AttendanceRecordsDTO> GetAttendanceRecord(int employeeId, int noOfDays)
        {
            AccessEvents accessEvents = _accessEventsRepository.GetAccessEvents(employeeId);
            Employee employeeData = _employeeRepository.GetEmployee(employeeId);
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
                    OverTime = GetOverTime(workingHours, GetNoOfHoursToBeWorked(employeeData.Department())),
                    LateBy = GetLateByTime(workingHours, GetNoOfHoursToBeWorked(employeeData.Department()))
                };
                listOfAttendanceRecordDTO.Add(attendanceRecord);
            }

            return await Task.Run(() =>
            {
                return new AttendanceRecordsDTO()
                {
                    ListOfAttendanceRecordDTO = listOfAttendanceRecordDTO,
                    TotalWorkingHours = CalculateTotalWorkingHours(listOfAttendanceRecordDTO),
                    TotalDeficitOrExtraHours = CalculateDeficiateOrExtraTime(listOfAttendanceRecordDTO,GetNoOfHoursToBeWorked(employeeData.Department())),
                };
            });
        }



        public async Task<AttendanceRecordsDTO> GetAccessEventsForDateRange(int employeeId, DateTime fromDate, DateTime toDate)
        {
            var accessEvents = _accessEventsRepository.GetAccessEventsForDateRange(employeeId, fromDate, toDate);
            var datewiseAccessEvents = accessEvents.GetAllAccessEvents();
            Employee employeeData = _employeeRepository.GetEmployee(employeeId);

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
                    OverTime = GetOverTime(workingHours, GetNoOfHoursToBeWorked(employeeData.Department())),
                    LateBy = GetLateByTime(workingHours, GetNoOfHoursToBeWorked(employeeData.Department()))
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
                    TotalDeficitOrExtraHours = CalculateDeficiateOrExtraTime(listOfAttendanceRecordDTO, GetNoOfHoursToBeWorked(employeeData.Department())),
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

            double totalRequiredHoursToBeWorked = listOfAttendanceRecordDTO.Count * noOfHoursToBeWorked;
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

        private double GetNoOfHoursToBeWorked(Departments department)
        {
            return department == Departments.Design ? 10.0 : 9.0;
        }
    }
}
