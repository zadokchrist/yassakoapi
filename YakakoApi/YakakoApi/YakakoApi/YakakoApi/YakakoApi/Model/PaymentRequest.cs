using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YakakoApi
{
    public class PaymentRequest
    {
        public string MeterNo { get; set; }
        public string PaymentDate { get; set; }
        public string CustomerTel { get; set; }
        public string UtilityCode { get; set; }
        public string Amount { get; set; }
        public string TransactionId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string DigitalSignature { get; set; }

        internal PaymentResponse ValidatedRequest()
        {
            AppLogic logic = new AppLogic();
            PaymentResponse resp = new PaymentResponse();
            try
            {
                if (string.IsNullOrEmpty(this.MeterNo))
                {
                    resp.errorCode = "21";
                    resp.errorMsg = logic.GetErrorDescription(resp.errorCode);
                }
                else if (string.IsNullOrEmpty(this.PaymentDate))
                {
                    resp.errorCode = "23";
                    resp.errorMsg = logic.GetErrorDescription(resp.errorCode);
                }
                else if (string.IsNullOrEmpty(this.CustomerTel))
                {
                    resp.errorCode = "10";
                    resp.errorMsg = logic.GetErrorDescription(resp.errorCode);
                }
                else if (!logic.IsValidCustomerNumber(this.CustomerTel))
                {
                    resp.errorCode = "10";
                    resp.errorMsg = logic.GetErrorDescription(resp.errorCode);
                }
                else if (string.IsNullOrEmpty(this.UtilityCode))
                {
                    resp.errorCode = "22";
                    resp.errorMsg = logic.GetErrorDescription(resp.errorCode);
                }
                else if (string.IsNullOrEmpty(this.Amount))
                {
                    resp.errorCode = "27";
                    resp.errorMsg = logic.GetErrorDescription(resp.errorCode);
                }
                else if (string.IsNullOrEmpty(this.TransactionId))
                {
                    resp.errorCode = "24";
                    resp.errorMsg = logic.GetErrorDescription(resp.errorCode);
                }
                else if (string.IsNullOrEmpty(this.Username)||string.IsNullOrEmpty(this.Password)||!logic.IsValidVendorCreds(this.Username,this.Password))
                {
                    resp.errorCode = "100";
                    resp.errorMsg = logic.GetErrorDescription(resp.errorCode);
                }
                else if (string.IsNullOrEmpty(this.DigitalSignature))
                {
                    resp.errorCode = "20";
                    resp.errorMsg = logic.GetErrorDescription(resp.errorCode);
                }
                else if (logic.CustomerHasALoan(this.MeterNo,this.CustomerTel))
                {
                    resp.errorCode = "12";
                    resp.errorMsg = logic.GetErrorDescription(resp.errorCode);
                }
                else if (!this.DigitalSignature.Equals("ZefG4KxNXcG/JjfP00rTyg=="))
                {
                    resp.errorCode = "20";
                    resp.errorMsg = logic.GetErrorDescription(resp.errorCode);
                }
                else if (!logic.IsRightAmountToBorrow(this.CustomerTel,this.Amount))
                {
                    resp.errorCode = "27";
                    string maxamt = logic.GetMaximumAmountForCustomer(this.CustomerTel);
                    resp.errorMsg = logic.GetErrorDescription(resp.errorCode)+". MAX AMOUNT TO BORROW "+ maxamt;
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
                resp.errorMsg = logic.GetErrorDescription(resp.errorCode);
                logic.LogError("ValidatedRequest", "PaymentRequest", ex.Message);
            }
            return resp;
        }
    }
}