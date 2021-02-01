using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YakakoApi
{
    public class QueryCustomerDetailResponse:Error
    {
        public string sn { get; set; }
        public string meterType { get; set; }
        public string MeterNo { get; set; }
        public Meter meter { get; set; }
        public Customer customer { get; set; }
    }

    public class Meter
    {
        public string id { get; set; }
        public string crton { get; set; }
        public string MeterNo { get; set; }
        public string crtby { get; set; }
        public string mdfon { get; set; }
        public string mdfby { get; set; }
        public string msno { get; set; }
        public string assetno { get; set; }
        public string descr { get; set; }
        public string metertype_id { get; set; }
        public string status { get; set; }
        public string krn { get; set; }
        public string ti { get; set; }
        public string ken { get; set; }
        public string sgc { get; set; }
        public string collectid { get; set; }
        public string deviceid { get; set; }
        public string loanLimit { get; set; }
        public string lastvending { get; set; }
        public string lastfeevending { get; set; }
        public string currentmonthunits { get; set; }
        public string currentmonthmoney { get; set; }
    }

    public class Customer
    {
        public string id { get; set; }
        public string crton { get; set; }
        public string crtby { get; set; }
        public string mdfon { get; set; }
        public string mdfby { get; set; }
        public string username { get; set; }
        public string identityid { get; set; }
        public string password { get; set; }
        public string customertype { get; set; }
        public string status { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string sex { get; set; }
        public string tel { get; set; }
        public string mobile { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string postcode { get; set; }
        public string MaxAmountToBorrow { get; set; }
        public string CustBalance { get; set; }
        public string RepaymentDate { get; set; }
    }

    
}