using System;
using System.Web.Script.Serialization;

namespace RST
{
    public class BposRequests
    {
        public static string NextRRN(string lastRRN)
        {
            if (String.Equals(lastRRN, Variables.requestStateError)) return Variables.requestStateError;

            int nextRRNint = Convert.ToInt32(lastRRN.Remove(0, 11)) + 1;
            string nextRRN = nextRRNint.ToString();

            for (int i = nextRRNint.ToString().Length; i < 5; i++)
                nextRRN = "0" + nextRRN;

            return lastRRN.Remove(5, 11) + String.Format("{0:yyMMdd}", DateTime.Now) + nextRRN;
        }

        public static string AuthRequest(string processingServiceName, string terminalId, string terminalPassword)
        {
            string cardNum = string.Empty;
            string clientType = "ProcessingTest";
            string clientVersion = "1.0.0.0";
            string employeeId = "Pinger";
            string hardwareID = AdditionalFunc.GetMACAddress();
            int isOffline = 0;
            string requestId = "0000000000000000";
            string shiftId = string.Empty;
            string type = "AUTH";
            string ver = "0.1";
            string sign = AdditionalFunc.StringToMD5("cardNum=" + cardNum + ";clientType=" + clientType + ";clientVersion=" + clientVersion + ";employeeId=" + employeeId + ";hardwareID=" + hardwareID 
                + ";isOffline=" + isOffline + ";requestId=" + requestId + ";shiftId=" + shiftId + ";terminalId=" + terminalId + ";terminalPassword=" + terminalPassword + ";type=" + type + ";ver=" + ver + ";" + hardwareID);

            string data = "{\"authRequest\":{\"request\":{\"ver\":\"" + ver + "\",\"clientType\":\"" + clientType + "\",\"clientVersion\":\"" + clientVersion + "\",\"type\":\"" + type + "\"," +
                "\"requestId\":\"" + requestId + "\",\"terminalId\":\"" + terminalId + "\",\"requestDate\":\"" + String.Format("{0:yyyy-MM-ddTHH:mm:ss}", DateTime.Now) + "Z\",\"employeeId\":\"" + employeeId +"\","
                +"\"cardNum\":\"\",\"isOffline\":" + isOffline + ",\"shiftId\":\"" + shiftId + "\",\"sign\":\"" + sign + "\"},\"auth\":{\"terminalPassword\":\"" + terminalPassword + "\",\"hardwareID\":\"" 
                + hardwareID + "\"}}}";

            string req_result = HTTPRequests.HTTPRequest(Variables.post, processingServiceName + "bpsApi/bpos/auth", data);

            if (String.Equals(req_result, Variables.requestStateError))
                return Variables.requestStateError;
            else
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                BPSResp resp = js.Deserialize<BPSResp>(req_result);

                if (String.Equals(resp.authResponse.response.requestState, Variables.requestStateError))
                    return Variables.requestStateError;
                else
                    return resp.authResponse.response.requestState;
            }
        }

        public static string RRN(string cardNum, string processingServiceName, string terminalId)
        {
            string clientType = "ProcessingTest";
            string clientVersion = "1.0.0.0";
            string employeeId = "Pinger";
            string hardwareID = AdditionalFunc.GetMACAddress();
            int isOffline = 0;
            string requestId = "0000000000000000";
            string shiftId = string.Empty;
            string type = "LAST_RRN";
            string ver = "0.1";
            string sign = AdditionalFunc.StringToMD5("cardNum=" + cardNum + ";clientType=" + clientType + ";clientVersion=" + clientVersion + ";employeeId=" + employeeId 
                + ";isOffline=" + isOffline + ";requestId=" + requestId + ";shiftId=" + shiftId + ";terminalId=" + terminalId + ";type=" + type + ";ver=" + ver + ";" + hardwareID);

            string data = "{\"getLastRrnRequest\":{\"request\":{\"ver\":\"" + ver + "\",\"clientType\":\"" + clientType + "\",\"clientVersion\":\"" + clientVersion + "\",\"type\":\"" + type + "\"," +
                "\"requestId\":\"" + requestId + "\",\"terminalId\":\"" + terminalId + "\",\"requestDate\":\"" + String.Format("{0:yyyy-MM-ddTHH:mm:ss}", DateTime.Now) + "Z\"," +
                "\"employeeId\":\"" + employeeId + "\",\"cardNum\":\"" + cardNum + "\",\"isOffline\":" + isOffline + ",\"shiftId\":\"" + shiftId + "\",\"sign\":\"" + sign + "\"}}}";

            string req_result = HTTPRequests.HTTPRequest(Variables.post, processingServiceName + "bpsApi/bpos/rrn", data);

            if (String.Equals(req_result, Variables.requestStateError))
                return Variables.requestStateError;
            else
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                BPSResp resp = js.Deserialize<BPSResp>(req_result);

                if (String.Equals(resp.getLastRrnResponse.response.requestState, Variables.requestStateError))
                    return Variables.requestStateError;
                else
                    return resp.getLastRrnResponse.rrn.rrNumber;
            }
        }

        public static string BalanceRequest(string cardNum, string processingServiceName, string terminalId, string requestId)
        {
            string clientType = "ProcessingTest";
            string clientVersion = "1.0.0.0";
            string employeeId = "Pinger";
            string hardwareID = AdditionalFunc.GetMACAddress();
            int isOffline = 0;
            string shiftId = string.Empty;
            string type = "BALANCE";
            string ver = "0.1";

            /*string authRes = AuthRequest(processingServiceName, terminalId, "80626056");
            if (String.Equals(authRes, Variables.errorResponce))
                return Variables.errorResponce;*/

            /*string requestId = NextRRN(RRN(cardNum, processingServiceName, terminalId));
        
            if (String.Equals(requestId, Variables.errorResponce))
                return Variables.errorResponce;*/

            string sign = AdditionalFunc.StringToMD5("cardNum=" + cardNum + ";clientType=" + clientType + ";clientVersion=" + clientVersion + ";employeeId=" + employeeId 
                + ";isOffline=" + isOffline + ";requestId=" + requestId + ";shiftId=" + shiftId + ";terminalId=" + terminalId + ";type=" + type + ";ver=" + ver + ";" + hardwareID);        

            string data = "{\"getBalanceRequest\":{\"request\":{\"ver\":\"" + ver + "\",\"clientType\":\"" + clientType + "\",\"clientVersion\":\"" + clientVersion + "\",\"type\":\"" + type + "\"," +
                "\"requestId\":\"" + requestId + "\",\"terminalId\":\"" + terminalId + "\",\"requestDate\":\"" + String.Format("{0:yyyy-MM-ddTHH:mm:ss}", DateTime.Now) + "Z\"," +
                "\"employeeId\":\"" + employeeId + "\",\"cardNum\":\"" + cardNum + "\",\"isOffline\":" + isOffline + ",\"shiftId\":\"" + shiftId + "\",\"sign\":\"" + sign + "\"}}}";

            string req_result = HTTPRequests.HTTPRequest(Variables.post, processingServiceName + "bpsApi/bpos/balance", data);

            if (String.Equals(req_result, Variables.requestStateError))
                return Variables.requestStateError;
            else
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                BPSResp resp = js.Deserialize<BPSResp>(req_result);

                if (String.Equals(resp.getBalanceResponse.response.requestState, Variables.requestStateError) & !String.Equals(resp.getBalanceResponse.response.requestStateCode, Variables.incorrectRequestId))
                    return Variables.requestStateError;
                else
                    return resp.getBalanceResponse.response.requestStateCode;
            }
        }
    }

    public class BPSResp
    {
        public getLastRrnResponse getLastRrnResponse { get; set; }
        public getBalanceResponse getBalanceResponse { get; set; }
        public authResponse authResponse { get; set; }
    }

    public class authResponse
    {
        public response response { get; set; } 
    }

    public class getBalanceResponse
    {
        public response response { get; set; }
        public balance balance { get; set; }
    }

    public class getLastRrnResponse
    {
        public response response { get; set; }
        public rrn rrn { get; set; }
    }

    public class response
    {
        public string requestState { get; set; }
        public string requestStateCode { get; set; }
        public string responseId { get; set; }
    }

    public class balance
    {
        public string percentRate { get; set; }
    }

    public class rrn
    {
        public string rrNumber { get; set; }
    }
}
