using System;
using System.Collections.Generic;
using System.Text;
using NSubstitute;
using UseCaseBoundary;
using NUnit.Framework;
using UseCases;
using DomainModel;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using UseCaseBoundary.Model;
using System.Reflection;

namespace Klipper.Tests
{

    public class AccessEventsBuilder
    {
        private string accessEventsFilePath;

        public AccessEventsBuilder()
        {
            string currentDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

            accessEventsFilePath = currentDirectory.Remove(currentDirectory.Length - 3) + "AccessEventsDummyData.json";
        }

        public AccessEvents Build()
        {

            List<AccessEvent> dummyAccessEvent = new List<AccessEvent>();

            var jsonData = File.ReadAllText(accessEventsFilePath);

            dummyAccessEvent = JsonConvert.DeserializeObject<List<AccessEvent>>(jsonData);

            AccessEvents dummyAccessEvents = new AccessEvents(dummyAccessEvent);

            return dummyAccessEvents;

        }
    }

    public class AttendanceServiceTest
    {
        private IAccessEventsRepository accessEventsContainer;

        [SetUp]
        public void Setup()
        {
            accessEventsContainer = Substitute.For<IAccessEventsRepository>();
        }

        [Test]
        public async Task GivenSevenDaysThenSevenRecordsAreDisplayed()
        {
            AttendanceService attendanceService =
                new AttendanceService(accessEventsContainer);

            var dummyAccessevents = new AccessEventsBuilder().Build();

            accessEventsContainer.GetAccessEvents(48).Returns(dummyAccessevents);

            var listOfAttendanceRecordForSpecifiedDays = await attendanceService.GetAttendanceRecord(48, 7);

            Assert.That(listOfAttendanceRecordForSpecifiedDays.Count, Is.EqualTo(7));
        }

        [Test]
        public async Task GivenSetOfAccessEventsCalculatesAccurateLateByTime()
        {
            AttendanceService attendanceService =
                new AttendanceService(accessEventsContainer);

            var dummyAccessevents = new AccessEventsBuilder().Build();

            accessEventsContainer.GetAccessEvents(48).Returns(dummyAccessevents);

            var listOfAttendanceRecordForSpecifiedDays = await attendanceService.GetAttendanceRecord(48, 7);

            Assert.That(listOfAttendanceRecordForSpecifiedDays[4].LateBy.Hour, Is.EqualTo(0));

            Assert.That(listOfAttendanceRecordForSpecifiedDays[4].LateBy.Minute, Is.EqualTo(22));
        }


        [Test]
        public async Task GivenSetOfAccessEventsCalculatesAccurateOvertime()
        {
            AttendanceService attendanceService =
                new AttendanceService(accessEventsContainer);

            var dummyAccessevents = new AccessEventsBuilder().Build();

            accessEventsContainer.GetAccessEvents(48).Returns(dummyAccessevents);

            var listOfAttendanceRecordForSpecifiedDays = await attendanceService.GetAttendanceRecord(48, 7);

            Assert.That(listOfAttendanceRecordForSpecifiedDays[1].OverTime.Hour, Is.EqualTo(2));

            Assert.That(listOfAttendanceRecordForSpecifiedDays[1].OverTime.Minute, Is.EqualTo(1));
        }

        [Test]
        public async Task GivenSetOfAccessEventsCalcualtesAccurateWorkingHours()
        {
            AttendanceService attendanceService =
                new AttendanceService(accessEventsContainer);

            var dummyAccessevents = new AccessEventsBuilder().Build();

            accessEventsContainer.GetAccessEvents(48).Returns(dummyAccessevents);

            var listOfAttendanceRecordForSpecifiedDays = await attendanceService.GetAttendanceRecord(48, 7);

            Assert.That(listOfAttendanceRecordForSpecifiedDays[4].WorkingHours.Hour, Is.EqualTo(8));

            Assert.That(listOfAttendanceRecordForSpecifiedDays[4].WorkingHours.Minute, Is.EqualTo(37));
        }


    }
}
