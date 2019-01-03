﻿using System.Collections.Generic;

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
        public int EmployeeId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public List<EmployeeRoles> Role { get; set; }

        public bool Authenticate(string userName, string password)
        {
            return UserName.Equals(userName) && Password.Equals(password);
        }
    }
}