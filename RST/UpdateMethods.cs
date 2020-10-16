using System;
using System.IO;
using System.Net;
using System.Windows;

namespace RST
{
    public class UpdateMethods
    {
        public static int Update()
        {
            WebClient client = new WebClient();

            string exeFile = "RST.exe";
            string exeFileURL = "http://192.168.4.150:90/UpdateUtils/rst/" + exeFile;

            if (RemoteFileExists(exeFileURL))
            {
                if (isNeedUpdateRemoteFile(exeFileURL, exeFile))
                {
                    if (File.Exists(exeFile + "_old"))
                        File.Delete(exeFile + "_old");

                    File.Move(exeFile, exeFile + "_old");

                    client.DownloadFile(exeFileURL, exeFile);

                    //MessageBox.Show("Обновление прошло успешно!\nТребуется рестарт приложения", "Сообщение");
                    return 0;
                }

                return 2;
            }
            else
            {
                MessageBox.Show("Проблемы с сервером обновлений!", "Ошибка");
                return 1;
            }
        }

        private static bool RemoteFileExists(string url)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "HEAD";
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                response.Close();
                return (response.StatusCode == HttpStatusCode.OK);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool isNeedUpdateRemoteFile(string fileUrl, string localFile)
        {
            try
            {
                HttpWebRequest file = (HttpWebRequest)WebRequest.Create(fileUrl);
                HttpWebResponse fileResponse = (HttpWebResponse)file.GetResponse();

                fileResponse.Close();

                DateTime localFileModifiedTime = File.GetLastWriteTime(localFile);
                DateTime onlineFileModifiedTime = fileResponse.LastModified;

                if (localFileModifiedTime < onlineFileModifiedTime)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
