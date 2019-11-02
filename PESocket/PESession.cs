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
        /// 为Session对象指定套接字和closeCB，并开始异步接收数据，并使用PEPkg来存储和解析
        /// </summary>
        /// <param name="skt"></param>
        /// <param name="closeCB"></param>
        public void StartRcvData(Socket skt, Action closeCB) {
            try {
                this.skt = skt;
                this.closeCB = closeCB;

                OnConnected();

                PEPkg pack = new PEPkg();
                //开始接收一个数据头
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

        /// <summary>
        /// 异步接收数据头信息的回调,它按照把数据头存放在headBuff里，数据体存放在bodyBuff里循环进行
        /// </summary>
        /// <param name="ar"></param>
        private void RcvHeadData(IAsyncResult ar) {
            try {
                PEPkg pack = (PEPkg)ar.AsyncState;
                int len = skt.EndReceive(ar);
                if (len > 0) {
                    pack.headIndex += len;
                    //当数据头信息不完整时，异步接收数据表头剩余信息
                    if (pack.headIndex < pack.headLen) {
                        skt.BeginReceive(
                            pack.headBuff,
                            pack.headIndex,
                            pack.headLen - pack.headIndex,
                            SocketFlags.None,
                            new AsyncCallback(RcvHeadData),
                            pack);
                    }
                    //当数据头信息完整时，异步接收主体信息
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
                //当接收到0字节数据，说明与远程终端断联
                else {
                    OnDisConnected();
                    Clear();
                }
            }
            catch (Exception e) {
                PETool.LogMsg("RcvHeadError:" + e.Message, LogLevel.Error);
            }
        }

        /// <summary>
        /// 异步接收数据主体信息的回调,它按照把数据头存放在headBuff里，数据体存放在bodyBuff里循环进行
        /// </summary>
        /// <param name="ar"></param>
        private void RcvBodyData(IAsyncResult ar) {
            try {
                PEPkg pack = (PEPkg)ar.AsyncState;
                int len = skt.EndReceive(ar);
                if (len > 0) {
                    pack.bodyIndex += len;
                    //当数据主体信息不完整时,继续接受剩余的数据体信息
                    if (pack.bodyIndex < pack.bodyLen) {
                        skt.BeginReceive(pack.bodyBuff,
                            pack.bodyIndex,
                            pack.bodyLen - pack.bodyIndex,
                            SocketFlags.None,
                            new AsyncCallback(RcvBodyData),
                            pack);
                    }
                    //当数据主体完整时，执行解析数据的逻辑,清空pkg的所有缓冲区内容，并重新接受数据头信息
                    else {
                        //这里消息被完成的解析成对象
                        T msg = PETool.DeSerialize<T>(pack.bodyBuff);
                        //对消息对象调用一段自定义逻辑
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
                PETool.LogMsg("RcvBodyError:" + e.Message, LogLevel.Error);//这段日志一般在出现连接中断时打印
            }
        }
        #endregion

        #region Send
        /// <summary>
        /// 将一个（PEMsg）对象实例转换为一个带有字节长度信息的字节数组，并发送给远程主机
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
        /// 在PESocket建立了与远程主机的连接，并开始异步接收数据之前调用
        /// 重写此类方法，一般用于设置日志窗口，在特殊的时间结点打印在控制台上
        /// </summary>
        protected virtual void OnConnected() {
            PETool.LogMsg("New Seesion Connected.", LogLevel.Info);
        }

        /// <summary>
        /// 当接受到数据时调用；
        /// 实际上，它是在异步接收到完整的一条数据体信息时开始调用
        /// </summary>
        protected virtual void OnReciveMsg(T msg) {
            PETool.LogMsg("Receive Network Message.", LogLevel.Info);
        }

        /// <summary>
        /// 当断开连接时调用；
        /// 触发此方法调用的时机：远程主机断开连接,本机异步接收到一个空报包时调用
        /// </summary>
        protected virtual void OnDisConnected() {
            PETool.LogMsg("Session Disconnected.", LogLevel.Info);
        }
    }
}