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
        public const string AboutDetailPath = @"skin\about.png";

        public const string ZoomInPath = @"skin\zoom_in.png";
        public const string ZoomOutPath = @"skin\zoom_out.png";
        public const string MapTypePath = @"skin\map_type.png";
        public const string SMSPath = @"skin\sms.png";

        public const int MainButtonsHeight = 100;
        public const int MainButtonsSpace = 10;

        public const int MainButtonsTop = 20;

        // MainWindow
        public const int MainWindowListItemHeight = 85;
        public const int MainWindowListFontSize = 20;
        public const int MainWindowListArrowTopDis = 35;
        public const int MainWindowListArrowLeftDis = 445;

        // LocateWindow define
        public const int MapWidth = 480;
        public const int MapHeight = 500;
        public const int AddressTitleHeight = 32;
        public const int AddressHeight = 84;

        public const int AddressFontSize = 18;

        // History
        public const int HistoryListItemHeight = 85;
        public const int HistoryListAddressFontSize = 18;
        public const int HistoryListDateFontSize = 16;

        // About 
        public const int AboutFontSize = 18;
        public const int CopyrightFontSize = 16;

        // 消息ID定义
        private const int LocateGridMenuIdBase = 1000;
        public const int LocateGridMenuId_ZoomIn = LocateGridMenuIdBase + 1;
        public const int LocateGridMenuId_ZoomOut = LocateGridMenuIdBase + 2;
        public const int LocateGridMenuId_MapType = LocateGridMenuIdBase + 3;
        public const int LocateGridMenuId_SendSMS = LocateGridMenuIdBase + 4;
    }
}
