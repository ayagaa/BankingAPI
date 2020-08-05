using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankingAPI.Models
{
    public class RegisterStatus
    {
        [JsonProperty(PropertyName ="isSuccessfull")]
        public bool IsSuccessfull { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
}
