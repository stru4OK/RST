using Oracle.ManagedDataAccess.Client;
using System;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace RST
{
    public class AdditionalFunc
    {
        public static string DataBaseSQL(string oracleDBConnection, string sql, bool needResult)
        {
            DBResult DBResult = DBSQL(oracleDBConnection, sql, needResult);

            while (string.Equals(DBResult.state, Variables.ERROR))
            {
                Thread.Sleep(3000);
                DBResult = DBSQL(oracleDBConnection, sql, needResult);
            }

            return DBResult.result;
        }

        public static DBResult DBSQL(string oracleDBConnection, string sql, bool needResult)
        {
            Trace(sql);
            DBResult DBResult = new DBResult();

            DBResult.result = String.Empty;
            DBResult.state = Variables.SUCCESS;

            OracleConnection conn = new OracleConnection(oracleDBConnection);

            try
            {
                conn.Open();
                OracleCommand dbcmd = conn.CreateCommand();
                dbcmd.CommandText = sql;
                OracleDataReader reader = dbcmd.ExecuteReader();

                if (needResult)
                {
                    while (reader.Read())
                    {
                        DBResult.result = (string)reader["data"].ToString();
                        Trace("Result: " + DBResult.result);
                    }
                }

                reader.Close();
                reader = null;

                return DBResult;
            }
            catch (Exception ex)
            {
                Trace(ex.ToString());
                DBResult.state = Variables.ERROR;

                return DBResult;
            }
            finally
            {
                OracleConnection.ClearPool(conn);
                conn.Dispose();
                conn.Close();
                conn = null;
            }
        }

        public static void Trace(string text)
        {
            string TracePath = "Trace.txt";
            File.AppendAllText(TracePath, DateTime.Now.ToString("HH:mm:ss") + "\n" + text + "\n\n");
            FileInfo file = new FileInfo(TracePath);
            if (file.Length > 10000000) file.Delete();
        }

        public static string[] GetStatus(string FlagFilePath, string[] SleepTime)
        {
            string[] SleepTimeList = new string[SleepTime.Length];
            bool SleepTimeB = false;
            string SleepTimeS = String.Empty;

            string[] result = { "RST v." + Assembly.GetExecutingAssembly().GetName().Version.ToString(), Variables.sendMessage, Variables.stableColor, String.Empty };

            SleepTimeList = SleepTime;

            for (int i = 0; i < SleepTimeList.Length; i++)
            {
                if (DateTime.Now.ToString("HH") == SleepTimeList[i])
                {
                    SleepTimeS = SleepTimeList[i];
                    SleepTimeB = true;
                }
            }

            if (File.Exists(FlagFilePath) & SleepTimeB)
            {
                if ((DateTime.Now - File.GetCreationTime(FlagFilePath)).TotalHours >= 3)
                {
                    result[0] = result[0] + " Флагу " + FlagFilePath + " больше 3ч. Слиптайм " + SleepTimeS;
                    result[2] = Variables.attentionColor;
                }
                else
                {
                    result[0] = result[0] + " Флаг " + FlagFilePath + " Слиптайм " + SleepTimeS;
                }
                result[1] = Variables.notSendSMS;
            }
            else if (!(File.Exists(FlagFilePath)) & SleepTimeB)
            {
                result[0] = result[0] + " Слиптайм " + SleepTimeS;
                result[1] = Variables.notSendSMS;
            }
            else if (File.Exists(FlagFilePath) & !SleepTimeB)
            {
                if ((DateTime.Now - File.GetCreationTime(FlagFilePath)).TotalHours >= 3)
                {
                    result[0] = result[0] + " Флагу " + FlagFilePath + " больше 3ч. " + SleepTimeS;
                    result[2] = Variables.attentionColor;
                    result[3] = "Флаг " + FlagFilePath + " найден - но старше 3 часов.\n";
                }
                else
                {
                    result[0] = result[0] + " Флаг " + FlagFilePath;
                    result[1] = Variables.notSendSMS;
                }
            }

            return result;
        }

        public static string SendSMS(string phone, string sender_name, string message)
        {
            try
            {
                Encoding enc = Encoding.GetEncoding("UTF-8");

                string timestamp = HTTPRequests.HTTPRequest(Variables.get, "http://enter.mirsms.ru/external/get/timestamp.php", String.Empty);
                //string timestamp = (string)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds.ToString("#");
                string strASCII = string.Empty;
                string signature = StringToMD5(Variables.mirSMSLogin + phone + sender_name + message + timestamp + Variables.mirSMSAPIKey);

                byte[] bytes = enc.GetBytes(message);
                foreach (var byt in bytes)
                {
                    strASCII = strASCII + String.Format("%{0:X2}", byt);
                }

                string response = HTTPRequests.HTTPRequest(Variables.get, "http://enter.mirsms.ru/external/get/send.php?login=" + Variables.mirSMSLogin + "&signature=" + signature + "&phone=" + phone + "&text=" + strASCII + "&sender="
                    + sender_name + "&timestamp=" + timestamp + "", String.Empty);

                return response;
            }
            catch (Exception ex)
            {
                Trace("SMS send error: \n" + ex.ToString() + "\n\n\n");
                return String.Empty;
            }
        }

        public static string StringToMD5(string stringToMD5)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            byte[] hash = Encoding.UTF8.GetBytes(stringToMD5);
            byte[] hashenc = md5.ComputeHash(hash);
            string result = String.Empty;

            foreach (var b in hashenc)
            {
                result += b.ToString("x2");
            }

            return result;
        }

        public static string GetMACAddress()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            String sMacAddress = string.Empty;
            foreach (NetworkInterface adapter in nics)
            {
                // only return MAC Address from first card  
                if (sMacAddress == String.Empty)
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    sMacAddress = string.Join(":", (from z in adapter.GetPhysicalAddress().GetAddressBytes() select z.ToString("X2")).ToArray());
                }
            }
            return sMacAddress;
        }
    }
}