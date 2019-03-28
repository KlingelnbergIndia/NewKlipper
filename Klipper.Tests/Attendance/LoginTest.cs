using NUnit.Framework;
using NSubstitute;
using UseCaseBoundary;
using UseCases;
using DomainModel;
using System.Collections.Generic;
using System;
using UseCaseBoundary.DTO;

namespace Tests
{

    public class EmployeeBuilder
    {
        private string userName;
        private string password;
        private int id;
        private List<EmployeeRoles> employeeRoles = new List<EmployeeRoles>();
        private List<int> reportees = new List<int>();
        private Departments Department = Departments.Software;

        public EmployeeBuilder()
        {
            id = 1;
            userName = "dummy";
            password = "dummy";
            employeeRoles.Add(EmployeeRoles.Employee);
        }

        public EmployeeBuilder WithUserName(string userName)
        {
            this.userName = userName;
            return this;
        }

        public EmployeeBuilder WithPassword(string password)
        {
            this.password = password;
            return this;
        }

        public EmployeeBuilder WithID(int id)
        {
            this.id = id;
            return this;
        }

        public EmployeeBuilder WithRoles(List<EmployeeRoles> roles)
        {
            this.employeeRoles = roles;
            return this;
        }

        public EmployeeBuilder WithReportees(List<int> employeereportees)
        {
            this.reportees = employeereportees;
            return this;
        }

        public EmployeeBuilder WithDepartment(Departments department)
        {
            this.Department = department;
            return this;
        }

        public Employee BuildEmployee(string firstName = "Sidhdesh", string lastName = "Vadgaonkar",
                                      string title = "Software Developer",
                                      List<int> reportees = null)
        {
            return new Employee(id, userName, password, firstName,
                                lastName, title, employeeRoles, this.reportees, Department);
        }

        internal EmployeeBuilder WithRole(EmployeeRoles employee)
        {
            this.employeeRoles.Add(employee);
            return this;
        }
    }


    public class LoginTest
    {
        private IEmployeeRepository employeeDataContainer;
        private IAuthMongoDBRepository userDataContainer;

        [SetUp]
        public void Setup()
        {
            employeeDataContainer = Substitute.For<IEmployeeRepository>();
            userDataContainer = Substitute.For<IAuthMongoDBRepository>();
        }

        [Test]
        public void GivenValidUserIdAndPasswordLoginSucceeds()
        {

            // Setup
            Login login = new Login(employeeDataContainer,userDataContainer);

            var dummyEmployee =
                new EmployeeBuilder()
                .WithUserName("Sidhdesh.Vadgaonkar")
                .WithPassword("LxkwbyCWEaPHOOUJKq4nmSuTIfhz8JBZhvGlSq0L5j0=")
                .BuildEmployee();

            employeeDataContainer.GetEmployee("sidhdesh.vadgaonkar").
                Returns(dummyEmployee);

            // Execute the use case
            var employeeDetails = login.LoginUser("sidhdesh.vadgaonkar", "26-12-1995");


            Assert.IsNotNull(employeeDetails);
        }

        [Test]
        public void GivenInvalidUserIdAndCorrectPasswordLoginFails()
        {
            var login = new Login(employeeDataContainer,
                userDataContainer);

            var dummyEmployee =
                new EmployeeBuilder()
                .WithUserName("Sidhdesh.Vadgaonkar")
                .WithPassword("26-12-1995")
                .BuildEmployee();

            employeeDataContainer.GetEmployee("sidhdesh.vadgaonkar").
                Returns(dummyEmployee);

            var employeeDetails = login.LoginUser("sdhdesh.vadgaonkar", "26-12-1995");

            Assert.IsNull(employeeDetails);
        }

        [Test]
        public void GivenValidUserIdAndIncorrectPasswordLoginFails()
        {
            var login = new Login(employeeDataContainer,
                userDataContainer);

            var dummyEmployee =
                new EmployeeBuilder()
                .WithUserName("Sidhdesh.Vadgaonkar")
                .WithPassword("26-12-1995")
                .BuildEmployee();

            employeeDataContainer.GetEmployee("sidhdesh.vadgaonkar").
                Returns(dummyEmployee);

            var employeeDetails = login.LoginUser("sidhdesh.vadgaonkar", "2-12-1995");

            Assert.IsNull(employeeDetails);
        }

        [Test]
        public void OnSuccessfullLoginEmployeeNameAndTitleAreDisplayed()
        {
            var login = new Login(employeeDataContainer,
                userDataContainer);

            var dummyEmployee =
                new EmployeeBuilder()
                .WithUserName("Sidhdesh.Vadgaonkar")
                .WithPassword("LxkwbyCWEaPHOOUJKq4nmSuTIfhz8JBZhvGlSq0L5j0=")
                .BuildEmployee();

            employeeDataContainer.GetEmployee("sidhdesh.vadgaonkar").
                Returns(dummyEmployee);

            var employeeDetails = login.LoginUser("sidhdesh.vadgaonkar", "26-12-1995");

            StringAssert.IsMatch(employeeDetails.FirstName(), "Sidhdesh");
            StringAssert.IsMatch(employeeDetails.LastName(), "Vadgaonkar");
            StringAssert.IsMatch(employeeDetails.Title(), "Software Developer");
        }

        [Test]
        public void EmployeeUserNameIsNotCaseSensitive()
        {
            var login = new Login(employeeDataContainer,
                userDataContainer);

            var dummyEmployee =
                new EmployeeBuilder()
                .WithUserName("Sidhdesh.Vadgaonkar")
                .WithPassword("LxkwbyCWEaPHOOUJKq4nmSuTIfhz8JBZhvGlSq0L5j0=")
                .BuildEmployee();

            employeeDataContainer.GetEmployee("sidhdesh.vadgaonkar").
                Returns(dummyEmployee);

            var employeeDetails = login.LoginUser("SiDhDeSh.vaDgAonKar", "26-12-1995");

            Assert.IsNotNull(employeeDetails);
        }

        [Test]
        public void GivenCorrectCurrentPasswordGetNewPasswordSaveResponse()
        {
            // Setup
            Login login = new Login(employeeDataContainer, userDataContainer);

            var dummyEmployee =
                new EmployeeBuilder()
                    .WithID(63)
                    .WithUserName("Sidhdesh.Vadgaonkar")
                    .WithPassword("LxkwbyCWEaPHOOUJKq4nmSuTIfhz8JBZhvGlSq0L5j0=")
                    .BuildEmployee();

            employeeDataContainer.GetEmployee(63).
                Returns(dummyEmployee);

            // Execute the use case
            var response = login.ChangePassword(63, "26-12-1995","123");

            Assert.That(response, Is.EqualTo(ServiceResponseDTO.Saved));
        }

        [Test]
        public void GivenInCorrectCurrentPasswordGetInCorrectPasswordResponse()
        {
            // Setup
            Login login = new Login(employeeDataContainer, userDataContainer);

            var dummyEmployee =
                new EmployeeBuilder()
                    .WithID(63)
                    .WithUserName("Sidhdesh.Vadgaonkar")
                    .WithPassword("LxkwbyCWEaPHO")
                    .BuildEmployee();

            employeeDataContainer.GetEmployee(63).
                Returns(dummyEmployee);

            // Execute the use case
            var response = login.ChangePassword(63, "26-12-1995", "123");

            Assert.That(response, Is.EqualTo(ServiceResponseDTO.PassWordIncorrect));
        }

        [Test]
        public void GivenNewPasswordAndIncorrectEmployeeIdGetUserNameNotExistsResponse()
        {
            // Setup
            Login login = new Login(employeeDataContainer, userDataContainer);

            // Execute the use case
            var response = login.ChangePassword(63, "26-12-1995", "123");

            Assert.That(response, Is.EqualTo(ServiceResponseDTO.UserNameNotExists));
        }
    }
}