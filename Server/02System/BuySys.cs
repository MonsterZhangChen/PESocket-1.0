/****************************************************
	文件：BuySys.cs
	作者：章校长
	邮箱: 1728722243@qq.com
	日期：2019/11/17 17:30   	
	功能：交易购买业务
*****************************************************/
public class BuySys
{
    private CacheSvc cacheSvc = null;
    private CfgSvc cfgSvc = null;
    private static BuySys instance = null;
    public static BuySys Instance
    {
        get
        {
            if (instance == null) instance = new BuySys();
            return instance;
        }
    }

    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
        cfgSvc = CfgSvc.Instance;
        PECommon.Log("BuySys Init Done!");
    }

    public void ReqBuy(MsgPack pack)
    {
        ReqBuy data = pack.msg.reqBuy;
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspBuy,
        };
        PlayerData pd = cacheSvc.GetPlayerDataBySettion(pack.session);

        if (data.cost > pd.diamond)
        {
            msg.err = (int)ErrorCode.LackDiamond;
        }
        else
        {
            pd.diamond -= data.cost;
            switch (data.type)
            {
                case 0:
                    pd.power += 100;
                    break;
                case 1:
                    pd.coin += 1000;
                    break;
            }
            if (cacheSvc.UpdatePlayerData(pd.id, pd))
            {
                msg.RspBuy = new RspBuy
                {
                    type = data.type,
                    coin = pd.coin,
                    power = pd.power,
                    diamond = pd.diamond,
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
