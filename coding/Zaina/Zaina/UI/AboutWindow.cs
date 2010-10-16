using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using MeizuSDK.Presentation;
using MeizuSDK.Drawing;

namespace Zaina
{
    class AboutWindow : Form
    {
        protected ToolBar toolbar = new ToolBar();
        PictureBox banner = new PictureBox();
        PictureBox about = new PictureBox();

        //private ImagingHelper imgBanner = new ImagingHelper();

        public AboutWindow()
        {
            this.AnimationIn = MeizuSDK.Drawing.AnimationType.ScrollRightToLeftPush;
            this.AnimationOut = MeizuSDK.Drawing.AnimationType.ScrollLeftToRightPush;
        }

        protected override void OnLoad(EventArgs e)
        {
            toolbar.SetTextButton(ToolBarButtonIndex.LeftTextButton, true, true, L10n.Return);
            toolbar.ButtonClick += new EventHandler<ToolBar.ButtonEventArgs>(toolbar_ButtonClick);
            Controls.Add(toolbar);

            string path = Path.Combine(Application.StartupPath, Define.BannerPath);
            banner.LoadFromFile(path);
            banner.Location = new Point(0, 0);
            banner.Size = new Size(Screen.ClientWidth, Define.BannerHeight);
            banner.PaintMode = MeizuSDK.Drawing.PaintMode.Normal;
            Controls.Add(banner);

            path = Path.Combine(Application.StartupPath, Define.AboutDetailPath);
            about.LoadFromFile(path);
            about.Location = new Point(0, Define.BannerHeight);
            about.Size = new Size(Screen.ClientWidth, Height - Define.BannerHeight - ToolBar.HEIGHT);
            about.PaintMode = MeizuSDK.Drawing.PaintMode.Normal;
            Controls.Add(about);

            base.OnLoad(e);
        }

        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    imgBanner.Draw(e.Graphics, e.UpdateRectangle, false, false);
        //}

        protected virtual void toolbar_ButtonClick(object sender, ToolBar.ButtonEventArgs e)
        {
            if (e.Index == ToolBarButtonIndex.LeftTextButton)
            {
                if (IsDialog)
                {
                    DialogResult = DialogResult.Yes;
                }
                else
                    Close();
            }
        }
    }
}
