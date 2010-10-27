using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using MeizuSDK.Presentation;
using MeizuSDK.Core;

namespace Zaina
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [MTAThread]
        static void Main()
        {
            if (Application.ActivatePreviousInstance("M8Zaina.Main"))
                return;

            try
            {
                Application.Run(new MainWindow());
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                Networking.GprsDisconnect();
            }
        }
    }
}