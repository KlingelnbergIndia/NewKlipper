using System.Collections.Generic;
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

        [SetUp]
        public void Setup()
        {
            EmployeeBuilder employeeBuilder = new EmployeeBuilder();
            employeeDataContainer = Substitute.For<IEmployeeRepository>();
        }

        public Employee CreateEmployee(int id, string userName, string password, string firstName,
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

        public ReporteeDTO ConvertEmployeeToReporteeData(Employee employee)
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

            List<EmployeeRoles> employeeRoles = new List<EmployeeRoles>()
            {
                EmployeeRoles.Ädmin,
                EmployeeRoles.TeamLeader,
                EmployeeRoles.Employee
            };

            List<int> dummyreportees = new List<int>() { 40, 46 };

            var dummyEmployee = CreateEmployee(29, "Kiran.Kharade", "01-06-1975", "Kiran", "Kharade",
                "Chief Developer and Team Lead", employeeRoles, dummyreportees);

            employeeDataContainer.GetEmployee(29).
                Returns(dummyEmployee);
            //----------------------------------------------------------------------------
            List<EmployeeRoles> employeeRolesOfReportee40 = new List<EmployeeRoles>()
            {
                EmployeeRoles.Employee
            };

            List<int> dummyreporteesForReportee40 = new List<int>();

            var dummyReportee40 = CreateEmployee(40, "Sagar.Shende", "20-03-1987", "Sagar", "Shende",
                "Senior Software Tester", employeeRolesOfReportee40, dummyreporteesForReportee40);

            employeeDataContainer.GetEmployee(40).Returns(dummyReportee40);

            //----------------------------------------------------------------------------
            List<EmployeeRoles> employeeRolesOfReportee46 = new List<EmployeeRoles>()
            {
                EmployeeRoles.Employee
            };

            List<int> dummyreporteesForReportee46 = new List<int>();

            var dummyReportee46 = CreateEmployee(46, "Krutika.Sawarkar", "21-09-1994", "Krutika", "Sawarkar",
                "Software Developer", employeeRolesOfReportee46, dummyreporteesForReportee46);

            employeeDataContainer.GetEmployee(46).Returns(dummyReportee46);

            //-------------------------------------------------------------
            var actualreporteesData = reporteeService.GetReporteesData(29);

            var dummyreporteesData = new List<UseCaseBoundary.DTO.ReporteeDTO>();

            dummyreporteesData.Add(ConvertEmployeeToReporteeData(dummyReportee40));
            dummyreporteesData.Add(ConvertEmployeeToReporteeData(dummyReportee46));

            Assert.AreEqual(dummyreporteesData.ToArray(), actualreporteesData.ToArray());
        }

        [Test]
        public void GivenEmployeeWithValidRoleAndWithNoReporteesGetEmptyListOfReportees()
        {
            ReporteeService reporteeService = new ReporteeService(employeeDataContainer);

            List<EmployeeRoles> employeeRoles = new List<EmployeeRoles>()
            {
                EmployeeRoles.Ädmin,
                EmployeeRoles.TeamLeader,
                EmployeeRoles.Employee
            };

            List<int> dummyreportees = new List<int>() { };

            var dummyEmployee = CreateEmployee(29, "Kiran.Kharade", "01-06-1975", "Kiran", "Kharade",
                "Chief Developer and Team Lead", employeeRoles, dummyreportees);

            employeeDataContainer.GetEmployee(29).
                Returns(dummyEmployee);

            //-------------------------------------------------------------
            var actualreporteesData = reporteeService.GetReporteesData(29);

            var dummyreporteesData = new List<UseCaseBoundary.DTO.ReporteeDTO>();

            Assert.That(dummyreporteesData, Is.EquivalentTo(actualreporteesData));

        }

    }
}