using DomainModel;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UseCaseBoundary;
using UseCaseBoundary.Model;
using UseCases;

namespace Klipper.Tests
{

    class AttendanceServiceForGivenDateRangeTest
    {
        private IAccessEventsRepository accessEventsData;

        [SetUp]
        public void setup()
        {
            accessEventsData = Substitute.For<IAccessEventsRepository>();
        }

        [Test]
        public void GivenDateRangeAndEmployeeIdShouldDisplayCorrectDataCount()
        {
            AttendanceService attendanceService = new AttendanceService(accessEventsData);
            var dummyAccessevents = new AccessEventsBuilder().Build();
            accessEventsData.GetAccessEventsForDateRange(48, DateTime.Parse("2018-01-01"), DateTime.Parse("2018-01-07"))
                .Returns(dummyAccessevents);

            var accessEvents = attendanceService.GetAccessEventsForDateRange(48, DateTime.Parse("2018-01-01"), DateTime.Parse("2018-01-07"));

            Assert.That(accessEvents.Result.Count, Is.EqualTo(dummyAccessevents.GetAllAccessEvents().Count));
        }

        [Test]
        public void GivenDateRangeAndEmployeeIdShouldDisplayCorrectData()
        {
            AttendanceService attendanceService = new AttendanceService(accessEventsData);
            var dummyAccessevents = new AccessEventsBuilder().Build();
            accessEventsData.GetAccessEventsForDateRange(48, DateTime.Parse("2018-01-01"), DateTime.Parse("2018-01-07"))
                .Returns(dummyAccessevents);

            var accessEvents = attendanceService.GetAccessEventsForDateRange(48, DateTime.Parse("2018-01-01"), DateTime.Parse("2018-01-07")).Result;
            var dummyAccessEvents = dummyAccessevents.GetAllAccessEvents();
            var accessEventDto = ConvertToAccessEventsDTO(dummyAccessEvents);

            Assert.That(accessEvents[0].Date, Is.EqualTo(accessEventDto[0].Date));
            Assert.That(accessEvents[0].LateBy.Hour, Is.EqualTo(accessEventDto[0].LateBy.Hour));
            Assert.That(accessEvents[0].LateBy.Minute, Is.EqualTo(accessEventDto[0].LateBy.Minute));

            Assert.That(accessEvents[0].OverTime.Hour, Is.EqualTo(accessEventDto[0].OverTime.Hour));
            Assert.That(accessEvents[0].OverTime.Minute, Is.EqualTo(accessEventDto[0].OverTime.Minute));

            Assert.That(accessEvents[0].TimeIn.Hour, Is.EqualTo(accessEventDto[0].TimeIn.Hour));
            Assert.That(accessEvents[0].TimeIn.Minute, Is.EqualTo(accessEventDto[0].TimeIn.Minute));

            Assert.That(accessEvents[0].TimeOut.Hour, Is.EqualTo(accessEventDto[0].TimeOut.Hour));
            Assert.That(accessEvents[0].TimeOut.Minute, Is.EqualTo(accessEventDto[0].TimeOut.Minute));
        }

        private List<AttendanceRecordDTO> ConvertToAccessEventsDTO(IList<PerDayWorkRecord> accessEventsFromDummy)
        {
            List<AttendanceRecordDTO> listOfAttendanceRecord = new List<AttendanceRecordDTO>();
            foreach (var perDayWorkRecord in accessEventsFromDummy)
            {
                var timeIn = perDayWorkRecord.GetTimeIn();
                var timeOut = perDayWorkRecord.GetTimeOut();
                var workingHours = perDayWorkRecord.CalculateWorkingHours();

                AttendanceRecordDTO attendanceRecord = new AttendanceRecordDTO()
                {
                    Date = perDayWorkRecord.Date,
                    TimeIn = new Time(timeIn.Hours, timeIn.Minutes),
                    TimeOut = new Time(timeOut.Hours, timeOut.Minutes),
                    WorkingHours = new Time(workingHours.Hours, workingHours.Minutes),
                    OverTime = GetOverTime(workingHours),
                    LateBy = GetLateByTime(workingHours)
                };
                listOfAttendanceRecord.Add(attendanceRecord);
            }
            return listOfAttendanceRecord;
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
