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
    /// 自定义消息类的基类，它表示网络传输的实体对象，即最终把这个对象的实例转换为字节数组发送
    /// </summary>
    [Serializable]
    public abstract class PEMsg {
        public int seq;
        /// <summary>
        /// 表示消息的类型，它是解析消息方式的依据，它需要在消息实例化时指定
        /// </summary>
        public int cmd;//遗留：这个表征消息类型的属性是否应当放在构造函数里
        /// <summary>
        /// 表示消息的错误码
        /// </summary>
        public int err;
    }
}