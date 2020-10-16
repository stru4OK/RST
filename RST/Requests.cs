using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace RST
{
    public class HTTPRequests
    {
        public static string HTTPRequest(string typeRequest, string request, string data)
        {
            return HTTPRequest(null, typeRequest, request, data);
        }

        public static string HTTPRequest(TelegramParams TelegramParams, string typeRequest, string request, string data)
        {
            //AdditionalFunc.Trace("Request: \n" + request + "\n\n" + "Data:\n" + data + "\n\n");

            try
            {
                ServicePointManager.Expect100Continue = true;

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                CredentialCache cache = new CredentialCache();
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(request);

                if(TelegramParams!=null)
                {
                    req.Proxy = new WebProxy(TelegramParams.Proxy.IP, TelegramParams.Proxy.Port);
                    //req.Proxy = new WebProxy("http://" + TelegramParams.Proxy.IP + ":" + TelegramParams.Proxy.Port + "/", true);

                    if (!String.IsNullOrEmpty(TelegramParams.Proxy.Login) & !String.IsNullOrEmpty(TelegramParams.Proxy.Password))
                        req.Proxy.Credentials = new NetworkCredential(TelegramParams.Proxy.Login, TelegramParams.Proxy.Password);
                }
  
                //Игнорируем недостоверный сертификат SSL
                ServicePointManager.ServerCertificateValidationCallback += delegate (object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                        System.Security.Cryptography.X509Certificates.X509Chain chain,
                        System.Net.Security.SslPolicyErrors sslPolicyErrors)
                {
                    return true;
                };

                req.KeepAlive = false;
                req.PreAuthenticate = true;
                req.Method = typeRequest;
                req.Timeout = 10000;

                if (!String.Equals(typeRequest, Variables.get))
                {
                    byte[] byteArray = Encoding.UTF8.GetBytes(data);
                    req.ContentType = "application/json";
                    req.ContentLength = byteArray.Length;
                    Stream dataStreamReq = req.GetRequestStream();
                    dataStreamReq.Write(byteArray, 0, byteArray.Length);
                    dataStreamReq.Close();
                }

                WebResponse response = req.GetResponse();
                Stream dataStreamResp = response.GetResponseStream();

                StreamReader reader = new StreamReader(dataStreamResp, Encoding.UTF8);
                string responseFromServer = reader.ReadToEnd();
                reader.Close();
                dataStreamResp.Close();
                response.Close();

                //AdditionalFunc.Trace("Responce: \n" + responseFromServer + "\n\n\n");
                return responseFromServer;
            }
            catch (Exception ex)
            {
                AdditionalFunc.Trace("Request: \n" + request + "\n\n" + "Data:\n" + data);
                AdditionalFunc.Trace("Responce: \n" + ex.ToString());
                return Variables.requestStateError;
            }
        }

        public static string[] GetVersion(string URL)
        {
            string s = HTTPRequest(Variables.get, @URL, String.Empty);
            if (String.Equals(s, Variables.requestStateError))
                return new[] { Variables.offline, String.Empty };

            string[] array = { String.Empty, String.Empty };

            MatchCollection matches = Regex.Matches(s, @"(\s|>)\d+\.\d+\.\d+", RegexOptions.None);
            foreach (Match m in matches)
            {
                array[0] = Variables.online;
                array[1] = m.ToString().Remove(0, 1);
                return array;
            }

            array[0] = Variables.online;
            array[1] = Variables.notAvailable;
            return array;
        }
    }
}