using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Common;
using Models.Core.Employment;
using Newtonsoft.Json;

namespace Klipper.Web.Application.Reportee.Data_Access
{
    public class ReporteesAccessor : IReporteeAccessor
    {
        public async Task<List<Employee>> GetReporteesByEmployeeId(int employeeId)
        {
            var klipperClient = CommonHelper.GetClient(AddressResolver.GetAddress("KlipperApi", false));
            string klipperClientString = "api/Reportees/byEmployeeId?employeeId=" + employeeId.ToString();
            HttpResponseMessage responseFromKlipperApi = await klipperClient.GetAsync(klipperClientString);
            var jsonStringForKlipperApi = await responseFromKlipperApi.Content.ReadAsStringAsync();
            var reporteesData = JsonConvert.DeserializeObject<List<Employee>>(jsonStringForKlipperApi);
            return reporteesData;
        }
    }
}
