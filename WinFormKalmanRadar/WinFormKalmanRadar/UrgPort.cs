using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.IO.Ports;
using SCIP_library;
using System.Collections.Concurrent;

namespace WinFormKalmanRadar {
    class UrgPort : IPort {

        private static SerialPort basePort;
        private static UrgPort urgPort;

        // 定时接收数据
        private static System.Timers.Timer timer = new System.Timers.Timer(100);

        //起始和终止位置
        private const int startStep = 0;
        private const int endStep = 760;
        //数据记录
        private static long timeStamp;
        private static List<long> distance = new List<long>();

        public static bool isClosing = false;
        public static bool isReading = false;

        public delegate void processData(List<long> list);
        public static processData processDel;

        // 单例模式，使用getInstance获取实例
        private UrgPort() {

        }
        private UrgPort(String portName) {
            basePort = new SerialPort(portName, 115200);
            timer.Interval = 1000;
            timer.Elapsed += timer_Elapsed;
            timer.AutoReset = true;
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            if (isClosing) {
                return;
            }
            isReading = true;
            // 接收数据
            string receiveData = basePort.ReadLine();
            // 清除缓存
            basePort.DiscardInBuffer();
            lock (distance) {
                if (!SCIP_Reader.MD(receiveData, ref timeStamp, ref distance)) {
                    //Console.WriteLine(receiveData);
                    return;
                }
            }
            isReading = false;
            if (processDel != null) {
                processDel(getDistance());
            }
        }

        // 返回可操作的distance数组
        private static List<long> disCopy;
        public static List<long> getDistance() {
            lock (distance) {
                disCopy = new List<long>(distance);
            }
            return disCopy;
        }

        void basePort_DataReceived(object sender, SerialDataReceivedEventArgs e) {
            
        }

        /// <summary>
        /// 获取激光雷达实例
        /// </summary>
        /// <param name="portName"></param>
        /// <returns></returns>
        public static UrgPort getInstance(string portName) {
            if (urgPort == null) {
                urgPort = new UrgPort(portName);
            }
            return urgPort;
        }

        /// <summary>
        /// 打开串口方法
        /// </summary>
        /// <returns>串口打开是否成功</returns>
        public bool openPort() {
            // 串口打开返回成功
            basePort.NewLine = "\n\n";
            if (basePort.IsOpen) {
                isClosing = false;
                timer.Enabled = true;
                return true;
            }
            // 尝试打开串口
            try {
                basePort.Open();
            }
            catch {
                return false;
            }
            // 写入读取数据命令
            basePort.Write(SCIP_Writer.SCIP2());
            basePort.ReadLine();
            basePort.Write(SCIP_Writer.MD(startStep, endStep));
            basePort.ReadLine();
            basePort.DataReceived += basePort_DataReceived;
            isClosing = false;
            timer.Enabled = true;
            return true;
        }

        /// <summary>
        /// 关闭激光雷达串口
        /// </summary>
        /// <returns>返回关闭激光雷达是否成功</returns>
        public bool closePort() {
            // 串口已经关闭返回成功
            if (!basePort.IsOpen) {
                return true;
            }
            isClosing = true;
            Console.WriteLine("Here");
            // 写入停止读取数据命令
            basePort.DataReceived -= basePort_DataReceived;

            timer.Enabled = false;

            // 100ms后关闭激光雷达串口
            while (isReading) {
                System.Windows.Forms.Application.DoEvents();
            }
            basePort.Write(SCIP_Writer.QT());
            basePort.ReadLine();
            // 尝试关闭串口
            try {
                basePort.Close();
            }
            catch {
                return false;
            }
            return true;
        }


    }
}