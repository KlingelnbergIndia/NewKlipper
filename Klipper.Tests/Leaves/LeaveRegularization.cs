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

            var department = new Department(Departments.Software);
            departmentData.GetDepartment(Departments.Software).Returns(department);
        }

        [Test]
        public void IdentifyThatGivenDateIsNotRealisedLeaveDate()
        {
            //SETUP
            Leave leave = new DummyLeaveBuilder()
                .WithEmployeeId(63)
                .WithLeaveType(LeaveType.CasualLeave)
                .WithLeaveStatusType(StatusType.Approved)
                .WithLeaveDates(new List<DateTime>() { DateTime.Now.AddDays(1) })
                .Build();
            leaveRecordData.GetAllLeavesInfo(63).Returns(new List<Leave>() { leave });

            //CALL USECASE
            LeaveService leaveService = new LeaveService(leaveRecordData, employeeData, departmentData, carryForwardLeavesData);
            var resultData = leaveService.GetAppliedLeaves(63);

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
                .WithLeaveDates(new List<DateTime>() { DateTime.Now.AddDays(1) })
                .Build();
            leaveRecordData.GetAllLeavesInfo(63).Returns(new List<Leave>() { leave });

            var dummyCarryForwardLeave = new CarryForwardLeaves(63, DateTime.Parse("2019-02-22"), 2, 6, 2, 21, 6, 0);
            carryForwardLeavesData.GetCarryForwardLeaveAsync(63).Returns(dummyCarryForwardLeave);

            //CALL USECASE
            LeaveService leaveService = new LeaveService(leaveRecordData, employeeData, departmentData, carryForwardLeavesData);
            leaveService.ApplyLeave(63, DateTime.Parse("2019-01-01"), DateTime.Parse("2019-01-01"), LeaveType.CasualLeave, "");
            var summaryData = leaveService.GetTotalSummary(63);

            Assert.That(summaryData.TotalCasualLeaveTaken, Is.EqualTo(3));
        }

    }
}
