using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankingAPI.Models
{
    public class Transaction
    {
        [JsonProperty(PropertyName = "transactionToken")]
        public string TransactionToken { get; set; }

        [JsonProperty(PropertyName = "transactionEmail")]
        public string TransactionEmail { get; set; }

        [JsonProperty(PropertyName = "confirmationToken")]
        public string ConfirmationToken { get; set; }

        [JsonProperty(PropertyName = "transactionType")]
        public TransactionType TransactionType { get; set; }

        [JsonProperty(PropertyName = "transactionAmount")]
        public float TransactionAmount { get; set; }

        [JsonProperty(PropertyName = "balanceAmount")]
        public float BalanceAmount { get; set; }

        [JsonProperty(PropertyName = "isValid")]
        public bool IsValid { get; set; }

        [JsonProperty(PropertyName = "isPosted")]
        public bool IsPosted { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }

    public enum TransactionType
    {
        Deposit = 1,
        Withdraw = 2
    }
}
