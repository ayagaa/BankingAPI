using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankingAPI.Models
{
    public interface ISmsSender
    {
        Task SendSmsAsync(User user, string message);
    }
}
