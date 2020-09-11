/****************************************************
	文件：PowerSys.cs
	作者：章晨
	邮箱: 1728722243@qq.com
	日期：2019/11/24 18:18   	
	功能：体力恢复系统
*****************************************************/
using System.Collections.Generic;
public class PowerSys
{
    private TimerSvc timerSvc = null;
    private CacheSvc cacheSvc = null;
    private static PowerSys instance = null;
    public static PowerSys Instance
    {
        get
        {
            if (instance == null) instance = new PowerSys();
            return instance;
        }
    }

    public void Init()
    {
        timerSvc = TimerSvc.Instance;
        cacheSvc = CacheSvc.Instance;
        timerSvc.AddTimeTask(CalcPowerAdd, PECommon.PowerAddMinute, PETimeUnit.Second, 0);
        PECommon.Log("PowerSys Init Done");
    }

    /// <summary>
    /// 在线玩家计算体力增长与消息推送
    /// </summary>
    /// <param name="tid"></param>
    public void CalcPowerAdd(int tid)
    {
        //计算体力增长
        PECommon.Log("All Online Player Calc Power Incress....");
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.PshPower
        };
        msg.pshPower = new PshPower();

        //所有在线玩家获得实时的体力增长推送数据
        Dictionary<ServerSession,PlayerData> onlineDic = cacheSvc.GetOnlineCache();
        foreach (var item in onlineDic)
        {
            PlayerData pd = item.Value;
            ServerSession session = item.Key;
            int power = pd.power;
            int powerMax = PECommon.GetPowerLimit(pd);
            if (pd.power >= powerMax)
            {
                continue;
            }
            else
            {
                pd.power += PECommon.PowerAddCount;
                pd.time = timerSvc.GetNowTime();
                if (pd.power > powerMax)
                {
                    pd.power = powerMax;
                }
            }
            if (pd.power != power)
            {
                pd.time = timerSvc.GetNowTime();
                if (!cacheSvc.UpdatePlayerData(pd.id, pd))
                {
                    msg.err = (int)ErrorCode.UpdateDbError;
                }
                else
                {
                    msg.pshPower.power = pd.power;
                    session.SendMsg(msg);//推送给在线玩家
                }
            }
        }
    }
}

