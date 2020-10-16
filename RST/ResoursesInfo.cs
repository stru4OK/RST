using System;
using System.Diagnostics;

namespace RST
{
    public static class ResoursesInfo
    {
        public static Resources GetResourseInfo(Applications Applications)
        {
            string sucReqElectCard = String.Empty;
            string pingResponse = String.Empty;
            ProcessingResult ProcessingResult = new ProcessingResult();
            Stopwatch ResponsePing = new Stopwatch();

            ResponsePing.Start();
            string[] version = HTTPRequests.GetVersion(Applications.Address);
            ResponsePing.Stop();

            if (!String.Equals(version[0], Variables.offline))
                pingResponse = Math.Round(Convert.ToDecimal(ResponsePing.ElapsedMilliseconds) / 1000, 3).ToString();

            switch (Applications.AppType)
            {
                case Variables.processingType:
                    ProcessingResult = Processing.ProcessingTest(Applications.DataSource, Applications.Address, Applications.TerminalId, Applications.TerminalPassword, Applications.requestId, Applications.cardNum);
                    pingResponse = ProcessingResult.Ping;

                    if (String.Equals(ProcessingResult.Error, Variables.requestStateError))
                    {
                        pingResponse = Variables.error;
                        version[0] = Variables.offline;
                    }

                    sucReqElectCard = AdditionalFunc.DataBaseSQL(Applications.DataSource, "select count(*) as data from requests "
                        + "where trunc(ins_date) = trunc(sysdate) "
                        + "and(request_type = 'PAYMENT_AND_CONFIRM' or request_type = 'DEPOST' or "
                        + "request_type = 'PAYMENT' or request_type = 'CANCEL' or "
                        + "request_type = 'PAYMENT_CONFIRM') "
                        + "and (request_state = 'PROCESSED' or request_state = 'READY' or request_state = 'SUCCESS')", true);
                    break;
                case Variables.mobileType:
                    sucReqElectCard = AdditionalFunc.DataBaseSQL(Applications.DataSource, "select count(*) as data from card_emission_units where state = 'IN_STOCK'", true);
                    break;
                default:
                    break;
            }

            return new Resources
            {
                address = "Адрес ресурса: " + Applications.Address,
                name = Applications.Name,
                status = version[0],
                version = version[1],
                sucReqElectCard = sucReqElectCard,
                responsePing = pingResponse,
                requestId = ProcessingResult.requestId,
                cardNum = ProcessingResult.cardNum
            };
        }
    }
}