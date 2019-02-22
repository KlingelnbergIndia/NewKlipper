using DomainModel;
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

namespace Klipper.Tests
{
    public class ApplyleaveTest
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
        public void GivingAlreadyAppliedDateAndSickLeaveThenGetRecordExistsResponse()
        {
            LeaveService leaveService =
               new LeaveService(leaveRecordData, employeeData, departmentData, carryForwardLeavesData);
            
            List<Leave> listOfLeave = new List<Leave>();
            List<DateTime> listOfDate = new List<DateTime>() { DateTime.Parse("2019-02-22"), DateTime.Parse("2019-02-22") };
            listOfLeave.Add(new Leave(63, listOfDate, LeaveType.SickLeave, "one day sick leave apply", StatusType.Approved, "id1"));
            var dummyLeaveRecord = listOfLeave;
            leaveRecordData.GetAllLeavesInfo(63).Returns(dummyLeaveRecord);
            var dummyCarryForwardLeave = new CarryForwardLeaves(63, DateTime.Parse("2019-02-22"), 2,2,2,21,6,0);
            carryForwardLeavesData.GetCarryForwardLeaveAsync(63).Returns(dummyCarryForwardLeave);
            var dummyEmployee =
              new EmployeeBuilder()
              .WithUserName("Sidhdesh.Vadgaonkar")
              .WithPassword("26-12-1995")
              .BuildEmployee();
            employeeData.GetEmployee(63).Returns(dummyEmployee);
            var department = new Department(Departments.Software);
            departmentData.GetDepartment(Departments.Software).Returns(department);

            var respone =leaveService.ApplyLeave(63, DateTime.Parse("2019-02-22"), DateTime.Parse("2019-02-22"), Leave.LeaveType.SickLeave, "one day sick leave apply");

            Assert.That(respone, Is.EqualTo(ServiceResponseDTO.RecordExists));
        }

        [Test]
        public void GivingAlreadyAppliedDateAndCasualLeaveThenGetRecordExistsResponse()
        {
            LeaveService leaveService =
               new LeaveService(leaveRecordData, employeeData, departmentData, carryForwardLeavesData);

            List<Leave> listOfLeave = new List<Leave>();
            List<DateTime> listOfDate = new List<DateTime>() { DateTime.Parse("2019-02-22"), DateTime.Parse("2019-02-22") };
            listOfLeave.Add(new Leave(63, listOfDate, LeaveType.SickLeave, "one day sick leave apply", StatusType.Approved, "id1"));
            var dummyLeaveRecord = listOfLeave;
            leaveRecordData.GetAllLeavesInfo(63).Returns(dummyLeaveRecord);
            var dummyCarryForwardLeave = new CarryForwardLeaves(63, DateTime.Parse("2019-02-22"), 2, 2, 2, 21, 6, 0);
            carryForwardLeavesData.GetCarryForwardLeaveAsync(63).Returns(dummyCarryForwardLeave);
            var dummyEmployee =
              new EmployeeBuilder()
              .WithUserName("Sidhdesh.Vadgaonkar")
              .WithPassword("26-12-1995")
              .BuildEmployee();
            employeeData.GetEmployee(63).Returns(dummyEmployee);
            var department = new Department(Departments.Software);
            departmentData.GetDepartment(Departments.Software).Returns(department);

            var respone = leaveService.ApplyLeave(63, DateTime.Parse("2019-02-22"), DateTime.Parse("2019-02-22"), Leave.LeaveType.CasualLeave, "one day sick leave apply");

            Assert.That(respone, Is.EqualTo(ServiceResponseDTO.RecordExists));
        }
        [Test]
        public void GivingAlreadyAppliedDateAndCompOffLeaveThenGetRecordExistsResponse()
        {
            LeaveService leaveService =
               new LeaveService(leaveRecordData, employeeData, departmentData, carryForwardLeavesData);

            List<Leave> listOfLeave = new List<Leave>();
            List<DateTime> listOfDate = new List<DateTime>() { DateTime.Parse("2019-02-22"), DateTime.Parse("2019-02-22") };
            listOfLeave.Add(new Leave(63, listOfDate, LeaveType.SickLeave, "one day sick leave apply", StatusType.Approved,null));
            var dummyLeaveRecord = listOfLeave;
            leaveRecordData.GetAllLeavesInfo(63).Returns(dummyLeaveRecord);
            var dummyCarryForwardLeave = new CarryForwardLeaves(63, DateTime.Parse("2019-02-22"), 2, 2, 2, 21, 6, 0);
            carryForwardLeavesData.GetCarryForwardLeaveAsync(63).Returns(dummyCarryForwardLeave);
            var dummyEmployee =
              new EmployeeBuilder()
              .WithUserName("Sidhdesh.Vadgaonkar")
              .WithPassword("26-12-1995")
              .BuildEmployee();
            employeeData.GetEmployee(63).Returns(dummyEmployee);
            var department = new Department(Departments.Software);
            departmentData.GetDepartment(Departments.Software).Returns(department);

            var respone = leaveService.ApplyLeave(63, DateTime.Parse("2019-02-22"), DateTime.Parse("2019-02-22"), Leave.LeaveType.CompOff, "one day sick leave apply");

            Assert.That(respone, Is.EqualTo(ServiceResponseDTO.RecordExists));
        }

        [Test]
        public void ApplySickLeaveOfNonWorkingDayThenGetInValidDaysResponse()
        {
            LeaveService leaveService =
               new LeaveService(leaveRecordData, employeeData, departmentData, carryForwardLeavesData);

            List<Leave> listOfLeave = new List<Leave>();
            List<DateTime> listOfDate = new List<DateTime>() { DateTime.Parse("2019-02-22"), DateTime.Parse("2019-02-22") };
            listOfLeave.Add(new Leave(63, listOfDate, LeaveType.SickLeave, "one day sick leave apply", StatusType.Approved, null));
            var dummyLeaveRecord = listOfLeave;
            leaveRecordData.GetAllLeavesInfo(63).Returns(dummyLeaveRecord);
            var dummyCarryForwardLeave = new CarryForwardLeaves(63, DateTime.Parse("2019-02-22"), 2, 6, 2, 21, 6, 0);
            carryForwardLeavesData.GetCarryForwardLeaveAsync(63).Returns(dummyCarryForwardLeave);
            var dummyEmployee =
              new EmployeeBuilder()
              .WithUserName("Sidhdesh.Vadgaonkar")
              .WithPassword("26-12-1995")
              .BuildEmployee();
            employeeData.GetEmployee(63).Returns(dummyEmployee);
            var department = new Department(Departments.Software);
            departmentData.GetDepartment(Departments.Software).Returns(department);

            var respone = leaveService.ApplyLeave(63, DateTime.Parse("2019-02-02"), DateTime.Parse("2019-02-02"), Leave.LeaveType.SickLeave, "one day sick leave apply");

            Assert.That(respone, Is.EqualTo(ServiceResponseDTO.InvalidDays));
        }

        [Test]
        public void ApplyCasualLeaveOfNonWorkingDayThenGetInValidDaysResponse()
        {
            LeaveService leaveService =
               new LeaveService(leaveRecordData, employeeData, departmentData, carryForwardLeavesData);

            List<Leave> listOfLeave = new List<Leave>();
            List<DateTime> listOfDate = new List<DateTime>() { DateTime.Parse("2019-02-22"), DateTime.Parse("2019-02-22") };
            listOfLeave.Add(new Leave(63, listOfDate, LeaveType.SickLeave, "one day sick leave apply", StatusType.Approved, null));
            var dummyLeaveRecord = listOfLeave;
            leaveRecordData.GetAllLeavesInfo(63).Returns(dummyLeaveRecord);
            var dummyCarryForwardLeave = new CarryForwardLeaves(63, DateTime.Parse("2019-02-22"), 2, 6, 2, 21, 6, 0);
            carryForwardLeavesData.GetCarryForwardLeaveAsync(63).Returns(dummyCarryForwardLeave);
            var dummyEmployee =
              new EmployeeBuilder()
              .WithUserName("Sidhdesh.Vadgaonkar")
              .WithPassword("26-12-1995")
              .BuildEmployee();
            employeeData.GetEmployee(63).Returns(dummyEmployee);
            var department = new Department(Departments.Software);
            departmentData.GetDepartment(Departments.Software).Returns(department);

            var respone = leaveService.ApplyLeave(63, DateTime.Parse("2019-02-02"), DateTime.Parse("2019-02-02"), Leave.LeaveType.CasualLeave, "one day sick leave apply");

            Assert.That(respone, Is.EqualTo(ServiceResponseDTO.InvalidDays));
        }

        [Test]
        public void ApplyCompOffLeaveOfNonWorkingDayThenGetInValidDaysResponse()
        {
            LeaveService leaveService =
               new LeaveService(leaveRecordData, employeeData, departmentData, carryForwardLeavesData);

            List<Leave> listOfLeave = new List<Leave>();
            List<DateTime> listOfDate = new List<DateTime>() { DateTime.Parse("2019-02-22"), DateTime.Parse("2019-02-22") };
            listOfLeave.Add(new Leave(63, listOfDate, LeaveType.SickLeave, "one day sick leave apply", StatusType.Approved, null));
            var dummyLeaveRecord = listOfLeave;
            leaveRecordData.GetAllLeavesInfo(63).Returns(dummyLeaveRecord);
            var dummyCarryForwardLeave = new CarryForwardLeaves(63, DateTime.Parse("2019-02-22"), 2, 6, 2, 21, 6, 0);
            carryForwardLeavesData.GetCarryForwardLeaveAsync(63).Returns(dummyCarryForwardLeave);
            var dummyEmployee =
              new EmployeeBuilder()
              .WithUserName("Sidhdesh.Vadgaonkar")
              .WithPassword("26-12-1995")
              .BuildEmployee();
            employeeData.GetEmployee(63).Returns(dummyEmployee);
            var department = new Department(Departments.Software);
            departmentData.GetDepartment(Departments.Software).Returns(department);

            var respone = leaveService.ApplyLeave(63, DateTime.Parse("2019-02-02"), DateTime.Parse("2019-02-02"), Leave.LeaveType.CompOff, "one day sick leave apply");

            Assert.That(respone, Is.EqualTo(ServiceResponseDTO.InvalidDays));
        }

        [Test]
        public void ApplyNewSickLeaveThenGetSavedResponse()
        {
            LeaveService leaveService =
               new LeaveService(leaveRecordData, employeeData, departmentData, carryForwardLeavesData);

            List<Leave> listOfLeave = new List<Leave>();
            List<DateTime> listOfDate = new List<DateTime>() { DateTime.Parse("2019-02-22"), DateTime.Parse("2019-02-22") };
            listOfLeave.Add(new Leave(63, listOfDate, LeaveType.SickLeave, "one day sick leave apply", StatusType.Approved, null));
            var dummyLeaveRecord = listOfLeave;
            leaveRecordData.GetAllLeavesInfo(63).Returns(dummyLeaveRecord);
            var dummyCarryForwardLeave = new CarryForwardLeaves(63, DateTime.Parse("2019-02-22"), 2, 2, 2, 21, 6, 0);
            carryForwardLeavesData.GetCarryForwardLeaveAsync(63).Returns(dummyCarryForwardLeave);
            var dummyEmployee =
              new EmployeeBuilder()
              .WithUserName("Sidhdesh.Vadgaonkar")
              .WithPassword("26-12-1995")
              .BuildEmployee();
            employeeData.GetEmployee(63).Returns(dummyEmployee);
            var department = new Department(Departments.Software);
            departmentData.GetDepartment(Departments.Software).Returns(department);

            var respone = leaveService.ApplyLeave(63, DateTime.Parse("2019-02-05"), DateTime.Parse("2019-02-05"), Leave.LeaveType.SickLeave, "one day sick leave apply");

            Assert.That(respone, Is.EqualTo(ServiceResponseDTO.Saved));
        }

        [Test]
        public void ApplyNewCasualLeaveThenGetSavedResponse()
        {
            LeaveService leaveService =
               new LeaveService(leaveRecordData, employeeData, departmentData, carryForwardLeavesData);

            List<Leave> listOfLeave = new List<Leave>();
            List<DateTime> listOfDate = new List<DateTime>() { DateTime.Parse("2019-02-22"), DateTime.Parse("2019-02-22") };
            listOfLeave.Add(new Leave(63, listOfDate, LeaveType.SickLeave, "one day sick leave apply", StatusType.Approved, "id1"));
            var dummyLeaveRecord = listOfLeave;
            leaveRecordData.GetAllLeavesInfo(63).Returns(dummyLeaveRecord);
            var dummyCarryForwardLeave = new CarryForwardLeaves(63, DateTime.Parse("2019-02-22"), 2, 2, 2, 21, 6, 0);
            carryForwardLeavesData.GetCarryForwardLeaveAsync(63).Returns(dummyCarryForwardLeave);
            var dummyEmployee =
              new EmployeeBuilder()
              .WithUserName("Sidhdesh.Vadgaonkar")
              .WithPassword("26-12-1995")
              .BuildEmployee();
            employeeData.GetEmployee(63).Returns(dummyEmployee);
            var department = new Department(Departments.Software);
            departmentData.GetDepartment(Departments.Software).Returns(department);

            var respone = leaveService.ApplyLeave(63, DateTime.Parse("2019-02-05"), DateTime.Parse("2019-02-05"), Leave.LeaveType.CasualLeave, "one day sick leave apply");

            Assert.That(respone, Is.EqualTo(ServiceResponseDTO.Saved));
        }

        [Test]
        public void ApplyNewCompOffLeaveThenGetSavedResponse()
        {
            LeaveService leaveService =
               new LeaveService(leaveRecordData, employeeData, departmentData, carryForwardLeavesData);

            List<Leave> listOfLeave = new List<Leave>();
            List<DateTime> listOfDate = new List<DateTime>() { DateTime.Parse("2019-02-22"), DateTime.Parse("2019-02-22") };
            listOfLeave.Add(new Leave(63, listOfDate, LeaveType.SickLeave, "one day sick leave apply", StatusType.Approved, "id1"));
            var dummyLeaveRecord = listOfLeave;
            leaveRecordData.GetAllLeavesInfo(63).Returns(dummyLeaveRecord);
            var dummyCarryForwardLeave = new CarryForwardLeaves(63, DateTime.Parse("2019-02-22"), 2, 2, 2, 21, 6, 3);
            carryForwardLeavesData.GetCarryForwardLeaveAsync(63).Returns(dummyCarryForwardLeave);
            var dummyEmployee =
              new EmployeeBuilder()
              .WithUserName("Sidhdesh.Vadgaonkar")
              .WithPassword("26-12-1995")
              .BuildEmployee();
            employeeData.GetEmployee(63).Returns(dummyEmployee);
            var department = new Department(Departments.Software);
            departmentData.GetDepartment(Departments.Software).Returns(department);

            var respone = leaveService.ApplyLeave(63, DateTime.Parse("2019-02-05"), DateTime.Parse("2019-02-05"), Leave.LeaveType.CompOff, "one day sick leave apply");

            Assert.That(respone, Is.EqualTo(ServiceResponseDTO.Saved));
        }

        [Test]
        public void ApplySickLeaveAndSickLeaveIsExhaustedThenGetCanNotAppliedResponse()
        {
            LeaveService leaveService =
               new LeaveService(leaveRecordData, employeeData, departmentData, carryForwardLeavesData);

            List<Leave> listOfLeave = new List<Leave>();
            List<DateTime> listOfDate = new List<DateTime>() { DateTime.Parse("2019-02-22"), DateTime.Parse("2019-02-22") };
            listOfLeave.Add(new Leave(63, listOfDate, LeaveType.SickLeave, "one day sick leave apply", StatusType.Approved, "id1"));
            var dummyLeaveRecord = listOfLeave;
            leaveRecordData.GetAllLeavesInfo(63).Returns(dummyLeaveRecord);

            var dummyCarryForwardLeave = new CarryForwardLeaves(63, DateTime.Parse("2019-02-22"), 2, 6, 2, 21, 6, 3);
            carryForwardLeavesData.GetCarryForwardLeaveAsync(63).Returns(dummyCarryForwardLeave);
            var dummyEmployee =
              new EmployeeBuilder()
              .WithUserName("Sidhdesh.Vadgaonkar")
              .WithPassword("26-12-1995")
              .BuildEmployee();
            employeeData.GetEmployee(63).Returns(dummyEmployee);
            var department = new Department(Departments.Software);
            departmentData.GetDepartment(Departments.Software).Returns(department);

            var respone = leaveService.ApplyLeave(63, DateTime.Parse("2019-02-05"), DateTime.Parse("2019-02-05"), Leave.LeaveType.SickLeave, "one day sick leave apply");

            Assert.That(respone, Is.EqualTo(ServiceResponseDTO.CanNotApplied));
        }

        [Test]
        public void ApplyCasualLeaveAndCasualLeaveIsExhaustedThenGetSavedResponse()
        {
            LeaveService leaveService =
               new LeaveService(leaveRecordData, employeeData, departmentData, carryForwardLeavesData);

            List<Leave> listOfLeave = new List<Leave>();
            List<DateTime> listOfDate = new List<DateTime>() { DateTime.Parse("2019-02-22"), DateTime.Parse("2019-02-22") };
            listOfLeave.Add(new Leave(63, listOfDate, LeaveType.SickLeave, "one day sick leave apply", StatusType.Approved, "id1"));
            var dummyLeaveRecord = listOfLeave;
            leaveRecordData.GetAllLeavesInfo(63).Returns(dummyLeaveRecord);
            var dummyCarryForwardLeave = new CarryForwardLeaves(63, DateTime.Parse("2019-02-22"), 21, 2, 2, 21, 6, 3);
            carryForwardLeavesData.GetCarryForwardLeaveAsync(63).Returns(dummyCarryForwardLeave);
            var dummyEmployee =
              new EmployeeBuilder()
              .WithUserName("Sidhdesh.Vadgaonkar")
              .WithPassword("26-12-1995")
              .BuildEmployee();
            employeeData.GetEmployee(63).Returns(dummyEmployee);
            var department = new Department(Departments.Software);
            departmentData.GetDepartment(Departments.Software).Returns(department);

            var respone = leaveService.ApplyLeave(63, DateTime.Parse("2019-02-05"), DateTime.Parse("2019-02-05"), Leave.LeaveType.CasualLeave, "one day sick leave apply");

            Assert.That(respone, Is.EqualTo(ServiceResponseDTO.Saved));
        }

        [Test]
        public void ApplyCompOffLeaveAndCompOffLeaveIsExhaustedThenGetCanNotAppliedResponse()
        {
            LeaveService leaveService =
               new LeaveService(leaveRecordData, employeeData, departmentData, carryForwardLeavesData);

            List<Leave> listOfLeave = new List<Leave>();
            List<DateTime> listOfDate = new List<DateTime>() { DateTime.Parse("2019-02-22"), DateTime.Parse("2019-02-22") };
            listOfLeave.Add(new Leave(63, listOfDate, LeaveType.SickLeave, "one day sick leave apply", StatusType.Approved, "id1"));
            var dummyLeaveRecord = listOfLeave;
            leaveRecordData.GetAllLeavesInfo(63).Returns(dummyLeaveRecord);
            var dummyCarryForwardLeave = new CarryForwardLeaves(63, DateTime.Parse("2019-02-22"), 21, 2, 2, 21, 6, 2);
            carryForwardLeavesData.GetCarryForwardLeaveAsync(63).Returns(dummyCarryForwardLeave);
            var dummyEmployee =
              new EmployeeBuilder()
              .WithUserName("Sidhdesh.Vadgaonkar")
              .WithPassword("26-12-1995")
              .BuildEmployee();
            employeeData.GetEmployee(63).Returns(dummyEmployee);
            var department = new Department(Departments.Software);
            departmentData.GetDepartment(Departments.Software).Returns(department);

            var respone = leaveService.ApplyLeave(63, DateTime.Parse("2019-02-05"), DateTime.Parse("2019-02-05"), Leave.LeaveType.CompOff, "one day sick leave apply");

            Assert.That(respone, Is.EqualTo(ServiceResponseDTO.CanNotApplied));
        }
    }
}
