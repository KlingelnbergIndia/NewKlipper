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
        public void IdentifyThatGivenDateIsRealisedLeaveDate()
        {
            var dummyLeave = new List<Leave>() { new Leave(63, appliedLeaveDates, LeaveType.CasualLeave, "remark", StatusType.Approved, "") };

            Leave leave = new DummyLeaveBuilder()
                .WithEmployeeId(63)
                .WithLeaveType(LeaveType.CasualLeave)
                .WithLeaveStatusType(StatusType.Approved)
                .WithLeaveDates(appliedLeaveDates)
                .Build();
            leaveRecordData.GetAllLeavesInfo(63).Returns(new List<Leave>() { leave });

            LeaveService leaveService = new LeaveService(leaveRecordData, employeeData, departmentData, carryForwardLeavesData);
            var resultData = leaveService.GetAppliedLeaves(63);

            Assert.That(resultData[0].IsRealizedLeave, Is.EqualTo(true));
        }

        private List<LeaveRecordDTO> DummyLeaveData()
        {
            List<LeaveRecordDTO> dummyLeaveRecords = new List<LeaveRecordDTO>();
            dummyLeaveRecords.Add(new LeaveRecordDTO() { Date = appliedLeaveDates });
            return dummyLeaveRecords;
        }
    }
}
