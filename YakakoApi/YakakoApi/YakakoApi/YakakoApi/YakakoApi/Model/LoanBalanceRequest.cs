using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YakakoApi.Model
{
    public class LoanBalanceRequest: Credentials
    {
        public string Msisdn { get; set; }
        public string MeterNo { get; set; }

        internal LoanBalanceResponse ValidateRequest()
        {
            AppLogic logic = new AppLogic();
            LoanBalanceResponse resp = new LoanBalanceResponse();
            try
            {
                if (string.IsNullOrEmpty(this.Msisdn))
                {
                    resp.Errorcode = "10";
                    resp.ErrorMsg = logic.GetErrorDescription(resp.Errorcode);
                }
                else if (string.IsNullOrEmpty(this.Username)||string.IsNullOrEmpty(this.Password))
                {
                    resp.Errorcode = "100";
                    resp.ErrorMsg = logic.GetErrorDescription(resp.Errorcode);
                }
                else if (!logic.IsValidVendorCreds(this.Username, this.Password))
                {
                    resp.Errorcode = "100";
                    resp.ErrorMsg = logic.GetErrorDescription(resp.Errorcode);
                }
                else
                {
                    resp.Errorcode = "0";
                    resp.ErrorMsg = logic.GetErrorDescription(resp.Errorcode);
                }
            }
            catch (Exception ex)
            {
                resp.Errorcode = "1000";
                resp.ErrorMsg = logic.GetErrorDescription(resp.Errorcode);
                logic.LogError("ValidateRequest", "LoanBalanceRequest", ex.Message);
            }
            return resp;
        }
    }
}