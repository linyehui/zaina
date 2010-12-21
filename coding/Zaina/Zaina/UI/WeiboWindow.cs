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
    class WeiboWindow : Form
    {
        ToolBar toolbar = new ToolBar();
        PictureBox banner = new PictureBox();
        MultilineTextBox multilineTextBox = new MultilineTextBox();
        Button btnResetPwd = new Button();

        string m_WeiboMsg = "";

        public WeiboWindow(string msg)
        {
            m_WeiboMsg = msg;

            this.AnimationIn = MeizuSDK.Drawing.AnimationType.ScrollRightToLeftPush;
            this.AnimationOut = MeizuSDK.Drawing.AnimationType.ScrollLeftToRightPush;
        }

        protected override void OnLoad(EventArgs e)
        {
            string path = Path.Combine(Application.StartupPath, Define.WeiboLogoPath);
            banner.LoadFromFile(path);
            banner.Location = new Point(5, 5);
            banner.Size = new Size(Define.WeiboLogoWidth, Define.WeiboLogoHeight);
            banner.PaintMode = MeizuSDK.Drawing.PaintMode.Normal;
            Controls.Add(banner);

            multilineTextBox.Location = new Point(5, 80);
            multilineTextBox.Text = m_WeiboMsg;
            multilineTextBox.Size = new Size(460, 260);
            multilineTextBox.EnableMagnifier = false;// 启用放大镜（注意：启用放大镜会使应用程序内存占用大于10MB，而且无法释放，因此建议不启用放大镜）
            multilineTextBox.SetSipMode(SipMode.GEL_PY);
            Controls.Add(multilineTextBox);

            btnResetPwd.Location = new Point(5, 500);
            btnResetPwd.Size = new Size(ClientSize.Width, 80);
            btnResetPwd.Text = L10n.ResetPwd;
            btnResetPwd.ForeColor = Color.BurlyWood;
            btnResetPwd.ButtonType = Button.Type.LineTopBottom;
            btnResetPwd.Click += new EventHandler(btnResetPwd_Click);
            Controls.Add(btnResetPwd);

            // Toolbar
            toolbar.SetTextButton(ToolBarButtonIndex.LeftTextButton, true, true, L10n.Return);
            toolbar.SetTextButton(ToolBarButtonIndex.MiddleTextButton, true, true, L10n.Input);
            toolbar.SetTextButton(ToolBarButtonIndex.RightTextButton, true, true, L10n.Send);
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
                    DialogResult = DialogResult.Yes;
                    Close();
                }
                else
                    Close();
            }
            else if (e.Index == ToolBarButtonIndex.MiddleTextButton)
            {
                CloseSip();                 
            }
            else if (e.Index == ToolBarButtonIndex.RightTextButton)
            {
                OnClickSend();
            }
        }

        void btnResetPwd_Click(object sender, EventArgs e)
        {
            WaitDialog.Begin(this);
            Options options = new Options();
            options.SetSinaWeiboUserInfo("", "");
            WaitDialog.End();
            MessageBox.Show(L10n.ResetPwdOK);
        }

        protected void CloseSip()
        {
            if (SipHelper.IsSipOpen)
            {
                SipHelper.CloseSip();
            } 
        }

        protected bool OnClickSend()
        {
            WaitDialog.Begin(this);
            CloseSip();

            Options option = new Options();
            string userName, password;
            if (!option.GetSinaWeiboUserInfo(out userName, out password)
                || userName.Length <= 0
                || password.Length <= 0)
            {
                // 获取用户名和密码失败，弹出设置对话框
                ConfigWindow configWin = new ConfigWindow();
                DialogResult nRet = configWin.ShowDialog(this);
                if (DialogResult.Yes == nRet)
                {
                    option.GetSinaWeiboUserInfo(out userName, out password);
                    UpdateWeibo(userName, password);
                }
                else
                {
                    MessageBox.Show(L10n.WeiboNoUserInfo);
                }
            }
            else
            {
                UpdateWeibo(userName, password);
            }
            WaitDialog.End();

            return true;
        }

        protected bool UpdateWeibo(string userName, string password)
        {
            Weibo bobo = new Weibo();
            if (!bobo.CheckString(multilineTextBox.Text))
            {
                MessageBox.Show(L10n.MsgTooLong);
                return false;
            }

            if (!bobo.Update(userName, password, multilineTextBox.Text))
            {
                MessageBox.Show(L10n.UpdateMsgFailed);
                return false;
            }

            MessageBox.Show(L10n.UpdateMsgSuccessed);
            return true;
        }
    }
}
