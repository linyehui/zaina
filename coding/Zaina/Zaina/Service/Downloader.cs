using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace Zaina
{
    class Downloader
    {
        /// <summary>
        /// 从图片地址下载图片到本地磁盘
        /// </summary>
        /// <param name="ToLocalPath">图片本地磁盘地址</param>
        /// <param name="Url">图片网址</param>
        /// <returns></returns>
        public static bool SavePhotoFromUrl(string FileName, string Url)
        {
            WebResponse response = null;
            Stream stream = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                M8Helper.Network.InitMeizuNewwork(request);

                response = request.GetResponse();
                stream = response.GetResponseStream();

                if (!response.ContentType.ToLower().StartsWith("text/"))
                {
                    return SaveBinaryFile(response, FileName);
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("下载失败：" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Save a binary file to disk.
        /// </summary>
        /// <param name="response">The response used to save the file</param>
        // 将二进制文件保存到磁盘
        private static bool SaveBinaryFile(WebResponse response, string FileName)
        {
            bool Value = true;
            byte[] buffer = new byte[1024];

            try
            {
                if (File.Exists(FileName))
                    File.Delete(FileName);
                Stream outStream = System.IO.File.Create(FileName);
                Stream inStream = response.GetResponseStream();

                int l;
                do
                {
                    l = inStream.Read(buffer, 0, buffer.Length);
                    if (l > 0)
                        outStream.Write(buffer, 0, l);
                }
                while (l > 0);

                outStream.Close();
                inStream.Close();
            }
            catch
            {
                Value = false;
            }
            return Value;
        }
    }
}
