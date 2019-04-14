using System;
using System.Collections.Generic;
using System.Text;

namespace UseCaseBoundary.Email
{
    public interface IEmailConfiguration
    {
        string SmtpServer { get; }
        int SmtpPortNumber { get; }
        bool UseSSL { get; }

        string EmailId { get; }
        string Username { get; }
        string Password { get; }
    }
}
