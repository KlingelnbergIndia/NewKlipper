using Models.Core.HR.Attendance;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Klipper.Web.Application.Attendance.DataAccess
{
    public class AttendanceAccessor : IAttendanceAccessor
    {
        static HttpClient _client2 = new HttpClient() { Timeout = TimeSpan.FromMilliseconds(10000) };
        static string _baseAddress2 = "http://localhost:7000/";
        public IEnumerable<AccessEvent> GetAttendanceByDateIDAsync(int employeeId, DateTime startDate, DateTime endDate)
        {
            _client2.BaseAddress = new Uri(_baseAddress2);
            _client2.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var startStr = startDate.Year.ToString() + "-" + startDate.Month.ToString() + "-" + startDate.Day.ToString();
            var endStr = endDate.Year.ToString() + "-" + endDate.Month.ToString() + "-" + endDate.Day.ToString();
            var str = "api/attendance/" + employeeId.ToString() + "/" + startStr + "/" + endStr;
            HttpResponseMessage response = _client2.GetAsync(str).Result;
            if (response.IsSuccessStatusCode)
            {
                var jsonString = response.Content.ReadAsStringAsync().Result;
                var accessEvents = JsonConvert.DeserializeObject<IEnumerable<AccessEvent>>(jsonString);
                return accessEvents;
            }
            else
            {
                return new List<AccessEvent>();
            }
        }
    }
}
