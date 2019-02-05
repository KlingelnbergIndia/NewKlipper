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
using Tests;
using System.Linq;

namespace Klipper.Tests
{
    public class AttendanceServiceTest
    {
        private IAccessEventsRepository accessEventsContainer;
        private IEmployeeRepository employeeData;
        private IDepartmentRepository departmentData;

        [SetUp]
        public void Setup()
        {
            accessEventsContainer = Substitute.For<IAccessEventsRepository>();
            employeeData = Substitute.For<IEmployeeRepository>();
            departmentData = Substitute.For<IDepartmentRepository>();
            var department = new Department(Departments.Software);
            departmentData.GetDepartment(Departments.Software).Returns(department);
        }

        [Test]
        public async Task GivenSevenDaysThenSevenRecordsAreDisplayed()
        {
            AttendanceService attendanceService =
                new AttendanceService(accessEventsContainer, employeeData, departmentData);

            var dummyAccessevents = new AccessEventsBuilder().Build();

            accessEventsContainer.GetAccessEvents(48).Returns(dummyAccessevents);

            var dummyEmployee =
                new EmployeeBuilder()
                .WithUserName("Sidhdesh.Vadgaonkar")
                .WithPassword("26-12-1995")
                .BuildEmployee();

            employeeData.GetEmployee(48).Returns(dummyEmployee);

            var listOfAttendanceRecordForSpecifiedDays = await attendanceService.GetAttendanceRecord(48, 7);

            Assert.That(listOfAttendanceRecordForSpecifiedDays.ListOfAttendanceRecordDTO.Count, Is.EqualTo(7));
        }

        [Test]
        public async Task GivenSetOfAccessEventsCalculatesAccurateDeficitTimeForSpecificDay()
        {
            AttendanceService attendanceService =
                new AttendanceService(accessEventsContainer, employeeData, departmentData);

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
                new AttendanceService(accessEventsContainer, employeeData, departmentData);

            var dummyAccessevents = new AccessEventsBuilder().Build();

            accessEventsContainer.GetAccessEvents(48).Returns(dummyAccessevents);

            var dummyEmployee =
                new EmployeeBuilder()
                .WithUserName("Sidhdesh.Vadgaonkar")
                .WithPassword("26-12-1995")
                .BuildEmployee();

            employeeData.GetEmployee(48).Returns(dummyEmployee);

            var listOfAttendanceRecordForSpecifiedDays = await attendanceService.GetAttendanceRecord(48, 7);
            var expectedHoursData = listOfAttendanceRecordForSpecifiedDays.ListOfAttendanceRecordDTO.Single(x => x.Date == DateTime.Parse("2018/10/09"));

            Assert.That(expectedHoursData.OverTime.Hour, Is.EqualTo(2));

            Assert.That(expectedHoursData.OverTime.Minute, Is.EqualTo(3));
        }

        [Test]
        public async Task GivenSetOfAccessEventsCalcualtesAccurateWorkingHoursForSpecificDay()
        {
            AttendanceService attendanceService =
                new AttendanceService(accessEventsContainer, employeeData, departmentData);

            var dummyAccessevents = new AccessEventsBuilder().Build();

            accessEventsContainer.GetAccessEvents(48).Returns(dummyAccessevents);

            var dummyEmployee =
                new EmployeeBuilder()
                .WithUserName("Sidhdesh.Vadgaonkar")
                .WithPassword("26-12-1995")
                .BuildEmployee();

            employeeData.GetEmployee(48).Returns(dummyEmployee);
            var listOfAttendanceRecordForSpecifiedDays = await attendanceService.GetAttendanceRecord(48, 7);
            var expectedWorkingHours = listOfAttendanceRecordForSpecifiedDays.ListOfAttendanceRecordDTO.Single(x => x.Date == DateTime.Parse("2018/10/09"));

            Assert.That(
                expectedWorkingHours.WorkingHours.Hour,
                Is.EqualTo(11));
            Assert.That(
               expectedWorkingHours.WorkingHours.Minute,
               Is.EqualTo(3));
        }

        [Test]
        public async Task GivenSevenDaysAttendanceRecordCalculatesAccurateTotalWorkingTime()
        {

            AttendanceService attendanceService =
                new AttendanceService(accessEventsContainer, employeeData, departmentData);

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
                new AttendanceService(accessEventsContainer, employeeData, departmentData);

            var dummyAccessevents = new AccessEventsBuilder().Build();

            accessEventsContainer.GetAccessEvents(48).Returns(dummyAccessevents);

            var dummyEmployee =
                new EmployeeBuilder()
                .WithUserName("Sidhdesh.Vadgaonkar")
                .WithPassword("26-12-1995")
                .BuildEmployee();

            employeeData.GetEmployee(48).Returns(dummyEmployee);

            var listOfAttendanceRecordForSpecifiedDays = await attendanceService.GetAttendanceRecord(48, 7);

            Assert.That(listOfAttendanceRecordForSpecifiedDays.TotalDeficitOrExtraHours.Hour, Is.EqualTo(-43));

            Assert.That(listOfAttendanceRecordForSpecifiedDays.TotalDeficitOrExtraHours.Minute, Is.EqualTo(-21));

        }

        [Test]
        public async Task GivenOddNOoFGymEntryAccessEventsSetWorkingHoursZero()
        {
            AttendanceService attendanceService =
                new AttendanceService(accessEventsContainer, employeeData, departmentData);

            var dummyAccessevents = new AccessEventsBuilder().Build();
            accessEventsContainer.GetAccessEvents(48).Returns(dummyAccessevents);

            var dummyEmployee =
                new EmployeeBuilder()
                .WithUserName("Sidhdesh.Vadgaonkar")
                .WithPassword("26-12-1995")
                .BuildEmployee();
            employeeData.GetEmployee(48).Returns(dummyEmployee);
            var listOfAttendanceRecordForSpecifiedDays = await attendanceService.GetAttendanceRecord(48, 7);

            Assert.That(
                listOfAttendanceRecordForSpecifiedDays.ListOfAttendanceRecordDTO[4].WorkingHours.Hour,
                Is.EqualTo(0));
            Assert.That(
               listOfAttendanceRecordForSpecifiedDays.ListOfAttendanceRecordDTO[4].WorkingHours.Minute,
               Is.EqualTo(0));
        }

        [Test]
        public async Task GivenOddNOoFMainEntryAccessEventsSetWorkingHoursZero()
        {
            AttendanceService attendanceService =
                new AttendanceService(accessEventsContainer, employeeData, departmentData);

            var dummyAccessevents = new AccessEventsBuilder().Build();
            accessEventsContainer.GetAccessEvents(48).Returns(dummyAccessevents);

            var dummyEmployee =
                new EmployeeBuilder()
                .WithUserName("Sidhdesh.Vadgaonkar")
                .WithPassword("26-12-1995")
                .BuildEmployee();
            employeeData.GetEmployee(48).Returns(dummyEmployee);

            var listOfAttendanceRecordForSpecifiedDays = await attendanceService.GetAttendanceRecord(48, 7);
            var expectedWorkingHours = listOfAttendanceRecordForSpecifiedDays.ListOfAttendanceRecordDTO.Single(x=>x.Date == DateTime.Parse("2018/10/09"));

            Assert.That(
                expectedWorkingHours.WorkingHours.Hour,
                Is.EqualTo(0));
            Assert.That(
               expectedWorkingHours.WorkingHours.Minute,
               Is.EqualTo(0));
        }
    }
}
