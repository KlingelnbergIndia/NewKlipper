using DomainModel;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests;
using UseCaseBoundary;
using UseCaseBoundary.DTO;
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
                .WithID(48)
                .BuildEmployee();
            employeeData.GetEmployee(48).Returns(dummyEmployee);

            var accessEvents = attendanceService
                .GetAccessEventsForDateRange(48, DateTime.Parse("2018-10-01"), DateTime.Parse("2018-10-30"))
                .GetAwaiter()
                .GetResult();
            var actualData = accessEvents.ListOfAttendanceRecordDTO.Where(x => x.Date == DateTime.Parse("2018/10/09")).Single();

            var expectedData= new PerDayAttendanceRecordDTO()
            {
                Date = DateTime.Parse("2018/10/09"),
                TimeIn = new Time(2, 10),
                TimeOut = new Time(13, 13),
                OverTime = new Time(2, 3),
                LateBy = new Time(0, 0),
                WorkingHours = new Time(11,3)
            };

            Assert.That(actualData.Date, Is.EqualTo(expectedData.Date));
            Assert.That(actualData.TimeIn.Hour, Is.EqualTo(expectedData.TimeIn.Hour));
            Assert.That(actualData.TimeIn.Minute, Is.EqualTo(expectedData.TimeIn.Minute));

            Assert.That(actualData.LateBy.Hour, Is.EqualTo(expectedData.LateBy.Hour));
            Assert.That(actualData.LateBy.Minute, Is.EqualTo(expectedData.LateBy.Minute));

            Assert.That(actualData.OverTime.Hour, Is.EqualTo(expectedData.OverTime.Hour));
            Assert.That(actualData.OverTime.Minute, Is.EqualTo(expectedData.OverTime.Minute));

            Assert.That(actualData.TimeOut.Hour, Is.EqualTo(expectedData.TimeOut.Hour));
            Assert.That(actualData.TimeOut.Minute, Is.EqualTo(expectedData.TimeOut.Minute));
        }

        [Test]
        public async Task GivenDaterangeAttendanceRecordCalculatesAccurateTotalDeficitTime()
        {

            AttendanceService attendanceService = new AttendanceService(accessEventsData, employeeData);
            var dummyAccessevents = new AccessEventsBuilder().BuildBetweenDate(DateTime.Parse("2018-10-01"), DateTime.Parse("2018-10-03"));
            accessEventsData
                .GetAccessEventsForDateRange(48, DateTime.Parse("2018-10-01"), DateTime.Parse("2018-10-03"))
                .Returns(dummyAccessevents);

            var dummyEmployee =
                new EmployeeBuilder()
                .WithUserName("Sidhdesh.Vadgaonkar")
                .WithPassword("26-12-1995")
                .BuildEmployee();
            employeeData.GetEmployee(48).Returns(dummyEmployee);
            var listOfAttendanceRecords = await attendanceService.GetAccessEventsForDateRange(48, DateTime.Parse("2018-10-01"), DateTime.Parse("2018-10-30"));

            Assert.That(listOfAttendanceRecords.TotalDeficitOrExtraHours.Hour, Is.EqualTo(43));
            Assert.That(listOfAttendanceRecords.TotalDeficitOrExtraHours.Minute, Is.EqualTo(21));

        }

    }
}
