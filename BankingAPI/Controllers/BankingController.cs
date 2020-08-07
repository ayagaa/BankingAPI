﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankingAPI.DataAccess;
using BankingAPI.Models;
using BankingAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace BankingAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BankingController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private readonly BankingContext BankingContext;
        private readonly ReplicaContext ReplicaContext;

        public BankingController(IConfiguration configuration, IMemoryCache memoryCache, BankingContext bankingContext, ReplicaContext replicaContext)
        {
            Configuration = configuration;
            BankingContext = bankingContext;
            ReplicaContext = replicaContext;
            MemoryCacheUtils.MemoryCache = memoryCache;
        }

        //[DisableCors]
        [AllowAnonymous]
        [HttpPost("createaccount")]
        public async Task<ActionResult<string>> CreateAccount()
        {
            string newUser = HttpUtils.GetRequestBody(Request.Body);
            if (!string.IsNullOrEmpty(newUser))
            {
                var user = JsonUtils.ParseApiData<User>(newUser);
                if (user != null)
                {
                    var existingUsers = await DatabaseService.GetUsers();

                    if(existingUsers?.Count > 0)
                    {
                        var testUser = existingUsers.Find(u => u.Email == user.Email);

                        if (testUser != null)
                        {
                            var result = new RegisterStatus()
                            {
                                IsSuccessfull = false,
                                Message = "User already exists"
                            };
                            return Unauthorized(JsonUtils.SerializeResults<RegisterStatus>(result));
                        }
                        else
                        {
                            //Add code to post/ register the new user
                            DatabaseService.users.Add(user);

                            return Ok(JsonUtils.SerializeResults<RegisterStatus>(new RegisterStatus() { IsSuccessfull = true, Message = "Successfully registered" }));
                        }
                    }
                }
            }
            return Unauthorized(JsonUtils.SerializeResults<RegisterStatus>(new RegisterStatus() { IsSuccessfull = false, Message = "Failed to register. Please try again."}));
        }

        //[DisableCors]
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login()
        {
            await Task.Delay(0);
            string loginUser = HttpUtils.GetRequestBody(Request.Body);
            if (!string.IsNullOrEmpty(loginUser))
            {
                var user = JsonUtils.ParseApiData<User>(loginUser);
                if (user != null)
                {
                    var authUser = Authentication.AuthenticateUser(user);

                    if (authUser != null)
                    {
                        var token = Authentication.GenerateJSONWebToken(user, Configuration);
                        authUser.FirstName = user.FirstName;
                        authUser.Password = null;
                        authUser.Token = token;
                        authUser.IsAuthenticated = true;
                        authUser.Message = "User has successfully logged in";

                        var resultString = JsonUtils.SerializeResults<User>(authUser);

                        return Ok(resultString);
                    }
                    else
                    {
                        return NotFound(JsonUtils.SerializeResults<User>(new User() { IsAuthenticated = false, Message = "Wrong username/ password" }));
                    }
                }
            }
            return Unauthorized(JsonUtils.SerializeResults<User>(new User() { IsAuthenticated = false, Message="Oops! Something went wrong on server. Please try again."}));
        }

        //[DisableCors]
        [HttpGet("getbalance")]
        public async Task<ActionResult<string>> GetBalance([FromQuery]
                                                           string Token)
        {
            var tokenUser = Authentication.ValidateToken(Token, Configuration);

            if (!string.IsNullOrEmpty(Token) && tokenUser != null)
            {
                var user = new User() { FirstName = "Allan", Email = "odwar235@gmail.com", Password = "Email1234$", Phone = "+254722637496" };
                AuthMessageSender authMessageSender = new AuthMessageSender(Configuration);
                await authMessageSender.SendEmailAsync(user, "Test Token", "5555555");
                var result = new Transaction()
                {
                    TransactionEmail = "odwar235@gmail.com",
                    TransactionAmount = 5000,
                    TransactionType = TransactionType.Deposit,
                    ConfirmationToken = "TokenString",
                    TransactionToken = "SixFigure"
                };

                return Ok(JsonUtils.SerializeResults<Transaction>(result));
            }
            else
                return Unauthorized(default(Transaction));
        }
        //[DisableCors]
        //[HttpPost("depositamount")]
        //public async Task<ActionResult<string>> DepositAmount([FromQuery]
        //                                                       string Token)
        //{
        //    Console.WriteLine(Token);
        //    var tokenUser = Authentication.ValidateToken(Token, Configuration);

        //    string transactionString = HttpUtils.GetRequestBody(Request.Body);

        //    Console.WriteLine("Token" + transactionString);
        //    Console.WriteLine(tokenUser);

        //    if (!string.IsNullOrEmpty(transactionString) && tokenUser != null)
        //    {
        //        var transaction = JsonUtils.ParseApiData<Transaction>(transactionString);

        //        if(transaction != null)
        //        {
        //            Console.WriteLine("We got here");
        //            var user = new User() { FirstName = "Allan", Email = "odwar235@gmail.com", Password = "Email1234$", Phone = "+254722637496" };
        //            AuthMessageSender authMessageSender = new AuthMessageSender(Configuration);
        //            //Create the random verification token
        //            var verificationCode = "555555";
        //            await authMessageSender.SendEmailAsync(user, "Test Token", verificationCode);


        //            string transactionKey = string.Format("{0}%{1}", transaction.TransactionToken, verificationCode);

        //            if(!MemoryCacheUtils.MemoryCache.TryGetValue(transactionKey, out Transaction postedTransaction ))
        //            {
        //                MemoryCacheUtils.MemoryCache.Set(transactionKey, transaction);
        //            }
        //            else
        //            {
        //                MemoryCacheUtils.MemoryCache.Remove(transactionKey);

        //                MemoryCacheUtils.MemoryCache.Set(transactionKey, transaction);
        //            }

        //            transaction.IsValid = true;
        //            return Ok(JsonUtils.SerializeResults<Transaction>(transaction));
        //        }

        //        transaction.Message = "This transaction is not valid";
        //        return NotFound(JsonUtils.SerializeResults<Transaction>(transaction));

        //    }
        //    else
        //    {

        //        return Unauthorized(JsonUtils.SerializeResults<Transaction>(new Transaction() { Message = "This transaction is not authorized. Try login in again."}));
        //    }
        //}

        [HttpPost("depositamount")]
        public async Task<ActionResult<string>> DepositAmount()
        {
            string transactionString = HttpUtils.GetRequestBody(Request.Body);

            if (!string.IsNullOrEmpty(transactionString))
            {
                var transaction = JsonUtils.ParseApiData<Transaction>(transactionString);

                if (transaction != null)
                {
                    var tokenUser = Authentication.ValidateToken(transaction.TransactionToken, Configuration);

                    if(tokenUser != null)
                    {
                        //var user = DatabaseService.users.Find(us => us.Email.ToLower() == tokenUser.Email.ToLower() && us.Password == tokenUser.Password);

                        //if(user != null)
                        //{

                        //}
                        //var user = new User() { FirstName = "Allan", Email = "odwar235@gmail.com", Password = "Email1234$", Phone = "+254722637496" };
                        Random rnd = new Random();
                        int value = rnd.Next(100000, 999999);
                        AuthMessageSender authMessageSender = new AuthMessageSender(Configuration);
                        //Create the random verification token
                        var verificationCode = value.ToString();

                        await authMessageSender.SendEmailAsync(tokenUser, "VERIFICATION TOKEN", verificationCode);

                        string transactionKey = string.Format("{0}%{1}", transaction.TransactionToken, verificationCode);

                        if (!MemoryCacheUtils.MemoryCache.TryGetValue(transactionKey, out Transaction postedTransaction))
                        {
                            MemoryCacheUtils.MemoryCache.Set(transactionKey, transaction);
                        }
                        else
                        {
                            MemoryCacheUtils.MemoryCache.Remove(transactionKey);

                            MemoryCacheUtils.MemoryCache.Set(transactionKey, transaction);
                        }

                        transaction.IsValid = true;
                    }

                    return Ok(JsonUtils.SerializeResults<Transaction>(transaction));
                }

                transaction.Message = "This transaction is not valid";
                return NotFound(JsonUtils.SerializeResults<Transaction>(transaction));

            }
            else
            {

                return Unauthorized(JsonUtils.SerializeResults<Transaction>(new Transaction() { Message = "This transaction is not authorized. Try login in again." }));
            }
        }
        
        //[DisableCors]
        [HttpPost("withdrawamount")]
        public async Task<ActionResult<string>> WithdrawAmount()
        {
            string transactionString = HttpUtils.GetRequestBody(Request.Body);

            if (!string.IsNullOrEmpty(transactionString))
            {
                var transaction = JsonUtils.ParseApiData<Transaction>(transactionString);

                if (transaction != null)
                {
                    var tokenUser = Authentication.ValidateToken(transaction.TransactionToken, Configuration);

                    if (tokenUser != null)
                    {
                        //var user = new User() { FirstName = "Allan", Email = "odwar235@gmail.com", Password = "Email1234$", Phone = "+254722637496" };
                        Random rnd = new Random();
                        int value = rnd.Next(100000, 999999);
                        AuthMessageSender authMessageSender = new AuthMessageSender(Configuration);
                        //Create the random verification token
                        var verificationCode = value.ToString();
                        await authMessageSender.SendEmailAsync(tokenUser, "VERIFICATION CODE", verificationCode);

                        string transactionKey = string.Format("{0}%{1}", transaction.TransactionToken, verificationCode);

                        if (!MemoryCacheUtils.MemoryCache.TryGetValue(transactionKey, out Transaction postedTransaction))
                        {
                            MemoryCacheUtils.MemoryCache.Set(transactionKey, transaction);
                        }
                        else
                        {
                            MemoryCacheUtils.MemoryCache.Remove(transactionKey);

                            MemoryCacheUtils.MemoryCache.Set(transactionKey, transaction);
                        }

                        transaction.IsValid = true;
                    }

                    return Ok(JsonUtils.SerializeResults<Transaction>(transaction));
                }

                transaction.Message = "This transaction is not valid";
                return NotFound(JsonUtils.SerializeResults<Transaction>(transaction));

            }
            else
            {

                return Unauthorized(JsonUtils.SerializeResults<Transaction>(new Transaction() { Message = "This transaction is not authorized. Try login in again." }));
            }
        }
        //[DisableCors]
        [HttpPost("verify")]
        public async Task<ActionResult<string>> TransactionVerified()
        {
            string transactionString = HttpUtils.GetRequestBody(Request.Body);

            if (!string.IsNullOrEmpty(transactionString))
            {
                var transaction = JsonUtils.ParseApiData<Transaction>(transactionString);

                if (transaction != null)
                {
                    Console.WriteLine("Transaction is Here");
                    var tokenUser = Authentication.ValidateToken(transaction.TransactionToken, Configuration);

                    if (tokenUser != null)
                    {
                        Console.WriteLine("Token user is valid");
                        //var user = new User() { FirstName = "Allan", Email = "odwar235@gmail.com", Password = "Email1234$", Phone = "+254722637496" };
                        //AuthMessageSender authMessageSender = new AuthMessageSender(Configuration);
                        //Create the random verification token
                        //var verificationCode = "555555";
                        //await authMessageSender.SendEmailAsync(user, "Test Token", verificationCode);

                        string transactionKey = string.Format("{0}%{1}", transaction.TransactionToken, transaction.ConfirmationToken);

                        if (MemoryCacheUtils.MemoryCache.TryGetValue(transactionKey, out Transaction postedTransaction))
                        {
                            transaction.IsPosted = true;
                            MemoryCacheUtils.MemoryCache.Set(transactionKey, transaction);
                        }
                        else
                        {
                            transaction.IsPosted = false;
                        }

                        transaction.IsValid = true;
                    }

                    return Ok(JsonUtils.SerializeResults<Transaction>(transaction));
                }

                transaction.Message = "This transaction is not valid";
                return NotFound(JsonUtils.SerializeResults<Transaction>(transaction));

            }
            else
            {

                return Unauthorized(JsonUtils.SerializeResults<Transaction>(new Transaction() { Message = "This transaction is not authorized. Try login in again." }));
            }
        }
    }
}
