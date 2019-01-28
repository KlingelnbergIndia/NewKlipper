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
                EmployeeRoles.Ädmin,
                EmployeeRoles.TeamLeader,
                EmployeeRoles.Employee
            };
        }

        private Employee CreateEmployee(int id, string userName, string password, string firstName,
                                       string lastName, string title, List<EmployeeRoles> roles,
                                       List<int> reportees)
        {

            return new EmployeeBuilder()
                .WithID(id)
                .WithUserName(userName)
                .WithPassword(password)
                .BuildEmployee(firstName, lastName, title,
                    roles, reportees);
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
        public void GivenEmployeeWithValidRoleGetReportees()
        {
            ReporteeService reporteeService = new ReporteeService(employeeDataContainer);
            List<int> dummyreportees = new List<int>() { 40, 46 };

            var teamLead = new EmployeeBuilder()
                .WithID(29)
                .WithUserName("Kiran.Kharade")
                .WithPassword("01-06-1975")
                .WithRoles(employeeRoles)
                .WithReportees(dummyreportees)
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

            var actualreporteesData = reporteeService.GetReporteesData(29);
            var dummyreporteesData = new List<UseCaseBoundary.DTO.ReporteeDTO>();
            dummyreporteesData.Add(ConvertEmployeeToReporteeData(reportee40));
            dummyreporteesData.Add(ConvertEmployeeToReporteeData(reportee46));

            Assert.AreEqual(dummyreporteesData, actualreporteesData);
        }

        [Test]
        public void GivenEmployeeWithValidRoleAndWithNoReporteesGetEmptyListOfReportees()
        {
            ReporteeService reporteeService = new ReporteeService(employeeDataContainer);
            var employee = new EmployeeBuilder()
                .WithID(29)
                .WithUserName("Kiran.Kharade")
                .WithPassword("01-06-1975")
                .WithRoles(employeeRoles)
                .BuildEmployee();
            employeeDataContainer.GetEmployee(29).Returns(employee);

            var actualreporteesData = reporteeService.GetReporteesData(29);
            var dummyreporteesData = new List<UseCaseBoundary.DTO.ReporteeDTO>();

            Assert.That(dummyreporteesData, Is.EquivalentTo(actualreporteesData));

        }

    }
}