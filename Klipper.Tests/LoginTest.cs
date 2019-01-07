using NUnit.Framework;
using NSubstitute;
using UseCaseBoundary;
using UseCases;
using DomainModel;
using System.Collections.Generic;

namespace Tests
{
    public class LoginTest
    {
        private IEmployeeRepository _employeeRepository;

        [SetUp]
        public void Setup()
        {
            _employeeRepository = Substitute.For<IEmployeeRepository>();
        }

        [Test]
        public void GivenValidUserIdAndPasswordLoginSucceeds()
        {
            Login login = new Login(_employeeRepository);

            var dummyEmployee = DummyEmployee(48, "Sidhdesh.Vadgaonkar", "26-12-1995", "Sidhdesh",
                "Vadgaonkar", "Software Developer");

            _employeeRepository.GetEmployee("sidhdesh.vadgaonkar").
                Returns(dummyEmployee);

            var employeeDto = login.LoginUser("sidhdesh.vadgaonkar", "26-12-1995");

            Assert.IsNotNull(employeeDto);
        }

        [Test]
        public void GivenInvalidUserIdAndPasswordLoginFails()
        {
            Login login = new Login(_employeeRepository);

            var dummyEmployee = DummyEmployee(48, "Sidhdesh.Vadgaonkar", "26-12-1995", "Sidhdesh",
                "Vadgaonkar", "Software Developer");

            _employeeRepository.GetEmployee("sidhdesh.vadgaonkar").
                Returns(dummyEmployee);

            var employeeDto = login.LoginUser("sdhdesh.vadgaonkar", "26-12-1995");

            Assert.IsNull(employeeDto);
        }

        [Test]
        public void OnSuccessfullLoginEmployeeNameAndTitleAreDisplayed()
        {
            Login login = new Login(_employeeRepository);

            var dummyEmployee = DummyEmployee(48, "Sidhdesh.Vadgaonkar", "26-12-1995", "Sidhdesh",
                "Vadgaonkar", "Software Developer");

            _employeeRepository.GetEmployee("sidhdesh.vadgaonkar").
                Returns(dummyEmployee);

            var employeeDto = login.LoginUser("sidhdesh.vadgaonkar", "26-12-1995");

            string employeeName = employeeDto.FirstName() + " " + employeeDto.LastName();
            string employeeTitle = employeeDto.Title();

            StringAssert.IsMatch(employeeName, "Sidhdesh Vadgaonkar");
            StringAssert.IsMatch(employeeTitle, "Software Developer");
        }

        [Test]
        public void EmployeeIdIsNotCaseSensitive()
        {
            Login login = new Login(_employeeRepository);

            var dummyEmployee = DummyEmployee(48, "Sidhdesh.Vadgaonkar", "26-12-1995", "Sidhdesh",
                "Vadgaonkar", "Software Developer");

            _employeeRepository.GetEmployee("SiDhDeSh.vaDgAonKar").
                Returns(dummyEmployee);

            var employeeDto = login.LoginUser("SiDhDeSh.vaDgAonKar", "26-12-1995");

            Assert.IsNotNull(employeeDto);
        }


        private Employee DummyEmployee(
            int id,
            string userName,
            string password,
            string firstName,
            string lastName,
            string title
            )
        {
            List<EmployeeRoles> employeeRoles = new List<EmployeeRoles>();
            employeeRoles.Add(EmployeeRoles.Employee);

            Employee dummyEmployee = new
                Employee(id, userName, password, firstName,
                lastName, title, employeeRoles);

            return dummyEmployee;
        }
    }
}