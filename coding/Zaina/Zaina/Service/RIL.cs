using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

public class RIL
{
    //CellTower��Ϣ
    private static string celltowerinfo = "";

    //ͨ��RIL��ȡCellID
    public static string GetCellTowerInfo()
    {
        //��ʼ�����
        IntPtr hRil = IntPtr.Zero;
        IntPtr hRes = IntPtr.Zero;

        //��ʼ���������
        celltowerinfo = "";

        //Ϊһ��Client��ʼ��RIL    
        hRes = RIL_Initialize(1,
                           new RILRESULTCALLBACK(rilResultCallBack),
                           null,
                           0,
                           0,
                           out hRil);

        if (hRes != IntPtr.Zero)
        {
            return "���ܳ�ʼ��RIL";
        }

        //��ȡ��ǰPhoneʹ�õĻ�վ��Ϣ
        hRes = RIL_GetCellTowerInfo(hRil);

        waithandle.WaitOne();

        //���RIL
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

        //������lpData�ӷ��й��ڴ����͵�rilCellTowerInfo�йܶ�����
        Marshal.PtrToStructure(lpData, rilCellTowerInfo);

        celltowerinfo = rilCellTowerInfo.dwCellID + "-" + rilCellTowerInfo.dwLocationAreaCode + "-" +
                        rilCellTowerInfo.dwMobileCountryCode + "-" + rilCellTowerInfo.dwMobileNetworkCode;

        //���¼�״̬����Ϊ��ֹ״̬������һ�������ȴ��̼߳���
        waithandle.Set();
    }

    public delegate void RILRESULTCALLBACK(uint dwCode, IntPtr hrCmdID, IntPtr lpData, uint cbData, uint dwParam);

    public delegate void RILNOTIFYCALLBACK(uint dwCode, IntPtr lpData, uint cbData, uint dwParam);

    //RIL��վ��Ϣ��
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

    /* ����API    
     * ��ʼ��RIL    
     * MSDN:   http://msdn.microsoft.com/zh-cn/library/aa919106(en-us).aspx */
    [DllImport("ril.dll")]
    private static extern IntPtr RIL_Initialize(uint dwIndex, RILRESULTCALLBACK pfnResult, RILNOTIFYCALLBACK pfnNotify, uint dwNotificationClasses, uint dwParam, out IntPtr lphRil);
    [DllImport("ril.dll")]
    private static extern IntPtr RIL_GetCellTowerInfo(IntPtr hRil);
    [DllImport("ril.dll")]
    private static extern IntPtr RIL_Deinitialize(IntPtr hRil);

}