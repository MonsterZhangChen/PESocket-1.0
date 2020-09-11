/****************************************************
	文件：ServerStart.cs
	作者：章晨
	邮箱: 1728722243@qq.com
	日期：2019/11/01 14:16   	
	功能：服务器开启
*****************************************************/
using PENet;
using System;
using System.Threading;

namespace Server
{
    class ServerStart
    {
        public static void Main(string[] args)
        {
            ServerRoot.Instance.Init();//如果在直接写在客户端类里，需要写很多静态方法
            while (true)//这里需要优化，不然一秒可能几千次
            {
                ServerRoot.Instance.Update();
                Thread.Sleep(20);
            }
        }
    }
}
