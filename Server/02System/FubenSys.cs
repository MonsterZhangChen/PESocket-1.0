/****************************************************
	文件：FubenSys.cs
	作者：章晨
	邮箱: 1728722243@qq.com
	日期：2019/12/12 16:31   	
	功能：副本业务系统
*****************************************************/

public class FubenSys
{
    private CacheSvc cacheSvc = null;
    private CfgSvc cfgSvc = null;
    private static FubenSys instance = null;
    public static FubenSys Instance
    {
        get
        {
            if (instance == null) instance = new FubenSys();
            return instance;
        }
    }

    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
        cfgSvc = CfgSvc.Instance;
        PECommon.Log("FubenSys Init Done!");
    }

    public void ReqFBFight(MsgPack pack)
    {
        ReqFBFight data = pack.msg.reqFBFight;

        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspFBFight
        };

        PlayerData pd = cacheSvc.GetPlayerDataBySettion(pack.session);
        int power = cfgSvc.GetMapCfg(data.fbid).power;

        if (pd.fuben < data.fbid)
        {
            msg.err = (int)ErrorCode.ClientDataError;
        }
        else if (pd.power < power)
        {
            msg.err = (int)ErrorCode.LackPower;
        }
        else
        {
            pd.power -= power;
            if (cacheSvc.UpdatePlayerData(pd.id, pd))
            {
                RspFBFight rspFBFight = new RspFBFight
                {
                    fbid = data.fbid,
                    power = pd.power
                };
                msg.rspFBFight = rspFBFight;
            }
            else
            {
                msg.err = (int)ErrorCode.UpdateDbError;
            }
        }
        pack.session.SendMsg(msg);
    }
}
