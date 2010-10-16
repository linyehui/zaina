using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Zaina
{
    internal class LocationItem
    {
        public string CheckinTime { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public string Address { get; set; }
        public string FileName { get; set; }
    }
}
