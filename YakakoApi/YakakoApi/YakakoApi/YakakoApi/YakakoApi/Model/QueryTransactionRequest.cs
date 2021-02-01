using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YakakoApi.Model
{
    public class QueryTransactionRequest
    {
        public string TransactionId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string TranType { get; set; }

        internal QueryTransactionResponse ValidateRequest()
        {
            QueryTransactionResponse resp = new QueryTransactionResponse();
            AppLogic logic = new AppLogic();
            try
            {
                if (string.IsNullOrEmpty(this.TransactionId))
                {
                    resp.errorCode = "24";
                    resp.errorMsg = logic.GetErrorDescription(resp.errorCode);
                }
                else if (string.IsNullOrEmpty(this.TranType))
                {
                    resp.errorCode = "5";
                    resp.errorMsg = logic.GetErrorDescription(resp.errorCode);
                }
                else if (string.IsNullOrEmpty(this.Username)||string.IsNullOrEmpty(this.Password))
                {
                    resp.errorCode = "100";
                    resp.errorMsg = logic.GetErrorDescription(resp.errorCode);
                }
                else if (!logic.IsValidVendorCreds(this.Username,this.Password))
                {
                    resp.errorCode = "100";
                    resp.errorMsg = logic.GetErrorDescription(resp.errorCode);
                }
                else
                {
                    resp.errorCode = "0";
                    resp.errorMsg= logic.GetErrorDescription(resp.errorCode);
                }
            }
            catch (Exception ex)
            {
                resp.errorCode = "1000";
                resp.errorMsg = logic.GetErrorDescription(resp.errorCode);
                logic.LogError("ValidateRequest", "QueryTransactionRequest", ex.Message);
            }
            return resp;
        }
    }
}