using System;
using System.Diagnostics;

namespace RST
{
    public class Processing
    {
        public static ProcessingResult ProcessingTest(string dataSource, string processingServiceName, string terminalId, string terminalPassword, string requestId, string cardNum)
        {
            ProcessingResult ProcessingResult = new ProcessingResult();
            Stopwatch ResponsePing = new Stopwatch();

            if (string.IsNullOrEmpty(cardNum))
            {
                cardNum = AdditionalFunc.DataBaseSQL(dataSource, "select card_num as data from cards where rownum = 1 and is_delete = 0 and is_locked = 0", true);

                if (string.IsNullOrEmpty(cardNum))
                {
                    ProcessingResult.Error = Variables.requestStateError;
                    return ProcessingResult;
                }
            }

            ProcessingResult.cardNum = cardNum;

            if (string.IsNullOrEmpty(requestId))
            {
                string authRes = BposRequests.AuthRequest(processingServiceName, terminalId, terminalPassword);

                if (string.Equals(authRes, Variables.requestStateError))
                {
                    ProcessingResult.Error = Variables.requestStateError;
                    return ProcessingResult;
                }

                requestId = BposRequests.RRN(cardNum, processingServiceName, terminalId);

                if (string.Equals(requestId, Variables.requestStateError))
                {
                    ProcessingResult.Error = Variables.requestStateError;
                    return ProcessingResult;
                }
            }

            ProcessingResult.requestId = BposRequests.NextRRN(requestId);
            
            ResponsePing.Start();
            string res = BposRequests.BalanceRequest(cardNum, processingServiceName, terminalId, ProcessingResult.requestId);
            ResponsePing.Stop();

            //string clearDB = AdditionalFunc.DataBaseSQL(dataSource, "delete from requests where employee_code='Pinger' and terminal_id = (select terminal_id from terminals where code='100')", false);

            if (String.Equals(res, Variables.requestStateError))
                ProcessingResult.Error = Variables.requestStateError;
            else
                ProcessingResult.Ping = Math.Round(Convert.ToDecimal(ResponsePing.ElapsedMilliseconds) / 1000, 3).ToString();

            return ProcessingResult;
        }
    }

    public class ProcessingResult
    {
        public string Ping { get; set; }
        public string requestId { get; set; }
        public string cardNum { get; set; }
        public string Error { get; set; }
    }
}
