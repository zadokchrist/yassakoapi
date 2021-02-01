using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YakakoApi
{
    public class QueryCustomerDetailRequest
    {
        public string MeterNo { get; set; }
        public string CustomerTel { get; set; }
        public string Vendorcode { get; set; }
        public string UtilityCode { get; set; }
        public string Password { get; set; }

        /// <summary>
        /// Validates QueryCustomerDetailRequest
        /// </summary>
        /// <returns></returns>
        internal QueryCustomerDetailResponse ValidateRequest()
        {
            AppLogic logic = new AppLogic();
            QueryCustomerDetailResponse resp = new QueryCustomerDetailResponse();
            try
            {
                if (string.IsNullOrEmpty(this.CustomerTel))
                {
                    resp.errorCode = "10";
                    resp.errorMsg = logic.GetErrorDescription(resp.errorCode);
                }
                else if (!logic.IsValidCustomerNumber(this.CustomerTel))
                {
                    resp.errorCode = "10";
                    resp.errorMsg = "YOU DONT QUALIFY FOR THE SERVICE";//logic.GetErrorDescription(resp.errorCode);
                }
                else if (string.IsNullOrEmpty(Vendorcode)||string.IsNullOrEmpty(this.Password))
                {
                    resp.errorCode = "100";
                    resp.errorMsg = logic.GetErrorDescription(resp.errorCode);
                }
                else if (string.IsNullOrEmpty(this.UtilityCode))
                {
                    resp.errorCode = "22";
                    resp.errorMsg = logic.GetErrorDescription(resp.errorCode);
                }
                else if (!logic.IsValidVendorCreds(this.Vendorcode,this.Password))
                {
                    resp.errorCode = "100";
                    resp.errorMsg = logic.GetErrorDescription(resp.errorCode);
                }
                else
                {
                    resp.errorCode = "0";
                    resp.errorMsg = logic.GetErrorDescription(resp.errorCode);
                }
            }
            catch (Exception ex)
            {
                resp.errorCode = "1000";
                resp.errorMsg= logic.GetErrorDescription(resp.errorCode);
                logic.LogError("ValidateRequest", "QueryCustomerDetailRequest", ex.Message);
            }
            return resp;
        }
    }
}