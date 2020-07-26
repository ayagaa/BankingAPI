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

        public BankingController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("createaccount")]
        public async Task<ActionResult<string>> CreateAccount()
        {
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
        [Authorize(Policy = Policies.User)]
        public async Task<ActionResult<string>> DeleteAccount()
        {
            return NotFound("Failed to delete account");
        }

        [HttpPost("getbalance")]
        //[Authorize(Policy = Policies.User)]
        public async Task<ActionResult<string>> GetBalance([FromQuery]
                                                           string Token)
        {
            if (!string.IsNullOrEmpty(Token) && Authentication.ValidateToken(Token, Configuration))
            {
                return Ok("Balance is 0");

            }
            else
                return NotFound("Need More Work");
        }

        [HttpPost("depositamount")]
        [Authorize(Policy = Policies.User)]
        public async Task<ActionResult<string>> DepositAmount()
        {
            return NotFound("Failed to deposit amount");
        }

        [HttpPost("withdrawamount")]
        [Authorize(Policy = Policies.User)]
        public async Task<ActionResult<string>> WithdrawAmount()
        {
            return NotFound("Failed to withraw amount");
        }
    }
}
