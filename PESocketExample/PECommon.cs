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

    /// <summary>
    /// 计算玩家的战斗力(策划提供公式)；
    /// 数值运算写在PECommon里的原因:客户端需要拿他作为显示的数据，服务端需要拿这些数据做计算和存储；
    /// </summary>
    /// <param name="pd">玩家数据</param>
    /// <returns></returns>
    public static int GetFightByProps(PlayerData pd)
    {
        return pd.lv * 100 + pd.ad + pd.ap + pd.addef + pd.apdef + pd.dodge + pd.dodge;
    }

    /// <summary>
    /// 获得玩家的体力上限(策划提供公式)
    /// </summary>
    /// <param name="pd"></param>
    /// <returns></returns>
    public static int GetPowerLimit(PlayerData pd)
    {
        return (pd.lv - 1) / 1 * 15 + 150;
    }

    /// <summary>
    /// 通过等级，获取升级所需的经验值
    /// </summary>
    /// <param name="lv">等级</param>
    /// <returns></returns>
    public static int GetExpValByLv(int lv)
    {
        return 100 * lv * lv;
    }

    /// <summary>
    /// 计算玩家经验引起的等级和经验变化
    /// </summary>
    /// <param name="pd">玩家数据</param>
    /// <param name="addExp">增加的经验</param>
    public static void CaculExp(PlayerData pd, int addExp)
    {
        int curtLv = pd.lv;
        int curtExp = pd.exp;
        int remExp = addExp;
        while (true)
        {
            int upNeedExp = PECommon.GetExpValByLv(curtLv) - curtExp;
            if (remExp >= upNeedExp)
            {
                curtLv++;
                curtExp = 0;
                remExp -= upNeedExp;
            }
            else
            {
                pd.lv = curtLv;
                pd.exp = curtExp + remExp;
                break;
            }
        }
    }

    /// <summary>
    /// 体力恢复的间隔时间(单位：分钟，测试单位为秒钟)
    /// </summary>
    public const int PowerAddMinute = 5;
    /// <summary>
    /// 体力恢复的数量
    /// </summary>
    public const int PowerAddCount = 2;
}
