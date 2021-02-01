using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Services;
using YakakoApi.Model;

namespace YakakoApi
{
    /// <summary>
    /// Summary description for YakakoApi
    /// </summary>
    [WebService(Namespace = "http://yassakoapi")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class YakakoApi : System.Web.Services.WebService
    {
        private static ILog Logger
        {
            get { return _Logger ?? (_Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType)); }
        }
        private static ILog _Logger;
        AppLogic logic = new AppLogic();

        [WebMethod]
        public QueryCustomerDetailResponse QueryCustomerDetails(QueryCustomerDetailRequest req)
        {
            ConfigureLog4Net();
            QueryCustomerDetailResponse resp = new QueryCustomerDetailResponse();
            try
            {
                logic.LogRequests(req.MeterNo, req.CustomerTel, "VALIDATION", req.Vendorcode);
                resp = req.ValidateRequest();
                if (resp.errorCode.Equals("0"))
                {
                    resp = logic.GetCustomerDetails(req);
                }
            }
            catch (Exception ex)
            {
                resp.errorCode = "1000";
                resp.errorMsg = ex.Message;
                logic.LogError("QueryCustomerDetails", "YakakoApi", ex.Message);
            }
            return resp;
        }

        [WebMethod]
        public PaymentResponse MakePayment(PaymentRequest req)
        {
            ConfigureLog4Net();
            PaymentResponse resp = new PaymentResponse();
            try
            {
                logic.LogRequests(req.MeterNo, req.CustomerTel, "MakePayment", req.Username);
                resp = req.ValidatedRequest();
                if (resp.errorCode.Equals("0"))
                {
                    resp = logic.MakePayment(req);
                }
            }
            catch (Exception ex)
            {
                resp.errorCode = "1000";
                resp.errorMsg = logic.GetErrorDescription(resp.errorCode);
                logic.LogError("MakePayment", "YakakoApi", ex.Message);
            }
            return resp;
        }

        [WebMethod]
        public LoanPaymentResponse LoanRepayment(LoanRepaymentRequest req)
        {
            ConfigureLog4Net();
            LoanPaymentResponse resp = new LoanPaymentResponse();
            try
            {
                logic.LogRequests(req.MeterNo, req.CustTel, "LoanRepayment", req.Username);
                resp = req.ValidateRequest();
                if (resp.ErrorCode.Equals("0"))
                {
                    resp = logic.RepayLoan(req);
                }
            }
            catch (Exception ex)
            {
                resp.ErrorCode = "1000";
                resp.ErrorDescription = logic.GetErrorDescription(resp.ErrorCode);
                logic.LogError("LoanRepayment", "YakakoApi", ex.Message);
            }
            return resp;
        }

        [WebMethod] 
        public QueryTransactionResponse QueryTransactionStatus(QueryTransactionRequest request)
        {
            QueryTransactionResponse response = new QueryTransactionResponse();
            try
            {
                logic.LogRequests("", request.TransactionId, "QueryTransactionStatus", request.Username);
                response = request.ValidateRequest();
                if (response.errorCode.Equals("0"))
                {
                    response = logic.GetTransactionDetails(request);
                }
            }
            catch (Exception ex)
            {
                response.errorCode = "1000";
                response.errorMsg = logic.GetErrorDescription(response.errorCode);
                logic.LogError("QueryTransactionStatus", "YakakoApi", ex.Message);
            }
            return response;
        }

        [WebMethod]
        public LoanBalanceResponse QueryLoanBalance(LoanBalanceRequest req)
        {
            LoanBalanceResponse resp = new LoanBalanceResponse();
            try
            {
                logic.LogRequests(req.MeterNo, req.Msisdn, "QueryLoanBalance", req.Username);
                resp = req.ValidateRequest();
                if (resp.Errorcode.Equals("0"))
                {
                    resp = logic.GetCustomerLoanBalance(req.MeterNo, req.Msisdn,req.Username);
                }
            }
            catch (Exception ex)
            {
                resp.Errorcode = "1000";
                resp.ErrorMsg = logic.GetErrorDescription(resp.Errorcode);
                logic.LogError("QueryLoanBalance", "YakakoApi", ex.Message);
            }
            return resp;
        }

        [WebMethod]
        public ViewLastTokenResponse ViewLastToken(ViewLastTokenReq req)
        {
            ViewLastTokenResponse resp = new ViewLastTokenResponse();
            try
            {
                logic.LogRequests(req.MeterNo, req.Msisdn, "ViewLastToken", req.Username);
                resp = req.ValidateRequest();
                if (resp.Errorcode.Equals("0"))
                {
                    resp = logic.GetLastToken(req.Msisdn);
                }
            }
            catch (Exception ex)
            {
                resp.Errorcode = "1000";
                resp.ErrorMsg = logic.GetErrorDescription(resp.Errorcode);
                logic.LogError("ViewLastToken", "YakakoApi", ex.Message);
            }
            return resp;
        }

        private static void ConfigureLog4Net()
        {
            string date = DateTime.Now.ToString("yyyyMMdd");
            string filelocation = @"D:\Logs\Live\YassakoApi\logfile_" + date;
            var appender = new FileAppender()
            {
                Layout = new PatternLayout(),
                File = filelocation + ".txt",
                AppendToFile = true,
                LockingModel = new FileAppender.MinimalLock()
            };
            appender.ActivateOptions();
            BasicConfigurator.Configure(appender);
        }
    }
}
