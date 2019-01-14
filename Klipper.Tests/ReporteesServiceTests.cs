using System.Collections.Generic;
using DomainModel;
using NSubstitute;
using NUnit.Framework;
using Tests;
using UseCaseBoundary;
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

            List<int> dummyreportees = new List<int>(){ 40, 46};

            var dummyEmployee = 
                new EmployeeBuilder()
                    .WithID(29)
                    .WithUserName("Kiran.Kharade")
                    .WithPassword("01-06-1975")
                    .BuildEmployee("Kiran","Kharade", "Chief Developer and Team Lead",
                                    employeeRoles, dummyreportees);

            employeeDataContainer.GetEmployee(29).
                Returns(dummyEmployee);
            //----------------------------------------------------------------------------
            List<EmployeeRoles> employeeRolesOfReportee40 = new List<EmployeeRoles>()
            {
                EmployeeRoles.Employee
            };

            List<int> dummyreporteesForReportee40 = new List<int>();

            var dummyReportee40 = new EmployeeBuilder()
                .WithID(40)
                .WithUserName("Sagar.Shende")
                .WithPassword("20-03-1987")
                .BuildEmployee("Sagar", "Shende", "Senior Software Tester",
                    employeeRolesOfReportee40, dummyreporteesForReportee40);

            employeeDataContainer.GetEmployee(40).Returns(dummyReportee40);

            //----------------------------------------------------------------------------
            List<EmployeeRoles> employeeRolesOfReportee46 = new List<EmployeeRoles>()
            {
                EmployeeRoles.Employee
            };

            List<int> dummyreporteesForReportee46 = new List<int>();

            var dummyReportee46 = new EmployeeBuilder()
                .WithID(46)
                .WithUserName("Krutika.Sawarkar")
                .WithPassword("21-09-1994")
                .BuildEmployee("Krutika", "Sawarkar", "Software Developer",
                    employeeRolesOfReportee46, dummyreporteesForReportee46);

            employeeDataContainer.GetEmployee(46).Returns(dummyReportee46);

            //-------------------------------------------------------------
            var actualreportees = reporteeService.GetReporteesData(29);


            List<int> reportees = new List<int>();
            foreach (var reportee in actualreportees)
            {
                reportees.Add(reportee.ID);
            }

            Assert.That(reportees, Is.EquivalentTo(dummyreportees));
        }


    }
}