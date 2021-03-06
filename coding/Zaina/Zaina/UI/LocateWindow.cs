﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Diagnostics;
using MeizuSDK.Presentation;
using MeizuSDK.Drawing;
using MeizuSDK.Core;

namespace Zaina
{
    class LocateWindow : Form
    {
        private const int TextMargin = 10;

        ToolBar toolbar = new ToolBar();
        PictureBox picBoxMap = new PictureBox();
        PictureBox picBoxCaption = new PictureBox();
        ToucherHelper touchHelper = new ToucherHelper();

        GStaticMap staticMap = new GStaticMap();
        string Address = "";
        double Lat = 0;
        double Lng = 0;
        bool HistoryView = false;

        private ImageContainer imgContainer = new ImageContainer();

        private ImagingHelper imgZoomIn;
        private ImagingHelper imgZoomOut;
        private ImagingHelper imgMapType;
        private ImagingHelper imgSMS;
        private ImagingHelper imgShiftLeft;
        private ImagingHelper imgShiftRight;
        private ImagingHelper imgShiftUp;
        private ImagingHelper imgShiftDown;

        private GridMenu menu;

        static AutoResetEvent autoEvent = new AutoResetEvent(false);

        public LocateWindow()
        {
            this.AnimationIn = MeizuSDK.Drawing.AnimationType.ScrollRightToLeftPush;
            this.AnimationOut = MeizuSDK.Drawing.AnimationType.ScrollLeftToRightPush;
        }

        public bool InitHistory(double lat, double lng, string address)
        {
            if (lat == 0 || lng == 0 || address.Length <= 0)
                return false;

            Address = address;
            Lat = lat;
            Lng = lng;
            HistoryView = true;

            return true;
        }

        protected override void OnLoad(EventArgs e)
        {
            string path = Path.Combine(Application.StartupPath, Define.DefaultMapPath);
            picBoxMap.LoadFromFile(path);
            picBoxMap.Location = new Point(0, 0);
            picBoxMap.Size = new Size(Width, Define.MapHeight);
            picBoxMap.PaintMode = MeizuSDK.Drawing.PaintMode.Normal;
            Controls.Add(picBoxMap);

            path = Path.Combine(Application.StartupPath, Define.AddressTitlePath);
            picBoxCaption.LoadFromFile(path);
            picBoxCaption.Location = new Point(0, Define.MapHeight);
            picBoxCaption.Size = new Size(Width, Define.AddressTitleHeight);
            picBoxCaption.PaintMode = MeizuSDK.Drawing.PaintMode.Normal;
            Controls.Add(picBoxCaption);

            BuildGridMenu();

            toolbar.SetTextButton(ToolBarButtonIndex.LeftTextButton, true, true, L10n.Return);
            toolbar.SetTextButton(ToolBarButtonIndex.MiddleTextButton, true, true, L10n.MapOptions);
            if (!HistoryView)
                toolbar.SetTextButton(ToolBarButtonIndex.RightTextButton, true, true, L10n.ReLocate);

            toolbar.ButtonClick += new EventHandler<ToolBar.ButtonEventArgs>(toolbar_ButtonClick);
            Controls.Add(toolbar);

            touchHelper.TouchGestureEvent += new EventHandler<ToucherHelper.TouchHelperEventArgs>(OnGesture);

            this.Touch += new EventHandler<MultiTouchEventArgs>(MultiTouchWindow_Touch);

            base.OnLoad(e);

            NativeMethods.PostMessage(this.HWnd, Define.MZ_WM_COMMAND, Define.LoadFinish, 0);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Address.Length <= 0)
                return;

            Rectangle rcLabel = new Rectangle(0,
                Define.MapHeight + Define.AddressTitleHeight + TextMargin,
                Width,
                Define.AddressHeight - (TextMargin * 2));

            using (Graphics g = e.Graphics)
            using (Font fontText = new Font(FontFamily.GenericSansSerif, Define.AddressFontSize, FontStyle.Regular))
            using (StringFormat sf = new StringFormat(StringFormatFlags.NoClip))
            using (SolidBrush brushFore = new SolidBrush(ForeColor))
            {
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;

                // 一个简单的智能双行显示文字
                SizeF lineSize = g.MeasureString(Address, fontText);
                if ((lineSize.Width + (TextMargin * 2)) > rcLabel.Width)
                {
                    // 双行
                    Rectangle rcFirstLine = new Rectangle(rcLabel.X, rcLabel.Y, rcLabel.Width, rcLabel.Height / 2);
                    g.DrawString(Address.Substring(0, Address.Length / 2), fontText, brushFore, rcFirstLine, sf);
                    Rectangle rcSecondtLine = new Rectangle(rcLabel.X, rcLabel.Y + rcLabel.Height / 2, rcLabel.Width, rcLabel.Height / 2);
                    g.DrawString(Address.Substring(Address.Length / 2), fontText, brushFore, rcSecondtLine, sf);
                }
                else
                {
                    // 单行
                    g.DrawString(Address, fontText, brushFore, rcLabel, sf);
                }                
            }
        }

        protected void toolbar_ButtonClick(object sender, ToolBar.ButtonEventArgs e)
        {
            if (e.Index == ToolBarButtonIndex.LeftTextButton)
            {
                CloseMenu();

                if (IsDialog)
                    DialogResult = DialogResult.Yes;
                else
                    Close();

                return;
            }
            else if (e.Index == ToolBarButtonIndex.MiddleTextButton)
            {
                if (menu.IsContinue())
                {
                    menu.Close();
                }
                else
                {
                    menu.Show(HWnd, ToolBar.HEIGHT - 1);
                }

                return;
            }
            else if (e.Index == ToolBarButtonIndex.RightTextButton)
            {
                CloseMenu();
                Locate();
                return;
            }
        }

        void OnGesture(object sender, ToucherHelper.TouchHelperEventArgs e)
        {
            switch (e.Gesture)
            {
                // 这里是拖着地图走，所以对应的移动方向是反的
                case ToucherHelper.TouchGesture.None:
                    break;
                case ToucherHelper.TouchGesture.Left:
                    ShiftRight();
                    break;
                case ToucherHelper.TouchGesture.Right:
                    ShiftLeft();
                    break;
                case ToucherHelper.TouchGesture.Up:
                    ShiftDown();
                    break;
                case ToucherHelper.TouchGesture.Down:
                    ShiftUp();
                    break;
                case ToucherHelper.TouchGesture.ZoomIn:
                    ZoomIn();
                    break;
                case ToucherHelper.TouchGesture.ZoomOut:
                    ZoomOut();
                    break;
            }
        }

        void MultiTouchWindow_Touch(object sender, Form.MultiTouchEventArgs e)
        {
            if (e.Touches.Count > 0)
            {
                string msg = String.Format("Touch X: {0}, Y: {1}, Z: {2}", e.Touches[0].X.ToString(), e.Touches[0].Y.ToString(), e.Touches[0].Z.ToString());
                Debug.WriteLine(msg);
                touchHelper.Add(e.Touches[0]);
            }

            if (e.Touches.Count > 1)
            {
                string msg1 = String.Format("Touch1 X: {0}, Y: {1}, Z: {2}", e.Touches[0].X.ToString(), e.Touches[0].Y.ToString(), e.Touches[0].Z.ToString());
                string msg2 = String.Format("Touch2 X: {0}, Y: {1}, Z: {2}", e.Touches[1].X.ToString(), e.Touches[1].Y.ToString(), e.Touches[1].Z.ToString());

                Debug.WriteLine(msg1);
                Debug.WriteLine(msg2);

                touchHelper.Add(e.Touches[0], e.Touches[1]);
            }
        }

        protected override void OnMzCommand(uint wParam, uint lParam)
        {
            int id = (int)NativeMethods.LOWORD(wParam);
            GridMenuItem item = menu.Items[id];

            switch (id)
            {
                case (int)Define.LoadFinish:
                    OnLoadFinish();
                    break;
                case Define.LocateGridMenuId_ZoomIn:
                    ZoomIn();
                    break;
                case Define.LocateGridMenuId_ZoomOut:
                    ZoomOut();
                    break;
                case Define.LocateGridMenuId_MapType:
                    SwitchMapType();
                    break;
                case Define.LocateGridMenuId_UpdateWeibo:
                    UpdateWeibo();
                    break;
                case Define.LocateGridMenuId_ShiftLeft:
                    ShiftLeft();
                    break;
                case Define.LocateGridMenuId_ShiftRight:
                    ShiftRight();
                    break;
                case Define.LocateGridMenuId_ShiftUp:
                    ShiftUp();
                    break;
                case Define.LocateGridMenuId_ShiftDown:
                    ShiftDown();
                    break;
                default:
                    break;
            }

            base.OnMzCommand(wParam, lParam);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            CloseMenu();

            base.OnClosing(e);
        }

        protected void OnLoadFinish()
        {
            if (HistoryView)
            {
                ShowHistory();
            }
            else
            {
                Locate();
            }
        }

        protected void CloseMenu()
        {
            if (null == menu)
                return;
            
            if (menu.IsContinue())
            {
                menu.Close();
            }
        }

        protected void BuildGridMenu()
        {
            if (null != menu)
                Controls.Remove(menu);

            menu = new GridMenu();

            // GridMenu
            string path = Path.Combine(Application.StartupPath, Define.ZoomInPath);
            imgZoomIn = imgContainer.LoadImage(path);

            path = Path.Combine(Application.StartupPath, Define.ZoomOutPath);
            imgZoomOut = imgContainer.LoadImage(path);

            path = Path.Combine(Application.StartupPath, Define.MapTypePath);
            imgMapType = imgContainer.LoadImage(path);

            path = Path.Combine(Application.StartupPath, Define.SMSPath);
            imgSMS = imgContainer.LoadImage(path);

            path = Path.Combine(Application.StartupPath, Define.ShiftLeftPath);
            imgShiftLeft = imgContainer.LoadImage(path);
            path = Path.Combine(Application.StartupPath, Define.ShiftRightPath);
            imgShiftRight = imgContainer.LoadImage(path);
            path = Path.Combine(Application.StartupPath, Define.ShiftUpPath);
            imgShiftUp = imgContainer.LoadImage(path);
            path = Path.Combine(Application.StartupPath, Define.ShiftDownPath);
            imgShiftDown = imgContainer.LoadImage(path);

            //menu.ItemClicked += new GridMenuItemClickedEventHandler(menu_ItemClicked);

            GridMenuItem item = new GridMenuItem(Define.LocateGridMenuId_ZoomIn, L10n.MapZoomIn, imgZoomIn, imgZoomIn);
            menu.Items.Add(item);
            item = new GridMenuItem(Define.LocateGridMenuId_ShiftUp, L10n.MapShiftUp, imgShiftUp, imgShiftUp);
            menu.Items.Add(item);
            item = new GridMenuItem(Define.LocateGridMenuId_ZoomOut, L10n.MapZoomOut, imgZoomOut, imgZoomOut);
            menu.Items.Add(item);

            item = new GridMenuItem(Define.LocateGridMenuId_ShiftLeft, L10n.MapShiftLeft, imgShiftLeft, imgShiftLeft);
            menu.Items.Add(item);
            item = new GridMenuItem(Define.LocateGridMenuId_ShiftDown, L10n.MapShiftDown, imgShiftDown, imgShiftDown);
            menu.Items.Add(item);
            item = new GridMenuItem(Define.LocateGridMenuId_ShiftRight, L10n.MapShiftRight, imgShiftRight, imgShiftRight);
            menu.Items.Add(item);
            
            if (staticMap.ShowSatellite)
                item = new GridMenuItem(Define.LocateGridMenuId_MapType, L10n.RoadMap, imgMapType, imgMapType);
            else
                item = new GridMenuItem(Define.LocateGridMenuId_MapType, L10n.Satellite, imgMapType, imgMapType);
            menu.Items.Add(item);

            item = new GridMenuItem(Define.LocateGridMenuId_UpdateWeibo, L10n.Weibo, imgSMS, imgSMS);
            menu.Items.Add(item);

            Controls.Add(menu);
        }

        private void Locate()
        {
            WaitDialog.Begin(this);
            ThreadPool.QueueUserWorkItem(new WaitCallback(MultithreadLocate), autoEvent);
            autoEvent.WaitOne(Timeout.Infinite, false);
            WaitDialog.End();
        }

        private void ShowHistory()
        {
            WaitDialog.Begin(this);
            ThreadPool.QueueUserWorkItem(new WaitCallback(MultithreadShowHistory), autoEvent);
            autoEvent.WaitOne(Timeout.Infinite, false);
            WaitDialog.End();
        }

        private void ZoomIn()
        {
            WaitDialog.Begin(this);
            staticMap.ZoomIn();
            ThreadPool.QueueUserWorkItem(new WaitCallback(MultithreadRebuildMap), autoEvent);
            autoEvent.WaitOne(Timeout.Infinite, false);
            WaitDialog.End();
        }

        private void ZoomOut()
        {
            WaitDialog.Begin(this);
            staticMap.ZoomOut();
            ThreadPool.QueueUserWorkItem(new WaitCallback(MultithreadRebuildMap), autoEvent);
            autoEvent.WaitOne(Timeout.Infinite, false);
            WaitDialog.End();
        }

        private void SwitchMapType()
        {
            WaitDialog.Begin(this);
            staticMap.SwitchMapType();

            BuildGridMenu();
            ThreadPool.QueueUserWorkItem(new WaitCallback(MultithreadRebuildMap), autoEvent);
            autoEvent.WaitOne(Timeout.Infinite, false);
            WaitDialog.End();
        }

        private void ShiftLeft()
        {
            WaitDialog.Begin(this);
            staticMap.ShiftLeft();
            RegetAddress(staticMap.Lat, staticMap.Lng);
            ThreadPool.QueueUserWorkItem(new WaitCallback(MultithreadRebuildMap), autoEvent);
            autoEvent.WaitOne(Timeout.Infinite, false);
            WaitDialog.End();
        }
        private void ShiftRight()
        {
            WaitDialog.Begin(this);
            staticMap.ShiftRight();
            RegetAddress(staticMap.Lat, staticMap.Lng);
            ThreadPool.QueueUserWorkItem(new WaitCallback(MultithreadRebuildMap), autoEvent);
            autoEvent.WaitOne(Timeout.Infinite, false);
            WaitDialog.End();
        }
        private void ShiftUp()
        {
            WaitDialog.Begin(this);
            staticMap.ShiftUp();
            RegetAddress(staticMap.Lat, staticMap.Lng);
            ThreadPool.QueueUserWorkItem(new WaitCallback(MultithreadRebuildMap), autoEvent);
            autoEvent.WaitOne(Timeout.Infinite, false);
            WaitDialog.End();
        }
        private void ShiftDown()
        {
            WaitDialog.Begin(this);
            staticMap.ShiftDown();
            RegetAddress(staticMap.Lat, staticMap.Lng);
            ThreadPool.QueueUserWorkItem(new WaitCallback(MultithreadRebuildMap), autoEvent);
            autoEvent.WaitOne(Timeout.Infinite, false);
            WaitDialog.End();
        }

        private void UpdateWeibo()
        {
            string msg = String.Format(L10n.WeiboFormat, Lat.ToString(), Lng.ToString());
            WeiboWindow wDlg = new WeiboWindow(msg);
            wDlg.ShowDialog(this);
        }

        private void RegetAddress(double lat, double lng)
        {
            string address;
            if (!GetAddress(lat, lng, out address))
            {
                MessageBox.Show(L10n.GetAddressFailed);
                return;
            }
            Address = address;
            Lat = lat;
            Lng = lng;
        }

        private void MultithreadLocate(object stateInfo)
        {
            try
            {
                toolbar.EnableButton(ToolBarButtonIndex.MiddleTextButton, false);
                double lat = 0;
                double lng = 0;
                string address = "";
                if (!GetLatLng(out lat, out lng))
                {
                    MessageBox.Show(L10n.LocateFailed);
                    return;
                }

                if (!GetAddress(lat, lng, out address))
                {
                    MessageBox.Show(L10n.GetAddressFailed);
                    return;
                }

                Address = address;
                Lat = lat;
                Lng = lng;
                toolbar.EnableButton(ToolBarButtonIndex.MiddleTextButton, true);

                Options trackMan = new Options();
                trackMan.Add(System.DateTime.Now, lat, lng, address);

                string mapFileName = staticMap.GenMap(lat, lng);
                picBoxMap.LoadFromFile(mapFileName);
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                ((AutoResetEvent)stateInfo).Set();
            }
        }

        private void MultithreadShowHistory(object stateInfo)
        {
            try
            {
                if (Lat == 0 || Lng == 0 || Address.Length <= 0)
                    return;

                string mapFileName = staticMap.GenMap(Lat, Lng);
                picBoxMap.LoadFromFile(mapFileName);
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                ((AutoResetEvent)stateInfo).Set();
            }
        }

        private void MultithreadRebuildMap(object stateInfo)
        {
            try
            {
                string mapFileName = staticMap.RebuildMap();
                picBoxMap.LoadFromFile(mapFileName);
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                ((AutoResetEvent)stateInfo).Set();
            }
        }

        private bool GetLatLng(out double lat, out double lng)
        {
            lat = 0;
            lng = 0;
            
            // 最多进行3次
            int maxRetryTimes = 3;
            for (int i = 0; i < maxRetryTimes; i++)
            {
                if (CGeolocation.locate_GoogleGearsAPI(out lat, out lng))
                    return true;
            }

            return false;
        }

        private bool GetAddress(double lat, double lng, out string address)
        {
            address = "";

            // 最多进行3次
            int maxRetryTimes = 3;
            for (int i = 0; i < maxRetryTimes; i++)
            {
                if (CGeolocation.getLocations_GoogleGearsAPI(lat, lng, out address))
                    return true;
            }

            return false;
        }
    }
}
