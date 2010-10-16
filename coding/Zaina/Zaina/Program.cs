using System;
using System.Linq;
using System.Collections.Generic;
using MeizuSDK.Presentation;
using System.Diagnostics;
using MeizuSDK.License;
using System.IO;

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
        }
    }
}