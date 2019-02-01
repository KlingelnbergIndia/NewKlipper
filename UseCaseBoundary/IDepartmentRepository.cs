using System;
using System.Collections.Generic;
using System.Text;
using DomainModel;

namespace UseCaseBoundary
{
    public interface IDepartmentRepository
    {
        Department GetDepartment(Departments department);
    }
}
