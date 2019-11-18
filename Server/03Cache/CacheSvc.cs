/****************************************************
	文件：CacheSvc.cs
	作者：章校长
	邮箱: 1728722243@qq.com
	日期：2019/11/01 22:03   	
	功能：缓存层
*****************************************************/

using System.Collections.Generic;

/// <summary>
/// 缓存区存在的意义：内存读写效率要远大于硬盘读写效率；
/// 将业务逻辑与数据库分开，业务逻辑对缓冲区的数据操作代替对数据库的直接操作，从而加快数据的吞吐速度
/// 而缓冲区负责对数据库进行直接的读写操作；（对于写入操作，需要设计一套机制，如果提交数据失败，则需要重复提交）
/// 
/// </summary>
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

    private DBMgr dBMgr;
    public void Init()
    {
        dBMgr = DBMgr.Instance;
        PECommon.Log("CacheSvc Init Done!");
    }
    /// <summary>
    /// 用户账号与对应网络回话的字典
    /// </summary>
    private Dictionary<string, ServerSession> onlineAcctDic = new Dictionary<string, ServerSession>();
    /// <summary>
    /// 在线上的回话对应用户缓存数据的字典
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
    /// 获取线上的网络会话
    /// </summary>
    /// <returns></returns>
    public List<ServerSession> GetOnlineSvrSessions()
    {
        List<ServerSession> serverSessions = new List<ServerSession>();
        foreach (var item in onlineSessionDic)
        {
            serverSessions.Add(item.Key);
        }
        return serverSessions;
    }

    /// <summary>
    /// 获取玩家的数据，如果获取失败(账密不匹配)，则返回空
    /// </summary>
    /// <param name="acct">账号</param>
    /// <param name="pass">密码</param>
    /// <returns></returns>
    public PlayerData GetPlayerData(string acct, string pass)
    {
        //todo 从数据库加载数据
        return DBMgr.Instance.QueryPlayerData(acct,pass);
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

    /// <summary>
    /// 指定用户名称（昵称）是否存在于数据库中
    /// </summary>
    /// <param name="name"></param>
    public bool IsNameExit(string name)
    {
        return dBMgr.QueryNameExit(name);
    }

    /// <summary>
    /// 通过指定网络的回话来获取对应玩家的缓存数据
    /// </summary>
    /// <param name="session"></param>
    /// <returns></returns>
    public PlayerData GetPlayerDataBySettion(ServerSession session)
    {
        PlayerData playerData;
        onlineSessionDic.TryGetValue(session, out playerData);
        return playerData;
    }

    /// <summary>
    /// 缓冲区数据更新入数据库
    /// </summary>
    /// <param name="id"></param>
    /// <param name="playerData"></param>
    /// <returns></returns>
    public bool UpdatePlayerData(int id,PlayerData playerData)
    {
        return dBMgr.UpdatePlayerData(id,playerData);
    }

    /// <summary>
    /// 玩家下线,清空缓冲区
    /// </summary>
    /// <param name="session"></param>
    public void AcctOffLine(ServerSession session)
    {
        string sessionKey=null;
        foreach(var item in onlineAcctDic)
        {
            if (item.Value == session)
            {
                //onlineAcctDic.Remove(item.Key);foreach内部不要删除元素，否则会引起异常
                sessionKey = item.Key;
            }
        }
        if(sessionKey!=null)onlineAcctDic.Remove(sessionKey);
        bool remSucc = onlineSessionDic.Remove(session);
        PECommon.Log($"Offline Result:{remSucc},sessionID:{session.sessionID}");
    }
}
