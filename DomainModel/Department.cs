using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModel
{
    public enum Departments
    {
        Default = 1,
        Software = 24,
        Design = 25,
        Service = 26,
        Admin = 27,
        Finance = 28,
        TechCentre = 29,
        AfterSales = 30,
        Projects = 31,
        Sales = 32,
        CompetenceCentre = 33
    }

    public class Department
    {
        private Departments _department;
        public Department(Departments department)
        {
            _department = department;
        }

        public double GetNoOfHoursToBeWorked()
        {
            return _department == Departments.Design ? 10.0 : 9.0;
        }

        public Departments GetDepartment()
        {
            return _department;
        }

        public bool IsValidWorkingDay(DateTime date)
        {
            DayOfWeek WeekDay = date.DayOfWeek;
            int weekOfMonth = ((date.Day + (int)WeekDay) / 7) + 1;

            if (WeekDay == DayOfWeek.Sunday)
                return false;

            if ((_department == Departments.Software || _department == Departments.Design) && WeekDay == DayOfWeek.Saturday)
                return false;

            if (!(_department == Departments.Software || _department == Departments.Design) && weekOfMonth == 2 || weekOfMonth == 4)
                return false;

            return true;
        }
    }
}
