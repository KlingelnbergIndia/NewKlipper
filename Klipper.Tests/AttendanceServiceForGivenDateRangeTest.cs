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
        public void GivenDateRangeAndEmployeeIdShouldDisplayCorrectNumberOfRecords()
        {
            AttendanceService attendanceService = new AttendanceService(accessEventsData);
            var dummyAccessevents = new AccessEventsBuilder().Build();
            accessEventsData.GetAccessEventsForDateRange(48, DateTime.Parse("2018-10-01"), DateTime.Parse("2018-10-30"))
                .Returns(dummyAccessevents);

            var accessEvents = attendanceService
                .GetAccessEventsForDateRange(48, DateTime.Parse("2018-10-01"), DateTime.Parse("2018-10-30"))
                .GetAwaiter()
                .GetResult();

            Assert.That(accessEvents.ListOfAttendanceRecordDTO.Count, Is.EqualTo(9));
        }

        [Test]
        public void RetrivedAccessEventsHasAccurateData()
        {
            AttendanceService attendanceService = new AttendanceService(accessEventsData);
            var dummyAccessevents = new AccessEventsBuilder().Build();
            accessEventsData.GetAccessEventsForDateRange(48, DateTime.Parse("2019-10-01"), DateTime.Parse("2019-10-30"))
                .Returns(dummyAccessevents);

            var accessEvents = attendanceService
                .GetAccessEventsForDateRange(48, DateTime.Parse("2019-10-01"), DateTime.Parse("2019-10-30"))
                .GetAwaiter()
                .GetResult();

            Assert.That(accessEvents.ListOfAttendanceRecordDTO[0].Date, Is.EqualTo(DateTime.Parse("2018/10/12").Date));
            Assert.That(accessEvents.ListOfAttendanceRecordDTO[0].TimeIn.Hour, Is.EqualTo(2));
            Assert.That(accessEvents.ListOfAttendanceRecordDTO[0].TimeIn.Minute, Is.EqualTo(58));

            Assert.That(accessEvents.ListOfAttendanceRecordDTO[0].LateBy.Hour, Is.EqualTo(9));
            Assert.That(accessEvents.ListOfAttendanceRecordDTO[0].LateBy.Minute, Is.EqualTo(0));
            
            Assert.That(accessEvents.ListOfAttendanceRecordDTO[0].OverTime.Hour, Is.EqualTo(0));
            Assert.That(accessEvents.ListOfAttendanceRecordDTO[0].OverTime.Minute, Is.EqualTo(0));
            
            Assert.That(accessEvents.ListOfAttendanceRecordDTO[0].TimeOut.Hour, Is.EqualTo(0));
            Assert.That(accessEvents.ListOfAttendanceRecordDTO[0].TimeOut.Minute, Is.EqualTo(0));
        }

    }
}
