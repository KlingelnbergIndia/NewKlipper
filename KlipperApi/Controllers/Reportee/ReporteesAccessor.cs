using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Common;
using KlipperApi.Controllers.Auth;
using Models.Core.Employment;
using Models.Core.HR.Attendance;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KlipperApi.Controllers.Reportee
{
    public class ReporteesAccessor : IReporteesAccessor
    {
        private readonly IUserRepository _userRepository = null;


        public ReporteesAccessor(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }


        public async Task<List<Employee>> GetReporteesByEmployeeIDAsync(int employeeId)
        {
            var employeeApiClient = CommonHelper.GetClient(AddressResolver.GetAddress("EmployeeApi", false));
            var employeeApiString = "api/employees/" + employeeId.ToString();
            HttpResponseMessage responseForEmployeeApi = await employeeApiClient.GetAsync(employeeApiString);
            var jsonStringForEmployeeApi = await responseForEmployeeApi.Content.ReadAsStringAsync();

            var jsonObject = JObject.Parse(jsonStringForEmployeeApi);
            string reportees = jsonObject["reportees"].ToString();

            JArray totalReportees = (JArray)jsonObject["reportees"];

           List<Employee> dataOfReportees = new List<Employee>();

            for (int i =0;i<totalReportees.Count;i++)
            {
                var idOfReporteeFromTotalReportees = totalReportees[i].Value<int>();
                var empApiString = "api/employees/" + idOfReporteeFromTotalReportees.ToString();
                HttpResponseMessage responseForAttendanceApi = await employeeApiClient.GetAsync(empApiString);
                var jsonStrigForEmpApi = await responseForAttendanceApi.Content.ReadAsStringAsync();
                var employeeData = JsonConvert.DeserializeObject<Employee>(jsonStrigForEmpApi);
                dataOfReportees.Add(employeeData);
            }

            return dataOfReportees;
        }
    }
}
