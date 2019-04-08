using System;
using UseCaseBoundary.Email;

namespace EmailImplementation
{
    public class EmailConfiguration : IEmailConfiguration
    {
        public string SmtpServer { get; set; }
        public int SmtpPortNumber { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
