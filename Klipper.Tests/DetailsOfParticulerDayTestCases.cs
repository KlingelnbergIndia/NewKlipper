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
        public async Task WithRespectiveTimeInAndTimeOutGetTotalTimeSpendAsync()
        {
            AttendanceService attendanceService =
                new AttendanceService(accessEventsContainer, employeeData);

            var dummyAccessevents = new AccessEventsBuilder().Build();

            accessEventsContainer.GetAccessEvents(48).Returns(dummyAccessevents);

            var listOfAccessEventsRecord = await attendanceService.GetAccessPointDetails(48,DateTime.Parse("08-10-2018").Date);
            var timeIn = listOfAccessEventsRecord[1].TimeIn;
            TimeSpan inTime = new TimeSpan(timeIn.Hour, timeIn.Minute,00);
            var timeOut = listOfAccessEventsRecord[1].TimeOut;
            TimeSpan outTime = new TimeSpan(timeOut.Hour, timeOut.Minute, 00);
            TimeSpan spendTime = outTime - inTime;
            var timeSpend = new Time(spendTime.Hours, spendTime.Minutes);
            Assert.That(listOfAccessEventsRecord[1].TimeSpend, Is.EqualTo(timeSpend));
        }

    }
}
