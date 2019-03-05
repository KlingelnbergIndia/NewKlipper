using DomainModel;
using Klipper.Tests.Leaves;
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
using static DomainModel.Leave;

namespace Klipper.Tests
{

    class AttendanceServiceForGivenDateRangeTest
    {
        private IAccessEventsRepository accessEventsData;
        private IEmployeeRepository employeeData;
        private IDepartmentRepository departmentData;
        private IAttendanceRegularizationRepository regularizationData;
        private ILeavesRepository leaveData;

        [SetUp]
        public void setup()
        {
            accessEventsData = Substitute.For<IAccessEventsRepository>();
            employeeData = Substitute.For<IEmployeeRepository>();
            departmentData = Substitute.For<IDepartmentRepository>();
            regularizationData = Substitute.For<IAttendanceRegularizationRepository>();
            leaveData= Substitute.For<ILeavesRepository>();
            var department = new Department(Departments.Software);
            departmentData.GetDepartment(Departments.Software).Returns(department);
            regularizationData.GetRegularizedRecords(48).Returns(new List<Regularization>());
        }

        [Test]
        public void GivenDateRangeAndEmployeeIdShouldDisplayCorrectNumberOfRecords()
        {
            //Setup
            AttendanceService attendanceService = new AttendanceService
                (accessEventsData, employeeData, departmentData, regularizationData, leaveData);
            var dummyAccessevents = new AccessEventsBuilder().BuildBetweenDate(DateTime.Parse("2018-10-01"), DateTime.Parse("2018-10-08"));
            accessEventsData.GetAccessEventsForDateRange(48, DateTime.Parse("2018-10-01"), DateTime.Parse("2018-10-08"))
                .Returns(dummyAccessevents);
            var dummyEmployee =
                new EmployeeBuilder()
                .WithUserName("Sidhdesh.Vadgaonkar")
                .WithPassword("26-12-1995")
                .BuildEmployee();
            employeeData.GetEmployee(48).Returns(dummyEmployee);

            var dummyLeaves = new List<Leave>();
            leaveData.GetAllLeavesInfo(63).Returns(dummyLeaves);

            // Execute usecase
            var accessEvents = attendanceService
                .AttendanceReportForDateRange(48, DateTime.Parse("2018-10-01"), DateTime.Parse("2018-10-08"));
                

            Assert.That(accessEvents.ListOfAttendanceRecordDTO.Count, Is.EqualTo(6));
        }

        [Test]
        public void RetrivedAccessEventsHasAccurateData()
        {
            //Setup
            AttendanceService attendanceService = new AttendanceService
                (accessEventsData, employeeData, departmentData, regularizationData, leaveData);

            var dummyAccessevents = new AccessEventsBuilder()
                .BuildBetweenDate(DateTime.Parse("2018/10/09"), DateTime.Parse("2018/10/09"));
            accessEventsData.GetAccessEventsForDateRange(48, DateTime.Parse("2018/10/09"), DateTime.Parse("2018/10/09"))
                .Returns(dummyAccessevents);

            var dummyEmployee =
                new EmployeeBuilder()
                .WithUserName("Sidhdesh.Vadgaonkar")
                .WithPassword("26-12-1995")
                .WithID(48)
                .BuildEmployee();
            employeeData.GetEmployee(48).Returns(dummyEmployee);

            var expectedData = new PerDayAttendanceRecordDTO()
            {
                Date = DateTime.Parse("2018/10/09"),
                TimeIn = new Time(2, 10),
                TimeOut = new Time(13, 13),
                OverTime = new Time(2, 3),
                LateBy = new Time(0, 0),
                WorkingHours = new Time(11, 3)
            };

            var dummyLeaves = new List<Leave>();
            leaveData.GetAllLeavesInfo(63).Returns(dummyLeaves);

            // Execute usecase
            var accessEvents = attendanceService
                .AttendanceReportForDateRange(48, DateTime.Parse("2018/10/09"), DateTime.Parse("2018/10/09"));
                
            var actualData = accessEvents.ListOfAttendanceRecordDTO.Where(x => x.Date == DateTime.Parse("2018/10/09")).Single();

            // Assert
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
        public void CalculateTotalDeficitTimeForGivenDateRange()
        {
            // Setup
            AttendanceService attendanceService = new AttendanceService
                (accessEventsData, employeeData, departmentData, regularizationData, leaveData);
            var dummyAccessevents = new AccessEventsBuilder().BuildBetweenDate(DateTime.Parse("2018-10-05"), DateTime.Parse("2018-10-05"));
            accessEventsData
                .GetAccessEventsForDateRange(48, DateTime.Parse("2018-10-05"), DateTime.Parse("2018-10-05"))
                .Returns(dummyAccessevents);

            var dummyEmployee =
                new EmployeeBuilder()
                .WithUserName("Sidhdesh.Vadgaonkar")
                .WithPassword("26-12-1995")
                .BuildEmployee();
            employeeData.GetEmployee(48).Returns(dummyEmployee);

            var dummyLeaves = new List<Leave>();
            leaveData.GetAllLeavesInfo(63).Returns(dummyLeaves);

            // Execute usecase
            var actualData = attendanceService
                .AttendanceReportForDateRange(48, DateTime.Parse("2018-10-05"), DateTime.Parse("2018-10-05"))
                .TotalDeficitOrExtraHours;

            Assert.That(actualData.Hour, Is.EqualTo(0));
            Assert.That(actualData.Minute, Is.EqualTo(-25));
        }

        [Test]
        public void CalculateTotalExtraTimeForGivenDateRange()
        {
            // Setup
            AttendanceService attendanceService = new AttendanceService
                (accessEventsData, employeeData, departmentData, regularizationData, leaveData);

            var dummyAccessevents = new AccessEventsBuilder().BuildBetweenDate(DateTime.Parse("2018-10-09"), DateTime.Parse("2018-10-09"));
            accessEventsData
                .GetAccessEventsForDateRange(48, DateTime.Parse("2018-10-09"), DateTime.Parse("2018-10-09"))
                .Returns(dummyAccessevents);

            var dummyEmployee =
                new EmployeeBuilder()
                .WithUserName("Sidhdesh.Vadgaonkar")
                .WithPassword("26-12-1995")
                .BuildEmployee();
            employeeData.GetEmployee(48).Returns(dummyEmployee);

            var dummyLeaves = new List<Leave>();
            leaveData.GetAllLeavesInfo(63).Returns(dummyLeaves);

            // Execute usecase
            var actualData = attendanceService
                .AttendanceReportForDateRange(48, DateTime.Parse("2018-10-09"), DateTime.Parse("2018-10-09"))
                .TotalDeficitOrExtraHours;

            Assert.That(actualData.Hour, Is.EqualTo(2));
            Assert.That(actualData.Minute, Is.EqualTo(3));
        }

        [Test]
        public void  GivenOddNOoFGymEntryAccessEventsSetWorkingHoursZero()
        {
            AttendanceService attendanceService =
                new AttendanceService(accessEventsData, employeeData, departmentData, regularizationData, leaveData);

            var dummyAccessevents = new AccessEventsBuilder().BuildBetweenDate(DateTime.Parse("2018-10-30"), DateTime.Parse("2018-10-30"));
            accessEventsData.GetAccessEventsForDateRange(48, DateTime.Parse("2018-10-30"), DateTime.Parse("2018-10-30")).Returns(dummyAccessevents);

            var dummyEmployee =
                new EmployeeBuilder()
                .WithUserName("Sidhdesh.Vadgaonkar")
                .WithPassword("26-12-1995")
                .BuildEmployee();
            employeeData.GetEmployee(48).Returns(dummyEmployee);

            var dummyLeaves = new List<Leave>() ;
            leaveData.GetAllLeavesInfo(63).Returns(dummyLeaves);

            var listOfAttendanceRecordForSpecifiedDays = attendanceService.
                AttendanceReportForDateRange(48, DateTime.Parse("2018-10-30"), DateTime.Parse("2018-10-30"));

            Assert.That(
                listOfAttendanceRecordForSpecifiedDays.ListOfAttendanceRecordDTO[0].WorkingHours.Hour,
                Is.EqualTo(0));
            Assert.That(
               listOfAttendanceRecordForSpecifiedDays.ListOfAttendanceRecordDTO[0].WorkingHours.Minute,
               Is.EqualTo(0));
        }

        [Test]
        public void GivenOddNOoFMainEntryAccessEventsSetWorkingHoursZero()
        {
            AttendanceService attendanceService =
                new AttendanceService(accessEventsData, employeeData, departmentData, regularizationData, leaveData);

            var dummyAccessevents = new AccessEventsBuilder().BuildBetweenDate(DateTime.Parse("2018-10-01"), DateTime.Parse("2018-10-01"));
            accessEventsData.GetAccessEventsForDateRange(48, DateTime.Parse("2018-10-01"), DateTime.Parse("2018-10-01")).Returns(dummyAccessevents);

            var dummyEmployee =
                new EmployeeBuilder()
                .WithUserName("Sidhdesh.Vadgaonkar")
                .WithPassword("26-12-1995")
                .BuildEmployee();
            employeeData.GetEmployee(48).Returns(dummyEmployee);

            var dummyLeaves = new List<Leave>();
            leaveData.GetAllLeavesInfo(63).Returns(dummyLeaves);

            var listOfAttendanceRecordForSpecifiedDays =attendanceService.
                AttendanceReportForDateRange(48, DateTime.Parse("2018-10-01"), DateTime.Parse("2018-10-01"));


            Assert.That(
                listOfAttendanceRecordForSpecifiedDays.ListOfAttendanceRecordDTO[0].WorkingHours.Hour,
                Is.EqualTo(0));
            Assert.That(
               listOfAttendanceRecordForSpecifiedDays.ListOfAttendanceRecordDTO[0].WorkingHours.Minute,
               Is.EqualTo(0));
        }
    }
}
