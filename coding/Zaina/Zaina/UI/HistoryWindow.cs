using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Threading;
using MeizuSDK.Presentation;
using MeizuSDK.Drawing;
using System.Diagnostics;

namespace Zaina
{
    class HistoryWindow : Form
    {
        ToolBar toolbar = new ToolBar();
        private ImageContainer imgContainer = new ImageContainer();

        //private ImagingHelper img1;
        //private ImagingHelper img1Pressed;
        //private ImagingHelper img2;
        //private ImagingHelper img2Pressed;
        //private ImagingHelper img3;
        //private ImagingHelper img3Pressed;
        //private ImagingHelper img4;
        //private ImagingHelper img4Pressed;

        //private GridMenu menu;
        ListBox list = new ListBox();

        static AutoResetEvent autoEvent = new AutoResetEvent(false);

        public HistoryWindow()
        {
            this.AnimationIn = MeizuSDK.Drawing.AnimationType.ScrollRightToLeftPush;
            this.AnimationOut = MeizuSDK.Drawing.AnimationType.ScrollLeftToRightPush;
        }

        protected override void OnLoad(EventArgs e)
        {
            //BuildGridMenu();

            toolbar.SetTextButton(ToolBarButtonIndex.LeftTextButton, true, true, L10n.Return);
            //toolbar.SetTextButton(ToolBarButtonIndex.MiddleTextButton, true, true, L10n.Operator);
            toolbar.SetTextButton(ToolBarButtonIndex.RightTextButton, true, true, L10n.ClearData);
            toolbar.ButtonClick += new EventHandler<ToolBar.ButtonEventArgs>(toolbar_ButtonClick);
            Controls.Add(toolbar);

            list.Location = new Point(0, 0);
            list.Size = new Size(Size.Width, Size.Height - ToolBar.HEIGHT);
            list.DefaultItemHeight = Define.HistoryListItemHeight;
            list.UltraGridLines = true;// 启用额外列表线
            list.OwnerDrawItem = true;// 启用自定义绘制
            list.DrawItem += new DrawItemEventHandler(list_DrawItem);
            //list.ItemSelected += new ItemSelectedEventHandler(list_ItemSelected);
            list.Click += new System.EventHandler<MeizuSDK.Presentation.ListBoxClickEventArgs>(list_Click);
            Controls.Add(list);

            base.OnLoad(e);

            FillList();
        }

        protected void toolbar_ButtonClick(object sender, ToolBar.ButtonEventArgs e)
        {
            if (e.Index == ToolBarButtonIndex.LeftTextButton)
            {
                if (IsDialog)
                    DialogResult = DialogResult.Yes;
                else
                    Close();
            }
            else if (e.Index == ToolBarButtonIndex.MiddleTextButton)
            {
                //if (menu.IsContinue())
                //{
                //    menu.Close();
                //}
                //else
                //{
                //    menu.Show(HWnd, ToolBar.HEIGHT - 1);
                //}

                return;
            }
            else if (e.Index == ToolBarButtonIndex.RightTextButton)
            {
                ClearData();
            }

            //if (menu.IsContinue())
            //{
            //    menu.Close();
            //}
        }

        protected override void OnMzCommand(uint wParam, uint lParam)
        {
            uint id = NativeMethods.LOWORD(wParam);
            //GridMenuItem item = menu.Items[id];

            switch (id)
            {
                case Define.LocateGridMenuId_ZoomIn:
                    //ZoomIn();
                    break;
                case Define.LocateGridMenuId_ZoomOut:
                    //ZoomOut();
                    break;
                case Define.LocateGridMenuId_MapType:
                    //SwitchMapType();
                    break;
                case Define.LocateGridMenuId_SendSMS:
                    break;
                default:
                    break;
            }

            base.OnMzCommand(wParam, lParam);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            //if (menu.IsContinue())
            //{
            //    menu.Close();
            //}
        }

        void list_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (list.IsLButtonDown && !list.IsMouseDownAtScrolling && e.ItemIndex == list.LastClickedIndex)
            {
                e.DrawItemSelectedBackground();
            }

            ListItem item = list.Items[e.ItemIndex];

            if (item != null)
            {
                Graphics g = e.Graphics;

                using (SolidBrush brushText = new SolidBrush(ForeColor))
                {
                    using (Font fontText = new Font(FontFamily.GenericSansSerif, Define.HistoryListAddressFontSize, FontStyle.Regular))
                    {
                        StringFormat sf = new StringFormat();
                        sf.Alignment = StringAlignment.Near;
                        sf.LineAlignment = StringAlignment.Far;

                        Rectangle textRect = e.ItemRectangle;// 获取文本内容所在矩形
                        textRect.Inflate(-12, -12);

                        g.DrawString(item.Text, fontText, brushText, textRect, sf);
                    }
                }

                using (SolidBrush brushDate = new SolidBrush(Color.DarkGray))
                {
                    using (Font fontDate = new Font(FontFamily.GenericSansSerif, Define.HistoryListDateFontSize, FontStyle.Regular))
                    {
                        StringFormat sf = new StringFormat();
                        sf.Alignment = StringAlignment.Near;
                        sf.LineAlignment = StringAlignment.Near;

                        Rectangle textRect = e.ItemRectangle;// 获取文本内容所在矩形
                        textRect.Inflate(-12, -12);

                        LocationItem locateItem = (LocationItem)item.Value;
                        g.DrawString(locateItem.CheckinTime, fontDate, brushDate, textRect, sf);
                    }
                }
            }
        }

        void list_Click(object sender, ListBoxClickEventArgs e)
        {
            LocationItem item = (LocationItem)list.Items[e.Index].Value;
           
            LocateWindow locate = new LocateWindow();
            locate.InitHistory(item.Lat, item.Lng, item.Address);
            locate.ShowDialog(this);
        }

        /*
        protected void BuildGridMenu()
        {
            if (null != menu)
                Controls.Remove(menu);

            menu = new GridMenu();

            // GridMenu
            img1 = imgContainer.LoadImageFromMzResV2(MzResV2.Png_Tick, true);
            img1Pressed = imgContainer.LoadImageFromMzResV2(MzResV2.Png_Tick_Pressed, true);
            img2 = imgContainer.LoadImageFromMzResV2(MzResV2.Png_Download, true);
            img2Pressed = imgContainer.LoadImageFromMzResV2(MzResV2.Png_Download_Pressed, true);
            img3 = imgContainer.LoadImageFromMzResV2(MzResV2.Png_Cancel, true);
            img3Pressed = imgContainer.LoadImageFromMzResV2(MzResV2.Png_Cancel_Pressed, true);
            img4 = imgContainer.LoadImageFromMzResV2(MzResV2.Png_Search, true);
            img4Pressed = imgContainer.LoadImageFromMzResV2(MzResV2.Png_Search_Pressed, true);


            //menu.ItemClicked += new GridMenuItemClickedEventHandler(menu_ItemClicked);

            GridMenuItem item = new GridMenuItem(Define.LocateGridMenuId_ZoomIn, L10n.MapZoomIn, img1, img1Pressed);
            menu.Items.Add(item);
            item = new GridMenuItem(Define.LocateGridMenuId_ZoomOut, L10n.MapZoomOut, img2, img2Pressed);
            menu.Items.Add(item);

            item = new GridMenuItem(Define.LocateGridMenuId_MapType, L10n.RoadMap, img3, img3Pressed);
            menu.Items.Add(item);

            item = new GridMenuItem(Define.LocateGridMenuId_SendSMS, L10n.SendSMS, img4, img4Pressed);
            menu.Items.Add(item);

            Controls.Add(menu);
        }
        */

        private void FillList()
        {
            WaitDialog.Begin(this);
            ThreadPool.QueueUserWorkItem(new WaitCallback(MultithreadFillList), autoEvent);
            autoEvent.WaitOne(Timeout.Infinite, false);
            WaitDialog.End();
        }

        private void MultithreadFillList(object stateInfo)
        {
            try
            {
                if (list.Items.Count > 0)
                {
                    list.Items.Clear();
                }

                History history = new History();
                List<LocationItem> listHistory = history.GetHistory(-1);
                foreach (LocationItem item in listHistory)
                {
                    ListItem insertItem = new ListItem(item.Address, item);
                    list.Items.Add(insertItem);
                }
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

        private void ClearData()
        {
            if (MessageBox.DialogResult.OK != MessageBox.Show(
                    L10n.CheckIsClearData,
                    L10n.ApplicationName,
                    MessageBox.MessageBoxButtons.MZ_OKCANCEL))
                return;

            WaitDialog.Begin(this);
            ThreadPool.QueueUserWorkItem(new WaitCallback(MultithreadClearData), autoEvent);
            autoEvent.WaitOne(Timeout.Infinite, false);
            WaitDialog.End();
        }

        private void MultithreadClearData(object stateInfo)
        {
            try
            {
                if (list.Items.Count > 0)
                {
                    list.Items.Clear();
                }

                History history = new History();
                history.Clear();

                DeleteCachesFile();
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

        private void DeleteCachesFile()
        {
            string cacheDir = GStaticMap.GetCacheDir();
            string[] cachePaths = Directory.GetFiles(cacheDir);
            foreach (string fileName in cachePaths)
            {
                try
                {
                    File.Delete(fileName);
                }
                catch (System.Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }
    }
}
