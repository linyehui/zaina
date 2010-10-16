using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

public class RIL
{
    //CellTower信息
    private static string celltowerinfo = "";

    //通过RIL获取CellID
    public static string GetCellTowerInfo()
    {
        //初始化句柄
        IntPtr hRil = IntPtr.Zero;
        IntPtr hRes = IntPtr.Zero;

        //初始化结果变量
        celltowerinfo = "";

        //为一个Client初始化RIL    
        hRes = RIL_Initialize(1,
                           new RILRESULTCALLBACK(rilResultCallBack),
                           null,
                           0,
                           0,
                           out hRil);

        if (hRes != IntPtr.Zero)
        {
            return "不能初始化RIL";
        }

        //获取当前Phone使用的基站信息
        hRes = RIL_GetCellTowerInfo(hRil);

        waithandle.WaitOne();

        //解除RIL
        RIL_Deinitialize(hRil);

        return celltowerinfo;


    }

    private static AutoResetEvent waithandle = new AutoResetEvent(false);

    public static void rilResultCallBack(uint dwCode,
                                         IntPtr hrCmdID,
                                         IntPtr lpData,
                                         uint cdData,
                                         uint dwParam)
    {
        RILCELLTOWERINFO rilCellTowerInfo = new RILCELLTOWERINFO();

        //将数据lpData从非托管内存块封送到rilCellTowerInfo托管对象中
        Marshal.PtrToStructure(lpData, rilCellTowerInfo);

        celltowerinfo = rilCellTowerInfo.dwCellID + "-" + rilCellTowerInfo.dwLocationAreaCode + "-" +
                        rilCellTowerInfo.dwMobileCountryCode + "-" + rilCellTowerInfo.dwMobileNetworkCode;

        //将事件状态设置为终止状态，允许一个或多个等待线程继续
        waithandle.Set();
    }

    public delegate void RILRESULTCALLBACK(uint dwCode, IntPtr hrCmdID, IntPtr lpData, uint cbData, uint dwParam);

    public delegate void RILNOTIFYCALLBACK(uint dwCode, IntPtr lpData, uint cbData, uint dwParam);

    //RIL基站信息类
    public class RILCELLTOWERINFO
    {
        public uint cbSize;
        public uint dwParams;
        public uint dwMobileCountryCode;
        public uint dwMobileNetworkCode;
        public uint dwLocationAreaCode;
        public uint dwCellID;
        public uint dwBaseStationID;
        public uint dwBroadcastControlChannel;
        public uint dwRxLevel;
        public uint dwRxLevelFull;
        public uint dwRxLevelSub;
        public uint dwRxQuality;
        public uint dwRxQualityFull;
        public uint dwRxQualitySub;
        public uint dwIdleTimeSlot;
        public uint dwTimingAdvance;
        public uint dwGPRSCellID;
        public uint dwGPRSBaseStationID;
        public uint dwNumBCCH;


    }

    /* 调用API    
     * 初始化RIL    
     * MSDN:   http://msdn.microsoft.com/zh-cn/library/aa919106(en-us).aspx */
    [DllImport("ril.dll")]
    private static extern IntPtr RIL_Initialize(uint dwIndex, RILRESULTCALLBACK pfnResult, RILNOTIFYCALLBACK pfnNotify, uint dwNotificationClasses, uint dwParam, out IntPtr lphRil);
    [DllImport("ril.dll")]
    private static extern IntPtr RIL_GetCellTowerInfo(IntPtr hRil);
    [DllImport("ril.dll")]
    private static extern IntPtr RIL_Deinitialize(IntPtr hRil);

}