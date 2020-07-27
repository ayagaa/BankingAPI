using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankingAPI.Models
{
    public class User
    {
        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonIgnore]
        [JsonProperty(PropertyName = "phone")]
        public string Phone { get; set; }

        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }

        [JsonIgnore]
        public string UserRole
        {
            get { return "User"; }
        }

        public override string ToString()
        {
            return string.Format("Username: {0}, Email: {1}, Password: {2}", Username, Email, Password);
        }
    }
}
