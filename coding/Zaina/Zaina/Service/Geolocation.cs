using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;
using System.Diagnostics;
using MeizuSDK.Core;

class CGeolocation
{
    public class CellIDData
    {
        public int cellid;
        public int lac;
        public int mcc;
        public int mnc;
    }

    private static CellIDData GetCellIDData()
    {
        string[] cellidFields = RIL.GetCellTowerInfo().ToString().Split('-');
        string[] args ={
                              cellidFields[2],
                              "0",
                              cellidFields[1],
                              cellidFields[0]
                          };

        CellIDData cid = new CGeolocation.CellIDData();
        cid.mcc = Convert.ToInt32(cellidFields[2]);
        cid.mnc = Convert.ToInt32("0");
        cid.lac = Convert.ToInt32(cellidFields[1]);
        cid.cellid = Convert.ToInt32(cellidFields[0]);

        return cid;
    }

    private static bool GetGearsAPI_Value(string Data, string TagName, out double value)
    {
        value = 0;

        int beginPos, endPos;
        beginPos = Data.IndexOf(TagName);
        if (beginPos == -1)
            return false;
        endPos = Data.IndexOf(',', beginPos);

        value = double.Parse(Data.Substring(beginPos + TagName.Length,
                                            endPos - beginPos - TagName.Length));
        return true;
    }

    public static bool locate_GoogleGearsAPI(out double Lat, out double Lng)
    {
        try
        {
            CellIDData cid = GetCellIDData();
            HttpWebRequest request =
            (HttpWebRequest)WebRequest.Create("http://www.google.com/loc/json");
            request.Method = "POST";

            StringWriter StrWriter = new StringWriter();
            StrWriter.Write("{\n");
            StrWriter.Write("  \"version\": \"1.1.0\",\n");
            StrWriter.Write("  \"host\": \"maps.google.com\",\n");
            StrWriter.Write("  \"cell_towers\": [\n");
            StrWriter.Write("    {\n");
            StrWriter.Write("       \"cell_id\": {0},\n", cid.cellid.ToString());
            StrWriter.Write("       \"location_area_code\": {0},\n", cid.lac.ToString());
            StrWriter.Write("       \"mobile_country_code\": {0},\n", cid.mcc.ToString());
            StrWriter.Write("       \"mobile_network_code\": {0}\n", cid.mnc.ToString());
            StrWriter.Write("    }\n");
            StrWriter.Write("  ]\n");
            StrWriter.Write("}");

            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] byteArray = encoding.GetBytes(StrWriter.ToString());
            StrWriter.Close();

            M8Helper.Network.InitMeizuNewwork(request);
            
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

            if (GetGearsAPI_Value(result, "\"latitude\":", out Lat)
            && GetGearsAPI_Value(result, "\"longitude\":", out Lng))
                return true;

            Lat = 0;
            Lng = 0;
            return false;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            Lat = 0;
            Lng = 0;
            return false;
        }
    }

    public static bool getLocations_GoogleGearsAPI(double Lat, double Lng, out string address)
    {
        try
        {
            string url = "http://ditu.google.cn/maps/geo?output=csv&hl=zh-CN&key=lyh&q=";
            //string url = "http://ditu.google.cn/maps/geo?output=csv&hl=en-US&key=lyh&q=";
            url += Lat.ToString();
            url += ",";
            url += Lng.ToString();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            M8Helper.Network.InitMeizuNewwork(request);

            // Get the response.
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Console.WriteLine("[I] Request response: {0}", response.StatusDescription);

            // Read response
            Stream dataStream = response.GetResponseStream();
            StreamReader readStream = new StreamReader(dataStream, Encoding.UTF8);
            string result = readStream.ReadToEnd();

            readStream.Close();
            dataStream.Close();
            response.Close();

            int npos = result.IndexOf('"');
            if (-1 == npos)
            {
                address = result;
                return false;
            }

            address = result.Substring(npos + 1, result.Length - npos - 1 - 1);
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            address = "";
            return false;
        }
    }
}