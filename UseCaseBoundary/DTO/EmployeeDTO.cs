using System.Collections.Generic;
using DomainModel;

namespace UseCaseBoundary.Model
{
    public class EmployeeDTO
    {
        public int EmployeeId { get; set; }
        public string UserName { get; set; }
        public List<EmployeeRoles> Role { get; set; }

    }
}