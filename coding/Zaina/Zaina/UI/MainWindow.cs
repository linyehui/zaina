using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Threading;
using System.IO;

using MeizuSDK.Presentation;
using MeizuSDK.Core;
using MeizuSDK.Drawing;

namespace Zaina
{
    class MainWindow : Form
    {
        PictureBox banner = new PictureBox();
        ToolBar toolbar = new ToolBar();
        ListBox list = new ListBox();

        private ImageContainer imgContainer = new ImageContainer();
        private ImagingHelper imgArrow;

        public MainWindow()
        {
            UsbConnection.StatusChanged += new EventHandler<UsbConnectionEventArgs>(UsbConnection_StatusChanged);

            Text = L10n.ApplicationName;
            AnimationIn = AnimationType.ZoomIn;

            string path = Path.Combine(Application.StartupPath, Define.BannerPath);
            banner.LoadFromFile(path);
            banner.Location = new Point(0, 0);
            banner.Size = new Size(Screen.ClientWidth, Define.BannerHeight);
            banner.PaintMode = MeizuSDK.Drawing.PaintMode.Normal;
            //banner.SizeMode = bannerBoxSizeMode.CenterImage;
            Controls.Add(banner);

            toolbar.SetTextButton(ToolBarButtonIndex.LeftTextButton, true, true, L10n.Exit);
            toolbar.SetTextButton(ToolBarButtonIndex.RightTextButton, true, true, L10n.About);
            toolbar.ButtonClick += new EventHandler<ToolBar.ButtonEventArgs>(toolbar_ButtonClick);
            Controls.Add(toolbar);

            // 设置主窗口背景
            //BackgroundImage = MzGraphicsAPI.SnapshotClientScreen();
            //string bgImgPath = Path.Combine(Application.StartupPath, "Background.png");
            //if (File.Exists(bgImgPath))
            //{
            //    BackgroundImage = new Bitmap(bgImgPath);
            //}
        }
        /// <summary>
        /// 演示Onload函数
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //ThreadStart method = new ThreadStart(MultithreadInitControls);
            //Thread th = new Thread(method);
            //th.Start();

            MultithreadInitControls();
        }
        /// <summary>
        /// 多线程初始化控件
        /// </summary>
        private void MultithreadInitControls()
        {
            imgArrow = imgContainer.LoadImageFromMzResV2(MzResV2.Png_Arrow_Right, true);

            list.Size = new Size(Width, Height - ToolBar.HEIGHT - banner.Height);
            list.Location = new Point(0, banner.Height);
            list.DefaultItemHeight = Define.MainWindowListItemHeight;
            list.UltraGridLines = true;
            list.OwnerDrawItem = true;
            list.DrawItem += new DrawItemEventHandler(list_DrawItem);
            list.Click += new System.EventHandler<MeizuSDK.Presentation.ListBoxClickEventArgs>(list_Click);
            Controls.Add(list);

            ListItem itemLocate = new ListItem(L10n.BtnLocate, null);
            list.Items.Add(itemLocate);
            ListItem itemHistory = new ListItem(L10n.BtnHistory, null);
            list.Items.Add(itemHistory);
            ListItem itemLeaveMsg = new ListItem(L10n.BtnLeaveMsg, null);
            list.Items.Add(itemLeaveMsg);
       }

        void UsbConnection_StatusChanged(object sender, UsbConnectionEventArgs e)
        {
            if (e.Status == UsbConnectionStatus.MassStorage)
            {
                AnimationOut = AnimationType.ZoomOut;
                
                // 实在是没辙啊，估计是SDK的问题，导致USB退出时可能出现的crash
                // 这么做之后能减少出现的概率，本质上的解决估计还要从SDK上着手
                // edit by linyehui 2010/12/14 20:22
                //Application.Exit();
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }

        void toolbar_ButtonClick(object sender, ToolBar.ButtonEventArgs e)
        {
            switch (e.Index)
            {
                case ToolBarButtonIndex.LeftTextButton:
                    if (MessageBox.DialogResult.OK == MessageBox.Show(L10n.ExitConfirm, L10n.ApplicationName, MessageBox.MessageBoxButtons.MZ_OKCANCEL, MessageBox.HomeKeyReturnValue.SHK_RET_DEFAULT))
                    {
                        AnimationOut = AnimationType.ZoomOut;
                        Application.Exit();
                    }
                    break;
                case ToolBarButtonIndex.MiddleTextButton:
                    {
                    }
                    break;
                case ToolBarButtonIndex.RightTextButton:
                    {
                        AboutWindow about = new AboutWindow();
                        about.ShowDialog(this);
                    }
                    break;
            }
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
                    using (Font fontText = new Font(FontFamily.GenericSansSerif, Define.MainWindowListFontSize, FontStyle.Regular))
                    {
                        StringFormat sf = new StringFormat();
                        sf.Alignment = StringAlignment.Near;
                        sf.LineAlignment = StringAlignment.Center;

                        Rectangle textRect = e.ItemRectangle;// 获取文本内容所在矩形
                        textRect.Inflate(-12, -12);

                        g.DrawString(item.Text, fontText, brushText, textRect, sf);

                        Rectangle rcArrow = e.ItemRectangle;
                        rcArrow.Y = e.ItemRectangle.Y + Define.MainWindowListArrowTopDis;
                        rcArrow.X = e.ItemRectangle.X + Define.MainWindowListArrowLeftDis;
                        rcArrow.Height= imgArrow.ImageHeight;
                        rcArrow.Width = imgArrow.ImageWidth;
                        imgArrow.Draw(g, rcArrow, false, false);
                    }
                }
            }
        }

        void list_Click(object sender, ListBoxClickEventArgs e)
        {
            if (list == null)
                return;

            ListItem item = list.Items[e.Index];
            if (item.Text == L10n.BtnLocate)
            {
                LocateWindow locate = new LocateWindow();
                locate.ShowDialog(this);
            }
            else if (item.Text == L10n.BtnHistory)
            {
                HistoryWindow history = new HistoryWindow();
                history.ShowDialog(this);
            }
            else if (item.Text == L10n.BtnLeaveMsg)
            {
                WeiboWindow wDlg = new WeiboWindow(L10n.LeaveMsgToLYH);
                wDlg.ShowDialog(this);
            }
        }
    }
}
