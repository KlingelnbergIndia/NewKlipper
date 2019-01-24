using DomainModel;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Tests;
using UseCaseBoundary;
using UseCaseBoundary.Model;
using UseCases;

namespace Klipper.Tests
{

    class AttendanceServiceForGivenDateRangeTest
    {
        private IAccessEventsRepository accessEventsData;
        private IEmployeeRepository employeeData;
        

        [SetUp]
        public void setup()
        {
            accessEventsData = Substitute.For<IAccessEventsRepository>();
            employeeData = Substitute.For<IEmployeeRepository>();
        }

        [Test]
        public void GivenDateRangeAndEmployeeIdShouldDisplayCorrectNumberOfRecords()
        {
            AttendanceService attendanceService = new AttendanceService(accessEventsData, employeeData);
            var dummyAccessevents = new AccessEventsBuilder().Build();
            accessEventsData.GetAccessEventsForDateRange(48, DateTime.Parse("2018-10-01"), DateTime.Parse("2018-10-30"))
                .Returns(dummyAccessevents);

            var dummyEmployee =
                new EmployeeBuilder()
                .WithUserName("Sidhdesh.Vadgaonkar")
                .WithPassword("26-12-1995")
                .BuildEmployee();

            employeeData.GetEmployee(48).Returns(dummyEmployee);

            var accessEvents = attendanceService
                .GetAccessEventsForDateRange(48, DateTime.Parse("2018-10-01"), DateTime.Parse("2018-10-30"))
                .GetAwaiter()
                .GetResult();

            Assert.That(accessEvents.ListOfAttendanceRecordDTO.Count, Is.EqualTo(33));
        }

        [Test]
        public void RetrivedAccessEventsHasAccurateData()
        {
            AttendanceService attendanceService = new AttendanceService(accessEventsData, employeeData);
            var dummyAccessevents = new AccessEventsBuilder().Build();
            accessEventsData.GetAccessEventsForDateRange(48, DateTime.Parse("2018-10-01"), DateTime.Parse("2018-10-30"))
                .Returns(dummyAccessevents);

            var dummyEmployee =
                new EmployeeBuilder()
                .WithUserName("Sidhdesh.Vadgaonkar")
                .WithPassword("26-12-1995")
                .BuildEmployee();

            employeeData.GetEmployee(48).Returns(dummyEmployee);

            var accessEvents = attendanceService
                .GetAccessEventsForDateRange(48, DateTime.Parse("2018-10-01"), DateTime.Parse("2018-10-30"))
                .GetAwaiter()
                .GetResult();

            Assert.That(accessEvents.ListOfAttendanceRecordDTO[21].Date, Is.EqualTo(DateTime.Parse("2018/10/09").Date));
            Assert.That(accessEvents.ListOfAttendanceRecordDTO[21].TimeIn.Hour, Is.EqualTo(2));
            Assert.That(accessEvents.ListOfAttendanceRecordDTO[21].TimeIn.Minute, Is.EqualTo(10));

            Assert.That(accessEvents.ListOfAttendanceRecordDTO[21].LateBy.Hour, Is.EqualTo(0));
            Assert.That(accessEvents.ListOfAttendanceRecordDTO[21].LateBy.Minute, Is.EqualTo(0));
            
            Assert.That(accessEvents.ListOfAttendanceRecordDTO[21].OverTime.Hour, Is.EqualTo(2));
            Assert.That(accessEvents.ListOfAttendanceRecordDTO[21].OverTime.Minute, Is.EqualTo(3));
            
            Assert.That(accessEvents.ListOfAttendanceRecordDTO[21].TimeOut.Hour, Is.EqualTo(13));
            Assert.That(accessEvents.ListOfAttendanceRecordDTO[21].TimeOut.Minute, Is.EqualTo(13));
        }

    }
}
