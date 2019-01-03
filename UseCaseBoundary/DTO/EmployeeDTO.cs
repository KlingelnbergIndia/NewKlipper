using System.Collections.Generic;
using DomainModel;

namespace UseCaseBoundary.Model
{
    public class EmployeeDTO
    {
        private int _id;

        private string _userName;

        private List<EmployeeRoles> _roles;

        public EmployeeDTO(int id, string userName, List<EmployeeRoles> roles)
        {
            _id = id;
            _userName = userName;
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

        public List<EmployeeRoles> Roles()
        {
            return _roles;
        }

    }
}