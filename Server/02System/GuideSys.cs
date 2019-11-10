﻿/****************************************************
	文件：GuideSys.cs
	作者：章校长
	邮箱: 1728722243@qq.com
	日期：2019/11/09 16:50   	
	功能：任务引导业务系统
*****************************************************/

class GuideSys
{
    private CacheSvc cacheSvc = null;
    private CfgSvc cfgSvc = null;
    private static GuideSys instance = null;
    public static GuideSys Instance
    {
        get
        {
            if (instance == null) instance = new GuideSys();
            return instance;
        }
    }

    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
        cfgSvc = CfgSvc.Instance;
        PECommon.Log("GuideSys Init Done!");
    }

    /// <summary>
    /// 处理任务完成逻辑
    /// </summary>
    public void ReqGuide(MsgPack pack)
    {
        ReqGuide data = pack.msg.reqGuide;
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspGuide,
        };

        PlayerData pd = cacheSvc.GetPlayerDataBySettion(pack.session);
        if (pd.guideid == data.guideid)//一次安全校验，如果客户端数据和服务端对不上，直接踢下线
        {
            GuideTaskCfg gtc = cfgSvc.GetGuideTaskCfgData(pd.guideid);
            //缓存数据变化
            pd.guideid++;
            pd.coin += gtc.coin;
            CaculExp(pd, gtc.exp);
            if (cacheSvc.UpdatePlayerData(pd.id, pd))
            {
                msg.rspGuide = new RspGuide
                {
                    guideid = pd.guideid,
                    coin = pd.coin,
                    exp=pd.exp,
                    lv=pd.lv
                };
            }
            else
            {
                msg.err = (int)ErrorCode.UpdateDbError;
            }
        }
        else
        {
            msg.err = (int)ErrorCode.ServerDataError;
        }
        pack.session.SendMsg(msg);
    }

    /// <summary>
    /// 计算玩家经验引起的等级和经验变化
    /// </summary>
    /// <param name="pd">玩家数据</param>
    /// <param name="addExp">增加的经验</param>
    public void CaculExp(PlayerData pd,int addExp)
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
                pd.exp = curtExp+remExp;
                break;
            }
        }
    }
}
