/****************************************************
	文件：CacheSvc.cs
	作者：章校长
	邮箱: 1728722243@qq.com
	日期：2019/11/01 22:03   	
	功能：缓存层
*****************************************************/

using System.Collections.Generic;

public class CacheSvc
{
    private static CacheSvc instance = null;
    public static CacheSvc Instance
    {
        get
        {
            if (instance == null) instance = new CacheSvc();
            return instance;
        }
    }

    public void Init()
    {
        PECommon.Log("CacheSvc Init Done!");
    }

    /// <summary>
    /// 在线上的账号字典
    /// </summary>
    private Dictionary<string, ServerSession> onlineAcctDic = new Dictionary<string, ServerSession>();
    /// <summary>
    /// 在线上的回话
    /// </summary>
    private Dictionary<ServerSession, PlayerData> onlineSessionDic = new Dictionary<ServerSession, PlayerData>();

    /// <summary>
    /// 判断指定的账号是否在线上
    /// </summary>
    /// <returns></returns>
    public bool IsAcctOnLine(string acct)
    {
        return onlineAcctDic.ContainsKey(acct);
    }

    /// <summary>
    /// 获取玩家的数据:
    /// 1)如果账密匹配，则返回玩家数据；2）如果账密不匹配，则返回空；3）如果账号不存在，则创建一账号并生成信息
    /// </summary>
    /// <param name="acct">账号</param>
    /// <param name="pass">密码</param>
    /// <returns></returns>
    public PlayerData GetPlayerData(string acct, string pass)
    {
        //todo 从数据库加载数据
        return null;
    }

    /// <summary>
    /// 添加账号上线时的缓存数据
    /// </summary>
    /// <param name="acct">账号</param>
    /// <param name="session">网络回话</param>
    /// <param name="playerData">玩家信息</param>
    public void AddAcctOnline(string acct,ServerSession session,PlayerData playerData)
    {
        onlineAcctDic.Add(acct, session);
        onlineSessionDic.Add(session, playerData);
    }
}
