using DomainModel;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tests;
using UseCaseBoundary;
using UseCases;

namespace Klipper.Tests
{
    public class WorkingHoursTest
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
        }

        [Test]
        public async Task GivenNineHoursWorkedDeptartmentGetAccurateOverTimeAndDeficitHours()
        {
            var dummyEmployee =
               new EmployeeBuilder()
               .WithUserName("Sidhdesh.Vadgaonkar")
               .WithPassword("26-12-1995")
               .WithID(48)
               .BuildEmployee();
            employeeData.GetEmployee(48).Returns(dummyEmployee);

            var department = new Department(Departments.Software);
            departmentData.GetDepartment(Departments.Software).Returns(department);
        

        AttendanceService attendanceService =
                new AttendanceService(accessEventsContainer, employeeData, departmentData);
            var dummyAccessevents = new AccessEventsBuilder().BuildBetweenDate(DateTime.Parse("2018-10-05"), DateTime.Parse("2018-10-05"));
            accessEventsContainer.GetAccessEventsForDateRange(48, DateTime.Parse("2018-10-05"), DateTime.Parse("2018-10-05")).Returns(dummyAccessevents);

            var listOfAccessEventsRecord = await attendanceService.GetAccessEventsForDateRange(48, DateTime.Parse("2018-10-05"), DateTime.Parse("2018-10-05"));

            Assert.That(listOfAccessEventsRecord.ListOfAttendanceRecordDTO[0].OverTime.Hour, Is.EqualTo(0));
            Assert.That(listOfAccessEventsRecord.ListOfAttendanceRecordDTO[0].OverTime.Minute, Is.EqualTo(0));

            Assert.That(listOfAccessEventsRecord.ListOfAttendanceRecordDTO[0].LateBy.Hour, Is.EqualTo(0));
            Assert.That(listOfAccessEventsRecord.ListOfAttendanceRecordDTO[0].LateBy.Minute, Is.EqualTo(25));
        }

        [Test]
        public async Task GivenTenHoursWorkedDeptartmentGetAccurateOverTimeAndDeficitHours()
        {
            var dummyEmployee =
               new EmployeeBuilder()
               .WithUserName("Sidhdesh.Vadgaonkar")
               .WithPassword("26-12-1995")
               .WithID(48)
               .BuildEmployee();
            employeeData.GetEmployee(48).Returns(dummyEmployee);

            var department = new Department(Departments.Software);
            departmentData.GetDepartment(Departments.Software).Returns(department);

            AttendanceService attendanceService =
                    new AttendanceService(accessEventsContainer, employeeData, departmentData);
            var dummyAccessevents = new AccessEventsBuilder().BuildBetweenDate(DateTime.Parse("2018-10-05"), DateTime.Parse("2018-10-05"));
            accessEventsContainer.GetAccessEventsForDateRange(48, DateTime.Parse("2018-10-05"), DateTime.Parse("2018-10-05")).Returns(dummyAccessevents);

            var listOfAccessEventsRecord = await attendanceService.GetAccessEventsForDateRange(48, DateTime.Parse("2018-10-05"), DateTime.Parse("2018-10-05"));

            Assert.That(listOfAccessEventsRecord.ListOfAttendanceRecordDTO[0].OverTime.Hour, Is.EqualTo(0));
            Assert.That(listOfAccessEventsRecord.ListOfAttendanceRecordDTO[0].OverTime.Minute, Is.EqualTo(0));

            Assert.That(listOfAccessEventsRecord.ListOfAttendanceRecordDTO[0].LateBy.Hour, Is.EqualTo(1));
            Assert.That(listOfAccessEventsRecord.ListOfAttendanceRecordDTO[0].LateBy.Minute, Is.EqualTo(25));
        }

    }


}
