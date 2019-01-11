using DomainModel;
using System;
using System.Collections.Generic;
using System.Text;
using UseCaseBoundary;
using UseCaseBoundary.DTO;

namespace UseCases
{
    public class ReporteeService
    {
        private IEmployeeRepository _employeeRepository;

        public ReporteeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public List<ReporteeDTO> GetReporteesData(int employeeId)
        {
            Employee currentEmployee = _employeeRepository.GetEmployee(employeeId);

            if (currentEmployee == null)
            {
                return null;
            }

            List<ReporteeDTO> reporteeDtobjs = new List<ReporteeDTO>();

            var reporteesOfCurrentEmployee = currentEmployee.Reportees();

            if (reporteesOfCurrentEmployee.Count != 0)
            {
                foreach (var reportee in reporteesOfCurrentEmployee)
                {
                    ReporteeDTO reporteeDto = new ReporteeDTO();
                    var reporteeData = _employeeRepository.GetEmployee(reportee);
                    if (reporteeData != null)
                    {
                        reporteeDto.ID = reporteeData.Id();
                        reporteeDto.FirstName = reporteeData.FirstName();
                        reporteeDto.LastName = reporteeData.LastName();
                        reporteeDtobjs.Add(reporteeDto);
                    }
                }
            }
            return reporteeDtobjs;
        }
    }
}
