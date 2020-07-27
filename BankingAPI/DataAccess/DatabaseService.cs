using BankingAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Odbc;

namespace BankingAPI.DataAccess
{
    internal static class DatabaseService
    {
        internal static async Task<List<User>> GetUsers()
        {
            return new List<User>
            {
               new User(){Username = "Allan", Email = "odwar235@gmail.com", Password="Email1234$", Phone = "+254722637496"},
               new User(){Username = "Tony", Email = "Tn@gmail.com", Password="Email2345%", Phone = "+254722637496"},
               new User(){Username = "Reuben", Email = "Rn@gmail.com", Password = "Email3456&", Phone = "+254722637496"}
            };
        }

        internal static async Task<bool> CreateAccount()
        {
            return false;
        }

        internal static async Task<bool> DepositAmount()
        {
            return false;
        }

        internal static async Task<bool> WithdrawAmount()
        {
            return false;
        }

        internal static async Task<float> CheckBalance()
        {
            return 0;
        }
    }
}
