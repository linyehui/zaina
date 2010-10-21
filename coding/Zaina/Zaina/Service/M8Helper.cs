using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Diagnostics;
using MeizuSDK.Core;
using MeizuSDK.Presentation;

namespace M8Helper
{
    class Network
    {
        public static void InitMeizuNewwork(HttpWebRequest request)
        {
            if (!Networking.GprsConnect(GprsAppType.Default))
            {
                Debug.WriteLine(Networking.Status.ToString());
            }

            //MessageBox.Show("NetworkingStatus 2: " + Networking.Status.ToString());
            if (NetworkingStatus.EdgeProxy == Networking.Status)
            {
                //status = Networking.Status;
                //MessageBox.Show("NetworkingStatus: " + status.ToString());

                WebProxy wp = new WebProxy("10.0.0.172", 80);
                request.Proxy = wp;
            }
        }
    }
}
