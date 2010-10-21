using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using MeizuSDK.Presentation;
using MeizuSDK.Drawing;
using System.Reflection;

namespace Zaina
{
    class AboutWindow : Form
    {
        protected ToolBar toolbar = new ToolBar();
        PictureBox banner = new PictureBox();

        string fileVersion;

        //private ImagingHelper imgBanner = new ImagingHelper();

        public AboutWindow()
        {
            this.AnimationIn = MeizuSDK.Drawing.AnimationType.ScrollRightToLeftPush;
            this.AnimationOut = MeizuSDK.Drawing.AnimationType.ScrollLeftToRightPush;
        }

        protected override void OnLoad(EventArgs e)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();   
            AssemblyName assemblyName = assembly.GetName();
            Version version = assemblyName.Version;
            fileVersion = String.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);

            toolbar.SetTextButton(ToolBarButtonIndex.LeftTextButton, true, true, L10n.Return);
            toolbar.ButtonClick += new EventHandler<ToolBar.ButtonEventArgs>(toolbar_ButtonClick);
            Controls.Add(toolbar);

            string path = Path.Combine(Application.StartupPath, Define.BannerPath);
            banner.LoadFromFile(path);
            banner.Location = new Point(0, 0);
            banner.Size = new Size(Screen.ClientWidth, Define.BannerHeight);
            banner.PaintMode = MeizuSDK.Drawing.PaintMode.Normal;
            Controls.Add(banner);

            base.OnLoad(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using (Graphics g = e.Graphics)
            using (Font fontText = new Font(FontFamily.GenericSansSerif, Define.AboutFontSize, FontStyle.Regular))
            using (Font fontCopyright = new Font(FontFamily.GenericSansSerif, Define.CopyrightFontSize, FontStyle.Regular))
            using (StringFormat sf = new StringFormat(StringFormatFlags.NoClip))
            using (SolidBrush brushFore = new SolidBrush(ForeColor))
            using (SolidBrush brushCopyright = new SolidBrush(Color.DarkGray))
            {
                sf.Alignment = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Near;

                g.DrawString(L10n.AppVersion + fileVersion, fontText, brushFore, 20, 153, sf);
                g.DrawString(L10n.AppDescription, fontText, brushFore, 20, 190, sf);
                g.DrawString(L10n.AppThanks1, fontText, brushFore, 20, 220, sf);
                g.DrawString(L10n.AppThanks2, fontText, brushFore, 20, 250, sf);

                g.DrawString(L10n.AppAuthor, fontText, brushFore, 20, 420, sf);
                g.DrawString(L10n.Email, fontText, brushFore, 20, 450, sf);
                g.DrawString(L10n.HomePage, fontText, brushFore, 20, 480, sf);
                g.DrawString(L10n.Copyright, fontCopyright, brushCopyright, 20, 580, sf);
            }
        }

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
