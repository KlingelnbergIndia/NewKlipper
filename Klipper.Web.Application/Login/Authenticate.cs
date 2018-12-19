using Models.Core.Employment;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Klipper.Web.Application.Login
{
    public class Authenticate : IAuthenticate
    {
        private HttpClient _client;
        HttpResponseMessage _response;
        private string _userName;
        private int _employeeID;
        public bool Login(string userName, string password)
        {
            var user = new
            {
                UserName = userName,
                PasswordHash = ToSha256(password)
            };
            _userName = userName;
            _client = new HttpClient();
            Uri apiUrl = new Uri(Common.AddressResolver.GetAddress("KlipperApi"));
            _client.BaseAddress = apiUrl;
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(user, Newtonsoft.Json.Formatting.Indented);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            _response = _client.PostAsync("api/auth/login", httpContent).Result;
            var responseData = _response.Content.ReadAsStringAsync().Result;

            if (_response.ReasonPhrase != "Unauthorized")
            {
                var jsonObject = JObject.Parse(responseData);
                _employeeID = Convert.ToInt32(jsonObject["id"].ToString());
            }

            SetStatusMessage();
            return _response.IsSuccessStatusCode ? true : false;
        }

        //we need to write saparate api to get these data.
        public async Task<Employee> GetEmployeeDataAsync()
        {
            _client = new HttpClient();
            Uri apiUrl = new Uri(Common.AddressResolver.GetAddress("EmployeeApi"));
            _client.BaseAddress = apiUrl;
            _response = _client.GetAsync($"api/Employees/{_employeeID}").Result;

            string jsonString = await _response.Content.ReadAsStringAsync();
            Employee empData = JsonConvert.DeserializeObject<Employee>(jsonString);
            return empData;
        }

        public LoginResponse ResponseStatus
        {
            get
            {
                if (_response.IsSuccessStatusCode)
                {
                    return LoginResponse.Success;
                }

                return LoginResponse.AuthenticationFailed;
            }
        }

        public string ResponseMessage
        {
            get
            {
                return SetStatusMessage();
            }
        }

        private static string ToSha256(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.ASCII.GetBytes(input);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        private string SetStatusMessage()
        {
            if (_response.IsSuccessStatusCode)
            {
                return "Logged in successfully.";
            }
            else if (_response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return "Please enter valid username and password.";
            }
            else if (_response.StatusCode == HttpStatusCode.InternalServerError)
            {
                return "Internal server error.";
            }

            return string.Empty;
        }
    }
}
