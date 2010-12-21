using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Zaina
{
    class L10n
    {
        public const string ApplicationName = "在哪";

        public const string BtnLocate = "我在哪里";
        public const string BtnHistory = "我去过哪些地方";
        public const string BtnLeaveMsg = "给作者留言";

        public const string Exit = "退出";
        public const string About = "关于";
        public const string Return = "返回";
        public const string Save = "保存";
        public const string Send = "发送";
        public const string Input = "输入法";
        public const string ClearData = "全部删除";
        public const string ResetPwd = "清除保存的微博用户信息";

        public const string ExitConfirm = "确认要退出 在哪 ？";

        public const string MapZoomIn = "地图放大";
        public const string MapZoomOut = "地图缩小";

        public const string MapShiftLeft = "向左平移";
        public const string MapShiftRight = "向右平移";
        public const string MapShiftUp = "向上平移";
        public const string MapShiftDown = "向下平移";

        public const string RoadMap = "平面地图";
        public const string Satellite = "卫星地图";
        public const string Weibo = "发送微博";

        public const string MapOptions = "地图选项";
        public const string ReLocate = "重新定位";

        public const string Operator = "操作";

        public const string LocateFailed = "定位失败，请确认网络正常后重试。";
        public const string GetAddressFailed = "获取地址信息失败，请确认网络正常后重试。";

        public const string CheckIsClearData = "您确认要删除全部历史记录和相关的地图缓存";
        
        public const string AppVersion = "软件版本：";
		public const string AppDescription = "在哪是一款开源免费手机定位软件。";
        public const string AppThanks1 = "在哪的诞生离不开开源项目：";
        public const string AppThanks2 = "Managed Meizu SDK Wrapper 。";
		public const string AppAuthor   = "软件作者  ：linyehui (twitter)";
        public const string Email = "作者邮箱  ：M8Zaina@gmail.com";
        public const string HomePage = "作者网站  ：http://linyehui.com";
        public const string Copyright   = @"Copyright © 2010 linyehui. All Rights Reserved";

        // weibo
        public const string WeiboFormat = @"我在这附近 http://ditu.google.cn/?q={0},{1} #M8在哪#";
        public const string LeaveMsgToLYH = "#M8在哪# @linyehui ";

        public const string WeiboNoUserInfo = @"用户名与密码没有设置正确，不能发送微博";
        public const string UserNameIsNull = "用户名不能为空";
        public const string PasswordIsNull = "密码不能为空";
        public const string MsgTooLong = "微博字数超过限制";
        public const string UpdateMsgFailed = "更新微博失败";
        public const string UpdateMsgSuccessed = "更新微博成功";
        public const string ResetPwdOK = "用户信息清除成功，下次发送微博时需要重新设置";

        public const string UserName = "用户名：";
        public const string Password = "密　码：";
    }
}
