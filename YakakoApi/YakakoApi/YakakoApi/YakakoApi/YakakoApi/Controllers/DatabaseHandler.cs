using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace YakakoApi
{
    public class DatabaseHandler
    {
        Database DbConnection;
        DbCommand command;
        DataTable table;
        public DatabaseHandler()
        {
            try
            {
                DbConnection = DatabaseFactory.CreateDatabase("testyassakodbconnectionstring");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        internal DataTable GetLoanBalance(string meter, string custTel)
        {
            try
            {
                command = DbConnection.GetStoredProcCommand("GetCustomerBalance", meter, custTel);
                table = DbConnection.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return table;
        }

        /// <summary>
        /// Get Customer Balance by phone
        /// </summary>
        /// <param name="meter"></param>
        /// <param name="custTel"></param>
        /// <returns></returns>
        internal DataTable GetLoanBalanceByPhone(string custTel)
        {
            try
            {
                command = DbConnection.GetStoredProcCommand("GetCustomerBalanceByMsisdn", custTel);
                table = DbConnection.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return table;
        }

        /// <summary>
        /// Gets the error description from the db
        /// </summary>
        /// <param name="errorcode"></param>
        /// <returns></returns>
        internal DataTable GetErrorDescription(string errorcode)
        {
            try
            {
                command = DbConnection.GetStoredProcCommand("GetErrorDescription", errorcode);
                table = DbConnection.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return table;
        }

        /// <summary>
        /// Logs Error in the database
        /// </summary>
        /// <param name="method"></param>
        /// <param name="Level"></param>
        /// <param name="error"></param>
        /// <param name="loggedby"></param>
        internal void LogError(string method,string Level,string error,string loggedby)
        {
            try
            {
                command = DbConnection.GetStoredProcCommand("LogSystemError", method, Level, error, loggedby);
                DbConnection.ExecuteNonQuery(command);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets vendorcredentials from the database
        /// </summary>
        /// <param name="vendorcode"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        internal DataTable GetVendorCredentials(string vendorcode, string password)
        {
            try
            {
                command = DbConnection.GetStoredProcCommand("GetVendorCredentials", vendorcode, password);
                table = DbConnection.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return table;
        }

        /// <summary>
        /// Inserts Loan
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="customerTel"></param>
        /// <param name="meterNo"></param>
        /// <param name="paymentDate"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        internal DataTable MakePayment(string transactionId, string customerTel, string meterNo,string custName, DateTime paymentDate, string amount,string vendor,string trantype,string utility)
        {
            try
            {
                command = DbConnection.GetStoredProcCommand("InsertLoanRequests", transactionId, customerTel, meterNo, custName, paymentDate, amount, vendor, trantype, utility);
                table = DbConnection.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return table;
        }

        /// <summary>
        /// Gets loan request by id and vendor from the db
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        internal DataTable GetTransactionDetails(string transactionId, string username)
        {
            try
            {
                command = DbConnection.GetStoredProcCommand("LoanRequestByIdAndVendor", transactionId, username);
                table = DbConnection.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return table;
        }

        internal DataTable GetWhitelistedNumber(string formatednumber)
        {
            try
            {
                command = DbConnection.GetStoredProcCommand("GetWhitelistedNumber", formatednumber);
                table = DbConnection.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return table;
        }

        internal DataTable GetLoanLimit(string meternumber)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Inserts Umeme Customers
        /// </summary>
        /// <param name="meterno"></param>
        /// <param name="customerName"></param>
        internal void InsertCustomer(string meterno,string customerName,string utilitycode)
        {
            try
            {
                command = DbConnection.GetStoredProcCommand("InsertCustomer", meterno, customerName, utilitycode);
                DbConnection.ExecuteNonQuery(command);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the saved utility customers from the database
        /// </summary>
        /// <param name="customerRef"></param>
        /// <returns></returns>
        internal DataTable GetUtilityCustomerlocally(string customerRef)
        {
            try
            {
                command = DbConnection.GetStoredProcCommand("GetUtilityCustomerLocally", customerRef);
                table = DbConnection.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return table;
        }

        /// <summary>
        /// Gets the last utility ref from the yakako platform
        /// </summary>
        /// <param name="meterNo"></param>
        /// <returns></returns>
        internal DataTable getLastToken(string phone)
        {
            try
            {
                command = DbConnection.GetStoredProcCommand("GetlastUtilityRefForCustomer", phone);
                table = DbConnection.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return table;
        }

        /// <summary>
        /// Gets loan using meter
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        internal DataTable GetMeterNoAttachedToPhone(string phone)
        {
            try
            {
                command = DbConnection.GetStoredProcCommand("GetMeterNoAttachedToPhone", phone);
                table = DbConnection.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return table;
        }

        internal void ClearLoan(string transactionId, string custTel, string lastloanid)
        {
            try
            {
                command = DbConnection.GetStoredProcCommand("ClearLastLoan", transactionId, custTel, lastloanid);
                DbConnection.ExecuteNonQuery(command); 
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets loan id
        /// </summary>
        /// <param name="customerTel"></param>
        /// <returns></returns>
        internal DataTable GetLoanId(string customerTel)
        {
            try
            {
                command = DbConnection.GetStoredProcCommand("GetLastLoanHist", customerTel);
                table = DbConnection.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return table;
        }

        /// <summary>
        /// Logs Loans in the db
        /// </summary>
        /// <param name="customerRef"></param>
        /// <param name="customerTel"></param>
        /// <param name="amount"></param>
        /// <param name="loanid"></param>
        internal void LogLoanHist(string customerRef, string customerTel, string amount, string loanid)
        {
            try
            {
                command = DbConnection.GetStoredProcCommand("LogLoan", customerRef, customerTel, amount, loanid);
                DbConnection.ExecuteNonQuery(command);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal DataTable GetLoanDetails(string loanid)
        {
            try
            {
                command = DbConnection.GetStoredProcCommand("GetLoanDetailsById", loanid);
                table = DbConnection.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return table;
        }

        internal DataTable GetMaximumAmountForCustomer(string msisdn)
        {
            try
            {
                command = DbConnection.GetStoredProcCommand("CalculateCreditScore", msisdn);
                table = DbConnection.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return table;
        }

        internal void LogRequests(string meterno, string customertel, string requesttype, string vendorcode)
        {
            try
            {
                command = DbConnection.GetStoredProcCommand("LogRequests", meterno, customertel, requesttype, vendorcode);
                DbConnection.ExecuteNonQuery(command);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}