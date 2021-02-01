using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YakakoApi.Model
{
    public class UtilityCustomer : Error
    {
        public string CustomerName { get; set; }
        public string CustomerRef { get; set; }
        public string Utility { get; set; }
    }
}