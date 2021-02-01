using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YakakoApi.Model
{
    public class QueryTransactionResponse:Error
    {
        public string YakakoId { get; set; }
        public string Status { get; set; }
        public string TranType { get; set; }
        public string Narration1 { get; set; }
        public string Narration2 { get; set; }
        public string Narration3 { get; set; }
    }
}