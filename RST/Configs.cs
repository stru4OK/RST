using System;
using System.IO;
using System.Web.Script.Serialization;

namespace RST
{
    public class GetConfigs
    {
        public static ConfigResourceParameter ReadResourceConfig()
        {
            string line;
            ConfigResourceParameter ResourceData = new ConfigResourceParameter();

            try
            {
                StreamReader file = new StreamReader(@"ConfigResources.jsn");
                line = file.ReadToEnd();

                JavaScriptSerializer js = new JavaScriptSerializer();
                ConfigResourceParameter confPar = js.Deserialize<ConfigResourceParameter>(line);

                ResourceData.Applications = confPar.Applications;
                ResourceData.ExaminePause = confPar.ExaminePause;

                if (confPar.FlagFilePath != null)
                {
                    ResourceData.FlagFilePath = confPar.FlagFilePath;
                }

                if (confPar.SleepTime != null)
                {
                    ResourceData.SleepTime = confPar.SleepTime;
                }

                return ResourceData;
            }
            catch (Exception)
            {
                ResourceData.Error = "Ошибка ConfigResources.jsn";
                return ResourceData;
            }
        }

        public static ConfigNotificationParameter ReadNotifConfig()
        {
            string line;
            ConfigNotificationParameter NotifData = new ConfigNotificationParameter();

            try
            {
                StreamReader file = new StreamReader(@"ConfigNotifications.jsn");
                line = file.ReadToEnd();

                JavaScriptSerializer js = new JavaScriptSerializer();
                ConfigNotificationParameter confPar = js.Deserialize<ConfigNotificationParameter>(line);

                NotifData = confPar;

                return NotifData;
            }
            catch (Exception)
            {
                NotifData.Error = "Ошибка ConfigNotifications.jsn";
                return NotifData;
            }
        }
    }

    public class ConfigResourceParameter
    {
        private int _examinePause = 300;
        public Applications[] Applications { get; set; }
        public string FlagFilePath { get; set; }
        public int ExaminePause
        {
            get { return _examinePause; }
            set { _examinePause = value; }
        }
        public string[] SleepTime { get; set; }
        public string Error { get; set; }
    }

    public class ConfigNotificationParameter
    {
        public string Channel { get; set; }
        public TelegramParams TelegramParams { get; set; }
        public SMSParams SMSParams { get; set; }
        public string Error { get; set; }
    }

    public class TelegramParams
    {
        public string ChatId { get; set; }
        public Socks5Proxy Socks5Proxy { get; set; }
        public Socks5Proxy Proxy { get; set; }
    }

    public class SMSParams
    {
        public string SenderName { get; set; }
        public string[] MobileNotifications { get; set; }
    }

    public class Socks5Proxy
    {
        public string IP { get; set; }
        public int Port { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }

    public class Applications
    {
        private string _terminaldId = "100";
        private string _address = String.Empty;

        public string Name { get; set; }
        public string Address
        {
            get { return _address; }
            set
            {
                if (value.EndsWith("/"))
                    _address = value;
                else
                    _address = value + "/";
            }
        }
        public string TerminalId
        {
            get { return _terminaldId; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    _terminaldId = value;
            }
        }
        public string TerminalPassword { get; set; }
        public string DataSource { get; set; }
        public string AppType { get; set; }
        public string requestId { get; set; }
        public string cardNum { get; set; }
    }
}
