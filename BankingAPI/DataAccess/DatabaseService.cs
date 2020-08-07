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
        internal static List<User> users = new List<User>()
        {
             new User(){FirstName = "Allan", Email = "odwar235@gmail.com", Password="Allan123#", Phone = "+254722637496"},
             new User(){FirstName = "Tony", Email = "ngangatonny@gmail.com", Password="Tony123#", Phone = "+254722637496"},
             new User(){FirstName = "Reuben", Email = "rrorigi@gmail.com", Password = "Reuben123#", Phone = "+254722637496"},
             new User(){FirstName = "Stacy", Email = "salyiela@gmail.com", Password = "Stacy123#", Phone = "+254722637496"}
        };
        
        internal static async Task<List<User>> GetUsers()
        {
            return users;
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
