using System.Collections.Generic;

namespace DomainModel
{
    public enum EmployeeRoles
    {
        Ädmin,
        TeamLeader,
        Employee
    }

    public class Employee
    {
        private int _id;
        private string _userName;
        private string _password;
        private List<EmployeeRoles> _roles;
        public Employee(int id, string userName, string password, List<EmployeeRoles> roles)
        {
            _id = id;
            _userName = userName;
            _password = password;
            _roles = roles;
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

        public List<EmployeeRoles> Roles()
        {
            return _roles;
        }

        public bool Authenticate(string userName, string password)
        {
            userName = userName.ToLower();
            _userName = _userName.ToLower();
            return _userName.Equals(userName) && _password.Equals(password);
        }
    }
}