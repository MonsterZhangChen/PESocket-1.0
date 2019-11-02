/****************************************************
	文件：PECommon.cs
	作者：章校长
	邮箱: 1728722243@qq.com
	日期：2019/11/01 16:14   	
	功能：服务端和客户端的公共工具类
*****************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PENet;

public enum LogType
{
    Log=0,
    Warning=1,
    error=2,
    info=3
}

/// <summary>
/// 服务端和客户端的公共工具类
/// </summary>
public class PECommon
{
    /// <summary>
    /// 打印操作统一添加逻辑(默认引用的是dll,所有不能去动源代码)；
    /// 它会的调用PETool.log()方法，给消息补上一个时间前缀
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="tp"></param>
    public static void Log(string msg="",LogType tp = LogType.Log)
    {
        LogLevel lv = (LogLevel)tp;
        PETool.LogMsg(msg, lv);
    }
}
