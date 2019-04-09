using System;
using System.Collections.Generic;
using System.Text;
using DomainModel;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using UseCaseBoundary.Email;

namespace EmailImplementation
{
    public class EmailService : IEmailService
    {
        private readonly IEmailConfiguration _emailConfiguration;

        public EmailService(IEmailConfiguration emailConfiguration)
        {
            _emailConfiguration = emailConfiguration;
        }

        public void SendMailForAddNewLeave(Leave takenLeave)
        {
            using (var emailClient = new SmtpClient())
            {
                //The last parameter here is to use SSL (Which you should!)
                emailClient.Connect(
                    _emailConfiguration.SmtpServer,
                    _emailConfiguration.SmtpPortNumber, 
                    true);
                emailClient.AuthenticationMechanisms.Remove("XOAUTH2");
                emailClient.Authenticate(
                    _emailConfiguration.Username,
                    _emailConfiguration.Password);

                emailClient.Send(GetMessage());

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
