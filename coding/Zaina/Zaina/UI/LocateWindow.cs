using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Threading;
using MeizuSDK.Presentation;
using MeizuSDK.Drawing;

namespace Zaina
{
    class LocateWindow : Form
    {
        ToolBar toolbar = new ToolBar();
        PictureBox picBoxMap = new PictureBox();
        PictureBox picBoxCaption = new PictureBox();
        Label labelAddress = new Label();
        GStaticMap staticMap = new GStaticMap();

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

        static AutoResetEvent autoEvent = new AutoResetEvent(false);

        public LocateWindow()
        {
            this.AnimationIn = MeizuSDK.Drawing.AnimationType.ScrollRightToLeftPush;
            this.AnimationOut = MeizuSDK.Drawing.AnimationType.ScrollLeftToRightPush;
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

            labelAddress.Text = "";
            labelAddress.Location = new Point(0, Define.MapHeight + Define.AddressTitleHeight);
            labelAddress.VScroll = true;
            labelAddress.HScroll = true;
            labelAddress.Size = new Size(Width, Define.AddressHeight);
            Controls.Add(labelAddress);

            BuildGridMenu();

            toolbar.SetTextButton(ToolBarButtonIndex.LeftTextButton, true, true, L10n.Return);
            toolbar.SetTextButton(ToolBarButtonIndex.MiddleTextButton, true, true, L10n.MapOptions);
            toolbar.SetTextButton(ToolBarButtonIndex.RightTextButton, true, true, L10n.ReLocate);
            toolbar.ButtonClick += new EventHandler<ToolBar.ButtonEventArgs>(toolbar_ButtonClick);
            Controls.Add(toolbar);

            base.OnLoad(e);

            Locate();
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
                    menu.Show(HWnd, ToolBar.HEIGHT - 1);
                }

                return;
            }
            else if (e.Index == ToolBarButtonIndex.RightTextButton)
            {
                Locate();
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
                case Define.LocateGridMenuId_ZoomIn:
                    ZoomIn();
                    break;
                case Define.LocateGridMenuId_ZoomOut:
                    ZoomOut();
                    break;
                case Define.LocateGridMenuId_MapType:
                    SwitchMapType();
                    break;
                case Define.LocateGridMenuId_SendSMS:
                    break;
                default:
                    break;
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

            GridMenuItem item = new GridMenuItem(Define.LocateGridMenuId_ZoomIn, L10n.MapZoomIn, img1, img1Pressed);
            menu.Items.Add(item);
            item = new GridMenuItem(Define.LocateGridMenuId_ZoomOut, L10n.MapZoomOut, img2, img2Pressed);
            menu.Items.Add(item);

            if (staticMap.ShowSatellite)
                item = new GridMenuItem(Define.LocateGridMenuId_MapType, L10n.RoadMap, img3, img3Pressed);
            else
                item = new GridMenuItem(Define.LocateGridMenuId_MapType, L10n.Satellite, img3, img3Pressed);
            menu.Items.Add(item);

            item = new GridMenuItem(Define.LocateGridMenuId_SendSMS, L10n.SendSMS, img4, img4Pressed);
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
            staticMap.ShowSatellite = !staticMap.ShowSatellite;
            BuildGridMenu();
            ThreadPool.QueueUserWorkItem(new WaitCallback(MultithreadRebuildMap), autoEvent);
            autoEvent.WaitOne(Timeout.Infinite, false);
            WaitDialog.End();
        }

        private void MultithreadLocate(object stateInfo)
        {
            try
            {
                double lat = 0;
                double lng = 0;
                string address = "";
                CGeolocation.locate_GoogleGearsAPI(out lat, out lng);
                CGeolocation.getLocations_GoogleGearsAPI(lat, lng, out address);
                labelAddress.Text = address;

                string mapFileName = staticMap.GenMap(lat, lng);
                picBoxMap.LoadFromFile(mapFileName);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                MessageBox.Show(ex.Message);
            }
            finally
            {
                ((AutoResetEvent)stateInfo).Set();
            }
        }
    }
}
