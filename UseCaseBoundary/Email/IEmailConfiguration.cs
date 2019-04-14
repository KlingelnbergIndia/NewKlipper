using System;
using System.Collections.Generic;
using System.Text;

namespace UseCaseBoundary.Email
{
    public interface IEmailConfiguration
    {
        string SmtpServer { get; }
        int SmtpPortNumber { get; }

        string EmailId { get; }
        string Username { get; }
        string Password { get; }
    }
}
