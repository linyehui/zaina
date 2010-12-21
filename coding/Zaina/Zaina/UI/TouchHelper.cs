//////////////////////////////////////////////////////////////////////////
// Author: linyehui
// 2010-12-11 16:15:26
// 简单的手势处理
//////////////////////////////////////////////////////////////////////////

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using MeizuSDK.Presentation;
using System.Threading;

namespace Zaina
{
    public class ToucherHelper
    {
        //////////////////////////////////////////////////////////////////////////
        // 变量定义

        private const int nMinTouchDelta = 30;
        public enum TouchGesture
        {
            None = 0,
            Left,
            Right,
            Up,
            Down,
            ZoomIn,
            ZoomOut,
        }

        /// <summary>
        /// 手势回调
        /// </summary>
        public class TouchHelperEventArgs : EventArgs
        {
            public TouchHelperEventArgs(TouchGesture gesture, object arg)
            {
                Gesture = gesture;
                Arg = arg;
            }
            public TouchGesture Gesture;
            public object Arg;
        }

        //////////////////////////////////////////////////////////////////////////
        // 成员变量

        public event EventHandler<TouchHelperEventArgs> TouchGestureEvent;
        public List<Form.TouchData> ToucheDatas1st = new List<Form.TouchData>();
        public List<Form.TouchData> ToucheDatas2nd = new List<Form.TouchData>();

        private System.Threading.Timer timerCheckGesture;
        //////////////////////////////////////////////////////////////////////////
        // 成员函数

        public void Add(Form.TouchData data1)
        {
            if (ToucheDatas1st.Count <= 0)
            {
                timerCheckGesture = new System.Threading.Timer(new TimerCallback(timerCall), this, 1000, 0);
            }

            ToucheDatas1st.Add(data1);
        }

        public void Add(Form.TouchData data1, Form.TouchData data2)
        {
            if (ToucheDatas1st.Count <= 0)
            {
                timerCheckGesture = new System.Threading.Timer(new TimerCallback(timerCall), this, 1000, 0);
            }

            ToucheDatas1st.Add(data1);
            ToucheDatas2nd.Add(data2);
        }

        private void timerCall(object obj)
        {
            timerCheckGesture.Dispose();

            CheckGesture();

            ToucheDatas1st.Clear();
            ToucheDatas2nd.Clear();
        }

        private void CheckGesture()
        {
            TouchGesture result = TouchGesture.None;

            if (ToucheDatas1st.Count < 2)
                result = TouchGesture.None;
            else if (ToucheDatas2nd.Count < 2)
                result = GenGesture(ToucheDatas1st);
            else
            {
                result = GenMutiGesture(ToucheDatas1st, ToucheDatas2nd);
            }
                
            if (TouchGestureEvent != null)
                TouchGestureEvent(this, new TouchHelperEventArgs(result, 0));
        }

        TouchGesture GenGesture(List<Form.TouchData> datas)
        {
            if (datas.Count < 2)
            {
                return TouchGesture.None;
            }

            int xDelta = datas[datas.Count - 1].X - datas[0].X;
            int yDelta = datas[datas.Count - 1].Y - datas[0].Y;
            if (Math.Abs(xDelta) >= Math.Abs(yDelta))
            {
                if (nMinTouchDelta >= Math.Abs(xDelta))
                    return TouchGesture.None;
                else if (xDelta < 0)
                    return TouchGesture.Left;
                else if (xDelta > 0)
                    return TouchGesture.Right;
                else
                    return TouchGesture.None;
            }
            else
            {
                if (nMinTouchDelta >= Math.Abs(yDelta))
                    return TouchGesture.None;
                else if (yDelta < 0)
                    return TouchGesture.Up;
                else if (yDelta > 0)
                    return TouchGesture.Down;
                else
                    return TouchGesture.None;
            }            
        }

        TouchGesture GenMutiGesture(List<Form.TouchData> datas1, List<Form.TouchData> datas2)
        {
            int minLastIndex = Math.Min(datas1.Count, datas2.Count) - 1;

            // 最开始的直线距离
            int nStartPointDeltaX = Math.Abs(datas1[0].X - datas2[0].X);
            int nStartPointDeltaY = Math.Abs(datas1[0].Y - datas2[0].Y);
            int nStartLineDelta = nStartPointDeltaX * nStartPointDeltaX + nStartPointDeltaY * nStartPointDeltaY;

            // 最后的直线距离
            int nEndPointDeltaX = Math.Abs(datas1[minLastIndex].X - datas2[minLastIndex].X);
            int nEndPointDeltaY = Math.Abs(datas1[minLastIndex].Y - datas2[minLastIndex].Y);
            int nEndLineDelta = nEndPointDeltaX * nEndPointDeltaX + nEndPointDeltaY * nEndPointDeltaY;

            if (nStartLineDelta > nEndLineDelta)
                return TouchGesture.ZoomOut;
            else if (nStartLineDelta < nEndLineDelta)
                return TouchGesture.ZoomIn;
            else
                return TouchGesture.None;
        }
    }
}
