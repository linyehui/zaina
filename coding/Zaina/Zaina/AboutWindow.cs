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
        //protected Label lable = new Label();

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

            //lable.Text = "林叶辉\r\n林叶辉";
            //lable.Location = new Point(0, 0);
            //lable.Size = new Size(Width, 300);
            //Controls.Add(lable);

            //string path = Path.Combine(Application.StartupPath, Define.BannerPath);
            //imgBanner.LoadImage(path);

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
