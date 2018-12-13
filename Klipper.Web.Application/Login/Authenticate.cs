using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace Klipper.Web.Application.Login
{
    public class Authenticate : IAuthenticate
    {
        private HttpClient _client;
        HttpResponseMessage _response;
        public bool Login(string userName, string password)
        {
            var user = new
            {
                UserName = userName,
                PasswordHash = ToSha256(password)
            };
            _client = new HttpClient();
            _client.BaseAddress = new Uri("http://localhost:7000/");
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(user, Newtonsoft.Json.Formatting.Indented);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            _response = _client.PostAsync("api/auth/login", httpContent).Result;
            SetStatusMessage();

            return _response.IsSuccessStatusCode ? true : false;
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
                return "Invalid username or password.";
            }
            else if (_response.StatusCode == HttpStatusCode.InternalServerError)
            {
                return "Internal server error.";
            }

            return string.Empty;
        }

    }
}
