using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Tests;
using UseCaseBoundary;
using UseCaseBoundary.Model;
using UseCases;

namespace Klipper.Tests
{
    public class DetailsOfParticulerDayTestCases
    {
        private IAccessEventsRepository accessEventsContainer;
        private IEmployeeRepository employeeData;

        [SetUp]
        public void Setup()
        {
            accessEventsContainer = Substitute.For<IAccessEventsRepository>();
            employeeData = Substitute.For<IEmployeeRepository>();
        }

        [Test]
        public async Task WithRespectiveTimeInAndTimeOutGetTotalTimeSpend()
        {
            AttendanceService attendanceService =
                new AttendanceService(accessEventsContainer, employeeData);

            var dummyAccessevents = new AccessEventsBuilder().BuildForADay(DateTime.Parse("2018-10-05"));

            accessEventsContainer.GetAccessEventsForADay(48, DateTime.Parse("2018-10-05")).Returns(dummyAccessevents);

            var listOfAccessEventsRecord = await attendanceService.GetAccessPointDetails(48, DateTime.Parse("2018-10-05"));
           
            Assert.That(listOfAccessEventsRecord[0].TimeSpend.Hour, Is.EqualTo(8));
            Assert.That(listOfAccessEventsRecord[0].TimeSpend.Minute, Is.EqualTo(35));
        }

        //[Test]
        //public async Task WithRespectiveTimeInAndTimeOutGetTotalTimeSpend()
        //{
        //    AttendanceService attendanceService =
        //        new AttendanceService(accessEventsContainer, employeeData);

        //    var dummyAccessevents = new AccessEventsBuilder().BuildForADay(DateTime.Parse("2018-10-05"));

        //    accessEventsContainer.GetAccessEventsForADay(48, DateTime.Parse("2018-10-05")).Returns(dummyAccessevents);

        //    var listOfAccessEventsRecord = await attendanceService.GetAccessPointDetails(48, DateTime.Parse("2018-10-05"));

        //    Assert.That(listOfAccessEventsRecord[0].TimeSpend.Hour, Is.EqualTo(8));
        //    Assert.That(listOfAccessEventsRecord[0].TimeSpend.Minute, Is.EqualTo(35));
        //}

    }
}
