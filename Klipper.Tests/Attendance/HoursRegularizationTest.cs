using DomainModel;
using Klipper.Tests.Leaves;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tests;
using UseCaseBoundary;
using UseCases;
using static DomainModel.Leave;

namespace Klipper.Tests
{
    public class HoursRegularizationTest
    {
        private IAccessEventsRepository accessEventsData;
        private IEmployeeRepository employeeData;
        private IDepartmentRepository departmentData;
        private ILeavesRepository leaveData;
        private IAttendanceRegularizationRepository regularizationData;


        [SetUp]
        public void setup()
        {
            accessEventsData = Substitute.For<IAccessEventsRepository>();
            employeeData = Substitute.For<IEmployeeRepository>();
            departmentData = Substitute.For<IDepartmentRepository>();
            leaveData = Substitute.For<ILeavesRepository>();
            regularizationData = Substitute.For<IAttendanceRegularizationRepository>();
            var department = new Department(Departments.Software);
            departmentData.GetDepartment(Departments.Software).Returns(department);

        }

        [Test]
        public void ShowRegularizedHoursAsTotalWorkingHoursForADay()
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

            var regularizationsData = new List<Regularization>() {
                new Regularization(48,DateTime.Parse("2018-10-05"),TimeSpan.Parse("08:05:00"),"remark added")
            };
            regularizationData.GetRegularizedRecords(48).Returns(regularizationsData);

            // Execute usecase
            var actualData = attendanceService
                .AttendanceReportForDateRange(48, DateTime.Parse("2018-10-05"), DateTime.Parse("2018-10-05"))
                .TotalWorkingHours;

            Assert.That(actualData.Hour, Is.EqualTo(08));
            Assert.That(actualData.Minute, Is.EqualTo(05));
        }

        [Test]
        public void ShowActualWorkingHoursOfRegularizedRecord()
        {
            // Setup
            AttendanceService attendanceService = new AttendanceService
                (accessEventsData, employeeData, departmentData, regularizationData, leaveData);

            var dummyAccessevents = new AccessEventsBuilder().BuildBetweenDate(DateTime.Parse("2018/10/09"), DateTime.Parse("2018/10/09"));
            accessEventsData
                .GetAccessEventsForDateRange(48, DateTime.Parse("2018/10/09"), DateTime.Parse("2018/10/09"))
                .Returns(dummyAccessevents);

            var dummyEmployee =
                new EmployeeBuilder()
                .WithUserName("Sidhdesh.Vadgaonkar")
                .WithPassword("26-12-1995")
                .BuildEmployee();
            employeeData.GetEmployee(48).Returns(dummyEmployee);

            var dummyLeaves = new List<Leave>();
            leaveData.GetAllLeavesInfo(63).Returns(dummyLeaves);

            var regularizationsData = new List<Regularization>()
            {
                new Regularization(48,DateTime.Parse("2018/10/09"),TimeSpan.Parse("08:05:00"),"test remark")
            };
            regularizationData.GetRegularizedRecords(48).Returns(regularizationsData);

            // Execute usecase
            var accessEvents = attendanceService
                .AttendanceReportForDateRange(48, DateTime.Parse("2018/10/09"), DateTime.Parse("2018/10/09"));
               
            var actualData = accessEvents.ListOfAttendanceRecordDTO.Where(x => x.Date == DateTime.Parse("2018/10/09")).Single();

            // Assert
            Assert.That(actualData.WorkingHours.Hour, Is.EqualTo(11));
            Assert.That(actualData.WorkingHours.Minute, Is.EqualTo(3));
        }

        [Test]
        public void ShowRegularizedWorkingHoursOfRegularizedRecord()
        {
            // Setup
            AttendanceService attendanceService =
                new AttendanceService(accessEventsData, employeeData, departmentData, regularizationData, leaveData);
            var dummyAccessevents = new AccessEventsBuilder().BuildBetweenDate(DateTime.Parse("2018/10/09"), DateTime.Parse("2018/10/09"));
            accessEventsData
                .GetAccessEventsForDateRange(48, DateTime.Parse("2018/10/09"), DateTime.Parse("2018/10/09"))
                .Returns(dummyAccessevents);

            var dummyEmployee =
                new EmployeeBuilder()
                .WithUserName("Sidhdesh.Vadgaonkar")
                .WithPassword("26-12-1995")
                .BuildEmployee();
            employeeData.GetEmployee(48).Returns(dummyEmployee);

            var dummyLeaves = new List<Leave>();
            leaveData.GetAllLeavesInfo(63).Returns(dummyLeaves);

            var regularizationsData = new List<Regularization>()
            {
                new Regularization(48,DateTime.Parse("2018/10/09"),TimeSpan.Parse("08:05:00"),"test remark")
            };
            regularizationData.GetRegularizedRecords(48).Returns(regularizationsData);

            // Execute usecase
            var accessEvents = attendanceService
                .AttendanceReportForDateRange(48, DateTime.Parse("2018/10/09"), DateTime.Parse("2018/10/09"));
               
            var actualData = accessEvents.ListOfAttendanceRecordDTO.Where(x => x.Date == DateTime.Parse("2018/10/09")).Single();

            // Assert
            Assert.That(actualData.RegularizedHours.Hour, Is.EqualTo(8));
            Assert.That(actualData.RegularizedHours.Minute, Is.EqualTo(5));
        }

        [Test]
        public void ShowRemarkOfRegularizedRecord()
        {
            // Setup
            AttendanceService attendanceService = new AttendanceService
                (accessEventsData, employeeData, departmentData, regularizationData, leaveData);

            var dummyAccessevents = new AccessEventsBuilder()
                .BuildBetweenDate(DateTime.Parse("2018-10-05"), DateTime.Parse("2018-10-05"));
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

            var regularizationsData = new List<Regularization>() {
                new Regularization(48,DateTime.Parse("2018-10-05"),TimeSpan.Parse("08:05:00"),"test remark")
            };
            regularizationData.GetRegularizedRecords(48).Returns(regularizationsData);

            // Execute usecase
            var actualData = attendanceService
                .AttendanceReportForDateRange(48, DateTime.Parse("2018-10-05"), DateTime.Parse("2018-10-05"))
                .ListOfAttendanceRecordDTO
                .First();

            Assert.That(actualData.Remark, Is.EqualTo("test remark"));
        }
    }
}
