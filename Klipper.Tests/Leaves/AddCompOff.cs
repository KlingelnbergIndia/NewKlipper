using DomainModel;
using Klipper.Tests.Leaves;
using NSubstitute;
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
    public class AddCompOff
    {
        private ILeavesRepository leaveRecordData;
        private IEmployeeRepository employeeData;
        private IDepartmentRepository departmentData;
        private ICarryForwardLeaves carryForwardLeavesData;

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
        public void ApplyNewCompOffOnNonWorkingDayThenGetSavedResponse()
        {
            // Setup
            LeaveService leaveService =
                new LeaveService(leaveRecordData, employeeData, departmentData, carryForwardLeavesData);

            List<Leave> listOfLeave = new List<Leave>();
            List<DateTime> listOfDate = new List<DateTime>() { DateTime.Parse("2019-02-22"), DateTime.Parse("2019-02-22") };
            listOfLeave.Add(new Leave(63, listOfDate, LeaveType.CompOff, false, "one day sick leave apply", StatusType.Approved, "id1"));

            var dummyLeaveRecord = listOfLeave;
            leaveRecordData.GetAllLeavesInfo(63).Returns(dummyLeaveRecord);

            var dummyEmployee =
                new EmployeeBuilder()
                    .WithUserName("Sidhdesh.Vadgaonkar")
                    .WithPassword("26-12-1995")
                    .BuildEmployee();
            employeeData.GetEmployee(63).Returns(dummyEmployee);

            //CALL USECASE
            var respone = leaveService.ApplyCompOff(63, DateTime.Parse("2019-03-24"),
                DateTime.Parse("2019-03-24"), "one day sick leave apply");

            Assert.That(respone, Is.EqualTo(ServiceResponseDTO.Saved));
        }

        [Test]
        public void OnAddCompOffSummaryShouldGetUpdated()
        {
            //SETUP
            var dummyEmployee =
                new EmployeeBuilder()
                    .WithID(63)
                    .BuildEmployee();
            employeeData.GetEmployee(63).Returns(dummyEmployee);
             
            Leave leave = new DummyLeaveBuilder()
                .WithEmployeeId(63)
                .WithLeaveType(LeaveType.CompOff)
                .WithLeaveStatusType(StatusType.CompOffAdded)
                .WithLeaveDates(new List<DateTime>() { DateTime.Now.AddDays(1) })
                .Build();
            leaveRecordData.GetAllLeavesInfo(63).Returns(new List<Leave>() { leave });

            var dummyCarryForwardLeave = new CarryForwardLeaves(63, DateTime.Parse("2019-02-22"), 2, 6, 2, 21, 6, 0);
            carryForwardLeavesData.GetCarryForwardLeaveAsync(63).Returns(dummyCarryForwardLeave);

            //CALL USECASE
            LeaveService leaveService =
                new LeaveService(leaveRecordData, employeeData, departmentData, carryForwardLeavesData);
            leaveService.ApplyCompOff(63, DateTime.Parse("2019-01-01"),
                DateTime.Parse("2019-01-01"), " ");
            var summaryData = leaveService.TotalSummary(63);

            Assert.That(summaryData.MaximumCompOffLeave, Is.EqualTo(1));
        }

        [Test]
        public void OnUpdateCompOffGetResponseUpdated()
        {
            //SETUP
            var dummyEmployee =
                new EmployeeBuilder()
                    .WithID(63)
                    .BuildEmployee();
            employeeData.GetEmployee(63).Returns(dummyEmployee);

            Leave leave = new DummyLeaveBuilder()
                .WithEmployeeId(63)
                .WithLeaveType(LeaveType.CompOff)
                .WithLeaveStatusType(StatusType.CompOffUpdated)
                .WithLeaveDates(new List<DateTime>() { DateTime.Now.AddDays(1) })
                .Build();
            leaveRecordData.GetAllLeavesInfo(63).Returns(new List<Leave>() { leave });

            leaveRecordData.GetLeaveByLeaveId("sadjkf").Returns(leave);

            //CALL USECASE
            LeaveService leaveService =
                new LeaveService(leaveRecordData, employeeData, departmentData, carryForwardLeavesData);
            var response = leaveService.UpdateAddedCompOff("sadjkf",63, DateTime.Parse("2019-01-01"),
                DateTime.Parse("2019-01-01"), " ");
          
            Assert.That(response, Is.EqualTo(ServiceResponseDTO.Updated));
        }

        [Test]
        public void OnCancelAddedCompOffGetResponseCancel()
        {
            //SETUP
            var dummyEmployee =
                new EmployeeBuilder()
                    .WithID(63)
                    .BuildEmployee();
            employeeData.GetEmployee(63).Returns(dummyEmployee);

            Leave leave = new DummyLeaveBuilder()
                .WithEmployeeId(63)
                .WithLeaveType(LeaveType.CompOff)
                .WithLeaveStatusType(StatusType.CompOffUpdated)
                .WithLeaveDates(new List<DateTime>() { DateTime.Now.AddDays(1) })
                .Build();
            leaveRecordData.GetAllLeavesInfo(63).Returns(new List<Leave>() { leave });

            leaveRecordData.GetLeaveByLeaveId("sadjkf").Returns(leave);
            leaveRecordData.CancelCompOff("sadjkf").Returns(true);

            //CALL USECASE
            LeaveService leaveService =
                new LeaveService(leaveRecordData, employeeData, departmentData, carryForwardLeavesData);
            var response = leaveService.CancelCompOff("sadjkf", 63);

            Assert.That(response, Is.EqualTo(ServiceResponseDTO.Deleted));
        }

    }
}
