using System.Collections.Generic;
using System.Linq;

namespace DomainModel
{
    public enum EmployeeRoles
    {
        Admin,
        TeamLeader,
        Employee
    }

    public enum ValidRole
    {
        Admin,
        TeamLeader
    }

    public class Employee
    {
        private int _id;
        private string _userName;
        private string _password;
        private string _firstName;
        private string _lastName;
        private string _title;
        private List<EmployeeRoles> _roles;
        private List<int> _reportees;
        private Departments _department;


        public Employee(int id, string userName, string password,
            string firstName, string lastName, string title, 
            List<EmployeeRoles> roles, List<int> reportees, 
            Departments department)
        {
            _id = id;
            _userName = userName;
            _password = password;
            _firstName = firstName;
            _lastName = lastName;
            _roles = roles;
            _title = title;
            _reportees = reportees;
            _department = department;
        }

        public int Id()
        {
            return _id;
        }

        public string UserName()
        {
            return _userName;
        }

        public string Password()
        {
            return _password;
        }

        public string FirstName()
        {
            return _firstName;
        }
        public string LastName()
        {
            return _lastName;
        }

        public string Title()
        {
            return _title;
        }

        public List<EmployeeRoles> Roles()
        {
            return _roles;
        }

        public List<int> Reportees()
        {
            return _reportees;
        }

        public Departments Department()
        {
            return _department;
        }

        public double GetNoOfHoursToBeWorked()
        {
            return _department == Departments.Design ? 10.0 : 9.0;
        }

        public bool Authenticate(string userName, string password)
        {
            userName = userName.ToLower();
            _userName = _userName.ToLower();
            return _userName.Equals(userName) && _password.Equals(password);
        }

        public static bool CanContainReportees(List<EmployeeRoles> roles)
        {
            if (roles != null)
            {
                return roles.Any(x => x.ToString() == ValidRole.TeamLeader.ToString() ||
                                      x.ToString() == ValidRole.TeamLeader.ToString());
            }
            else
            {
                return false;
            }
        }
    }
}