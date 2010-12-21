using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using MeizuSDK.Presentation;
using MeizuSDK.Drawing;
using MeizuSDK.Core;

namespace Zaina
{
    class ConfigWindow : Form
    {
        ToolBar toolbar = new ToolBar();
        PictureBox banner = new PictureBox();
        Label labelUserName = new Label();
        Label labelPassword = new Label();
        TextBox textUserName = new TextBox();
        TextBox textPasswrod= new TextBox();
       
        public ConfigWindow()
        {
            this.AnimationIn = MeizuSDK.Drawing.AnimationType.ScrollRightToLeftPush;
            this.AnimationOut = MeizuSDK.Drawing.AnimationType.ScrollLeftToRightPush;
        }

        protected override void OnLoad(EventArgs e)
        {
            int nEditWidth = 330;
            int nEditHeight = 65;

            string path = Path.Combine(Application.StartupPath, Define.WeiboLogoPath);
            banner.LoadFromFile(path);
            banner.Location = new Point(5, 5);
            banner.Size = new Size(Define.WeiboLogoWidth, Define.WeiboLogoHeight);
            banner.PaintMode = MeizuSDK.Drawing.PaintMode.Normal;
            Controls.Add(banner);

            labelUserName.Location = new Point(5, 80);
            labelUserName.AutoSize = true;
            labelUserName.Text = L10n.UserName;
            Controls.Add(labelUserName);

            labelPassword.Location = new Point(5, 165);
            labelPassword.AutoSize = true;
            labelPassword.Text = L10n.Password;
            Controls.Add(labelPassword);

            textUserName.Location = new Point(140, 95);
            textUserName.Size = new Size(nEditWidth, nEditHeight);
            textUserName.SetSipMode(SipMode.Mail_Letter);
            Controls.Add(textUserName);

            textPasswrod.Location = new Point(140, 180);
            textPasswrod.Size = new Size(nEditWidth, nEditHeight);
            textUserName.SetSipMode(SipMode.Mail_Letter);
            Controls.Add(textPasswrod);

            // Toolbar
            toolbar.SetTextButton(ToolBarButtonIndex.LeftTextButton, true, true, L10n.Return);
            toolbar.SetTextButton(ToolBarButtonIndex.MiddleTextButton, true, true, L10n.Input);
            toolbar.SetTextButton(ToolBarButtonIndex.RightTextButton, true, true, L10n.Save);
            toolbar.ButtonClick += new EventHandler<ToolBar.ButtonEventArgs>(toolbar_ButtonClick);
            Controls.Add(toolbar);

            base.OnLoad(e);
        }

        protected virtual void toolbar_ButtonClick(object sender, ToolBar.ButtonEventArgs e)
        {
            if (e.Index == ToolBarButtonIndex.LeftTextButton)
            {
                if (IsDialog)
                {
                    DialogResult = DialogResult.No;
                    Close();
                }
                else
                    Close();
            }
            else if (e.Index == ToolBarButtonIndex.MiddleTextButton)
            {
                if (SipHelper.IsSipOpen)
                {
                    SipHelper.CloseSip();
                }
            }
            else if (e.Index == ToolBarButtonIndex.RightTextButton)
            {
                if (!CheckInput())
                    return;

                bool bSuccessed = SaveConfig();

                if (IsDialog)
                {
                    if (bSuccessed)
                        DialogResult = DialogResult.Yes;
                    else
                        DialogResult = DialogResult.No;

                    Close();
                }
                else
                    Close();
            }
        }

        protected bool CheckInput()
        {
            if (textUserName.Text.Length <= 0)
            {
                MessageBox.Show(L10n.UserNameIsNull);
                return false;
            }
            if (textPasswrod.Text.Length <= 0)
            {
                MessageBox.Show(L10n.PasswordIsNull);
                return false;
            }

            return true;
        }

        protected bool SaveConfig()
        {
            Options option = new Options();
            return option.SetSinaWeiboUserInfo(textUserName.Text, textPasswrod.Text);
        }
    }
}
