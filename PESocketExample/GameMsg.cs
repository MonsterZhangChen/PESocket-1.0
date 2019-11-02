using System;
using PENet;

/// <summary>
/// 游戏消息数据的具体类，网络消息数据的单位；
/// 它应该是客户端和服务端公用的类型
/// </summary>
[Serializable]
public class GameMsg : PEMsg
{
    public ReqLogin reqLogin;
    public RspLogin rspLogin;
}

/// <summary>
/// 请求登录数据
/// </summary>
[Serializable]
public class ReqLogin
{
    public string acct;
    public string pass;
}

/// <summary>
/// 登录响应数据
/// </summary>
[Serializable]
public class RspLogin
{
    public PlayerData playerData;
}

/// <summary>
/// 玩家数据
/// </summary>
[Serializable]
public class PlayerData
{
    public int id;
    public string name;
    public int lv;
    public int exp;
    public int power;
    public int coin;
    public int diamond;
}

public enum ErrorCode
{
    /// <summary>
    /// 没有错误
    /// </summary>
    None=0,
    /// <summary>
    /// 账号已上线
    /// </summary>
    AcctIsOnline,
    /// <summary>
    /// 密码错误
    /// </summary>
    ErrorPass,
}


/// <summary>
/// 用于判断消息类型的枚举,该枚举直接赋值给
/// </summary>
public enum CMD
{
    None=0,
    //登录相关 100
    ReqLogin=101,
    RspLogin=102,
}

public class SrvCfg
{
    public const string sevAddress = "127.0.0.1";
    public const int sevPort = 17666;
}
