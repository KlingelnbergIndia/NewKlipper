using System.Collections.Generic;
using DomainModel;

namespace UseCaseBoundary.Model
{
    public class EmployeeDTO
    {
        private int _id;

        private string _userName;

        private string _firstName;

        private string _lastName;

        private string _title;

        private List<EmployeeRoles> _roles;

        public EmployeeDTO(int id, string userName, string firstName, string lastName,string title,List<EmployeeRoles> roles)
        {
            _id = id;
            _userName = userName;
            _roles = roles;
            _firstName = firstName;
            _lastName = lastName;
            _title = title;
        }

        public int Id()
        {
            return _id;
        }

        public string UserName()
        {
            return _userName;
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

    }
}