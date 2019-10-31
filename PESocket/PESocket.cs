/****************************************************
	文件：PESocket.cs
	作者：Plane
	邮箱: 1785275942@qq.com
	日期：2018/10/30 11:20   	
	功能：PESocekt核心类
*****************************************************/

using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace PENet {
    /// <summary>
    /// 网络传输的发起与接收单位，它可以担任客户端或者服务端的角色；
    /// 它包含了传输套接字和网络回话
    /// 使用IPV4地址族，Stream消息类型，Tcp协议
    /// </summary>
    /// <typeparam name="T">网络会话的类型</typeparam>
    /// <typeparam name="K">网络消息的类型</typeparam>
    public class PESocket<T, K>
        where T : PESession<K>, new()
        where K : PEMsg {
        private Socket skt = null;
        public T session = null;
        public int backlog = 10;
        List<T> sessionLst = new List<T>();

        public PESocket() {
            skt = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        #region Server
        /// <summary>
        /// 以客户端的形式启动套接字
        /// 1.进行绑定，Listen和异步Sccept()的常规操作
        /// 2.如果1成功,为Settion创建一个实例,并开始异步的接收数据;如果失败，Settion保持为空
        /// </summary>
        public void StartAsServer(string ip, int port) {
            try {
                skt.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
                skt.Listen(backlog);
                skt.BeginAccept(new AsyncCallback(ClientConnectCB), skt);
                PETool.LogMsg("\nServer Start Success!\nWaiting for Connecting......", LogLevel.Info);
            }
            catch (Exception e) {
                PETool.LogMsg(e.Message, LogLevel.Error);
            }
        }

        void ClientConnectCB(IAsyncResult ar) {
            try {
                Socket clientSkt = skt.EndAccept(ar);
                T session = new T();
                session.StartRcvData(clientSkt, () => {
                    if (sessionLst.Contains(session)) {
                        sessionLst.Remove(session);
                    }
                });
                sessionLst.Add(session);
            }
            catch (Exception e) {
                PETool.LogMsg(e.Message, LogLevel.Error);
            }
            skt.BeginAccept(new AsyncCallback(ClientConnectCB), skt);
        }
        #endregion

        #region Client
        /// <summary>
        /// 以客户端身份启动套接字：
        /// 1.发送一个异步的远程终端连接请求；
        /// 2.如果1成功，则将实例化Sesion对象，并开始异步的接收数据；如果连接失败，例如远程主机未开启，则Settion值会保持为null
        /// 这个方法中包含了连接服务端的逻辑,如果连接失败，session值将保持空值
        /// </summary>
        public void StartAsClient(string ip, int port) {
            try {
                skt.BeginConnect(new IPEndPoint(IPAddress.Parse(ip), port), new AsyncCallback(ServerConnectCB), skt);
                PETool.LogMsg("\nClient Start Success!\nConnecting To Server......", LogLevel.Info);
            }
            catch (Exception e) {
                PETool.LogMsg(e.Message, LogLevel.Error);
            }
        }

        void ServerConnectCB(IAsyncResult ar) {
            try {
                skt.EndConnect(ar);
                session = new T();
                session.StartRcvData(skt, null);
            }
            catch (Exception e) {
                PETool.LogMsg(e.Message, LogLevel.Error);
            }
        }
        #endregion

        public void Close() {
            if (skt != null) {
                skt.Close();
            }
        }

        /// <summary>
        /// Log
        /// </summary>
        /// <param name="log">log switch</param>
        /// <param name="logCB">log function</param>
        public void SetLog(bool log = true, Action<string, int> logCB = null) {
            if (log == false) {
                PETool.log = false;
            }
            if (logCB != null) {
                PETool.logCB = logCB;
            }
        }
    }
}