using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Tests;
using UseCaseBoundary;
using UseCaseBoundary.Model;
using UseCases;

namespace Klipper.Tests
{
    public class DetailsOfParticulerDayTestCases
    {
        private IAccessEventsRepository accessEventsContainer;
        private IEmployeeRepository employeeData;
        private IDepartmentRepository departmentData;
        private IAttendanceRegularizationRepository regularizationData;
        private ILeavesRepository leaveData;

        [SetUp]
        public void Setup()
        {
            accessEventsContainer = Substitute.For<IAccessEventsRepository>();
            employeeData = Substitute.For<IEmployeeRepository>();
            departmentData = Substitute.For<IDepartmentRepository>();
            regularizationData = Substitute.For<IAttendanceRegularizationRepository>();
            leaveData = Substitute.For<ILeavesRepository>();
        }

        [Test]
        public async Task GivenDateGetCountOfAccessEventRecords()
        {
            // Setup
            AttendanceService attendanceService =
                new AttendanceService(accessEventsContainer, employeeData, departmentData, regularizationData, leaveData);
            var dummyAccessevents = new AccessEventsBuilder().BuildForADay(DateTime.Parse("2018-10-01"));
            accessEventsContainer.GetAccessEventsForADay(48, DateTime.Parse("2018-10-01")).Returns(dummyAccessevents);

            var listOfAccessEventsRecord = await attendanceService.AccessPointDetails(48, DateTime.Parse("2018-10-01"));

            Assert.That(listOfAccessEventsRecord.Count, Is.EqualTo(4));
        }

        [Test]
        public async Task WithRespectiveTimeInAndTimeOutOfMainEntryGetTotalTimeSpend()
        {
            // Setup
            AttendanceService attendanceService =
                new AttendanceService(accessEventsContainer, employeeData, departmentData, regularizationData, leaveData);
            var dummyAccessevents = new AccessEventsBuilder().BuildForADay(DateTime.Parse("2018-10-05"));
            accessEventsContainer.GetAccessEventsForADay(48, DateTime.Parse("2018-10-05")).Returns(dummyAccessevents);

            // Execute usecase
            var listOfAccessEventsRecord = await attendanceService.AccessPointDetails(48, DateTime.Parse("2018-10-05"));

            Assert.That(listOfAccessEventsRecord[0].TimeSpend.Hour, Is.EqualTo(8));
            Assert.That(listOfAccessEventsRecord[0].TimeSpend.Minute, Is.EqualTo(35));
        }

        [Test]
        public async Task WithRespectiveTimeInAndTimeOutOfRecreationEntryGetTotalTimeSpend()
        {
            // Setup
            AttendanceService attendanceService =
                new AttendanceService(accessEventsContainer, employeeData, departmentData, regularizationData, leaveData);
            var dummyAccessevents = new AccessEventsBuilder().BuildForADay(DateTime.Parse("2018-10-08"));
            accessEventsContainer.GetAccessEventsForADay(48, DateTime.Parse("2018-10-08")).Returns(dummyAccessevents);

            // Execute usecase
            var listOfAccessEventsRecord = await attendanceService.AccessPointDetails(48, DateTime.Parse("2018-10-08"));

            Assert.That(listOfAccessEventsRecord[1].TimeSpend.Hour, Is.EqualTo(0));
            Assert.That(listOfAccessEventsRecord[1].TimeSpend.Minute, Is.EqualTo(45));
        }

        [Test]
        public async Task WithRespectiveTimeInAndTimeOutOfGymnasiumEntryGetTotalTimeSpend()
        {
            // Setup
            AttendanceService attendanceService =
                new AttendanceService(accessEventsContainer, employeeData, departmentData, regularizationData, leaveData);

            var dummyAccessevents = new AccessEventsBuilder().BuildForADay(DateTime.Parse("2018-09-28"));

            accessEventsContainer.GetAccessEventsForADay(48, DateTime.Parse("2018-09-28")).Returns(dummyAccessevents);

            // Execute usecase
            var listOfAccessEventsRecord = await attendanceService.AccessPointDetails(48, DateTime.Parse("2018-09-28"));

            Assert.That(listOfAccessEventsRecord[0].TimeSpend.Hour, Is.EqualTo(0));
            Assert.That(listOfAccessEventsRecord[0].TimeSpend.Minute, Is.EqualTo(7));
        }

        [Test]
        public async Task WithGymnasiumEntryPointAndSingleAccessEventSetTimeOutAndTimeSpendZero()
        {
            // Setup
            AttendanceService attendanceService =
                new AttendanceService(accessEventsContainer, employeeData, departmentData, regularizationData, leaveData);

            var dummyAccessevents = new AccessEventsBuilder().BuildForADay(DateTime.Parse("2018-09-29"));

            accessEventsContainer.GetAccessEventsForADay(48, DateTime.Parse("2018-09-29")).Returns(dummyAccessevents);

            // Execute usecase
            var listOfAccessEventsRecord = await attendanceService.AccessPointDetails(48, DateTime.Parse("2018-09-29"));

            Assert.That(listOfAccessEventsRecord[0].TimeOut.Hour, Is.EqualTo(0));
            Assert.That(listOfAccessEventsRecord[0].TimeOut.Minute, Is.EqualTo(0));

            Assert.That(listOfAccessEventsRecord[0].TimeSpend.Hour, Is.EqualTo(0));
            Assert.That(listOfAccessEventsRecord[0].TimeSpend.Minute, Is.EqualTo(0));
        }

        [Test]
        public async Task WithRecreationEntryPointAndSingleAccessEventSetTimeOutAndTimeSpendZero()
        {
            // Setup
            AttendanceService attendanceService =
                new AttendanceService(accessEventsContainer, employeeData, departmentData, regularizationData, leaveData);

            var dummyAccessevents = new AccessEventsBuilder().BuildForADay(DateTime.Parse("2018-09-29"));

            accessEventsContainer.GetAccessEventsForADay(48, DateTime.Parse("2018-09-29")).Returns(dummyAccessevents);

            // Execute usecase
            var listOfAccessEventsRecord = await attendanceService.AccessPointDetails(48, DateTime.Parse("2018-09-29"));

            Assert.That(listOfAccessEventsRecord[0].TimeOut.Hour, Is.EqualTo(0));
            Assert.That(listOfAccessEventsRecord[0].TimeOut.Minute, Is.EqualTo(0));

            Assert.That(listOfAccessEventsRecord[0].TimeSpend.Hour, Is.EqualTo(0));
            Assert.That(listOfAccessEventsRecord[0].TimeSpend.Minute, Is.EqualTo(0));
        }

        [Test]
        public async Task WithMainEntryPointAndSingleAccessEventSetTimeOutAndTimeSpendZero()
        {
            // Setup
            AttendanceService attendanceService =
                new AttendanceService(accessEventsContainer, employeeData, departmentData, regularizationData, leaveData);

            var dummyAccessevents = new AccessEventsBuilder().BuildForADay(DateTime.Parse("2018-09-29"));

            accessEventsContainer.GetAccessEventsForADay(48, DateTime.Parse("2018-09-29")).Returns(dummyAccessevents);

            // Execute usecase
            var listOfAccessEventsRecord = await attendanceService.AccessPointDetails(48, DateTime.Parse("2018-09-29"));

            Assert.That(listOfAccessEventsRecord[0].TimeOut.Hour, Is.EqualTo(0));
            Assert.That(listOfAccessEventsRecord[0].TimeOut.Minute, Is.EqualTo(0));

            Assert.That(listOfAccessEventsRecord[0].TimeSpend.Hour, Is.EqualTo(0));
            Assert.That(listOfAccessEventsRecord[0].TimeSpend.Minute, Is.EqualTo(0));
        }

        [Test]
        public async Task WithMainEntryPointAndOddNoOfAccessEventsGetTimeInAndTimeOutSetTimeSpendZero()
        {
            // Setup
            AttendanceService attendanceService =
                new AttendanceService(accessEventsContainer, employeeData, departmentData, regularizationData, leaveData);

            var dummyAccessevents = new AccessEventsBuilder().BuildForADay(DateTime.Parse("2018-10-01"));

            accessEventsContainer.GetAccessEventsForADay(48, DateTime.Parse("2018-10-01")).Returns(dummyAccessevents);

            // Execute usecase
            var listOfAccessEventsRecord = await attendanceService.AccessPointDetails(48, DateTime.Parse("2018-10-01"));

            Assert.That(listOfAccessEventsRecord[0].TimeIn.Hour, Is.EqualTo(3));
            Assert.That(listOfAccessEventsRecord[0].TimeIn.Minute, Is.EqualTo(4));
            Assert.That(listOfAccessEventsRecord[0].TimeOut.Hour, Is.EqualTo(7));
            Assert.That(listOfAccessEventsRecord[0].TimeOut.Minute, Is.EqualTo(57));
            Assert.That(listOfAccessEventsRecord[0].TimeSpend.Hour, Is.EqualTo(0));
            Assert.That(listOfAccessEventsRecord[0].TimeSpend.Minute, Is.EqualTo(0));

            Assert.That(listOfAccessEventsRecord[3].TimeIn.Hour, Is.EqualTo(12));
            Assert.That(listOfAccessEventsRecord[3].TimeIn.Minute, Is.EqualTo(5));
            Assert.That(listOfAccessEventsRecord[3].TimeOut.Hour, Is.EqualTo(0));
            Assert.That(listOfAccessEventsRecord[3].TimeOut.Minute, Is.EqualTo(0));
            Assert.That(listOfAccessEventsRecord[3].TimeSpend.Hour, Is.EqualTo(0));
            Assert.That(listOfAccessEventsRecord[3].TimeSpend.Minute, Is.EqualTo(0));
            }

        [Test]
        public async Task WithRecreationEntryPointAndOddNoOfAccessEventsGetTimeInAndTimeOutSetTimeSpendZero()
        {
            // Setup
            AttendanceService attendanceService =
                new AttendanceService(accessEventsContainer, employeeData, departmentData, regularizationData, leaveData);

            var dummyAccessevents = new AccessEventsBuilder().BuildForADay(DateTime.Parse("2018-09-30"));

            accessEventsContainer.GetAccessEventsForADay(48, DateTime.Parse("2018-09-30")).Returns(dummyAccessevents);

            // Execute usecase
            var listOfAccessEventsRecord = await attendanceService.AccessPointDetails(48, DateTime.Parse("2018-09-30"));

            Assert.That(listOfAccessEventsRecord[0].TimeIn.Hour, Is.EqualTo(7));
            Assert.That(listOfAccessEventsRecord[0].TimeIn.Minute, Is.EqualTo(56));
            Assert.That(listOfAccessEventsRecord[0].TimeOut.Hour, Is.EqualTo(8));
            Assert.That(listOfAccessEventsRecord[0].TimeOut.Minute, Is.EqualTo(32));
            Assert.That(listOfAccessEventsRecord[0].TimeSpend.Hour, Is.EqualTo(0));
            Assert.That(listOfAccessEventsRecord[0].TimeSpend.Minute, Is.EqualTo(0));

            Assert.That(listOfAccessEventsRecord[1].TimeIn.Hour, Is.EqualTo(11));
            Assert.That(listOfAccessEventsRecord[1].TimeIn.Minute, Is.EqualTo(21));
            Assert.That(listOfAccessEventsRecord[1].TimeOut.Hour, Is.EqualTo(0));
            Assert.That(listOfAccessEventsRecord[1].TimeOut.Minute, Is.EqualTo(0));
            Assert.That(listOfAccessEventsRecord[1].TimeSpend.Hour, Is.EqualTo(0));
            Assert.That(listOfAccessEventsRecord[1].TimeSpend.Minute, Is.EqualTo(0));
        }

        [Test]
        public async Task WithGymEntryPointAndOddNoOfAccessEventsGetTimeInAndTimeOutSetTimeSpendZero()
        {
            // Setup
            AttendanceService attendanceService =
                new AttendanceService(accessEventsContainer, employeeData, departmentData, regularizationData, leaveData);

            var dummyAccessevents = new AccessEventsBuilder().BuildForADay(DateTime.Parse("2018-09-30"));

            accessEventsContainer.GetAccessEventsForADay(48, DateTime.Parse("2018-09-30")).Returns(dummyAccessevents);

            // Execute usecase
            var listOfAccessEventsRecord = await attendanceService.AccessPointDetails(48, DateTime.Parse("2018-09-30"));

            Assert.That(listOfAccessEventsRecord[2].TimeIn.Hour, Is.EqualTo(12));
            Assert.That(listOfAccessEventsRecord[2].TimeIn.Minute, Is.EqualTo(52));
            Assert.That(listOfAccessEventsRecord[3].TimeIn.Hour, Is.EqualTo(13));
            Assert.That(listOfAccessEventsRecord[3].TimeIn.Minute, Is.EqualTo(10));

            Assert.That(listOfAccessEventsRecord[2].TimeOut.Hour, Is.EqualTo(12));
            Assert.That(listOfAccessEventsRecord[2].TimeOut.Minute, Is.EqualTo(56));
            Assert.That(listOfAccessEventsRecord[3].TimeOut.Hour, Is.EqualTo(0));
            Assert.That(listOfAccessEventsRecord[3].TimeOut.Minute, Is.EqualTo(0));

            Assert.That(listOfAccessEventsRecord[2].TimeSpend.Hour, Is.EqualTo(0));
            Assert.That(listOfAccessEventsRecord[2].TimeSpend.Minute, Is.EqualTo(0));
            Assert.That(listOfAccessEventsRecord[3].TimeSpend.Hour, Is.EqualTo(0));
            Assert.That(listOfAccessEventsRecord[3].TimeSpend.Minute, Is.EqualTo(0));
        }

    }
}
