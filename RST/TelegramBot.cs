using System;
using System.Text;

namespace RST
{
    public class TelegramBot
    {
        public static string SendTelegram(TelegramParams TelegramParams, string text)
        {
            Encoding enc = Encoding.GetEncoding("UTF-8");
            string strASCII = string.Empty;

            byte[] bytes = enc.GetBytes(text);
            foreach (var byt in bytes)
            {
                strASCII = strASCII + String.Format("%{0:X2}", byt);
            }

            text = strASCII;

            if (TelegramParams.Socks5Proxy != null)
                return Socks5.SendViaSocket5(TelegramParams, text);
            else
                return HTTPRequests.HTTPRequest(TelegramParams, "GET", "https://api.telegram.org/bot426158475:AAHwCjhlnvNOXlaanVy5Tja3W5h6QySxK_g/sendMessage?chat_id=" 
                    + TelegramParams.ChatId + "&text=" + text, String.Empty);
        }
    }
}
