using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Zaina
{
    class Define
    {
        public const int BannerHeight = 133;

        public const int WeiboLogoWidth = 168;
        public const int WeiboLogoHeight = 68;

        public const string BannerPath = @"skin\banner.png";
        public const string DefaultMapPath = @"skin\default_map.png";
        public const string AddressTitlePath = @"skin\address_title.png";
        public const string AboutDetailPath = @"skin\about.png";
        public const string WeiboLogoPath = @"skin\weibo_logo.png";

        public const string ZoomInPath = @"skin\zoom_in.png";
        public const string ZoomOutPath = @"skin\zoom_out.png";
        public const string MapTypePath = @"skin\map_type.png";
        public const string SMSPath = @"skin\sms.png";

        public const string ShiftLeftPath = @"skin\shift_left.png";
        public const string ShiftRightPath = @"skin\shift_right.png";
        public const string ShiftUpPath = @"skin\shift_up.png";
        public const string ShiftDownPath = @"skin\shift_down.png";

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

        public const uint MZFC_WM_MESSAGE = 0xFFF10000;
        public const uint MZ_WM_COMMAND = (MZFC_WM_MESSAGE + 1);

        private const int LocateCommandBase = 500;
        public const uint LoadFinish = LocateCommandBase + 1;

        private const int LocateGridMenuIdBase = 1000;
        public const int LocateGridMenuId_ZoomIn = LocateGridMenuIdBase + 1;
        public const int LocateGridMenuId_ZoomOut = LocateGridMenuIdBase + 2;
        public const int LocateGridMenuId_MapType = LocateGridMenuIdBase + 3;
        public const int LocateGridMenuId_UpdateWeibo = LocateGridMenuIdBase + 4;
        
        public const int LocateGridMenuId_ShiftLeft = LocateGridMenuIdBase + 5;
        public const int LocateGridMenuId_ShiftRight = LocateGridMenuIdBase + 6;
        public const int LocateGridMenuId_ShiftUp = LocateGridMenuIdBase + 7;
        public const int LocateGridMenuId_ShiftDown = LocateGridMenuIdBase + 8;
    }
}
