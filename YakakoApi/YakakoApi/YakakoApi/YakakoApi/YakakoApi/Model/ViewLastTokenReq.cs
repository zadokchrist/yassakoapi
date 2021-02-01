using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YakakoApi.Model
{
    public class ViewLastTokenReq:Credentials
    {
        public string MeterNo { get; set; }
        public string Msisdn { get; set; }

        internal ViewLastTokenResponse ValidateRequest()
        {
            ViewLastTokenResponse resp = new ViewLastTokenResponse();
            AppLogic logic = new AppLogic();
            try
            {
                if (string.IsNullOrEmpty(this.Msisdn))
                {
                    resp.Errorcode = "10";
                    resp.ErrorMsg = logic.GetErrorDescription(resp.Errorcode);
                }
                else if (string.IsNullOrEmpty(this.Username)||string.IsNullOrEmpty(this.Password)||!logic.IsValidVendorCreds(this.Username, this.Password))
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
                logic.LogError("ValidateRequest", "ViewLastTokenReq", ex.Message);
                resp.Errorcode = "1000";
                resp.ErrorMsg = logic.GetErrorDescription(resp.Errorcode);
            }
            return resp;
        }
    }
}