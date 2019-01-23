using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
//using System.Windows.Forms;

namespace test
{
    /// <summary>
    /// 1、设置默认参数（波特率、包大小、安全等级）
    /// 2、打开设备
    /// 3、录入指纹
    /// 4、搜索指纹
    /// 5、读取指纹库（FingerID）
    /// 6、删除指纹
    /// 7、取消操作
    /// 8、关闭设备
    /// </summary>
    public class SynoAPIExHelper
    {
        private IntPtr pHandle = IntPtr.Zero;
        private int nAddr = 1;

        private ReturnValue ConvertRV(int rv)
        {
            return (ReturnValue)rv;
        }
        /// <summary>
        /// 打开指纹设备
        /// </summary>
        /// <returns>执行结果</returns>
        public ReturnValue OpenDevice()
        {
            int n = PSOpenDeviceEx(out pHandle, DEVICE_UDisk, nAddr);
            return ConvertRV(n);
        }
        /// <summary>
        /// 关闭指纹设备
        /// </summary>
        /// <returns>执行结果</returns>
        public ReturnValue CloseDevice()
        {
            int n = 0;
            if (pHandle != IntPtr.Zero)
            {
                n = PSCloseDeviceEx(pHandle);
            }
            pHandle = IntPtr.Zero;
            return ConvertRV(n);
        }


        /// <summary>
        /// 获取有效指纹个数
        /// </summary>
        /// <param name="num">指纹个数</param>
        /// <returns>执行结果</returns>
        public ReturnValue GetFingerNum(out int num)
        {
            num = 0;
            int n = PSTemplateNum(pHandle, nAddr, out num);
            return ConvertRV(n);
        }
        /// <summary>
        /// 清空所有指纹
        /// </summary>
        /// <returns>执行结果</returns>
        public ReturnValue ClearAllFinger()
        {
            int n = PSEmpty(pHandle, nAddr);
            return ConvertRV(n);
        }
        /// <summary>
        /// 删除指纹库中的一个指纹
        /// </summary>
        /// <param name="figerID">要删除的指纹id</param>
        /// <returns>执行结果</returns>
        public ReturnValue DelFinger(int figerID)
        {
            int n = PSDelChar(pHandle, nAddr, figerID, 1);
            return ConvertRV(n);
        }
        /// <summary>
        /// 录入指纹
        /// </summary>
        /// <param name="figerID">指纹id号</param>
        /// <returns>执行结果</returns>
        public ReturnValue AddFinger(out int figerID)
        {
            figerID = 0;
            int n = PSEnroll(pHandle, nAddr, out figerID);
            return ConvertRV(n);
        }
        /// <summary>
        /// 查找指纹库中的一个指纹
        /// </summary>
        /// <param name="figerID">要删除的指纹id</param>
        /// <returns>执行结果</returns>
        public ReturnValue FindFinger(out int figerID)
        {
            int n = PSIdentify(pHandle, nAddr, out figerID);
            return ConvertRV(n);
        }
        /// <summary>
        /// 获取现有指纹库
        /// </summary>
        /// <param name="figerIDList">现有指纹id列表</param>
        /// <returns></returns>
        public ReturnValue GetAllFinger(out List<int> figerIDList)
        {
            figerIDList = new List<int>();
            int num = 0;
            var n = GetFingerNum(out num);
            if (n != ReturnValue.PS_OK)
            {
                return n;
            }
            else
            {
                int index = 0;
                for (int i = 0; i < 4; i++)
                {
                    IndexTable_STATUS userContent;
                    var nn = PSReadIndexTable(pHandle, nAddr, i, out userContent);
                    if (nn == (int)ReturnValue.PS_OK)
                    {
                        foreach (byte item in userContent.UserContent)
                        {
                            string str = Convert.ToString(item, 2);
                            for (int j = str.Length - 1; j >= 0; j--)
                            {
                                if (str[j] == '1')
                                {
                                    figerIDList.Add(index);
                                }
                                index++;
                            }
                        }
                    }
                }
                return n;
            }
        }
        /// <summary>
        /// 保存当前指纹图片
        /// </summary>
        /// <param name="path">保存指纹图片的路径</param>
        /// <returns>执行结果</returns>
        /// 
        public int n;
        public unsafe ReturnValue SaveFigerBmp(string path)
        {
                n = PSGetImage(pHandle, nAddr);
                if (n == (int)ReturnValue.PS_OK)
                {
               
                    int t = 0;
                    byte[] data = new byte[256 * 288];
                    fixed (byte* array = data)
                    {
                        n = PSUpImage(pHandle, nAddr, array, out t);
                        if (n == (int)ReturnValue.PS_OK)
                        {
                            n = PSImgData2BMP(array, path);
                            // MessageBox.Show("Image Saved");
                        }
                        else
                        {
                            n = PSImgData2BMP(array, path);
                        }
                    }
                
                }
                else {
                    // MessageBox.Show("NO Detection");
                }
                return ConvertRV(n);
            
        }
        #region api定义
        private static int PS_MAXWAITTIME = 2000;
        private static int DELAY_TIME = 150;
        ///////////////缓冲区//////////////////////////////
        private static int CHAR_BUFFER_A = 0x01;
        private static int CHAR_BUFFER_B = 0x02;
        private static int MODEL_BUFFER = 0x03;
        //////////////////串口号////////////////////////
        private static int COM1 = 0x01;
        private static int COM2 = 0x02;
        private static int COM3 = 0x03;
        //////////////////波特率////////////////////////
        //4API 函数接口库使用手册
        private static int BAUD_RATE_9600 = 0x01;
        private static int BAUD_RATE_19200 = 0x02;
        private static int BAUD_RATE_38400 = 0x04;
        private static int BAUD_RATE_57600 = 0x06;//default
        private static int BAUD_RATE_115200 = 0x0C;
        private static int MAX_PACKAGE_SIZE_ = 350;// 数据包最大长度
        private static int CHAR_LEN_AES1711 = 1024;// 512->1024 [2009.11.12] AES1711使用大小模版
        private static int CHAR_LEN_NORMAL = 512;// 512 通用版本使用大小的模版
        private static int DEVICE_USB = 0;
        private static int DEVICE_COM = 1;
        private static int DEVICE_UDisk = 2;
        private static int IMAGE_X = 256;
        private static int IMAGE_Y = 288;
        #endregion

        #region api Fuc
        [DllImport("SynoAPIEx.dll", CharSet = CharSet.Unicode)]
        private static extern int PSOpenDeviceEx(out IntPtr pHandle, int nDeviceType, int iCom = 1, int iBaud = 1, int nPackageSize = 2, int iDevNum = 0);
        [DllImport("SynoAPIEx.dll", CharSet = CharSet.Unicode)]
        private static extern int PSCloseDeviceEx(IntPtr hHandle);
        [DllImport("SynoAPIEx.dll", CharSet = CharSet.Unicode)]
        private static extern int PSGetImage(IntPtr hHandle, int nAddr);
        [DllImport("SynoAPIEx.dll", CharSet = CharSet.Ansi)]
        private static extern unsafe int PSUpImage(IntPtr hHandle, int nAddr, byte* pImageData, out int pFileName);
        [DllImport("SynoAPIEx.dll", CharSet = CharSet.Ansi)]
        private static extern unsafe int PSImgData2BMP(byte* pImageData, string pImageFile);
        [DllImport("SynoAPIEx.dll", CharSet = CharSet.Unicode)]
        private static extern int PSReadIndexTable(IntPtr hHandle, int nAddr, int nPage, out IndexTable_STATUS UserContent);
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
        private struct IndexTable_STATUS
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] UserContent;
        };
        [DllImport("SynoAPIEx.dll", CharSet = CharSet.Unicode)]
        private static extern int PSEmpty(IntPtr hHandle, int nAddr);
        [DllImport("SynoAPIEx.dll", CharSet = CharSet.Unicode)]
        private static extern int PSTemplateNum(IntPtr hHandle, int nAddr, out int iMbNum);
        [DllImport("SynoAPIEx.dll", CharSet = CharSet.Unicode)]
        private static extern int PSDelChar(IntPtr hHandle, int nAddr, int iStartPageID, int nDelPageNum);
        [DllImport("SynoAPIEx.dll", CharSet = CharSet.Unicode)]
        private static extern int PSEnroll(IntPtr hHandle, int nAddr, out int nID);
        [DllImport("SynoAPIEx.dll", CharSet = CharSet.Unicode)]
        private static extern int PSIdentify(IntPtr hHandle, int nAddr, out int iMbAddress);
        #endregion
    }
    /// <summary>
    /// 错误返回码
    /// </summary>
    public enum ReturnValue
    {
        PS_OK = 0x00,
        PS_COMM_ERR = 0x01,
        PS_NO_FINGER = 0x02,
        PS_GET_IMG_ERR = 0x03,
        PS_FP_TOO_DRY = 0x04,
        PS_FP_TOO_WET = 0x05,
        PS_FP_DISORDER = 0x06,
        PS_LITTLE_FEATURE = 0x07,
        PS_NOT_MATCH = 0x08,
        PS_NOT_SEARCHED = 0x09,
        PS_MERGE_ERR = 0x0a,
        PS_ADDRESS_OVER = 0x0b,
        PS_READ_ERR = 0x0c,
        PS_UP_TEMP_ERR = 0x0d,
        PS_RECV_ERR = 0x0e,
        PS_UP_IMG_ERR = 0x0f,
        PS_DEL_TEMP_ERR = 0x10,
        PS_CLEAR_TEMP_ERR = 0x11,
        PS_SLEEP_ERR = 0x12,
        PS_INVALID_PASSWORD = 0x13,
        PS_RESET_ERR = 0x14,
        PS_INVALID_IMAGE = 0x15,
        PS_HANGOVER_UNREMOVE = 0X17,
    }
}
