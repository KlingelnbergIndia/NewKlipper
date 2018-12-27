using Common;
using Models.Core.HR.Attendance;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Klipper.Web.Application.Attendance.DomainModel;

namespace Klipper.Web.Application.Attendance.DataAccess
{
    public class AttendanceAccessor : IAttendanceAccessor
    {

        public async Task<IEnumerable<AccessEvent>> GetAttendanceByEmployeeIdAsync(int employeeId)
        {
            var client = CommonHelper.GetClient(AddressResolver.GetAddress("KlipperApi", false));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var str = "api/attendance/employeeId?employeeId=" + employeeId.ToString();
            HttpResponseMessage response = await client.GetAsync(str);
            var jsonString = await response.Content.ReadAsStringAsync();
            var accessEvents = JsonConvert.DeserializeObject<IEnumerable<AccessEvent>>(jsonString);

            return accessEvents;
        }

        public async Task<AccessEvents> GetAccessEventsAsync(int employeeId)
        {
            List<AccessEvent> acccessEvent = (await GetAttendanceByEmployeeIdAsync(employeeId)).ToList();
            AccessEvents accessEvents = new AccessEvents(acccessEvent);
            return accessEvents;
        }


        public IEnumerable<AccessEvent> GetAttendanceByDateIDAsync(int employeeId, DateTime startDate, DateTime endDate)
        {
            var client = CommonHelper.GetClient(AddressResolver.GetAddress("KlipperApi", false));
            var startStr = startDate.Year.ToString() + "-" + startDate.Month.ToString() + "-" + startDate.Day.ToString();
            var endStr = endDate.Year.ToString() + "-" + endDate.Month.ToString() + "-" + endDate.Day.ToString();
            var str = "api/attendance/" + employeeId.ToString() + "/" + startStr + "/" + endStr;
            HttpResponseMessage response = client.GetAsync(str).Result;
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
