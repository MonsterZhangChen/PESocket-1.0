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
            rspLogin = new RspLogin()
            {
            },
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
            if (playerData != null)//账密匹配 产生玩家数据(这里包含了创建账号)
            {
                cacheSvc.AddAcctOnline(data.acct, pack.session, playerData);
                rspMsg.rspLogin.playerData = playerData;
            }
            else//账密不匹配 产生错误码
            {
                rspMsg.err = (int)ErrorCode.ErrorPass;
            }
        }

        //回应客户端
        pack.session.SendMsg(rspMsg);
    }

    /// <summary>
    /// 处理重命名请求
    /// </summary>
    /// <param name="pack"></param>
    public void ReqRename(MsgPack pack)
    {
        ReqRename data = pack.msg.reqRename;
        //给客户端的回应消息
        GameMsg rspMsg = new GameMsg()
        {
            cmd = (int)CMD.RsqRename,
        };

        //名字已经存在 返回错误码
        if (cacheSvc.IsNameExit(data.name))
        {
            rspMsg.err = (int)ErrorCode.NameIsExit;
        }
        //名字不存在 更新缓存与数据库 再返回给客户端
        else
        {
            //更新缓冲区数据（代替更新数据库）
            PlayerData cachePlayerData = cacheSvc.GetPlayerDataBySettion(pack.session);
            if (cachePlayerData != null)
            {
                cachePlayerData.name = data.name;
            }
            //更新数据库失败，返回错误码（这里是简化处理了，仍然同步写入数据库；对于高并发操作，应该由驱动负责将数据更新入数据库中，但是会分离逻辑，更为复杂）
            if (!cacheSvc.UpdatePlayerData(cachePlayerData.id, cachePlayerData))
            {
                rspMsg.err = (int)ErrorCode.UpdateDbError;
            }
            //更新数据库成功 返回数据
            else
            {
                rspMsg.rspRename = new RspRename
                {
                    name = data.name
                };
            }
        }
        pack.session.SendMsg(rspMsg);
    }


    /// <summary>
    /// 清空下线玩家的缓存数据
    /// </summary>
    /// <param name="session"></param>
    public void ClearOffLineData(ServerSession session)
    {
        cacheSvc.AcctOffLine(session);
    }
}
