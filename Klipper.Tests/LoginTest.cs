using NUnit.Framework;
using NSubstitute;
using UseCaseBoundary;
using UseCases;
using DomainModel;
using System.Collections.Generic;

namespace Tests
{

    public class EmployeeBuilder
    {
        private string userName;
        private string password;
        private int id;
        private List<EmployeeRoles> employeeRoles = new List<EmployeeRoles>();
        private List<int> reportees = new List<int>();

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
        public Employee Build()
        {
            return new Employee(id, userName, password, "Sidhdesh",
                "Vadgaonkar", "Software Developer", employeeRoles, reportees);
        }

    }


    public class LoginTest
    {
        private IEmployeeRepository employeeDataContainer;

        [SetUp]
        public void Setup()
        {
            employeeDataContainer = Substitute.For<IEmployeeRepository>();
        }

        [Test]
        public void GivenValidUserIdAndPasswordLoginSucceeds()
        {

            // Setup
            Login login = new Login(employeeDataContainer);

            var dummyEmployee =
                new EmployeeBuilder()
                .WithUserName("Sidhdesh.Vadgaonkar")
                .WithPassword("26-12-1995")
                .Build();

            employeeDataContainer.GetEmployee("sidhdesh.vadgaonkar").
                Returns(dummyEmployee);

            // Execute the use case
            var employeeDetails = login.LoginUser("sidhdesh.vadgaonkar", "26-12-1995");


            Assert.IsNotNull(employeeDetails);
        }

        [Test]
        public void GivenInvalidUserIdAndCorrectPasswordLoginFails()
        {
            Login login = new Login(employeeDataContainer);

            var dummyEmployee =
                new EmployeeBuilder()
                .WithUserName("Sidhdesh.Vadgaonkar")
                .WithPassword("26-12-1995")
                .Build();

            employeeDataContainer.GetEmployee("sidhdesh.vadgaonkar").
                Returns(dummyEmployee);

            var employeeDetails = login.LoginUser("sdhdesh.vadgaonkar", "26-12-1995");

            Assert.IsNull(employeeDetails);
        }

        [Test]
        public void GivenValidUserIdAndIncorrectPasswordLoginFails()
        {
            Login login = new Login(employeeDataContainer);

            var dummyEmployee =
                new EmployeeBuilder()
                .WithUserName("Sidhdesh.Vadgaonkar")
                .WithPassword("26-12-1995")
                .Build();

            employeeDataContainer.GetEmployee("sidhdesh.vadgaonkar").
                Returns(dummyEmployee);

            var employeeDetails = login.LoginUser("sidhdesh.vadgaonkar", "2-12-1995");

            Assert.IsNull(employeeDetails);
        }

        [Test]
        public void OnSuccessfullLoginEmployeeNameAndTitleAreDisplayed()
        {
            Login login = new Login(employeeDataContainer);

            var dummyEmployee =
                new EmployeeBuilder()
                .WithUserName("Sidhdesh.Vadgaonkar")
                .WithPassword("26-12-1995")
                .Build();

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
            Login login = new Login(employeeDataContainer);

            var dummyEmployee =
                new EmployeeBuilder()
                .WithUserName("Sidhdesh.Vadgaonkar")
                .WithPassword("26-12-1995")
                .Build();

            employeeDataContainer.GetEmployee("sidhdesh.vadgaonkar").
                Returns(dummyEmployee);

            var employeeDetails = login.LoginUser("SiDhDeSh.vaDgAonKar", "26-12-1995");

            Assert.IsNotNull(employeeDetails);
        }

    }
}