/****************************************************
	文件：PESession.cs
	作者：Plane
	邮箱: 1785275942@qq.com
	日期：2018/10/30 11:20   	
	功能：网络会话管理
*****************************************************/

using System;
using System.Net.Sockets;

namespace PENet {
    /// <summary>
    /// 一个Session代表一种网络会话，它包含着需要进行回话的套接字，需要处理消息的类型以及进行通信的方法；
    /// 它被PESocket持有，并在PESocket执行Start()相关方法被实例化;
    /// 它持有的Socket即PESocket的Socket实例
    /// </summary>
    /// <typeparam name="T">网络回话需要处理的消息的类型</typeparam>
    public abstract class PESession<T> where T : PEMsg {
        private Socket skt;
        private Action closeCB;

        #region Recevie
        /// <summary>
        /// 为Session对象指定套接字和closeCB，并开始异步接收数据
        /// </summary>
        /// <param name="skt"></param>
        /// <param name="closeCB"></param>
        public void StartRcvData(Socket skt, Action closeCB) {
            try {
                this.skt = skt;
                this.closeCB = closeCB;

                OnConnected();

                PEPkg pack = new PEPkg();
                skt.BeginReceive(
                    pack.headBuff,
                    0,
                    pack.headLen,
                    SocketFlags.None,
                    new AsyncCallback(RcvHeadData),
                    pack);
            }
            catch (Exception e) {
                PETool.LogMsg("StartRcvData:" + e.Message, LogLevel.Error);
            }
        }


        private void RcvHeadData(IAsyncResult ar) {
            try {
                PEPkg pack = (PEPkg)ar.AsyncState;
                int len = skt.EndReceive(ar);
                if (len > 0) {
                    pack.headIndex += len;
                    if (pack.headIndex < pack.headLen) {
                        skt.BeginReceive(
                            pack.headBuff,
                            pack.headIndex,
                            pack.headLen - pack.headIndex,
                            SocketFlags.None,
                            new AsyncCallback(RcvHeadData),
                            pack);
                    }
                    else {
                        pack.InitBodyBuff();
                        skt.BeginReceive(pack.bodyBuff,
                            0,
                            pack.bodyLen,
                            SocketFlags.None,
                            new AsyncCallback(RcvBodyData),
                            pack);
                    }
                }
                else {
                    OnDisConnected();
                    Clear();
                }
            }
            catch (Exception e) {
                PETool.LogMsg("RcvHeadError:" + e.Message, LogLevel.Error);
            }
        }

        private void RcvBodyData(IAsyncResult ar) {
            try {
                PEPkg pack = (PEPkg)ar.AsyncState;
                int len = skt.EndReceive(ar);
                if (len > 0) {
                    pack.bodyIndex += len;
                    if (pack.bodyIndex < pack.bodyLen) {
                        skt.BeginReceive(pack.bodyBuff,
                            pack.bodyIndex,
                            pack.bodyLen - pack.bodyIndex,
                            SocketFlags.None,
                            new AsyncCallback(RcvBodyData),
                            pack);
                    }
                    else {
                        T msg = PETool.DeSerialize<T>(pack.bodyBuff);
                        OnReciveMsg(msg);

                        //loop recive
                        pack.ResetData();
                        skt.BeginReceive(
                            pack.headBuff,
                            0,
                            pack.headLen,
                            SocketFlags.None,
                            new AsyncCallback(RcvHeadData),
                            pack);
                    }
                }
                else {
                    OnDisConnected();
                    Clear();
                }
            }
            catch (Exception e) {
                PETool.LogMsg("RcvBodyError:" + e.Message, LogLevel.Error);

            }
        }
        #endregion

        #region Send
        /// <summary>
        /// 将一个对象实例转换为一个带有字节长度信息的字节数组，并发送给远程主机
        /// </summary>
        public void SendMsg(T msg) {
            byte[] data = PETool.PackLenInfo(PETool.Serialize<T>(msg));
            SendMsg(data);
        }

        /// <summary>
        /// 将字节数组发送给远程主机
        /// </summary>
        public void SendMsg(byte[] data) {
            NetworkStream ns = null;
            try {
                ns = new NetworkStream(skt);
                if (ns.CanWrite) {
                    ns.BeginWrite(
                        data,
                        0,
                        data.Length,
                        new AsyncCallback(SendCB),
                        ns);
                }
            }
            catch (Exception e) {
                PETool.LogMsg("SndMsgError:" + e.Message, LogLevel.Error);
            }
        }

        private void SendCB(IAsyncResult ar) {
            NetworkStream ns = (NetworkStream)ar.AsyncState;
            try {
                ns.EndWrite(ar);
                ns.Flush();
                ns.Close();
            }
            catch (Exception e) {
                PETool.LogMsg("SndMsgError:" + e.Message, LogLevel.Error);
            }
        }
        #endregion

        /// <summary>
        /// Release Resource
        /// </summary>
        private void Clear() {
            if (closeCB != null) {
                closeCB();
            }
            skt.Close();
        }

        /// <summary>
        /// Connect network
        /// </summary>
        protected virtual void OnConnected() {
            PETool.LogMsg("New Seesion Connected.", LogLevel.Info);
        }

        /// <summary>
        /// Receive network message
        /// </summary>
        protected virtual void OnReciveMsg(T msg) {
            PETool.LogMsg("Receive Network Message.", LogLevel.Info);
        }

        /// <summary>
        /// Disconnect network
        /// </summary>
        protected virtual void OnDisConnected() {
            PETool.LogMsg("Session Disconnected.", LogLevel.Info);
        }
    }
}