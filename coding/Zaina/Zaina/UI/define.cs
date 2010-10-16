using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Zaina
{
    class Define
    {
        public const int BannerHeight = 133;
        public const string BannerPath = @"skin\banner.png";
        public const string DefaultMapPath = @"skin\default_map.png";
        public const string AddressTitlePath = @"skin\address_title.png";

        public const int MainButtonsHeight = 100;
        public const int MainButtonsSpace = 10;

        public const int MainButtonsTop = 20;

        // LocateWindow define
        public const int MapWidth = 480;
        public const int MapHeight = 500;
        public const int AddressTitleHeight = 32;
        public const int AddressHeight = 84;


        // 消息ID定义
        private const int LocateGridMenuIdBase = 1000;
        public const int LocateGridMenuId_ZoomIn = LocateGridMenuIdBase + 1;
        public const int LocateGridMenuId_ZoomOut = LocateGridMenuIdBase + 2;
        public const int LocateGridMenuId_MapType = LocateGridMenuIdBase + 3;
        public const int LocateGridMenuId_SendSMS = LocateGridMenuIdBase + 4;
    }
}
