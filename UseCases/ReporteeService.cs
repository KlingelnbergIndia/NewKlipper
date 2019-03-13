using DomainModel;
using System.Collections.Generic;
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

        public List<ReporteeDTO> ReporteesData(int employeeId)
        {
            Employee currentEmployee = 
                _employeeRepository.GetEmployee(employeeId);

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
                    //ReporteeDTO reporteeDto = new ReporteeDTO();
                    var reporteeData = _employeeRepository.GetEmployee(reportee);
                    if (reporteeData != null)
                    {
                        var reporteeDto = new ReporteeDTO();
                        reporteeDto.ID = reporteeData.Id();
                        reporteeDto.FirstName = reporteeData.FirstName();
                        reporteeDto.LastName = reporteeData.LastName();
                        reporteeDtobjs.Add(reporteeDto);
                    }
                }
            }
            return reporteeDtobjs;
        }

        public ReporteeDTO TeamLeadData(int employeeId)
        {
            Employee currentEmployee = _employeeRepository.GetEmployee(employeeId);

            if (currentEmployee != null)
            {
                var reporteesOfCurrentEmployee = currentEmployee.Reportees();

                if (reporteesOfCurrentEmployee.Count != 0)
                {
                    ReporteeDTO reporteeDto = new ReporteeDTO();
                    var teamLeadData = _employeeRepository.GetEmployee(employeeId);
                    if (teamLeadData != null)
                    {
                        reporteeDto.ID = teamLeadData.Id();
                        reporteeDto.FirstName = teamLeadData.FirstName();
                        reporteeDto.LastName = teamLeadData.LastName();
                    }
                    return reporteeDto;
                }
            }
            return null;
        }
    }
}
