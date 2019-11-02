/****************************************************
	文件：LoginSys.cs
	作者：章校长
	邮箱: 1728722243@qq.com
	日期：2019/11/01 14:38   	
	功能：服务器登录业务系统
*****************************************************/
using PENet;

class LoginSys
{
    private CacheSvc cacheSvc = null;
    private static LoginSys instance = null;
    public static LoginSys Instance
    {
        get
        {
            if (instance == null) instance = new LoginSys();
            return instance;
        }
    }

    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
        PECommon.Log("LoginSys Init Done!");
    }

    /// <summary>
    /// 处理登录注册请求
    /// </summary>
    /// <param name="msg"></param>
    public void ReqLogin(MsgPack pack)
    {
        ReqLogin data = pack.msg.reqLogin;
        //给客户端的回应消息
        GameMsg rspMsg = new GameMsg()
        {
            cmd = (int)CMD.RspLogin,
           
        };
        //当前账号已上线 产生错误码
        if (cacheSvc.IsAcctOnLine(data.acct))
        {
            rspMsg.err = (int)ErrorCode.AcctIsOnline;
        }
        //当前账号未上线
        else
        {
            PlayerData playerData = cacheSvc.GetPlayerData(data.acct, data.pass);
            if (playerData != null)//账密匹配 产生玩家数据
            {
                cacheSvc.AddAcctOnline(data.acct, pack.session, playerData);
                rspMsg.rspLogin = new RspLogin()
                {
                    playerData = playerData,
                };
            }
            else//账密不匹配 产生错误码
            {
                rspMsg.err = (int)ErrorCode.ErrorPass;
            }

        }

        //回应客户端
        pack.session.SendMsg(rspMsg);
    }
}
