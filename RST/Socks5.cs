using System;
using xNet;

namespace RST
{
    public class Socks5
    {
        public static string SendViaSocket5(TelegramParams TelegramParams, string text)
        {
            try
            {
                using (var request = new HttpRequest())
                {
                    request.Proxy = Socks5ProxyClient.Parse(TelegramParams.Socks5Proxy.IP +":" + TelegramParams.Socks5Proxy.Port);

                    if(!String.IsNullOrEmpty(TelegramParams.Socks5Proxy.Login))
                    {
                        request.Proxy.Username = TelegramParams.Socks5Proxy.Login;
                        request.Proxy.Password = TelegramParams.Socks5Proxy.Password;
                    }

                    return request.Get("https://api.telegram.org/bot426158475:AAHwCjhlnvNOXlaanVy5Tja3W5h6QySxK_g/sendMessage?chat_id=" + TelegramParams.ChatId + "&text=" + text).ToString();
                }
            }

            catch (HttpException ex)
            {
                return Variables.requestStateError;
            }
        }
    }
}
