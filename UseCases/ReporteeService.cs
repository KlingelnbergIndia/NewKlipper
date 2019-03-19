using DomainModel;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
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

            if (TeamLeadIsAdmin(currentEmployee) == true)
            {
                return GetAllEmployeeDataAsAReportees(currentEmployee.Id());
            }

           return GetAllReporteesData(currentEmployee);
        }

        public ReporteeDTO TeamLeadData(int employeeId)
        {
            Employee currentEmployee = _employeeRepository.GetEmployee(employeeId);

            if (currentEmployee != null)
            {
                var reporteesOfCurrentEmployee = currentEmployee.Reportees();

                if (reporteesOfCurrentEmployee.Count != 0 || currentEmployee.Roles().Contains(EmployeeRoles.Admin))
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

        public bool TeamLeadIsAdmin(Employee Employee)
        {
            return Employee.Roles().Contains(EmployeeRoles.Admin);
        }

        private List<ReporteeDTO> GetAllEmployeeDataAsAReportees(int adminId)
        {
            _employeeRepository.GetAllEmployeeExceptAdmin(adminId);
           
                var reporteeDtobjs = new List<ReporteeDTO>();

                var reporteesOfAdmin = _employeeRepository.GetAllEmployeeExceptAdmin(adminId);

            if (reporteesOfAdmin.Count != 0)
                {
                foreach (var reportee in reporteesOfAdmin)
                {
                var reporteeDto = new ReporteeDTO();
                reporteeDto.ID = reportee.Id();
                reporteeDto.FirstName = reportee.FirstName();
                reporteeDto.LastName = reportee.LastName();
                reporteeDto.LastName = reportee.LastName();
                reporteeDtobjs.Add(reporteeDto);
            }
            }
            return reporteeDtobjs;
        }

        private List<ReporteeDTO> GetAllReporteesData(Employee currentEmployee)
        {
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
