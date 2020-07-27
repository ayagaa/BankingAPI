using BankingAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


using Twilio;
using Twilio.Types;
using Twilio.Rest.Api.V2010.Account;
using Microsoft.Extensions.Configuration;
using Twilio.Clients;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;

namespace BankingAPI.DataAccess
{
    public class AuthMessageSender : IEmailSender, ISmsSender
    {

        IConfiguration configuration = null;

        public AuthMessageSender(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public Task SendEmailAsync(User user, string subject, string message)
        {
            MailMessage mailMessage = new MailMessage(configuration["Email:email"], user.Email, subject, message);

            mailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(message, null, MediaTypeNames.Text.Plain));
            mailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(message, null, MediaTypeNames.Text.Html));
            string.Format("From: {0}, To: {1}, smtp: {2}, password: {3}", configuration["Email:email"], user.Email, configuration["Email:smtp"], configuration["Email:password"]);
            SmtpClient smtpClient = new SmtpClient(configuration["Email:smtp"], Convert.ToInt32(587));
            NetworkCredential credential = new NetworkCredential(configuration["Email:email"], configuration["Email:password"]);
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = credential;
            smtpClient.EnableSsl = true;
            smtpClient.Send(mailMessage);

            return Task.FromResult(0);
        }

        public Task SendSmsAsync(User user, string message)
        {
            return Task.FromResult(0);
        }
    }
}
