using DomainModel;
using Klipper.Tests.Leaves;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tests;
using UseCaseBoundary;
using UseCaseBoundary.DTO;
using UseCases;
using static DomainModel.Leave;

namespace Klipper.Tests
{
    public class WorkingHoursTest
    {
        private IAccessEventsRepository accessEventsContainer;
        private IEmployeeRepository employeeData;
        private IDepartmentRepository departmentData;
        private ILeavesRepository leaveData;
        private IAttendanceRegularizationRepository regularizationData;
        private ICarryForwardLeaves carryForwardLeavesData;

        [SetUp]
        public void Setup()
        {
            accessEventsContainer = Substitute.For<IAccessEventsRepository>();
            employeeData = Substitute.For<IEmployeeRepository>();
            departmentData = Substitute.For<IDepartmentRepository>();
            leaveData = Substitute.For<ILeavesRepository>();
            regularizationData = Substitute.For<IAttendanceRegularizationRepository>();
            regularizationData.GetRegularizedRecords(48).Returns(new List<Regularization>());
            carryForwardLeavesData = Substitute.For<ICarryForwardLeaves>();
        }

        [Test]
        public void GivenNineHoursWorkedDeptartmentGetAccurateOverTimeAndDeficitHours()
        {
            // Setup
            var dummyEmployee =
               new EmployeeBuilder()
               .WithUserName("Sidhdesh.Vadgaonkar")
               .WithPassword("26-12-1995")
               .WithID(48)
               .BuildEmployee();
            employeeData.GetEmployee(48).Returns(dummyEmployee);

            var dummyLeaves = new List<Leave>();
            leaveData.GetAllLeavesInfo(63).Returns(dummyLeaves);

            var department = new Department(Departments.Software);
            departmentData.GetDepartment(Departments.Software).Returns(department);
        

        AttendanceService attendanceService =
                new AttendanceService(accessEventsContainer, employeeData, departmentData, regularizationData, leaveData);
            var dummyAccessevents = new AccessEventsBuilder().BuildBetweenDate(DateTime.Parse("2018-10-05"), DateTime.Parse("2018-10-05"));
            accessEventsContainer.GetAccessEventsForDateRange(48, DateTime.Parse("2018-10-05"), DateTime.Parse("2018-10-05")).Returns(dummyAccessevents);

            // Execute usecase
            var listOfAccessEventsRecord = attendanceService.AttendanceReportForDateRange(48, DateTime.Parse("2018-10-05"), DateTime.Parse("2018-10-05"));

            Assert.That(listOfAccessEventsRecord.ListOfAttendanceRecordDTO[0].OverTime.Hour, Is.EqualTo(0));
            Assert.That(listOfAccessEventsRecord.ListOfAttendanceRecordDTO[0].OverTime.Minute, Is.EqualTo(0));

            Assert.That(listOfAccessEventsRecord.ListOfAttendanceRecordDTO[0].LateBy.Hour, Is.EqualTo(0));
            Assert.That(listOfAccessEventsRecord.ListOfAttendanceRecordDTO[0].LateBy.Minute, Is.EqualTo(25));
        }

        [Test]
        public void GivenTenHoursWorkedDeptartmentGetAccurateOverTimeAndDeficitHours()
        {
            // Setup
            var dummyEmployee =
               new EmployeeBuilder()
               .WithUserName("Sidhdesh.Vadgaonkar")
               .WithPassword("26-12-1995")
               .WithDepartment(Departments.Design)
               .WithID(48)
               .BuildEmployee();
            employeeData.GetEmployee(48).Returns(dummyEmployee);

            var department = new Department(Departments.Design);
            departmentData.GetDepartment(Departments.Design).Returns(department);

            var dummyLeaves = new List<Leave>();
            leaveData.GetAllLeavesInfo(63).Returns(dummyLeaves);

            AttendanceService attendanceService =
                    new AttendanceService(accessEventsContainer, employeeData, departmentData, regularizationData, leaveData);
            var dummyAccessevents = new AccessEventsBuilder().BuildBetweenDate(DateTime.Parse("2018-10-05"), DateTime.Parse("2018-10-05"));
            accessEventsContainer.GetAccessEventsForDateRange(48, DateTime.Parse("2018-10-05"), DateTime.Parse("2018-10-05")).Returns(dummyAccessevents);

            // Execute usecase
            var listOfAccessEventsRecord =attendanceService.AttendanceReportForDateRange(48, DateTime.Parse("2018-10-05"), DateTime.Parse("2018-10-05"));

            Assert.That(listOfAccessEventsRecord.ListOfAttendanceRecordDTO[0].OverTime.Hour, Is.EqualTo(0));
            Assert.That(listOfAccessEventsRecord.ListOfAttendanceRecordDTO[0].OverTime.Minute, Is.EqualTo(0));

            Assert.That(listOfAccessEventsRecord.ListOfAttendanceRecordDTO[0].LateBy.Hour, Is.EqualTo(1));
            Assert.That(listOfAccessEventsRecord.ListOfAttendanceRecordDTO[0].LateBy.Minute, Is.EqualTo(25));
        }

        [Test]
        public void OnApplyLeaveGetTotalWorkingHours10According10HourWorkingDepartment()
        {
            // Setup
            var dummyEmployee =
               new EmployeeBuilder()
               .WithUserName("Sidhdesh.Vadgaonkar")
               .WithPassword("26-12-1995")
               .WithDepartment(Departments.Design)
               .WithID(48)
               .BuildEmployee();
            employeeData.GetEmployee(48).Returns(dummyEmployee);

            var department = new Department(Departments.Design);
            departmentData.GetDepartment(Departments.Design).Returns(department);

            List<DateTime> listOfDate = new List<DateTime>() { DateTime.Parse("2018-03-05")};
            Leave leave = new DummyLeaveBuilder()
           .WithEmployeeId(48)
           .WithLeaveType(LeaveType.CompOff)
           .WithLeaveStatusType(StatusType.Approved)
           .WithLeaveDates(listOfDate)
           .Build();
            
            var  dummyLeaves = new List<Leave>() { leave };
            leaveData.GetAllLeavesInfo(48).Returns(dummyLeaves);

            var dummyAccessevents = new AccessEventsBuilder()
               .BuildBetweenDate(DateTime.Parse("2018-03-05"), DateTime.Parse("2018-03-05"));

            accessEventsContainer
                .GetAccessEventsForDateRange(48, DateTime.Parse("2018-03-05"), DateTime.Parse("2018-03-05"))
                .Returns(dummyAccessevents);

            AttendanceService attendanceService =
                   new AttendanceService(accessEventsContainer, employeeData, departmentData, regularizationData, leaveData);

            // Execute usecase
            var listOfAccessEventsRecord = attendanceService
                .AttendanceReportForDateRange(48, DateTime.Parse("2018-03-05"), DateTime.Parse("2018-03-05"));

            Assert.That(listOfAccessEventsRecord.TotalWorkingHours.Hour, Is.EqualTo(10));
            Assert.That(listOfAccessEventsRecord.TotalWorkingHours.Minute, Is.EqualTo(00));
        }

        [Test]
        public void OnApplyHalfDayLeaveGetTotalWorkingHours5According10HourWorkingDepartment()
        {
            // Setup
            var dummyEmployee =
               new EmployeeBuilder()
               .WithUserName("Sidhdesh.Vadgaonkar")
               .WithPassword("26-12-1995")
               .WithDepartment(Departments.Design)
               .WithID(48)
               .BuildEmployee();
            employeeData.GetEmployee(48).Returns(dummyEmployee);

            var department = new Department(Departments.Design);
            departmentData.GetDepartment(Departments.Design).Returns(department);

            List<DateTime> listOfDate = new List<DateTime>() { DateTime.Parse("2018-03-05") };
            Leave leave = new DummyLeaveBuilder()
           .WithEmployeeId(48)
           .WithLeaveType(LeaveType.CompOff)
           .WithLeaveStatusType(StatusType.Approved)
           .WithIsHalfDayLeave(true)
           .WithLeaveDates(listOfDate)
           .Build();

            var dummyLeaves = new List<Leave>() { leave };
            leaveData.GetAllLeavesInfo(48).Returns(dummyLeaves);


            var dummyAccessevents = new AccessEventsBuilder()
               .BuildBetweenDate(DateTime.Parse("2018-03-05"), DateTime.Parse("2018-03-05"));

            accessEventsContainer
                .GetAccessEventsForDateRange(48, DateTime.Parse("2018-03-05"), DateTime.Parse("2018-03-05"))
                .Returns(dummyAccessevents);

            AttendanceService attendanceService =
                   new AttendanceService(accessEventsContainer, employeeData, departmentData, regularizationData, leaveData);

            // Execute usecase
            var listOfAccessEventsRecord = attendanceService
                .AttendanceReportForDateRange(48, DateTime.Parse("2018-03-05"), DateTime.Parse("2018-03-05"));

            Assert.That(listOfAccessEventsRecord.TotalWorkingHours.Hour, Is.EqualTo(05));
            Assert.That(listOfAccessEventsRecord.TotalWorkingHours.Minute, Is.EqualTo(00));
        }
    }


}
