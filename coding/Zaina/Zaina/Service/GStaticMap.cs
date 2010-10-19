using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Zaina
{
    class GStaticMap
    {
        const int MinZoomLevel = 0;
        const int MaxZoomLevel = 21;
        const int DefaultZoomLevel = 16;

        bool HasInit = false;
        int m_currentZoomLevel = DefaultZoomLevel;
        DateTime m_currentTime;
        double m_lat;
        double m_lng;

        public bool ShowSatellite
        {
            get;set;
        }

        
        public GStaticMap()
        {
            m_currentTime = System.DateTime.Now;
        }

        public string GenMap(double lat, double lng)
        {
            m_lat = lat;
            m_lng = lng;
            HasInit = true;

            return RebuildMap();
        }

        public string RebuildMap()
        {
            if (!HasInit)
                return "";

            string url = GenMapUrl(m_lat, m_lng, m_currentZoomLevel, ShowSatellite);
            string fileName = GenCacheFileName(m_lat, m_lng, ShowSatellite, m_currentZoomLevel);
            
            // 如果之前下载过就用之前的
            if (IsCorrectPictureFormat(fileName))
                return fileName;
            else
                return DownloadMap(url, fileName);
        }

        public void ZoomIn()
        {
            int newZoomLevel = m_currentZoomLevel + 1;
            if (newZoomLevel >= MinZoomLevel && newZoomLevel <= MaxZoomLevel)
                m_currentZoomLevel = newZoomLevel;
        }

        public void ZoomOut()
        {
            int newZoomLevel = m_currentZoomLevel - 1;
            if (newZoomLevel >= MinZoomLevel && newZoomLevel <= MaxZoomLevel)
                m_currentZoomLevel = newZoomLevel;
        }

        public static string GetCacheDir()
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            path += "\\cache\\";

            return path;
        }

        protected string GenCacheFileName(double lat, double lng, bool satellite, int zoomLevel)
        {
            string path = GetCacheDir();
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            //path += currentTime.ToString("yyyyMMdd_HHmmss");
            path += lat.ToString();
            path += "_";
            path += lng.ToString();
            path += "_";
            path += zoomLevel.ToString();
            path += "_";
            path += satellite ? "s" : "r";
            path += ".gif";

            return path;
        }

        protected string GenMapUrl(double lat, double lng, int zoomLevel, bool satellite)
        {
            if (zoomLevel >= MinZoomLevel && zoomLevel <= MaxZoomLevel)
                m_currentZoomLevel = zoomLevel;

            string url = "http://maps.google.com/staticmap?&key=lyh&maptype=";
            url += (satellite ? "satellite" : "roadmap");
            url += "&markers=";
            url += m_lat.ToString().Replace(',', '.');
            url += ",";
            url += m_lng.ToString().Replace(',', '.');
            url += "&center=,&size=480x500&zoom=";
            url += m_currentZoomLevel.ToString();
            url += "&superreflash=" + System.DateTime.Now.ToString("yyyyMMdd_hhmmss");
            
            return url;
        }

        protected string DownloadMap(string url, string fileName)
        {
            try
            {
                // 最多进行3次
                int maxRetryTimes = 3;
                for (int i = 0; i < maxRetryTimes; i++)
                {
                    Downloader.SavePhotoFromUrl(fileName, url);
                    if (IsCorrectPictureFormat(fileName))
                    {
                        return fileName;
                    }
                }

                return "";
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return "";
            }
        }

        protected bool IsCorrectPictureFormat(string fileName)
        {
            if (!File.Exists(fileName))
                return false;

            StreamReader sr = new StreamReader(fileName);
            String line = sr.ReadLine();
            if (line == null)
                return false;
            
            return line.StartsWith("GIF");
        }
    }
}
