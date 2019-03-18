using System.Collections.Generic;
using System.Linq;
using DomainModel;
using NSubstitute;
using NUnit.Framework;
using Tests;
using UseCaseBoundary;
using UseCaseBoundary.DTO;
using UseCases;

namespace Klipper.Tests
{
    public class ReporteesServiceTests
    {
        private IEmployeeRepository employeeDataContainer;
        private List<EmployeeRoles> employeeRoles;

        [SetUp]
        public void Setup()
        {
            EmployeeBuilder employeeBuilder = new EmployeeBuilder();
            employeeDataContainer = Substitute.For<IEmployeeRepository>();
            employeeRoles = new List<EmployeeRoles>()
            {
                EmployeeRoles.Admin,
                EmployeeRoles.TeamLeader,
                EmployeeRoles.Employee
            };
        }

        private ReporteeDTO ConvertEmployeeToReporteeData(Employee employee)
        {
            ReporteeDTO reporteeData = new ReporteeDTO();
            reporteeData.ID = employee.Id();
            reporteeData.FirstName = employee.FirstName();
            reporteeData.LastName = employee.LastName();
            return reporteeData;
        }

        [Test]
        public void GivenEmployeeWithTeamLeadRoleGetReportees()
        {
            ReporteeService reporteeService = new ReporteeService(employeeDataContainer);
            List<int> dummyreportees = new List<int>() { 40, 46 };

            var teamLead = new EmployeeBuilder()
                .WithID(29)
                .WithUserName("Kiran.Kharade")
                .WithPassword("01-06-1975")
                .WithReportees(dummyreportees)
                .WithRole(EmployeeRoles.TeamLeader)
                .BuildEmployee();
            employeeDataContainer.GetEmployee(29).Returns(teamLead);

            var reportee40 = new EmployeeBuilder()
                .WithID(40)
                .WithUserName("Sagar.Shende")
                .WithPassword("0-03-1987")
                .WithRole(EmployeeRoles.Employee)
                .WithRole(EmployeeRoles.TeamLeader)
                .BuildEmployee();
            employeeDataContainer.GetEmployee(40).Returns(reportee40);

            var empRoles = employeeRoles.Where(employeeRoleItem => employeeRoleItem == EmployeeRoles.Employee).ToList();
            var reportee46 = new EmployeeBuilder()
                .WithID(46)
                .WithUserName("Krutika.Sawarkar")
                .WithPassword("21-09-1994")
                .WithRoles(empRoles)
                .BuildEmployee();
            employeeDataContainer.GetEmployee(46).Returns(reportee46);

            // Execute usecase
            var actualreporteesData = reporteeService.ReporteesData(29);
            var dummyreporteesData = new List<UseCaseBoundary.DTO.ReporteeDTO>();
            dummyreporteesData.Add(ConvertEmployeeToReporteeData(reportee40));
            dummyreporteesData.Add(ConvertEmployeeToReporteeData(reportee46));

            Assert.AreEqual(dummyreporteesData, actualreporteesData);
        }

        [Test]
        public void GivenEmployeeAsEmployeeRoleGetEmptyListOfReportees()
        {
            ReporteeService reporteeService = new ReporteeService(employeeDataContainer);
            var employee = new EmployeeBuilder()
                .WithID(29)
                .WithUserName("Kiran.Kharade")
                .WithPassword("01-06-1975")
                .WithRole(EmployeeRoles.Employee)
                .BuildEmployee();
            employeeDataContainer.GetEmployee(29).Returns(employee);

            // Execute usecase
            var actualreporteesData = reporteeService.ReporteesData(29);
            var dummyreporteesData = new List<UseCaseBoundary.DTO.ReporteeDTO>();

            Assert.That(dummyreporteesData, Is.EquivalentTo(actualreporteesData));

        }

        [Test]
        public void GivenEmployeeAsAdminRoleGetAllEmployeeListAsAReportees()
        {
            ReporteeService reporteeService = new ReporteeService(employeeDataContainer);
            var employee = new EmployeeBuilder()
                .WithID(29)
                .WithUserName("Kiran.Kharade")
                .WithPassword("01-06-1975")
                .WithRole(EmployeeRoles.Admin)
                .BuildEmployee();
            employeeDataContainer.GetEmployee(29).Returns(employee);

            var listOfEmployees = new List<Employee>()
            {
                new EmployeeBuilder()
                    .WithID(40)
                    .WithUserName("Sagar.Shende")
                    .WithPassword("0-03-1987")
                    .WithRole(EmployeeRoles.Employee)
                    .WithRole(EmployeeRoles.TeamLeader)
                    .BuildEmployee(),
                new EmployeeBuilder()
                    .WithID(41)
                    .WithUserName("Sagar.Shende")
                    .WithPassword("0-03-1987")
                    .WithRole(EmployeeRoles.Employee)
                    .WithRole(EmployeeRoles.TeamLeader)
                    .BuildEmployee()
            };
              
            employeeDataContainer.GetAllEmployeeExceptAdmin(29).Returns(listOfEmployees);

            // Execute usecase
            var actualreporteesData = reporteeService.GetReporteesData(29);

            Assert.AreEqual(actualreporteesData.Count(),2);

        }
    }
}