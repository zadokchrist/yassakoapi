using System;
using System.Data;
using YakakoApi.Model;

namespace YakakoApi
{
    /// <summary>
    /// This Class is responsible of handling of all the application logic of the yakako api
    /// </summary>
    public class AppLogic
    {
        private bool Istrue = false;
        private DatabaseHandler dh = new DatabaseHandler();
        private DataTable table;
        private bool IsTrue = false;

        /// <summary>
        /// Validates the customer details at the utility
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        internal QueryCustomerDetailResponse GetCustomerDetails(QueryCustomerDetailRequest req)
        {
            QueryCustomerDetailResponse resp = new QueryCustomerDetailResponse();
            try
            {
                Customer customer = new Customer();
                if (string.IsNullOrEmpty(req.MeterNo))
                {
                    req.CustomerTel = FormatTelephone256(req.CustomerTel);
                    if (CustomerHasALoan(req.MeterNo, req.CustomerTel))
                    {
                        UtilityCustomer yassakocustomer = GetMeterNoAttachedToPhone(req.CustomerTel);
                        resp.MeterNo = yassakocustomer.CustomerRef;
                        if (string.IsNullOrEmpty(resp.MeterNo))
                        {

                        }
                        else
                        {
                            string[] splitedname = yassakocustomer.CustomerName.Split(' ');
                            customer.firstname = splitedname[0];
                            customer.lastname = splitedname[1];
                            customer.customertype = "PREPAID";
                        }

                        customer.CustBalance = CustomerLoanBalance(req.MeterNo, req.CustomerTel).ToString();
                        resp.customer = customer;
                        resp.errorCode = "0";
                        resp.errorMsg = GetErrorDescription(resp.errorCode);
                        resp.customer.RepaymentDate = GetLoanRepaymentDate(req.CustomerTel);
                    }
                    else
                    {
                        resp.errorCode = "6";
                        resp.errorMsg= GetErrorDescription(resp.errorCode);
                    }
                }
                else
                {
                    YakakoApiReferences.Controllers.AppLogic referencelogic = new YakakoApiReferences.Controllers.AppLogic();
                    YakakoApiReferences.Models.ValidateMeterResponse validatemeterresp = new YakakoApiReferences.Models.ValidateMeterResponse();
                    validatemeterresp = referencelogic.LiveValidateMeter(req.MeterNo);

                    if (validatemeterresp.respmsg.Equals("SUCCESS"))
                    {
                        double umemebalance = double.Parse(validatemeterresp.info4);
                        if (umemebalance < 500)
                        {
                            resp.MeterNo = req.MeterNo;
                            string name = validatemeterresp.info2;
                            string[] splitedname = name.Split(' ');
                            customer.CustBalance = CustomerLoanBalance(req.MeterNo, req.CustomerTel).ToString();
                            dh.InsertCustomer(req.MeterNo, name, req.UtilityCode);
                            resp.errorCode = "0";
                            customer.firstname = splitedname[0];
                            customer.lastname = splitedname[1];
                            customer.customertype = validatemeterresp.info3;
                            resp.MeterNo += "Name : " + customer.firstname + " " + customer.lastname;
                        }
                        else
                        {
                            resp.errorCode = "4";
                        }
                        resp.customer = customer;
                        resp.errorMsg = GetErrorDescription(resp.errorCode);
                    }
                    else
                    {
                        resp.errorCode = "3";
                        resp.errorMsg = validatemeterresp.respmsg;
                    }
                }
               
            }
            catch (Exception ex)
            {
                resp.errorCode = "1000";
                resp.errorMsg = ex.Message;
                LogError("GetCustomerDetails", "AppLogic", ex.Message);
            }
            return resp;
        }

        /// <summary>
        /// Checks whether the number is valid abd as well whitelisted
        /// </summary>
        /// <param name="customerTel"></param>
        /// <returns></returns>
        internal bool IsValidCustomerNumber(string customerTel)
        {
            bool isValidCustomer = false;
            try
            {
                string formatednumber = FormatTelephone256(customerTel);
                if (formatednumber.Length.Equals(12))
                {
                    isValidCustomer = IsWhitelisted(formatednumber);
                }
                else
                {
                    isValidCustomer = false;
                }
            }
            catch (Exception ex)
            {
                LogError("IsValidCustomerNumber", "AppLogic", ex.Message);
            }
            return isValidCustomer;
        }

        /// <summary>
        /// Checks whether number has been whitelisted
        /// </summary>
        /// <param name="customerTel"></param>
        /// <returns></returns>
        private bool IsWhitelisted(string customerTel)
        {
            bool IsWhiteListed = false;
            try
            {
                string formatednumber = FormatTelephone256(customerTel);
                DataTable whitelistednumber = dh.GetWhitelistedNumber(formatednumber);
                if (whitelistednumber.Rows.Count>0)
                {
                    IsWhiteListed = true;
                }
            }
            catch (Exception ex)
            {
                LogError("IsWhitelisted", "AppLogic", ex.Message);
            }
            return IsWhiteListed;
        }

        /// <summary>
        /// format phone to 256 which is standed for all ugandan telecom companies
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private string FormatTelephone256(string number)
        {
            string okNumber = "";
            if (number.Trim().StartsWith("000256") && number.Length == 15)
            {
                okNumber = number.Remove(0, 3);
            }
            else if (number.Trim().StartsWith("00256") && number.Length == 14)
            {
                okNumber = number.Remove(0, 2);
            }
            else if ((number.Trim().StartsWith("256") && number.Length == 12))
            {
                okNumber = number;
            }
            else if ((number.Trim().StartsWith("0") && number.Length == 10))
            {
                okNumber = number.Remove(0, 1);
                okNumber = "256" + okNumber;
            }
            else if ((number.Trim().StartsWith("7") && number.Length == 9))
            {
                okNumber = "256" + number;
            }
            else if ((number.Trim().StartsWith("+") && number.Length == 13))
            {
                okNumber = number.Remove(0, 1);
            }
            else
            {
                okNumber = number;
            }
            return okNumber;
        }

        private string GetLoanRepaymentDate(string customerTel)
        {
            string repaymentdate = "";
            try
            {
                DataTable table = dh.getLastToken(customerTel);
                repaymentdate = table.Rows[0]["RepaymentDate"].ToString();
            }
            catch (Exception ex)
            {
                LogError("GetLoanRepaymentDate", "AppLogic", ex.Message);
            }
            return repaymentdate;
        }

        /// <summary>
        /// Gets status of the transaction from the database
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal QueryTransactionResponse GetTransactionDetails(QueryTransactionRequest request)
        {
            QueryTransactionResponse resp = new QueryTransactionResponse();
            try
            {
                table = dh.GetTransactionDetails(request.TransactionId, request.Username);
                if (table.Rows.Count > 0)
                {
                    resp.errorCode = "0";
                    resp.errorMsg = GetErrorDescription(resp.errorCode);
                    resp.Status = table.Rows[0]["Status"].ToString();
                    resp.Narration1 = table.Rows[0]["Reason"].ToString(); ;
                }
            }
            catch (Exception ex)
            {
                resp.errorCode = "1000";
                resp.errorMsg = GetErrorDescription(resp.errorCode);
                LogError("GetTransactionDetails", "AppLogic", ex.Message);
            }
            return resp;
        }

        /// <summary>
        /// Inserts loanRepayment
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        internal LoanPaymentResponse RepayLoan(LoanRepaymentRequest req)
        {
            LoanPaymentResponse response = new LoanPaymentResponse();
            try
            {
                req.CustTel = FormatTelephone256(req.CustTel);
                UtilityCustomer yassakocustomer = GetMeterNoAttachedToPhone(req.CustTel);
                if (yassakocustomer.errorCode.Equals("0"))
                {
                    LoanBalanceResponse loanBalance = GetCustomerLoanBalance(yassakocustomer.CustomerRef, req.CustTel, req.Username);
                    double loanbal = double.Parse(loanBalance.Balance);
                    double tranabount = double.Parse(req.Amount);
                    if (tranabount > loanbal)
                    {
                        response.ErrorCode = "13";
                    }
                    else
                    {
                        double repaymentamount = double.Parse(req.Amount);
                        int balanticipated = (int)Math.Ceiling(loanbal - repaymentamount);
                        if (balanticipated.Equals(0))
                        {
                            string lastloanid = GetLoanId(req.CustTel);
                            ClearLoan(req.TransactionId, req.CustTel, lastloanid);
                        }
                        else if (balanticipated<500)
                        {
                            response.ErrorCode = "13";
                        }
                            string loanid = LogLoanTransaction(req.TransactionId, req.CustTel, yassakocustomer.CustomerRef, yassakocustomer.CustomerName, req.PaymentDate, req.Amount, req.Username, "LP", yassakocustomer.Utility);
                            if (string.IsNullOrEmpty(loanid))
                            {
                                response.ErrorCode = "11";
                            }
                            else
                            {
                                response.ErrorCode = "0";
                                response.transactionId = loanid;
                                response.DueAmount = balanticipated.ToString();
                            }
                        
                    }
                    response.ErrorDescription = GetErrorDescription(response.ErrorCode);
                }
                else
                {
                    response.ErrorCode = yassakocustomer.errorCode;
                    response.ErrorDescription = GetErrorDescription(response.ErrorCode);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = "1000";
                response.ErrorDescription = GetErrorDescription(response.ErrorCode);
                LogError("RepayLoan", "AppLogic", ex.Message);
            }
            return response;
        }

        private void ClearLoan(string transactionId, string custTel, string lastloanid)
        {
            try
            {
                dh.ClearLoan(transactionId, custTel, lastloanid);
            }
            catch (Exception ex)
            {
                LogError("ClearLoan", "Applogic", ex.Message);
            }
        }

        internal void LogRequests(string meterno, string customertel, string requesttype,string vendorcode) 
        {
            try
            {
                dh.LogRequests(meterno, customertel, requesttype, vendorcode);
            }
            catch (Exception ex)
            {
                dh.LogError("LogRequests", "AppLogic", ex.Message, "Yakako Api");
            }
        }
        private string GetInitialLoanBalance(string customertel)
        {
            string amount = "";
            try
            {
                DataTable dataTable = dh.GetLoanId(customertel);
                amount = dataTable.Rows[0]["Amount"].ToString();
            }
            catch (Exception ex)
            {
                LogError("GetInitialLoanBalance", "AppLogic", ex.Message);
            }
            return amount;
        }

        private string GetLoanId(string customerTel)
        {
            string loanid = "";
            try
            {
                DataTable table = dh.GetLoanId(customerTel);
                if (table.Rows.Count>0)
                {
                    loanid = table.Rows[0]["LoanId"].ToString();
                }
            }
            catch (Exception ex)
            {
                LogError("GetLoanId", "AppLogic", ex.Message);
            }
            return loanid;
        }

        /// <summary>
        /// Gets the meter that borrowed 
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        private UtilityCustomer GetMeterNoAttachedToPhone(string phone)
        {
            UtilityCustomer yassakocustomer = new UtilityCustomer();
            try
            {
                DataTable metertable = dh.GetMeterNoAttachedToPhone(phone);
                if (metertable.Rows.Count > 0)
                {
                    string loanid = metertable.Rows[0]["LoanId"].ToString();
                    DataTable loandetails = dh.GetLoanDetails(loanid);
                    yassakocustomer.CustomerName = loandetails.Rows[0]["CustName"].ToString();
                    yassakocustomer.CustomerRef = loandetails.Rows[0]["CustRef"].ToString();
                    yassakocustomer.Utility = loandetails.Rows[0]["Utility"].ToString();
                    yassakocustomer.errorCode = "0";
                    yassakocustomer.errorMsg = GetErrorDescription(yassakocustomer.errorCode);
                }
                else
                {
                    yassakocustomer.errorCode = "6";
                    yassakocustomer.errorMsg = GetErrorDescription(yassakocustomer.errorCode);
                }
            }
            catch (Exception ex)
            {
                LogError("GetMeterNoAttachedToPhone", "AppLogic", ex.Message);
            }
            return yassakocustomer;
        }

        /// <summary>
        /// Gets the last token for electricity customers
        /// </summary>
        /// <param name="meterNo"></param>
        /// <returns></returns>
        internal ViewLastTokenResponse GetLastToken(string msisdn)
        {
            ViewLastTokenResponse resp = new ViewLastTokenResponse();
            try
            {
                string formatedmsisdn = FormatTelephone256(msisdn);
                DataTable table = dh.getLastToken(formatedmsisdn);
                if (table.Rows.Count > 0)
                {
                    resp.Errorcode = "0";
                    resp.ErrorMsg = GetErrorDescription(resp.Errorcode);
                    resp.Token = table.Rows[0]["UtilityRef"].ToString();
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                resp.Errorcode = "1000";
                resp.ErrorMsg = GetErrorDescription(resp.Errorcode);
                LogError("GetLastToken", "AppLogic", ex.Message);
            }
            return resp;
        }

        /// <summary>
        /// Records loan in the db
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        internal PaymentResponse MakePayment(PaymentRequest req)
        {
            PaymentResponse resp = new PaymentResponse();
            try
            {
                req.CustomerTel = FormatTelephone256(req.CustomerTel);
                UtilityCustomer utilitycustomer = GetUtilityCustomerLocally(req.MeterNo);
                if (utilitycustomer.errorCode.Equals("0"))
                {
                    double tranAmount = double.Parse(req.Amount)+190;
                    req.Amount = int.Parse(tranAmount.ToString()).ToString();
                    LogLoanHist(req.MeterNo, req.CustomerTel, req.Amount, req.TransactionId);
                    string loanid = LogLoanTransaction(req.TransactionId, req.CustomerTel, req.MeterNo, utilitycustomer.CustomerName, req.PaymentDate, req.Amount, req.Username, "LR", utilitycustomer.Utility);
                    if (string.IsNullOrEmpty(loanid))
                    {
                        resp.errorCode = "11";
                    }
                    else
                    {
                        double interest = (tranAmount) * 0.15;
                        int amountdue = (int)Math.Ceiling(tranAmount + interest);
                        resp.errorCode = "0";
                        resp.transactionId = loanid;
                        resp.AmountDue = amountdue.ToString();
                        resp.RepaymentDate = DateTime.Now.AddDays(30).ToString();
                    }
                    resp.errorMsg = GetErrorDescription(resp.errorCode);
                }
                else
                {
                    resp.errorCode = utilitycustomer.errorCode;
                    resp.errorMsg = utilitycustomer.errorMsg;
                }

            }
            catch (Exception ex)
            {
                resp.errorCode = "1000";
                resp.errorMsg = GetErrorDescription(resp.errorCode);
                LogError("MakePayment", "AppLogic", ex.Message);
            }
            return resp;
        }

        /// <summary>
        /// Logs a loan tranaction in the database
        /// </summary>
        /// <param name="TransactionId"></param>
        /// <param name="CustomerTel"></param>
        /// <param name="MeterNo"></param>
        /// <param name="date"></param>
        /// <param name="Amount"></param>
        /// <param name="Username"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        private string LogLoanTransaction(string TransactionId, string CustomerTel, string MeterNo, string customerName, string paymentdate, string Amount, string Username, string status, string Utility)
        {
            string loanid = "";
            try
            {
                DateTime date = DateTime.Now;
                DataTable result = dh.MakePayment(TransactionId, CustomerTel, MeterNo, customerName, date, Amount, Username, status, Utility);
                if (result.Rows.Count > 0)
                {
                    loanid = result.Rows[0]["YassakoId"].ToString();
                }
            }
            catch (Exception ex)
            {
                LogError("LogLoanTransaction", "AppLogic", ex.Message);
            }
            return loanid;
        }

        private void LogLoanHist(string customerRef,string CustomerTel,string Amount,string loanid)
        {
            try
            {
                dh.LogLoanHist(customerRef, CustomerTel, Amount, loanid);
            }
            catch (Exception ex)
            {
                LogError("LogLoanHist", "AppLogic", ex.Message);
            }
        }
        /// <summary>
        /// Checks whether the vendor credentials are correct
        /// </summary>
        /// <param name="vendorcode"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        internal bool IsValidVendorCreds(string vendorcode, string password)
        {
            try
            {
                table = dh.GetVendorCredentials(vendorcode, password);
                if (table.Rows.Count.Equals(1))
                {
                    if (bool.Parse(table.Rows[0]["Active"].ToString()))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError("IsValidVendorCreds", "AppLogic", ex.Message);
            }
            return IsTrue;
        }

        /// <summary>
        /// Returns the Loan limit of a meter number from the db
        /// </summary>
        /// <param name="meternumber"></param>
        /// <returns></returns>
        internal DataTable GetLoanLimit(string meternumber)
        {
            try
            {
                table = dh.GetLoanLimit(meternumber);
            }
            catch (Exception ex)
            {
                LogError("GetLoanLimit", "AppLogic", ex.Message);
            }
            return table;
        }

        /// <summary>
        /// Gets loan balance  
        /// </summary>
        /// <param name="meterno"></param>
        /// <param name="Msisdn"></param>
        /// <returns></returns>
        internal LoanBalanceResponse GetCustomerLoanBalance(string meterno, string Msisdn, string vendorcode)
        {
            LoanBalanceResponse resp = new LoanBalanceResponse();
            try
            {
                string formatedmsisdn = FormatTelephone256(Msisdn);
                resp.Errorcode = "0";
                resp.ErrorMsg = GetErrorDescription(resp.Errorcode);
                resp.Balance = CustomerLoanBalance(meterno, formatedmsisdn).ToString();
            }
            catch (Exception ex)
            {
                resp.Errorcode = "1000";
                resp.ErrorMsg = GetErrorDescription(resp.Errorcode);
                LogError("GetCustomerLoanBalance", "AppLogic", ex.Message);
            }
            return resp;
        }

        /// <summary>
        /// Check whether the customer has a loan with yakako
        /// </summary>
        /// <param name="meterno"></param>
        /// <param name="Msisdn"></param>
        /// <returns></returns>
        internal bool CustomerHasALoan(string meterno, string Msisdn)
        {
            bool HasLoan = true;
            try
            {
                string formatedmsisdn = FormatTelephone256(Msisdn);
                double customerloanbalance = CustomerLoanBalance(meterno, formatedmsisdn);
                if (customerloanbalance.Equals(0))
                {
                    HasLoan = false;
                }

            }
            catch (Exception ex)
            {
                LogError("CustomerHasALoan", "AppLogic", ex.Message);
            }
            return HasLoan;
        }

        /// <summary>
        /// Check whether the customer is requesting amount with in his credit score
        /// </summary>
        /// <param name="msisdn"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        internal bool IsRightAmountToBorrow(string msisdn, string amount)
        {
            bool Istrue = false;
            try
            {
                string allowedamt = GetMaximumAmountForCustomer(msisdn);
                double allowedamount = double.Parse(allowedamt);
                double custamt = double.Parse(amount);
                if (custamt<= allowedamount && (custamt+190)>=2000)
                {
                    Istrue = true;
                }
            }
            catch (Exception ex)
            {
                LogError("IsRightAmountToBorrow", "AppLogic", ex.Message);
            }
            return Istrue;
        }

        /// <summary>
        /// Gets the credit score for the customer basing on the previous loanhist
        /// </summary>
        /// <param name="msisdn"></param>
        /// <returns></returns>
        internal string GetMaximumAmountForCustomer(string msisdn)
        {
            string maxamt = "";
            try
            {
                string formatedmsisdn = FormatTelephone256(msisdn);
                DataTable maxamount = dh.GetMaximumAmountForCustomer(formatedmsisdn);
                if (maxamount.Rows.Count>0)
                {
                    maxamt = maxamount.Rows[0]["loanlimit"].ToString();
                }
                else
                {
                    maxamt = "2000";// return the default value if the system has failed to compute the credit score
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return maxamt;
        }

        /// <summary>
        /// Gets the loan balance of the customer
        /// </summary>
        /// <param name="meterno"></param>
        /// <param name="Msisdn"></param>
        /// <returns></returns>
        private double CustomerLoanBalance(string meterno, string Msisdn)
        {
            double loanbalance = 0;
            try
            {
                if (string.IsNullOrEmpty(meterno.Trim()))
                {
                    DataTable table2 = GetCustomerBalanceByMsisdn(Msisdn);
                    if (table2.Rows.Count > 0)
                    {
                        loanbalance = double.Parse(table2.Rows[0]["Balance"].ToString());
                    }
                }
                else
                {
                    DataTable table1 = GetLoanBalance(meterno, Msisdn);
                    if (table1.Rows.Count > 0)
                    {
                        loanbalance = double.Parse(table1.Rows[0]["Balance"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                loanbalance = 1;
                LogError("CustomerLoanBalance", "AppLogic", ex.Message);
            }
            return loanbalance;
        }

        /// <summary>
        /// Gets the loan Balance from the customer
        /// </summary>
        /// <param name="meter"></param>
        /// <param name="custTel"></param>
        /// <returns></returns>
        internal DataTable GetLoanBalance(string meter, string custTel)
        {
            try
            {
                table = dh.GetLoanBalance(meter, custTel);
            }
            catch (Exception ex)
            {
                LogError("GetLoanBalance", "AppLogic", ex.Message);
            }
            return table;
        }

        /// <summary>
        /// Gets Balance Attached to a phone number
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        internal DataTable GetCustomerBalanceByMsisdn(string phone)
        {
            try
            {
                table = dh.GetLoanBalanceByPhone(phone);
            }
            catch (Exception ex)
            {
                LogError("GetLoanBalance", "AppLogic", ex.Message);
            }
            return table;
        }
        /// <summary>
        /// Logs error in the database
        /// </summary>
        /// <param name="method"></param>
        /// <param name="Level"></param>
        /// <param name="error"></param>
        /// <param name="loggedby"></param>
        internal void LogError(string method, string Level, string error)
        {
            try
            {
                dh.LogError(method, Level, error, "YakakoAggregatorApi");
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Gets the error from the database
        /// </summary>
        /// <param name="errorcode"></param>
        /// <returns></returns>
        internal string GetErrorDescription(string errorcode)
        {
            string errordesc = "";
            try
            {
                table = dh.GetErrorDescription(errorcode);
                if (table.Rows.Count > 0)
                {
                    errordesc = table.Rows[0]["ErrorDescription"].ToString();
                }
            }
            catch (Exception ex)
            {
                LogError("GetErrorDescription", "AppLogic", ex.Message);
            }
            return errordesc;
        }

        /// <summary>
        /// Querys utility customers locally
        /// </summary>
        /// <param name="customerRef"></param>
        /// <returns></returns>
        internal UtilityCustomer GetUtilityCustomerLocally(string customerRef)
        {
            UtilityCustomer customer = new UtilityCustomer();
            try
            {
                DataTable data = dh.GetUtilityCustomerlocally(customerRef);
                if (data.Rows.Count > 0)
                {
                    customer.CustomerName = data.Rows[0]["CustomerName"].ToString();
                    customer.CustomerRef = customerRef;
                    customer.Utility = data.Rows[0]["Utility"].ToString();
                    customer.errorCode = "0";
                }
                else
                {
                    customer.errorCode = "4";
                }
                customer.errorMsg = GetErrorDescription(customer.errorCode);
            }
            catch (Exception ex)
            {
                LogError("GetUtilityCustomerLocally", "AppLogic", ex.Message);
                customer.errorCode = "1000";
                customer.errorMsg = GetErrorDescription(customer.errorCode);
            }
            return customer;
        }
    }
}