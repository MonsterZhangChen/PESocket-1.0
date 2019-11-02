/****************************************************
	文件：PEPkg.cs
	作者：Plane
	邮箱: 1785275942@qq.com
	日期：2018/10/30 11:20   	
	功能：存储与格式化网络消息包
*****************************************************/

using System;

namespace PENet {
    /// <summary>
    /// 用于存储和解析接收到的字节数组
    /// </summary>
    class PEPkg {
        /// <summary>
        /// 表示单位表头信息的完整长度;将此值与bodyIndex比较，依次验证数据头的完整性
        /// </summary>
        public int headLen = 4;
        /// <summary>
        /// 接收数据头信息的缓冲区，它用于专门存储数据包的长度信息
        /// </summary>
        public byte[] headBuff = null;
        /// <summary>
        /// 头信息缓冲区开始存储的位置，亦表示当前所接收到的数据体的长度
        /// </summary>
        public int headIndex = 0;
        /// <summary>
        /// 表示数据体的完整长度，它是由headBuff计算而来;将此值与headIndex比较，依次验证数据主体的完整性
        /// </summary>
        public int bodyLen = 0;
        /// <summary>
        /// 接收数据主体的缓冲区，异步接收的数据将全部存入这个字节数组中
        /// </summary>
        public byte[] bodyBuff = null;
        public int bodyIndex = 0;

        public PEPkg() {
            headBuff = new byte[4];
        }

        /// <summary>
        /// 按照数据头信息来初始化数据体的缓冲区
        /// </summary>
        public void InitBodyBuff() {
            bodyLen = BitConverter.ToInt32(headBuff, 0);
            bodyBuff = new byte[bodyLen];
        }

        public void ResetData() {
            headIndex = 0;
            bodyLen = 0;
            bodyBuff = null;
            bodyIndex = 0;
        }
    }
}