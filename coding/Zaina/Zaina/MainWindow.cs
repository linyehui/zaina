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
        ScrollableControl scrollPanel = new ScrollableControl();
        Button btnWhere = new Button();
        Button btnHistory = new Button();

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
            scrollPanel = new ScrollableControl();
            scrollPanel.Size = new Size(Width, Height - ToolBar.HEIGHT - banner.Height);
            scrollPanel.Location = new Point(0, banner.Height);
            Controls.Add(scrollPanel);

            btnWhere.Text = "我在哪里";
            btnWhere.ButtonType = Button.Type.Green;
            btnWhere.Location = new Point(0, Define.MainButtonsTop);
            btnWhere.Size = new Size(scrollPanel.Width, Define.MainButtonsHeight);
            btnWhere.Click += new EventHandler(btnWhere_Click);
            scrollPanel.Controls.Add(btnWhere);

            btnHistory.Text = "我去过哪些地方";
            btnHistory.ButtonType = Button.Type.Default;
            btnHistory.Location = new Point(0, btnWhere.Location.Y + btnWhere.Size.Height + Define.MainButtonsSpace);
            btnHistory.Size = new Size(scrollPanel.Width, Define.MainButtonsHeight);
            btnHistory.Click += new EventHandler(btnHistory_Click);
            scrollPanel.Controls.Add(btnHistory);
        }

        void UsbConnection_StatusChanged(object sender, UsbConnectionEventArgs e)
        {
            if (e.Status == UsbConnectionStatus.MassStorage)
            {
                AnimationOut = AnimationType.ZoomOut;
                Application.Exit();
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
                        about.ShowDialog();
                    }
                    break;
            }
        }

        void btnWhere_Click(object sender, EventArgs e)
        {
            WaitDialog.Begin(this);
            Thread.Sleep(1000);
            WaitDialog.End();
        }

        void btnHistory_Click(object sender, EventArgs e)
        {

        }
    }
}
