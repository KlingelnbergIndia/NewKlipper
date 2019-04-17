using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DomainModel;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using UseCaseBoundary;
using UseCaseBoundary.DTO;
using UseCaseBoundary.Email;

namespace EmailImplementation
{
    public class EmailService : IEmailService
    {
        private readonly IEmailConfiguration _emailConfiguration;
        private readonly IEmployeeRepository _employeeRepository;

        public EmailService(
            IEmailConfiguration emailConfiguration,
            IEmployeeRepository employeeRepository)
        {
            _emailConfiguration = emailConfiguration;
            _employeeRepository = employeeRepository;
        }

        public void SendMailForAddNewLeave(Leave takenLeave)
        {
            var employeeDetails = _employeeRepository.GetEmployee(
                takenLeave.GetEmployeeId());
            var managerDetails = _employeeRepository.GetSupervisorDetaileOfEmployee(
                takenLeave.GetEmployeeId());

            var toEmail = employeeDetails.EmailId();
            var ccEmail = managerDetails.EmailId();

            var subject = "New leave applied";
            var htmlBody = CreateHtmlBodyForNewLeave(employeeDetails, takenLeave);
            SendMail(toEmail, ccEmail, subject, htmlBody);
        }

        private string CreateHtmlBodyForNewLeave(Employee employeeDetails, Leave takenLeave)
        {
            string htmlText = "<p><span style=\"color: #333399;\"><strong>Klipper:</strong> <strong>New Leave Added</strong></span></p>" +
            "<p><strong>Summary</strong></p>" +
            "<table style=\"border-collapse: collapse; width: 100%;\" border=\"1\">" +
            "<tbody>" +
            "<tr>" +
            "<td style=\"width: 299px;\">Name</td>" +
            "<td style=\"width: 298px;\">" +
            employeeDetails.FirstName() + " " + employeeDetails.LastName() +
            "</td>" +
            "</tr>" +
            "<tr>" +
            "<td style=\"width: 299px;\">From</td>" +
            "<td style=\"width: 298px;\">" + takenLeave.GetLeaveDate().Min() + "</td>" +
            "</tr>" +
            "<tr>" +
            "<td style=\"width: 299px;\">To</td>" +
            "<td style=\"width: 298px;\">" + takenLeave.GetLeaveDate().Max() + "</td>" +
            "</tr>" +
            "<tr>" +
            "<td style=\"width: 299px;\">Leave Type</td>" +
            "<td style=\"width: 298px;\">" + takenLeave.GetLeaveType().ToString() + "</td>" +
            "</tr>" +
            "<tr>" +
            "<td style=\"width: 299px;\">Applied Date</td>" +
            "<td style=\"width: 298px;\"> " + DateTime.Now + "</td>" +
            "</tr>" +
            "</tbody>" +
            "</table>";

            return htmlText;
        }

        private void SendMail(
            string toEmail,
            string ccEmail,
            string subject,
            string htmlBody)
        {
            using (var emailClient = new SmtpClient())
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Klipper Admin", _emailConfiguration.EmailId));
                message.To.Add(new MailboxAddress(toEmail));
                message.Cc.Add(new MailboxAddress(ccEmail));
                message.Subject = subject;
                message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = htmlBody
                };

                //emailClient.Connect(
                //    _emailConfiguration.SmtpServer,
                //    _emailConfiguration.SmtpPortNumber, 
                //    MailKit.Security.SecureSocketOptions.StartTlsWhenAvailable);

                emailClient.Connect(
                    _emailConfiguration.SmtpServer,
                    _emailConfiguration.SmtpPortNumber,
                    _emailConfiguration.UseSSL);

                emailClient.AuthenticationMechanisms.Remove("XOAUTH2");
                emailClient.Authenticate(
                    _emailConfiguration.Username,
                    _emailConfiguration.Password);
                emailClient.Send(message);

                emailClient.Disconnect(true);
            }
        }

        private MimeMessage GetMessage()
        {
            var message = new MimeMessage();
            message.To.Add(new MailboxAddress(""));
            message.From.Add(new MailboxAddress(""));
            message.Cc.Add(new MailboxAddress(""));

            message.Subject = "Klipper Leave Notification";
            //We will say we are sending HTML. But there are options for plaintext etc. 
            message.Body = new TextPart(TextFormat.Html)
            {
                Text = "this is text...."
            };

            return message;
        }

    }
}
