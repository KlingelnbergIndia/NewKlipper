﻿using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Tests;
using UseCaseBoundary;
using UseCases;

namespace Klipper.Tests
{
    public class DetailsOfParticulerDayTestCases
    {
        private IAccessEventsRepository accessEventsContainer;
        private IEmployeeRepository employeeData;

        [SetUp]
        public void Setup()
        {
            accessEventsContainer = Substitute.For<IAccessEventsRepository>();
            employeeData = Substitute.For<IEmployeeRepository>();
        }

        [Test]
        public async Task GivenDateThenAccessEventsOfThatSpecifiedDateAreDisplayed()
        {
            AttendanceService attendanceService =
                new AttendanceService(accessEventsContainer, employeeData);

            var dummyAccessevents = new AccessEventsBuilder().Build();

            accessEventsContainer.GetAccessEvents(48).Returns(dummyAccessevents);

            var dummyEmployee =
                new EmployeeBuilder()
                .WithUserName("Sidhdesh.Vadgaonkar")
                .WithPassword("26-12-1995")
                .BuildEmployee();

            employeeData.GetEmployee(48).Returns(dummyEmployee);

            var listOfAccessEventsOfSpecificDate = await attendanceService.GetAccessPointDetails(48,DateTime.Parse("08-10-2018").Date);

            Assert.That(listOfAccessEventsOfSpecificDate.Count, Is.EqualTo(7));
        }

        [Test]
        public async Task GivenDateForSpecificDay()
        {
            AttendanceService attendanceService =
                new AttendanceService(accessEventsContainer, employeeData);

            var dummyAccessevents = new AccessEventsBuilder().Build();

            accessEventsContainer.GetAccessEvents(48).Returns(dummyAccessevents);

            var dummyEmployee =
                new EmployeeBuilder()
                .WithUserName("Sidhdesh.Vadgaonkar")
                .WithPassword("26-12-1995")
                .BuildEmployee();

            employeeData.GetEmployee(48).Returns(dummyEmployee);

            var listOfAttendanceRecordForSpecifiedDays = await attendanceService.GetAttendanceRecord(48, 7);

            Assert.That(listOfAttendanceRecordForSpecifiedDays.ListOfAttendanceRecordDTO[4].LateBy.Hour, Is.EqualTo(9));

            Assert.That(listOfAttendanceRecordForSpecifiedDays.ListOfAttendanceRecordDTO[4].LateBy.Minute, Is.EqualTo(0));
        }


        [Test]
        public async Task GivenSetOfAccessEventsCalculatesAccurateOvertimeForSpecificDay()
        {
            AttendanceService attendanceService =
                new AttendanceService(accessEventsContainer, employeeData);

            var dummyAccessevents = new AccessEventsBuilder().Build();

            accessEventsContainer.GetAccessEvents(48).Returns(dummyAccessevents);

            var dummyEmployee =
                new EmployeeBuilder()
                .WithUserName("Sidhdesh.Vadgaonkar")
                .WithPassword("26-12-1995")
                .BuildEmployee();

            employeeData.GetEmployee(48).Returns(dummyEmployee);

            var listOfAttendanceRecordForSpecifiedDays = await attendanceService.GetAttendanceRecord(48, 7);

            Assert.That(listOfAttendanceRecordForSpecifiedDays.ListOfAttendanceRecordDTO[1].OverTime.Hour, Is.EqualTo(2));

            Assert.That(listOfAttendanceRecordForSpecifiedDays.ListOfAttendanceRecordDTO[1].OverTime.Minute, Is.EqualTo(3));
        }

        [Test]
        public async Task GivenSetOfAccessEventsCalcualtesAccurateWorkingHoursForSpecificDay()
        {
            AttendanceService attendanceService =
                new AttendanceService(accessEventsContainer, employeeData);

            var dummyAccessevents = new AccessEventsBuilder().Build();

            accessEventsContainer.GetAccessEvents(48).Returns(dummyAccessevents);

            var dummyEmployee =
                new EmployeeBuilder()
                .WithUserName("Sidhdesh.Vadgaonkar")
                .WithPassword("26-12-1995")
                .BuildEmployee();

            employeeData.GetEmployee(48).Returns(dummyEmployee);

            var listOfAttendanceRecordForSpecifiedDays = await attendanceService.GetAttendanceRecord(48, 7);

            Assert.That(listOfAttendanceRecordForSpecifiedDays.ListOfAttendanceRecordDTO[1].WorkingHours.Hour, Is.EqualTo(11));

            Assert.That(listOfAttendanceRecordForSpecifiedDays.ListOfAttendanceRecordDTO[1].WorkingHours.Minute, Is.EqualTo(3));
        }

        [Test]
        public async Task GivenSevenDaysAttendanceRecordCalculatesAccurateTotalWorkingTime()
        {

            AttendanceService attendanceService =
                new AttendanceService(accessEventsContainer, employeeData);

            var dummyAccessevents = new AccessEventsBuilder().Build();

            accessEventsContainer.GetAccessEvents(48).Returns(dummyAccessevents);

            var dummyEmployee =
                new EmployeeBuilder()
                .WithUserName("Sidhdesh.Vadgaonkar")
                .WithPassword("26-12-1995")
                .BuildEmployee();

            employeeData.GetEmployee(48).Returns(dummyEmployee);

            var listOfAttendanceRecordForSpecifiedDays = await attendanceService.GetAttendanceRecord(48, 7);

            Assert.That(listOfAttendanceRecordForSpecifiedDays.TotalWorkingHours.Hour, Is.EqualTo(19));

            Assert.That(listOfAttendanceRecordForSpecifiedDays.TotalWorkingHours.Minute, Is.EqualTo(39));

        }

        [Test]
        public async Task GivenSevenDaysAttendanceRecordCalculatesAccurateTotalDeficitTime()
        {

            AttendanceService attendanceService =
                new AttendanceService(accessEventsContainer, employeeData);

            var dummyAccessevents = new AccessEventsBuilder().Build();

            accessEventsContainer.GetAccessEvents(48).Returns(dummyAccessevents);

            var dummyEmployee =
                new EmployeeBuilder()
                .WithUserName("Sidhdesh.Vadgaonkar")
                .WithPassword("26-12-1995")
                .BuildEmployee();

            employeeData.GetEmployee(48).Returns(dummyEmployee);

            var listOfAttendanceRecordForSpecifiedDays = await attendanceService.GetAttendanceRecord(48, 7);

            Assert.That(listOfAttendanceRecordForSpecifiedDays.TotalDeficitOrExtraHours.Hour, Is.EqualTo(43));

            Assert.That(listOfAttendanceRecordForSpecifiedDays.TotalDeficitOrExtraHours.Minute, Is.EqualTo(21));

        }

        //[Test]
        //public async Task GivenSevenDaysAttendanceRecordCalculatesAccurateTotalOverTime()
        //{
        //    AttendanceService attendanceService =
        //        new AttendanceService(accessEventsContainer);

        //    var dummyAccessevents = new AccessEventsBuilder().Build();

        //    accessEventsContainer.GetAccessEvents(48).Returns(dummyAccessevents);

        //    var listOfAttendanceRecordForSpecifiedDays = await attendanceService.GetAttendanceRecord(48, 15);

        //    Assert.That(listOfAttendanceRecordForSpecifiedDays.TotalDeficitOrExtraHours.Hour, Is.EqualTo(17));

        //    Assert.That(listOfAttendanceRecordForSpecifiedDays.TotalDeficitOrExtraHours.Minute, Is.EqualTo(2));
        //}


    }
}
