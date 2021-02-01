using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace YakakoApi.Controllers
{
    public class CreditProcessor
    {
        private bool Istrue = false;
        AppLogic logic = new AppLogic();
        DataTable table;
        /// <summary>
        /// Thi method is responsible for checking the credit scoring of a given number
        /// </summary>
        /// <param name="meternumber"></param>
        /// <param name="custTel"></param>
        /// <returns></returns>
        internal Object CheckCreditScoring(string meternumber, string custTel, string ValidationType,string vendorcode)
        {
            Object obj = null;
            try
            {
                if (!MeterNoHasOutstandingLoan(meternumber, custTel))
                {
                    if (ValidationType.Equals("CustomerValidation"))
                    {
                        QueryCustomerDetailResponse resp = new QueryCustomerDetailResponse();
                        resp.errorCode = "1";
                        resp.errorMsg = logic.GetErrorDescription(resp.errorCode);
                        obj = resp;
                    }
                    else
                    {
                        PaymentResponse resp = new PaymentResponse();
                        resp.errorCode = "1";
                        resp.errorMsg = logic.GetErrorDescription(resp.errorCode);
                        obj = resp;
                    }
                }
                else
                {
                    string limit = GetLoanLimit(meternumber);
                    if (ValidationType.Equals("CustomerValidation"))
                    {
                        QueryCustomerDetailResponse resp = new QueryCustomerDetailResponse();
                        resp.errorCode = "0";
                        resp.errorMsg = logic.GetErrorDescription(resp.errorCode);
                        resp.meter.loanLimit = limit;
                        obj = resp;
                    }
                    else
                    {
                        PaymentResponse resp = new PaymentResponse();
                        resp.errorCode = "2";
                        resp.errorMsg = logic.GetErrorDescription(resp.errorCode);
                        obj = resp;
                    }
                }
            }
            catch (Exception ex)
            {

                logic.LogError("CheckCreditScoring", "CreditProcessor", ex.Message);
            }
            return obj;
        }

        /// <summary>
        /// Gets the LoanLimit of a given Meter. This computation is done on the db level
        /// </summary>
        /// <param name="meternumber"></param>
        /// <returns></returns>
        private string GetLoanLimit(string meternumber)
        {
            string limit = "";
            try
            {
                table = logic.GetLoanLimit(meternumber);
                if (table.Rows.Count > 0)
                {
                    limit = table.Rows[0]["Limit"].ToString();
                }
            }
            catch (Exception ex)
            {
                logic.LogError("GetLoanLimit", "CreditProcessor", ex.Message);
            }
            return limit;
        }

        /// <summary>
        /// Checks whether there is an outstanding balance in the database
        ///  that is attached to either the phone number or meter 
        /// </summary>
        /// <param name="meter"></param>
        /// <param name="custTel"></param>
        /// <returns></returns>
        private bool MeterNoHasOutstandingLoan(string meter, string custTel)
        {
            try
            {
                table = logic.GetLoanBalance(meter, custTel);
                if (table.Rows.Count > 0)
                {
                    Istrue = true;
                }

            }
            catch (Exception ex)
            {
                logic.LogError("MeterNoHasOutstandingLoan", "CreditProcessor", ex.Message);
            }
            return Istrue;
        }
    }
}