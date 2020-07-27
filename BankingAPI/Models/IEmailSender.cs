using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankingAPI.Models
{
    public interface IEmailSender
    {
        Task SendEmailAsync(User user, string subject, string message);
    }
}
