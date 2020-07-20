using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankingController : ControllerBase
    {
        [HttpPost("createaccount")]
        public async Task<ActionResult<string>> CreateAccount()
        {

            return NotFound(string.Empty);
        }

        [HttpPost("deleteaccount")]
        public async Task<ActionResult<string>> DeleteAccount()
        {
            return NotFound(string.Empty);
        }

        [HttpPost("getbalance")]
        public async Task<ActionResult<string>> GetBalance()
        {
            return NotFound(string.Empty);
        }

        [HttpPost("depositamount")]
        public async Task<ActionResult<string>> DepositAmount()
        {
            return NotFound(string.Empty);
        }

        [HttpPost("withdrawamount")]
        public async Task<ActionResult<string>> WithdrawAmount()
        {
            return NotFound(string.Empty);
        }
    }
}
