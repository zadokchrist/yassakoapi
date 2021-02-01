
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YakakoApi
{
    public class PaymentResponse:Error
    {
        public string transactionId { get; set; }
        public string token { get; set; }
        public string Units { get; set; }
        public string RepaymentDate { get; set; }
        public string AmountDue { get; set; }
    }
}