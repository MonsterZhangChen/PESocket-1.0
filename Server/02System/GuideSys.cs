/****************************************************
	文件：GuideSys.cs
	作者：章晨
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
            //是否为智者引导任务
            if (pd.guideid == 1001)
            {
                TaskSys.Instance.CalcTaskPrgs(pd, 1);
            }
            GuideTaskCfg gtc = cfgSvc.GetGuideTaskCfgData(pd.guideid);
            //缓存数据变化
            pd.guideid++;
            pd.coin += gtc.coin;
            PECommon.CaculExp(pd, gtc.exp);
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

   
}
