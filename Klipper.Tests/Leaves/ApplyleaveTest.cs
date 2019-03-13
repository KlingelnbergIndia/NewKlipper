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
            // Setup
            LeaveService leaveService =
               new LeaveService(leaveRecordData, employeeData, departmentData, carryForwardLeavesData);
            
            List<Leave> listOfLeave = new List<Leave>();
            List<DateTime> listOfDate = new List<DateTime>() { DateTime.Parse("2019-02-22"), DateTime.Parse("2019-02-22") };
            listOfLeave.Add(new Leave(63, listOfDate, LeaveType.SickLeave, false, "one day sick leave apply", StatusType.Approved, "id1"));

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

            //CALL USECASE
            var respone =leaveService.ApplyLeave(63, DateTime.Parse("2019-02-22"),
                DateTime.Parse("2019-02-22"), Leave.LeaveType.SickLeave, false, "one day sick leave apply");

            Assert.That(respone, Is.EqualTo(ServiceResponseDTO.RecordExists));
        }

        [Test]
        public void GivingAlreadyAppliedDateAndCasualLeaveThenGetRecordExistsResponse()
        {
            // Setup
            LeaveService leaveService =
               new LeaveService(leaveRecordData, employeeData, departmentData, carryForwardLeavesData);

            List<Leave> listOfLeave = new List<Leave>();
            List<DateTime> listOfDate = new List<DateTime>() { DateTime.Parse("2019-02-22"), DateTime.Parse("2019-02-22") };
            listOfLeave.Add(new Leave(63, listOfDate, LeaveType.SickLeave, false, "one day sick leave apply", StatusType.Approved, "id1"));

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

            //CALL USECASE
            var respone = leaveService.ApplyLeave(63, DateTime.Parse("2019-02-22"), 
                DateTime.Parse("2019-02-22"), Leave.LeaveType.CasualLeave, false, "one day sick leave apply");

            Assert.That(respone, Is.EqualTo(ServiceResponseDTO.RecordExists));
        }
        [Test]
        public void GivingAlreadyAppliedDateAndCompOffLeaveThenGetRecordExistsResponse()
        {
            // Setup
            LeaveService leaveService =
               new LeaveService(leaveRecordData, employeeData, departmentData, carryForwardLeavesData);

            List<Leave> listOfLeave = new List<Leave>();
            List<DateTime> listOfDate = new List<DateTime>() { DateTime.Parse("2019-02-22"), DateTime.Parse("2019-02-22") };
            listOfLeave.Add(new Leave(63, listOfDate, LeaveType.SickLeave, false, "one day sick leave apply", StatusType.Approved,null));

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

            //CALL USECASE
            var respone = leaveService.ApplyLeave(63, DateTime.Parse("2019-02-22"),
                DateTime.Parse("2019-02-22"), Leave.LeaveType.CompOff, false, "one day sick leave apply");

            Assert.That(respone, Is.EqualTo(ServiceResponseDTO.RecordExists));
        }

        [Test]
        public void ApplySickLeaveOfNonWorkingDayThenGetInValidDaysResponse()
        {
            // Setup
            LeaveService leaveService =
               new LeaveService(leaveRecordData, employeeData, departmentData, carryForwardLeavesData);

            List<Leave> listOfLeave = new List<Leave>();
            List<DateTime> listOfDate = new List<DateTime>() { DateTime.Parse("2019-02-22"), DateTime.Parse("2019-02-22") };
            listOfLeave.Add(new Leave(63, listOfDate, LeaveType.SickLeave, 
                false, "one day sick leave apply", StatusType.Approved, null));

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

            //CALL USECASE
            var respone = leaveService.ApplyLeave(63, DateTime.Parse("2019-02-02"),
                DateTime.Parse("2019-02-02"), Leave.LeaveType.SickLeave, false, "one day sick leave apply");

            Assert.That(respone, Is.EqualTo(ServiceResponseDTO.InvalidDays));
        }

        [Test]
        public void ApplyCasualLeaveOfNonWorkingDayThenGetInValidDaysResponse()
        {
            // Setup
            LeaveService leaveService =
               new LeaveService(leaveRecordData, employeeData, departmentData, carryForwardLeavesData);

            List<Leave> listOfLeave = new List<Leave>();
            List<DateTime> listOfDate = new List<DateTime>() { DateTime.Parse("2019-02-22"), DateTime.Parse("2019-02-22") };
            listOfLeave.Add(new Leave(63, listOfDate, LeaveType.SickLeave, false, "one day sick leave apply", StatusType.Approved, null));

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

            //CALL USECASE
            var respone = leaveService.ApplyLeave(63, DateTime.Parse("2019-02-02"), 
                DateTime.Parse("2019-02-02"), Leave.LeaveType.CasualLeave, false, "one day sick leave apply");

            Assert.That(respone, Is.EqualTo(ServiceResponseDTO.InvalidDays));
        }

        [Test]
        public void ApplyCompOffLeaveOfNonWorkingDayThenGetInValidDaysResponse()
        {
            // Setup
            LeaveService leaveService =
               new LeaveService(leaveRecordData, employeeData, departmentData, carryForwardLeavesData);

            List<Leave> listOfLeave = new List<Leave>();
            List<DateTime> listOfDate = new List<DateTime>()
            { DateTime.Parse("2019-02-22"), DateTime.Parse("2019-02-22") };
            listOfLeave.Add(new Leave(63, listOfDate, LeaveType.SickLeave,
                false, "one day sick leave apply", StatusType.Approved, null));

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

            //CALL USECASE
            var respone = leaveService.ApplyLeave(63, DateTime.Parse("2019-02-02"),
                DateTime.Parse("2019-02-02"), Leave.LeaveType.CompOff,false, "one day sick leave apply");

            Assert.That(respone, Is.EqualTo(ServiceResponseDTO.InvalidDays));
        }

        [Test]
        public void ApplyNewSickLeaveThenGetSavedResponse()
        {
            // Setup
            LeaveService leaveService =
               new LeaveService(leaveRecordData, employeeData, departmentData, carryForwardLeavesData);

            List<Leave> listOfLeave = new List<Leave>();
            List<DateTime> listOfDate = new List<DateTime>() { DateTime.Parse("2019-02-22"), DateTime.Parse("2019-02-22") };
            listOfLeave.Add(new Leave(63, listOfDate, LeaveType.SickLeave,
                false, "one day sick leave apply", StatusType.Approved, null));

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

            //CALL USECASE
            var respone = leaveService.ApplyLeave(63, DateTime.Parse("2019-02-05"),
                DateTime.Parse("2019-02-05"), Leave.LeaveType.SickLeave,false, "one day sick leave apply");

            Assert.That(respone, Is.EqualTo(ServiceResponseDTO.Saved));
        }

        [Test]
        public void ApplyNewCasualLeaveThenGetSavedResponse()
        {
            // Setup
            LeaveService leaveService =
               new LeaveService(leaveRecordData, employeeData, departmentData, carryForwardLeavesData);

            List<Leave> listOfLeave = new List<Leave>();
            List<DateTime> listOfDate = new List<DateTime>() { DateTime.Parse("2019-02-22"), DateTime.Parse("2019-02-22") };
            listOfLeave.Add(new Leave(63, listOfDate, LeaveType.SickLeave, false, "one day sick leave apply", StatusType.Approved, "id1"));

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

            //CALL USECASE
            var respone = leaveService.ApplyLeave(63, DateTime.Parse("2019-02-05"), 
                DateTime.Parse("2019-02-05"), Leave.LeaveType.CasualLeave,false, "one day sick leave apply");

            Assert.That(respone, Is.EqualTo(ServiceResponseDTO.Saved));
        }

        [Test]
        public void ApplyNewCompOffLeaveThenGetSavedResponse()
        {
            // Setup
            LeaveService leaveService =
               new LeaveService(leaveRecordData, employeeData, departmentData, carryForwardLeavesData);

            List<Leave> listOfLeave = new List<Leave>();
            List<DateTime> listOfDate = new List<DateTime>() { DateTime.Parse("2019-02-22"), DateTime.Parse("2019-02-22") };
            listOfLeave.Add(new Leave(63, listOfDate, LeaveType.SickLeave, false, "one day sick leave apply", StatusType.Approved, "id1"));

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

            //CALL USECASE
            var respone = leaveService.ApplyLeave(63, DateTime.Parse("2019-02-05"), 
                DateTime.Parse("2019-02-05"), Leave.LeaveType.CompOff,false, "one day sick leave apply");

            Assert.That(respone, Is.EqualTo(ServiceResponseDTO.Saved));
        }

        [Test]
        public void ApplySickLeaveAndSickLeaveIsExhaustedThenGetCanNotAppliedResponse()
        {
            // Setup
            LeaveService leaveService =
               new LeaveService(leaveRecordData, employeeData, departmentData, carryForwardLeavesData);

            List<Leave> listOfLeave = new List<Leave>();
            List<DateTime> listOfDate = new List<DateTime>() { DateTime.Parse("2019-02-22"), DateTime.Parse("2019-02-22") };
            listOfLeave.Add(new Leave(63, listOfDate, LeaveType.SickLeave,false, "one day sick leave apply", StatusType.Approved, "id1"));

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

            //CALL USECASE
            var respone = leaveService.ApplyLeave(63, DateTime.Parse("2019-02-05"), DateTime.Parse("2019-02-05"), Leave.LeaveType.SickLeave,false, "one day sick leave apply");

            Assert.That(respone, Is.EqualTo(ServiceResponseDTO.CanNotApplied));
        }

        [Test]
        public void ApplyCasualLeaveAndCasualLeaveIsExhaustedThenGetSavedResponse()
        {
            // Setup
            LeaveService leaveService =
               new LeaveService(leaveRecordData, employeeData, departmentData, carryForwardLeavesData);

            List<Leave> listOfLeave = new List<Leave>();
            List<DateTime> listOfDate = new List<DateTime>() { DateTime.Parse("2019-02-22"), DateTime.Parse("2019-02-22") };
            listOfLeave.Add(new Leave(63, listOfDate, LeaveType.SickLeave, false, "one day sick leave apply", StatusType.Approved, "id1"));

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

            //CALL USECASE
            var respone = leaveService.ApplyLeave(63, DateTime.Parse("2019-02-05"),
                DateTime.Parse("2019-02-05"), Leave.LeaveType.CasualLeave, false, "one day sick leave apply");

            Assert.That(respone, Is.EqualTo(ServiceResponseDTO.Saved));
        }

        [Test]
        public void ApplyCompOffLeaveAndCompOffLeaveIsExhaustedThenGetCanNotAppliedResponse()
        {
            // Setup
            LeaveService leaveService =
               new LeaveService(leaveRecordData, employeeData, departmentData, carryForwardLeavesData);

            List<Leave> listOfLeave = new List<Leave>();
            List<DateTime> listOfDate = new List<DateTime>() { DateTime.Parse("2019-02-22"), DateTime.Parse("2019-02-22") };
            listOfLeave.Add(new Leave(63, listOfDate, LeaveType.SickLeave, false, "one day sick leave apply", StatusType.Approved, "id1"));

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

            //CALL USECASE
            var response = leaveService.ApplyLeave(63, DateTime.Parse("2019-02-05"),
                DateTime.Parse("2019-02-05"), Leave.LeaveType.CompOff, false, "one day sick leave apply");

            Assert.That(response, Is.EqualTo(ServiceResponseDTO.CanNotApplied));
        }

        [Test]
        public void OnApplyCompOffLeaveThenRemainingCompOffLeaveAndAvailableCompOffLeaveIsUpdated()
        {
            // Setup
            LeaveService leaveService =
               new LeaveService(leaveRecordData, employeeData, departmentData, carryForwardLeavesData);

            List<Leave> listOfLeave = new List<Leave>();
            List<DateTime> listOfDate = new List<DateTime>() { DateTime.Parse("2019-02-22"), DateTime.Parse("2019-02-23") };

            var dummyLeaveRecord = listOfLeave;
            leaveRecordData.GetAllLeavesInfo(63).Returns(dummyLeaveRecord);

            var dummyCarryForwardLeave = new CarryForwardLeaves(63, DateTime.Parse("2019-02-22"), 21, 2, 0, 21, 6, 5);
            carryForwardLeavesData.GetCarryForwardLeaveAsync(63).Returns(dummyCarryForwardLeave);

            var dummyEmployee =
              new EmployeeBuilder()
              .WithUserName("Sidhdesh.Vadgaonkar")
              .WithPassword("26-12-1995")
              .BuildEmployee();
            employeeData.GetEmployee(63).Returns(dummyEmployee);

            var department = new Department(Departments.Software);
            departmentData.GetDepartment(Departments.Software).Returns(department);

            //CALL USECASE
            var response = leaveService.ApplyLeave(63, DateTime.Parse("2019-02-22"),
                DateTime.Parse("2019-02-23"), Leave.LeaveType.CompOff, false, "one day sick leave apply");
            if(response == ServiceResponseDTO.Saved)
            {

                listOfDate = new List<DateTime>() { DateTime.Parse("2019-02-22"), DateTime.Parse("2019-02-23") };
                listOfLeave.Add(new Leave(63, listOfDate, LeaveType.CompOff, false, "one day sick leave apply", StatusType.Approved, "id1"));

                dummyLeaveRecord = listOfLeave;
            }

            var summaryReport = leaveService.TotalSummary(63);

            Assert.That(summaryReport.RemainingCompOffLeave, Is.EqualTo(3));
            Assert.That(summaryReport.TotalCompOffLeaveTaken, Is.EqualTo(2));
        }

        [Test]
        public void OnApplyCasualLeaveLeaveThenRemainingCasualLeaveAndAvailableCasualLeaveIsUpdated()
        {
            // Setup
            LeaveService leaveService =
               new LeaveService(leaveRecordData, employeeData, departmentData, carryForwardLeavesData);

            List<Leave> listOfLeave = new List<Leave>();
            List<DateTime> listOfDate = new List<DateTime>() { DateTime.Parse("2019-02-22"), DateTime.Parse("2019-02-23") };

            var dummyLeaveRecord = listOfLeave;
            leaveRecordData.GetAllLeavesInfo(63).Returns(dummyLeaveRecord);

            var dummyCarryForwardLeave = new CarryForwardLeaves(63, DateTime.Parse("2019-02-22"), 1, 2, 0, 21, 6, 5);
            carryForwardLeavesData.GetCarryForwardLeaveAsync(63).Returns(dummyCarryForwardLeave);

            var dummyEmployee =
              new EmployeeBuilder()
              .WithUserName("Sidhdesh.Vadgaonkar")
              .WithPassword("26-12-1995")
              .BuildEmployee();
            employeeData.GetEmployee(63).Returns(dummyEmployee);

            var department = new Department(Departments.Software);
            departmentData.GetDepartment(Departments.Software).Returns(department);

            //CALL USECASE
            var response = leaveService.ApplyLeave(63, DateTime.Parse("2019-02-22"),
                DateTime.Parse("2019-02-23"), Leave.LeaveType.CasualLeave, false, "one day sick leave apply");
            if (response == ServiceResponseDTO.Saved)
            {

                listOfDate = new List<DateTime>() { DateTime.Parse("2019-02-22"), DateTime.Parse("2019-02-23") };
                listOfLeave.Add(new Leave(63, listOfDate, LeaveType.CasualLeave, 
                    false, "one day sick leave apply", StatusType.Approved, "id1"));

                dummyLeaveRecord = listOfLeave;
            }

            var summaryReport = leaveService.TotalSummary(63);

            Assert.That(summaryReport.RemainingCasualLeave, Is.EqualTo(18));
            Assert.That(summaryReport.TotalCasualLeaveTaken, Is.EqualTo(3));
        }

        [Test]
        public void OnApplySickLeaveThenRemainingSickLeaveAndAvailableSickLeaveIsUpdated()
        {
            // Setup
            LeaveService leaveService =
               new LeaveService(leaveRecordData, employeeData, departmentData, carryForwardLeavesData);

            List<Leave> listOfLeave = new List<Leave>();
            List<DateTime> listOfDate = new List<DateTime>() { DateTime.Parse("2019-02-22"), DateTime.Parse("2019-02-22") };

            var dummyLeaveRecord = listOfLeave;
            leaveRecordData.GetAllLeavesInfo(63).Returns(dummyLeaveRecord);

            var dummyCarryForwardLeave = new CarryForwardLeaves(63, DateTime.Parse("2019-02-22"), 1, 2, 0, 21, 6, 5);
            carryForwardLeavesData.GetCarryForwardLeaveAsync(63).Returns(dummyCarryForwardLeave);

            var dummyEmployee =
              new EmployeeBuilder()
              .WithUserName("Sidhdesh.Vadgaonkar")
              .WithPassword("26-12-1995")
              .BuildEmployee();
            employeeData.GetEmployee(63).Returns(dummyEmployee);

            var department = new Department(Departments.Software);
            departmentData.GetDepartment(Departments.Software).Returns(department);

            //CALL USECASE
            var response = leaveService.ApplyLeave(63, DateTime.Parse("2019-02-22"),
                DateTime.Parse("2019-02-23"), Leave.LeaveType.SickLeave, false, "one day sick leave apply");
            if (response == ServiceResponseDTO.Saved)
            {

                listOfDate = new List<DateTime>() { DateTime.Parse("2019-02-22"), DateTime.Parse("2019-02-23") };
                listOfLeave.Add(new Leave(63, listOfDate, LeaveType.SickLeave,
                    false, "one day sick leave apply", StatusType.Approved, "id1"));

                dummyLeaveRecord = listOfLeave;
            }

            var summaryReport = leaveService.TotalSummary(63);

            Assert.That(summaryReport.RemainingSickLeave, Is.EqualTo(2));
            Assert.That(summaryReport.TotalSickLeaveTaken, Is.EqualTo(4));
        }


        [Test]
        public void OnApplyHalfDaySickLeaveThenRemainingSickLeaveAndAvailableSickLeaveIsUpdated()
        {
            // Setup
            LeaveService leaveService =
               new LeaveService(leaveRecordData, employeeData, departmentData, carryForwardLeavesData);

            List<DateTime> listOfDate = new List<DateTime>() { DateTime.Parse("2019-02-22")};
            var listOfLeave= new List<Leave>();
            var dummyLeaveRecord = listOfLeave;
            leaveRecordData.GetAllLeavesInfo(63).Returns(dummyLeaveRecord);

            var dummyCarryForwardLeave = new CarryForwardLeaves(63, DateTime.Parse("2019-02-22"), 2, 2, 0, 21, 6, 5);
            carryForwardLeavesData.GetCarryForwardLeaveAsync(63).Returns(dummyCarryForwardLeave);

            var dummyEmployee =
              new EmployeeBuilder()
              .WithUserName("Sidhdesh.Vadgaonkar")
              .WithPassword("26-12-1995")
              .BuildEmployee();
            employeeData.GetEmployee(63).Returns(dummyEmployee);

            var department = new Department(Departments.Software);
            departmentData.GetDepartment(Departments.Software).Returns(department);

            //CALL USECASE
            var response = leaveService.ApplyLeave(63, DateTime.Parse("2019-02-22"),
                DateTime.Parse("2019-02-22"), Leave.LeaveType.SickLeave,true, "one day sick leave apply");
            if (response == ServiceResponseDTO.Saved)
            {
                Leave leave = new DummyLeaveBuilder()
               .WithLeaveId("bco0123ed")
               .WithEmployeeId(63)
               .WithLeaveType(LeaveType.SickLeave)
               .WithLeaveStatusType(StatusType.Approved)
               .WithLeaveDates(listOfDate)
               .WithIsHalfDayLeave(true)
               .Build();
                listOfLeave.Add(leave);
                dummyLeaveRecord = listOfLeave;
            }
            var summaryReport = leaveService.TotalSummary(63);

            Assert.That(summaryReport.RemainingSickLeave, Is.EqualTo(3.5));
            Assert.That(summaryReport.TotalSickLeaveTaken, Is.EqualTo(2.5));
        }

        [Test]
        public void OnApplyHalfDayCasualLeaveThenRemainingSickLeaveAndAvailableSickLeaveIsUpdated()
        {
            //SETUP
            LeaveService leaveService =
               new LeaveService(leaveRecordData, employeeData, departmentData, carryForwardLeavesData);

            List<DateTime> listOfDate = new List<DateTime>() { DateTime.Parse("2019-02-22") };
            var listOfLeave = new List<Leave>();
            var dummyLeaveRecord = listOfLeave;
            leaveRecordData.GetAllLeavesInfo(63).Returns(dummyLeaveRecord);

            var dummyCarryForwardLeave = new CarryForwardLeaves(63, DateTime.Parse("2019-02-22"), 2, 2, 0, 21, 6, 5);
            carryForwardLeavesData.GetCarryForwardLeaveAsync(63).Returns(dummyCarryForwardLeave);

            var dummyEmployee =
              new EmployeeBuilder()
              .WithUserName("Sidhdesh.Vadgaonkar")
              .WithPassword("26-12-1995")
              .BuildEmployee();
            employeeData.GetEmployee(63).Returns(dummyEmployee);

            var department = new Department(Departments.Software);
            departmentData.GetDepartment(Departments.Software).Returns(department);

            //CALL USECASE
            var response = leaveService.ApplyLeave(63, DateTime.Parse("2019-02-22"),
                DateTime.Parse("2019-02-22"), Leave.LeaveType.CasualLeave, true, "one day sick leave apply");
            if (response == ServiceResponseDTO.Saved)
            {
                Leave leave = new DummyLeaveBuilder()
               .WithLeaveId("bco0123ed")
               .WithEmployeeId(63)
               .WithLeaveType(LeaveType.CasualLeave)
               .WithLeaveStatusType(StatusType.Approved)
               .WithLeaveDates(listOfDate)
               .WithIsHalfDayLeave(true)
               .Build();
                listOfLeave.Add(leave);
                dummyLeaveRecord = listOfLeave;
            }
            var summaryReport = leaveService.TotalSummary(63);

            Assert.That(summaryReport.RemainingCasualLeave, Is.EqualTo(18.5));
            Assert.That(summaryReport.TotalCasualLeaveTaken, Is.EqualTo(2.5));
        }

        [Test]
        public void OnApplyHalfDayCompOffLeaveThenRemainingSickLeaveAndAvailableSickLeaveIsUpdated()
        {
            //SETUP
            LeaveService leaveService =
               new LeaveService(leaveRecordData, employeeData, departmentData, carryForwardLeavesData);

            List<DateTime> listOfDate = new List<DateTime>() { DateTime.Parse("2019-02-22") };
            var listOfLeave = new List<Leave>();
            var dummyLeaveRecord = listOfLeave;
            leaveRecordData.GetAllLeavesInfo(63).Returns(dummyLeaveRecord);

            var dummyCarryForwardLeave = new CarryForwardLeaves(63, DateTime.Parse("2019-02-22"), 2, 2, 0, 21, 6, 5);
            carryForwardLeavesData.GetCarryForwardLeaveAsync(63).Returns(dummyCarryForwardLeave);

            var dummyEmployee =
              new EmployeeBuilder()
              .WithUserName("Sidhdesh.Vadgaonkar")
              .WithPassword("26-12-1995")
              .BuildEmployee();
            employeeData.GetEmployee(63).Returns(dummyEmployee);

            var department = new Department(Departments.Software);
            departmentData.GetDepartment(Departments.Software).Returns(department);

            //CALL USECASE
            var response = leaveService.ApplyLeave(63, DateTime.Parse("2019-02-22"),
                DateTime.Parse("2019-02-22"), Leave.LeaveType.CompOff, true, "one day sick leave apply");
            if (response == ServiceResponseDTO.Saved)
            {
                Leave leave = new DummyLeaveBuilder()
               .WithLeaveId("bco0123ed")
               .WithEmployeeId(63)
               .WithLeaveType(LeaveType.CompOff)
               .WithLeaveStatusType(StatusType.Approved)
               .WithLeaveDates(listOfDate)
               .WithIsHalfDayLeave(true)
               .Build();
                listOfLeave.Add(leave);
                dummyLeaveRecord = listOfLeave;
            }
            var summaryReport = leaveService.TotalSummary(63);

            Assert.That(summaryReport.RemainingCompOffLeave, Is.EqualTo(4.5));
            Assert.That(summaryReport.TotalCompOffLeaveTaken, Is.EqualTo(0.5));
        }

        [Test]
        public void OnApplyHalfDayForMultipleDayLeaveThenGetResponseInValidDay()
        {
            //SETUP
            LeaveService leaveService =
               new LeaveService(leaveRecordData, employeeData, departmentData, carryForwardLeavesData);

            List<DateTime> listOfDate = new List<DateTime>() {};
            
            var dummyLeaveRecord = new List<Leave>();
            leaveRecordData.GetAllLeavesInfo(63).Returns(dummyLeaveRecord);

            var dummyEmployee =
              new EmployeeBuilder()
              .WithUserName("Sidhdesh.Vadgaonkar")
              .WithPassword("26-12-1995")
              .BuildEmployee();
            employeeData.GetEmployee(63).Returns(dummyEmployee);

            var department = new Department(Departments.Software);
            departmentData.GetDepartment(Departments.Software).Returns(department);

            //CALL USECASE
            var response = leaveService.ApplyLeave(63, DateTime.Parse("2019-02-22"),
                DateTime.Parse("2019-02-23"), Leave.LeaveType.CompOff, true, "one day sick leave apply");
            
            Assert.That(response, Is.EqualTo(ServiceResponseDTO.InvalidDays));
        }
    }
}
