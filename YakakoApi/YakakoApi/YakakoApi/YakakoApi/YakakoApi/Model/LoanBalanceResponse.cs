using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YakakoApi.Model
{
    public class LoanBalanceResponse:Response
    {
        public string Balance { get; set; }
        public string DueDate { get; set; }
    }
}