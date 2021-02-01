
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YakakoApi
{
    public class LoanPaymentResponse
    {
        public string ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string transactionId { get; set; }
        public string DueDate { get; set; }
        public string DueAmount { get; set; }
    }
}