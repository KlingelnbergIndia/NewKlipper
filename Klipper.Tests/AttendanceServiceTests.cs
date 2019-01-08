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
    public class AttendanceServiceTest
    {
        private IAccessEventsRepository _accessEventsRepository;

        [SetUp]
        public void Setup()
        {
            _accessEventsRepository = Substitute.For<IAccessEventsRepository>();
        }

        [Test]
        public async Task GivenSevenDaysThenSevenRecordsAreDisplayed()
        {
            AttendanceService attendanceRecordForEmployeeID =
                new AttendanceService(_accessEventsRepository);

            _accessEventsRepository.GetAccessEvents(48).Returns(DummyAccessEvents());

            var listOfAttendanceRecordOfSevenDays = await attendanceRecordForEmployeeID.GetAttendanceRecord(48, 7);

            Assert.That(listOfAttendanceRecordOfSevenDays.Count, Is.EqualTo(7));
        }

        [Test]
        public async Task GivenSetOfAccessEventsCalculatesAccurateLateByTime()
        {
            AttendanceService attendanceRecordForEmployeeID =
                new AttendanceService(_accessEventsRepository);

            _accessEventsRepository.GetAccessEvents(48).Returns(DummyAccessEvents());

            var listOfAttendanceRecordOfSevenDays = await attendanceRecordForEmployeeID.GetAttendanceRecord(48, 7);

            Assert.That(listOfAttendanceRecordOfSevenDays[4].LateBy.Hour, Is.EqualTo(0));

            Assert.That(listOfAttendanceRecordOfSevenDays[4].LateBy.Minute, Is.EqualTo(22));
        }


        [Test]
        public async Task GivenSetOfAccessEventsCalculatesAccurateOvertime()
        {
            AttendanceService attendanceRecordForEmployeeID =
                new AttendanceService(_accessEventsRepository);

            _accessEventsRepository.GetAccessEvents(48).Returns(DummyAccessEvents());

            var listOfAttendanceRecordOfSevenDays = await attendanceRecordForEmployeeID.GetAttendanceRecord(48, 7);

            Assert.That(listOfAttendanceRecordOfSevenDays[1].OverTime.Hour, Is.EqualTo(2));

            Assert.That(listOfAttendanceRecordOfSevenDays[1].OverTime.Minute, Is.EqualTo(1));
        }

        [Test]
        public async Task GivenSetOfAccessEventsCalcualtesAccurateWorkingHours()
        {
            AttendanceService attendanceRecordForEmployeeID =
                new AttendanceService(_accessEventsRepository);

            _accessEventsRepository.GetAccessEvents(48).Returns(DummyAccessEvents());

            var listOfAttendanceRecordOfSevenDays = await attendanceRecordForEmployeeID.GetAttendanceRecord(48, 7);

            Assert.That(listOfAttendanceRecordOfSevenDays[4].WorkingHours.Hour, Is.EqualTo(8));

            Assert.That(listOfAttendanceRecordOfSevenDays[4].WorkingHours.Minute, Is.EqualTo(37));
        }

        private AccessEvents DummyAccessEvents()
        {
            string currentDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

            string accessEventsFilePath = currentDirectory.Remove(currentDirectory.Length - 3) + "AccessEventsDummyData.json";

            List<AccessEvent> dummyAccessEvent = new List<AccessEvent>();

            var jsonData = File.ReadAllText(accessEventsFilePath);

            dummyAccessEvent = JsonConvert.DeserializeObject<List<AccessEvent>>(jsonData);

            AccessEvents accessEvents = new AccessEvents(dummyAccessEvent);

            return accessEvents;

        }
    }
}
