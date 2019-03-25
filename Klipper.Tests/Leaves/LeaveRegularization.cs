using DomainModel;
using NSubstitute;
using NSubstitute.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Tests;
using UseCaseBoundary;
using UseCaseBoundary.DTO;
using UseCases;
using static DomainModel.Leave;

namespace Klipper.Tests.Leaves
{
    public class LeaveRegularization
    {
        private ILeavesRepository leaveRecordData;
        private IEmployeeRepository employeeData;
        private IDepartmentRepository departmentData;
        private ICarryForwardLeaves carryForwardLeavesData;
        private IAccessEventsRepository accessEventsData;
        private IAttendanceRegularizationRepository regularizationData;

        List<DateTime> appliedLeaveDates = new List<DateTime>()
        {
            DateTime.Parse("2019-01-01"),
            DateTime.Parse("2019-01-02"),
            DateTime.Parse("2019-01-03")
        };

        [SetUp]
        public void setup()
        {
            leaveRecordData = Substitute.For<ILeavesRepository>();
            employeeData = Substitute.For<IEmployeeRepository>();
            departmentData = Substitute.For<IDepartmentRepository>();
            carryForwardLeavesData = Substitute.For<ICarryForwardLeaves>();
            accessEventsData = Substitute.For<IAccessEventsRepository>();
            regularizationData = Substitute.For<IAttendanceRegularizationRepository>();

            var department = new Department(Departments.Software);
            departmentData.GetDepartment(Departments.Software).Returns(department);

            var dummyEmployee =
                new EmployeeBuilder()
                    .WithUserName("Sidhdesh.Vadgaonkar")
                    .WithPassword("26-12-1995")
                    .WithDepartment(Departments.Software)
                    .BuildEmployee();
            employeeData.GetEmployee(63).Returns(dummyEmployee);
        }

        [Test]
        public void IdentifyThatGivenDateIsNotRealisedLeaveDate()
        {
            //SETUP
            Leave leave = new DummyLeaveBuilder()
                .WithEmployeeId(63)
                .WithLeaveType(LeaveType.CasualLeave)
                .WithLeaveStatusType(StatusType.Approved)
                .WithLeaveDates(new List<DateTime>() {DateTime.Now.AddDays(1)})
                .Build();
            leaveRecordData.GetAllLeavesInfo(63).Returns(new List<Leave>() {leave});

            //CALL USECASE
            LeaveService leaveService =
                new LeaveService(leaveRecordData, employeeData, departmentData, carryForwardLeavesData);
            var resultData = leaveService.AppliedLeaves(63);

            Assert.That(resultData[0].IsRealizedLeave, Is.EqualTo(false));
        }

        [Test]
        public void OnApplyingLeaveSummaryShouldGetUpdated()
        {
            //SETUP
            var dummyEmployee =
                new EmployeeBuilder()
                    .WithID(63)
                    .BuildEmployee();
            employeeData.GetEmployee(63).Returns(dummyEmployee);

            Leave leave = new DummyLeaveBuilder()
                .WithEmployeeId(63)
                .WithLeaveType(LeaveType.CasualLeave)
                .WithLeaveStatusType(StatusType.Approved)
                .WithLeaveDates(new List<DateTime>() {DateTime.Now.AddDays(1)})
                .Build();
            leaveRecordData.GetAllLeavesInfo(63).Returns(new List<Leave>() {leave});

            var dummyCarryForwardLeave = new CarryForwardLeaves(63, DateTime.Parse("2019-02-22"), 2, 6, 2, 21, 6, 0);
            carryForwardLeavesData.GetCarryForwardLeaveAsync(63).Returns(dummyCarryForwardLeave);

            //CALL USECASE
            LeaveService leaveService =
                new LeaveService(leaveRecordData, employeeData, departmentData, carryForwardLeavesData);
            leaveService.ApplyLeave(63, DateTime.Parse("2019-01-01"),
                DateTime.Parse("2019-01-01"), LeaveType.CasualLeave, false, "");
            var summaryData = leaveService.TotalSummary(63);

            Assert.That(summaryData.TotalCasualLeaveTaken, Is.EqualTo(3));
        }

        [Test]
        public void IdentifyThatGivenDateIsRealisedLeaveDate()
        {
            //SETUP
            Leave leave = new DummyLeaveBuilder()
                .WithLeaveId("bco0123ed")
                .WithEmployeeId(63)
                .WithLeaveType(LeaveType.CasualLeave)
                .WithLeaveStatusType(StatusType.Approved)
                .WithLeaveDates(appliedLeaveDates)
                .Build();
            leaveRecordData.GetAllLeavesInfo(63).Returns(new List<Leave>() {leave});
            leaveRecordData.GetLeaveByLeaveId("bco0123ed").Returns(leave);

            //CALL USECASE
            LeaveService leaveService =
                new LeaveService(leaveRecordData, employeeData, departmentData, carryForwardLeavesData);
            var resultData = leaveService.AppliedLeaves(63);

            Assert.That(resultData[0].IsRealizedLeave, Is.EqualTo(true));
        }

        [Test]
        public void OnApplyLeaveGetAttendanceRecordRegularized()
        {
            //SETUP
            Leave leave = new DummyLeaveBuilder()
                .WithEmployeeId(63)
                .WithLeaveType(LeaveType.CasualLeave)
                .WithLeaveStatusType(StatusType.Approved)
                .WithLeaveDates(new List<DateTime>() {DateTime.Parse("2019-02-22")})
                .Build();
            leaveRecordData.GetAllLeavesInfo(63).Returns(new List<Leave>() {leave});

            var regularizationsData = new List<Regularization>()
            {
                new Regularization(63, DateTime.Parse("2018-10-05"), TimeSpan.Parse("08:05:00"), "remark added")
            };
            regularizationData.GetRegularizedRecords(63).Returns(regularizationsData);

            var dummyAccessevents =
                new AccessEventsBuilder()
                    .BuildBetweenDate(DateTime.Parse("2019-02-22"), DateTime.Parse("2019-02-22"));

            accessEventsData.GetAccessEventsForDateRange(63, DateTime.Parse("2019-02-22"),
                    DateTime.Parse("2019-02-22"))
                .Returns(dummyAccessevents);

            //CALL USECASE
            var leaveService = new LeaveService(leaveRecordData, employeeData,
                departmentData, carryForwardLeavesData);

            var attendanceService = new AttendanceService(accessEventsData, employeeData,
                departmentData, regularizationData, leaveRecordData);


            var attendanceRecord = attendanceService.AttendanceReportForDateRange(63, DateTime.Parse("2019-02-22"),
                DateTime.Parse("2019-02-22"));

            Assert.That(attendanceRecord.ListOfAttendanceRecordDTO[0].RegularizedHours.Hour, Is.EqualTo(9));
        }

        [Test]
        public void OnAddCompOffGetAttendanceRecordNotRegularized()
        {
            //SETUP
            Leave leave = new DummyLeaveBuilder()
                .WithEmployeeId(63)
                .WithLeaveType(LeaveType.CasualLeave)
                .WithLeaveStatusType(StatusType.Approved)
                .WithLeaveDates(new List<DateTime>() {DateTime.Parse("2019-02-22")})
                .Build();
            leaveRecordData.GetAllLeavesInfo(63).Returns(new List<Leave>() {leave});

            var regularizationsData = new List<Regularization>()
            {
                new Regularization(63, DateTime.Parse("2018-10-05"), TimeSpan.Parse("08:05:00"), "remark added")
            };
            regularizationData.GetRegularizedRecords(63).Returns(regularizationsData);

            var dummyAccessevents =
                new AccessEventsBuilder()
                    .BuildBetweenDate(DateTime.Parse("2019-02-22"), DateTime.Parse("2019-02-22"));

            accessEventsData.GetAccessEventsForDateRange(63, DateTime.Parse("2019-02-22"),
                    DateTime.Parse("2019-02-22"))
                .Returns(dummyAccessevents);

            //CALL USECASE
            var leaveService = new LeaveService(leaveRecordData, employeeData,
                departmentData, carryForwardLeavesData);

            var attendanceService = new AttendanceService(accessEventsData, employeeData,
                departmentData, regularizationData, leaveRecordData);

            var attendanceRecord = attendanceService.AttendanceReportForDateRange(63, DateTime.Parse("2019-02-22"),
                DateTime.Parse("2019-02-22"));

            Assert.That(attendanceRecord.ListOfAttendanceRecordDTO[0].RegularizedHours.Hour, Is.EqualTo(9));
        }
    }
}
