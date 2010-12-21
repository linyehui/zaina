using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace Zaina
{
    class Weibo
    {
        const string API_URL = @"http://api.t.sina.com.cn/statuses/update.json";
        const int MAX_BYTE_NUM = 140 * 9;   // 140个汉字的ByteCount

        public bool CheckString(string msg)
        {
            int len = msg.Length;
            int byteCount = System.Text.Encoding.UTF8.GetByteCount(Uri.EscapeDataString(msg));
            if (MAX_BYTE_NUM < byteCount)
                return false;
            return true;
        }

        public bool Update(string username, string password, string msg)
        {
            if (!CheckString(msg))
                return false;

            try
            {
                string usernamePassword = username + ":" + password;
                CredentialCache mycache = new CredentialCache();
                mycache.Add(new Uri(API_URL), "Basic", new NetworkCredential(username, password));

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(API_URL);
                request.Method = "POST";
                request.Credentials = mycache;
                request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(new ASCIIEncoding().GetBytes(usernamePassword)));

                StringWriter StrWriter = new StringWriter();
                StrWriter.Write("source=" + AppKey.SINA_WEIBO_APP_KEY);
                StrWriter.Write("&status=" + Uri.EscapeDataString(msg));

                Encoding encode = System.Text.Encoding.UTF8;
                byte[] byteArray = encode.GetBytes(StrWriter.ToString());
                StrWriter.Close();

                M8Helper.Network.InitMeizuNewwork(request);
                System.Net.ServicePointManager.Expect100Continue = false;
                request.AllowWriteStreamBuffering = true;

                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteArray.Length;
                Stream postStream = request.GetRequestStream();
                postStream.Write(byteArray, 0, byteArray.Length);
                postStream.Close();

                // Get the response.
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Console.WriteLine("[I] Request response: {0}", response.StatusDescription);

                // Read response
                Stream dataStream = response.GetResponseStream();
                StreamReader readStream = new StreamReader(dataStream, Encoding.ASCII);
                string result = readStream.ReadToEnd();
                readStream.Close();
                dataStream.Close();
                response.Close();
                
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
