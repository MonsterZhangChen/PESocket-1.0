/****************************************************
	文件：PETool.cs
	作者：Plane
	邮箱: 1785275942@qq.com
	日期：2018/10/30 11:21   	
	功能：工具类
*****************************************************/

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace PENet {
    public class PETool {

        public static byte[] PackNetMsg<T>(T msg) where T : PEMsg {
            return PackLenInfo(Serialize(msg));
        }

        /// <summary>
        /// 将一个字节数组转换为带有自身长度的信息的字节数组并返回
        /// </summary>
        public static byte[] PackLenInfo(byte[] data) {
            int len = data.Length;
            byte[] pkg = new byte[len + 4];
            byte[] head = BitConverter.GetBytes(len);
            head.CopyTo(pkg, 0);
            data.CopyTo(pkg, 4);
            return pkg;
        }

        /// <summary>
        /// 将一个PEMsg实例转换成一个字节数组
        /// </summary>
        /// <typeparam name="T">类型参数，PEMsg的子类型</typeparam>
        /// <param name="pkg">要转换的对象</param>
        /// <returns></returns>
        public static byte[] Serialize<T>(T pkg) where T : PEMsg {
            using (MemoryStream ms = new MemoryStream()) {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, pkg);
                ms.Seek(0, SeekOrigin.Begin);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// 将字节数组转换为消息的实例
        /// </summary>
        /// <typeparam name="T">消息的类型,它是PEMsg的子类</typeparam>
        /// <param name="bs">消息的序列化字节流</param>
        /// <returns></returns>
        public static T DeSerialize<T>(byte[] bs) where T : PEMsg {
            using (MemoryStream ms = new MemoryStream(bs)) {
                BinaryFormatter bf = new BinaryFormatter();
                T pkg = (T)bf.Deserialize(ms);
                return pkg;
            }
        }

        #region Log
        public static bool log = true;
        public static Action<string, int> logCB = null;

        /// <summary>
        /// 打印日志；它会先给消息加一个时间前缀
        /// 如果委托成员logCB为空，调用控制台打印，否则调用logCB的逻辑；
        /// logCB由聚合它的PESocket()的SetLog方法设置
        /// </summary>
        /// <param name="msg">日志信息的内容</param>
        /// <param name="lv">日志信息的级别</param>
        public static void LogMsg(string msg, LogLevel lv = LogLevel.None) {
            if (log != true) {
                return;
            }
            //Add Time Stamp
            msg = DateTime.Now.ToLongTimeString() + " >> " + msg;
            if (logCB != null) {
                logCB(msg, (int)lv);
            }
            else {
                if (lv == LogLevel.None) {
                    Console.WriteLine(msg);
                }
                else if (lv == LogLevel.Warn) {
                    //Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("//--------------------Warn--------------------//");
                    Console.WriteLine(msg);
                    //Console.ForegroundColor = ConsoleColor.Gray;
                }
                else if (lv == LogLevel.Error) {
                    //Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("//--------------------Error--------------------//");
                    Console.WriteLine(msg);
                    //Console.ForegroundColor = ConsoleColor.Gray;
                }
                else if (lv == LogLevel.Info) {
                    //Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("//--------------------Info--------------------//");
                    Console.WriteLine(msg);
                    //Console.ForegroundColor = ConsoleColor.Gray;
                }
                else {
                    //Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("//--------------------Error--------------------//");
                    Console.WriteLine(msg + " >> Unknow Log Type\n");
                    //Console.ForegroundColor = ConsoleColor.Gray;
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// Log Level
    /// </summary>
    public enum LogLevel {
        None = 0,// None
        Warn = 1,//Yellow
        Error = 2,//Red
        Info = 3//Green
    }
}