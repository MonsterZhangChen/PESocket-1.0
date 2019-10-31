/****************************************************
	文件：PEMsg.cs
	作者：Plane
	邮箱: 1785275942@qq.com
	日期：2018/10/30 11:20   	
	功能：消息定义类
*****************************************************/

namespace PENet {

    using System;

    /// <summary>
    /// 自定义消息类，它表示网络传输的实体对象
    /// </summary>
    [Serializable]
    public abstract class PEMsg {
        public int seq;
        public int cmd;
        public int err;
    }
}