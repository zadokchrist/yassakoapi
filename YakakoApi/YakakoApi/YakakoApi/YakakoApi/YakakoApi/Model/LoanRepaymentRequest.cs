using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YakakoApi
{
    public class LoanRepaymentRequest
    {
        public string MeterNo { get; set; }
        public string PaymentDate { get; set; }
        public string Amount { get; set; }
        public string CustTel { get; set; }
        public string TransactionId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        internal LoanPaymentResponse ValidateRequest()
        {
            AppLogic logic = new AppLogic();
            LoanPaymentResponse resp = new LoanPaymentResponse();
            try
            {
                if (string.IsNullOrEmpty(this.Amount))
                {
                    resp.ErrorCode = "27";
                    resp.ErrorDescription = logic.GetErrorDescription(resp.ErrorCode);
                }
                else if (string.IsNullOrEmpty(this.CustTel))
                {
                    resp.ErrorCode = "10";
                    resp.ErrorDescription = logic.GetErrorDescription(resp.ErrorCode);
                }
                else if (!logic.IsValidCustomerNumber(this.CustTel))
                {
                    resp.ErrorCode = "10";
                    resp.ErrorDescription = logic.GetErrorDescription(resp.ErrorCode);
                }   
                else if (string.IsNullOrEmpty(this.Password)||string.IsNullOrEmpty(this.Username))
                {
                    resp.ErrorCode = "100";
                    resp.ErrorDescription = logic.GetErrorDescription(resp.ErrorCode);
                }
                else if (string.IsNullOrEmpty(this.TransactionId))
                {
                    resp.ErrorCode = "24";
                    resp.ErrorDescription = logic.GetErrorDescription(resp.ErrorCode);
                }
                else if (string.IsNullOrEmpty(this.PaymentDate))
                {
                    resp.ErrorCode = "23";
                    resp.ErrorDescription = logic.GetErrorDescription(resp.ErrorCode);
                }
                else if (!logic.IsValidVendorCreds(this.Username,this.Password))
                {
                    resp.ErrorCode = "100";
                    resp.ErrorDescription = logic.GetErrorDescription(resp.ErrorCode);
                }
                else
                {
                    resp.ErrorCode = "0";
                    resp.ErrorDescription = logic.GetErrorDescription(resp.ErrorCode);
                }
            }
            catch (Exception ex)
            {
                resp.ErrorCode = "1000";
                resp.ErrorDescription = logic.GetErrorDescription(resp.ErrorCode);
                logic.LogError("ValidateRequest", "LoanRepaymentRequest", ex.Message);
            }
            return resp;
        }
    }
}