/****************************************************
	文件：StrongSys.cs
	作者：章校长
	邮箱: 1728722243@qq.com
	日期：2019/11/16 17:08   	
	功能：强化升级系统
*****************************************************/

public class StrongSys
{
    private CacheSvc cacheSvc = null;
    private CfgSvc cfgSvc = null;
    private static StrongSys instance = null;
    public static StrongSys Instance
    {
        get
        {
            if (instance == null) instance = new StrongSys();
            return instance;
        }
    }

    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
        cfgSvc = CfgSvc.Instance;
        PECommon.Log("StrongSys Init Done!");
    }

    /// <summary>
    /// 处理请求升级逻辑
    /// </summary>
    /// <param name="pack"></param>
    public void ReqStrong(MsgPack pack)
    {
        ReqStrong data = pack.msg.reqStrong;
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspStrong,
        };
        PlayerData pd = cacheSvc.GetPlayerDataBySettion(pack.session);
        int curtStartLv = pd.strongArr[data.pos];//请求升级的武器
        StrongCfg nextSd = cfgSvc.GetStrongCfg(data.pos,curtStartLv+1);
        //数据校验（这是必须的，如果客户端均已校验，则此校验结果可以作为玩家是否作弊的依据）
        if (pd.lv < nextSd.minlv)
        {
            msg.err = (int)ErrorCode.LackLevel;
        }
        else if (pd.coin < nextSd.coin)
        {
            msg.err = (int)ErrorCode.LackCoin;
        }
        else if (pd.crystal < nextSd.crystal)
        {
            msg.err = (int)ErrorCode.LackCrystal;
        }
        else//校验通过
        {
            //资产变化
            pd.coin -= nextSd.coin;
            pd.crystal -= nextSd.crystal;
            pd.strongArr[data.pos] += 1;
            //属性变化
            pd.hp += nextSd.addhp;
            pd.ad += nextSd.addhurt;
            pd.ap += nextSd.addhurt;
            pd.addef += nextSd.adddef;
            pd.apdef += nextSd.adddef;
            //数据库更新
            if(cacheSvc.UpdatePlayerData(pd.id, pd))
            {
                msg.rspStrong = new RspStrong
                {
                    coin = pd.coin,
                    crystal=pd.crystal,
                    hp=pd.hp,
                    ad=pd.ad,
                    ap=pd.ap,
                    addef=pd.addef,
                    apdef=pd.apdef,
                    strongArr=pd.strongArr
                };
            }
            else
            {
                msg.err = (int)ErrorCode.UpdateDbError;
            }
        }
        pack.session.SendMsg(msg);
    }

}
