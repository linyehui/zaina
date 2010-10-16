using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using MeizuSDK.Presentation;
using MeizuSDK.Drawing;

namespace Zaina
{
    class LocateWindow : Form
    {
        ToolBar toolbar = new ToolBar();
        PictureBox map = new PictureBox();
        PictureBox Caption = new PictureBox();
        Label Address = new Label();
        Boolean ShowSatellite = false;

        private ImageContainer imgContainer = new ImageContainer();

        private ImagingHelper img1;
        private ImagingHelper img1Pressed;
        private ImagingHelper img2;
        private ImagingHelper img2Pressed;
        private ImagingHelper img3;
        private ImagingHelper img3Pressed;
        private ImagingHelper img4;
        private ImagingHelper img4Pressed;

        private GridMenu menu;

        public LocateWindow()
        {
            this.AnimationIn = MeizuSDK.Drawing.AnimationType.ScrollRightToLeftPush;
            this.AnimationOut = MeizuSDK.Drawing.AnimationType.ScrollLeftToRightPush;
        }

        protected override void OnLoad(EventArgs e)
        {
            string path = Path.Combine(Application.StartupPath, Define.DefaultMapPath);
            map.LoadFromFile(path);
            map.Location = new Point(0, 0);
            map.Size = new Size(Width, Define.MapHeight);
            map.PaintMode = MeizuSDK.Drawing.PaintMode.Normal;
            Controls.Add(map);

            path = Path.Combine(Application.StartupPath, Define.AddressTitlePath);
            Caption.LoadFromFile(path);
            Caption.Location = new Point(0, Define.MapHeight);
            Caption.Size = new Size(Width, Define.AddressTitleHeight);
            Caption.PaintMode = MeizuSDK.Drawing.PaintMode.Normal;
            Controls.Add(Caption);

            Address.Text = "珠海市科技创新海岸魅族科技楼";
            Address.Location = new Point(0, Define.MapHeight + Define.AddressTitleHeight);
            Address.VScroll = true;
            Address.HScroll = true;
            Address.Size = new Size(Width, Define.AddressHeight);
            Controls.Add(Address);

            BuildGridMenu();

            toolbar.SetTextButton(ToolBarButtonIndex.LeftTextButton, true, true, L10n.Return);
            toolbar.SetTextButton(ToolBarButtonIndex.MiddleTextButton, true, true, L10n.MapOptions);
            toolbar.SetTextButton(ToolBarButtonIndex.RightTextButton, true, true, L10n.ReLocate);
            toolbar.ButtonClick += new EventHandler<ToolBar.ButtonEventArgs>(toolbar_ButtonClick);
            Controls.Add(toolbar);

            base.OnLoad(e);
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
                if (menu.IsContinue())
                {
                    menu.Close();
                }
                else
                {
                    menu.Show(HWnd, ToolBar.HEIGHT);
                }

                return;
            }

            if (menu.IsContinue())
            {
                menu.Close();
            }
        }

        protected override void OnMzCommand(uint wParam, uint lParam)
        {
            int id = (int)NativeMethods.LOWORD(wParam);
            GridMenuItem item = menu.Items[id];

            switch (id)
            {
                case Define.LocateGridMenuId_ZoonIn:
                    break;
                case Define.LocateGridMenuId_ZoonOut:
                    break;
                case Define.LocateGridMenuId_MapType:
                    {
                        ShowSatellite = !ShowSatellite;
                        BuildGridMenu();
                    }
                    break;
                case Define.LocateGridMenuId_SendSMS:
                    break;
                default:
                    break;
            }

            if (item != null)
            {
                MessageBox.Show(string.Format("您点击了ID为[{0}]的菜单项[{1}]。", item.ItemId, item.Text), "GridMenu");
            }

            base.OnMzCommand(wParam, lParam);
        }

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

            GridMenuItem item = new GridMenuItem(Define.LocateGridMenuId_ZoonIn, L10n.MapZoomIn, img1, img1Pressed);
            menu.Items.Add(item);
            item = new GridMenuItem(Define.LocateGridMenuId_ZoonOut, L10n.MapZoomOut, img2, img2Pressed);
            menu.Items.Add(item);
            
            if (ShowSatellite)
                item = new GridMenuItem(Define.LocateGridMenuId_MapType, L10n.RoadMap, img3, img3Pressed);
            else
                item = new GridMenuItem(Define.LocateGridMenuId_MapType, L10n.Satellite, img3, img3Pressed);
            menu.Items.Add(item);

            item = new GridMenuItem(Define.LocateGridMenuId_SendSMS, L10n.SendSMS, img4, img4Pressed);
            menu.Items.Add(item);

            Controls.Add(menu);
        }
    }
}
