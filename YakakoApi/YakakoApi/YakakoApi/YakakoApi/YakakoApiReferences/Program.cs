using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YakakoApiReferences.Models;

namespace YakakoApiReferences
{
    public class Program
    {
        private static ezeemoneyapi.API api = new ezeemoneyapi.API();
        private const string SCid = "17614786";
        private const string userid = "4295422394";
        private const string password = "6F16033040";

        static void Main(string[] args)
        {
        }
        public ValidateMeterResponse ValidateMeter(string meterno)
        {
            ValidateMeterResponse resp = new ValidateMeterResponse();
            try
            {
                string respcode = "";  string info4 = "";
                string respmsg = "";   string info5 = "";
                string info1 = "";
                string info2 = "";
                string info3 = "";
                string accountno = meterno;
                string respstatus = api.ValidateBillAccount(SCid, userid, password, sctxnid: DateTime.Now.ToString("yyyyMMddhhmmss"), payeecode: "UMEME", accountno, remark1: "",
                    remark2: "",out respcode,out respmsg,out info1,out info2,out info3,out info4,out info5);
                resp.respcode = respcode;
                resp.info4 = info4;
                resp.respmsg = respmsg;
                resp.info5 = info5;
                resp.info1 = info1;
                resp.info2 = info2;
                resp.info3 = info3;
            }
            catch (Exception ex)
            {
                resp.respcode = "100";
                resp.respmsg = ex.Message;
            }
            return resp;
        }

        public void PayBill(string paymentDate,string amount,string metername,string phoneno,string meterno)
        {
            try
            {
                string respcode = ""; string info4 = "";
                string respmsg = ""; string info5 = "";
                string info1 = ""; string emtxid = "";
                string info2 = ""; string txnfee = "";
                string info3 = ""; string receiptno = "";
                string respstatus = api.PayBill(SCid, userid, password, paymentDate, txntype: "1", payeecode: "UMEME",
                    meterno, phoneno, amount, metername, remark2: "", out respcode,
                    out respmsg, out emtxid, out txnfee, out receiptno, out info1, out info2, out info3, out info4, out info5);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
