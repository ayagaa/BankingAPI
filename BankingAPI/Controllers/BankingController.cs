using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankingAPI.DataAccess;
using BankingAPI.Models;
using BankingAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public BankingController(IConfiguration configuration, BankingContext bankingContext, ReplicaContext replicaContext)
        {
            Configuration = configuration;
            BankingContext = bankingContext;
            ReplicaContext = replicaContext;
        }

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
                            return Unauthorized("User already exists");
                    }

                }
            }
            return NotFound("Failed to create account");
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login()
        {
            string loginUser = HttpUtils.GetRequestBody(Request.Body);
            if (!string.IsNullOrEmpty(loginUser))
            {
                var user = JsonUtils.ParseApiData<User>(loginUser);
                if (user != null)
                {
                    Console.WriteLine(user);
                    var authUser = Authentication.AuthenticateUser(user);

                    if (authUser != null)
                    {
                        var token = Authentication.GenerateJSONWebToken(user, Configuration);
                        authUser.Password = null;
                        authUser.Token = token;

                        var resultString = JsonUtils.SerializeResults<User>(authUser);

                        return Ok(resultString);
                    }
                }
            }
            return Unauthorized("Failed to login");
        }

        [HttpPost("deleteaccount")]
        public async Task<ActionResult<string>> DeleteAccount()
        {
            return NotFound("Failed to delete account");
        }

        [HttpPost("getbalance")]
        public async Task<ActionResult<string>> GetBalance([FromQuery]
                                                           string Token)
        {
            var tokenUser = Authentication.ValidateToken(Token, Configuration);

            if (!string.IsNullOrEmpty(Token) && tokenUser != null)
            {
                var user = new User() { Username = "Allan", Email = "odwar235@gmail.com", Password = "Email1234$", Phone = "+254722637496" };
                AuthMessageSender authMessageSender = new AuthMessageSender(Configuration);
                await authMessageSender.SendEmailAsync(user, "Test Token", "5555555");
                return Ok("Balance is 0");

            }
            else
                return Unauthorized("Need More Work");
        }

        [HttpPost("depositamount")]
        public async Task<ActionResult<string>> DepositAmount([FromQuery]
                                                               string Token)
        {
            var tokenUser = Authentication.ValidateToken(Token, Configuration);

            if (!string.IsNullOrEmpty(Token) && tokenUser != null)
            {
                return Ok("Balance is 0");

            }
            else
                return NotFound("Failed to deposit amount");
        }

        [HttpPost("withdrawamount")]
        public async Task<ActionResult<string>> WithdrawAmount([FromQuery]
                                                                string Token)
        {
            var tokenUser = Authentication.ValidateToken(Token, Configuration);

            if (!string.IsNullOrEmpty(Token) && tokenUser != null)
            {
                return Ok("Balance is 0");

            }
            else
                return NotFound("Failed to withraw amount");
        }
    }
}
