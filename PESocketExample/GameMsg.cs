using System;
using PENet;

/// <summary>
/// 游戏消息数据的具体类，网络消息数据的单位；
/// 它应该是客户端和服务端通用的类型
/// </summary>
[Serializable]
public class GameMsg : PEMsg
{
    public ReqLogin reqLogin;
    public RspLogin rspLogin;

    public ReqRename reqRename;
    public RspRename rspRename;

    public ReqGuide reqGuide;
    public RspGuide rspGuide;

    public ReqStrong reqStrong;
    public RspStrong rspStrong;
}

#region 登录相关数据
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
    /// <summary>
    /// 水晶
    /// </summary>
    public int crystal;

    public int hp;
    public int ad;
    public int ap;
    public int addef;
    public int apdef;
    /// <summary>
    /// //闪避概率
    /// </summary>
    public int dodge;
    /// <summary>
    /// 穿透比率
    /// </summary>
    public int pierce;
    /// <summary>
    /// 暴击概率
    /// </summary>
    public int critical;
    /// <summary>
    /// 任务引导id
    /// </summary>
    public int guideid;
    /// <summary>
    /// 玩家装备信息的数组，索引号代表装备的位置，索引的值代表该位置装备的星级
    /// </summary>
    public int[] strongArr;
}
#endregion

#region 命名相关
/// <summary>
/// 请求重命名数据
/// </summary>
[Serializable]
public class ReqRename
{
    public string name;
}

/// <summary>
/// 回应重命名数据
/// </summary>
[Serializable]
public class RspRename
{
    public string name;
}
#endregion

#region 引导任务相互
/// <summary>
/// 任务完成请求数据
/// </summary>
[Serializable]
public class ReqGuide
{
    /// <summary>
    /// 本次已完成任务的id
    /// </summary>
    public int guideid;
}

/// <summary>
/// 任务完成响应数据
/// </summary>
[Serializable]
public class RspGuide
{
    /// <summary>
    /// 下一个要进行的任务
    /// </summary>
    public int guideid;
    /// <summary>
    /// 完成任务后的总金币数
    /// </summary>
    public int coin;
    /// <summary>
    /// 完成任务后的经验值
    /// </summary>
    public int exp;
    /// <summary>
    /// 完成任务后的等级
    /// </summary>
    public int lv;
}

#endregion

#region 强化相关
/// <summary>
/// 请求强化数据
/// </summary>
[Serializable]
public class ReqStrong
{
    /// <summary>
    /// 请求强化装备的位置
    /// </summary>
    public int pos;
}
/// <summary>
/// 响应强化数据
/// </summary>
[Serializable]
public class RspStrong
{
    public int coin;
    public int crystal;
    public int hp;
    public int ad;
    public int ap;
    public int addef;
    public int apdef;
    public int[] strongArr;
}
#endregion

/// <summary>
/// 用于判断消息错误类型的枚举,该枚举直接赋值给PEMsg的err
/// 如果消息的错误类型不为None，则不再将消息分发到业务逻辑处理，而是直接在UI上显示错误内容
/// </summary>
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
    /// <summary>
    /// 名称已存在错误
    /// </summary>
    NameIsExit,
    /// <summary>
    /// 更新数据库错误
    /// </summary>
    UpdateDbError,
    /// <summary>
    /// 服务器数据异常
    /// </summary>
    ServerDataError,
    /// <summary>
    /// 等级不足
    /// </summary>
    LackLevel,
    /// <summary>
    /// 缺少金币
    /// </summary>
    LackCoin,
    /// <summary>
    /// 缺少钻石
    /// </summary>
    LackDiamond,
    /// <summary>
    /// 缺少水晶
    /// </summary>
    LackCrystal
}


/// <summary>
/// 命令号：用于判断消息类型的枚举,该枚举直接赋值给PEMsg的cmd字段
/// </summary>
public enum CMD
{
    None=0,
    //登录相关 100
    /// <summary>
    /// 登录请求消息类型
    /// </summary>
    ReqLogin=101,
    /// <summary>
    /// 登录响应消息类型
    /// </summary>
    RspLogin=102,
    /// <summary>
    /// 重命名请求消息类型
    /// </summary>
    ReqRename=103,
    /// <summary>
    /// 重命名响应消息类型
    /// </summary>
    RsqRename=104,
    //任务引导相关
    /// <summary>
    /// 完成任务请求数据类型
    /// </summary>
    ReqGuide=201,
    /// <summary>
    /// 完成任务响应数据类型
    /// </summary>
    RspGuide=202,
    //强化升级相关
    /// <summary>
    /// 请求强化类型
    /// </summary>
    ReqStrong=203,
    /// <summary>
    /// 响应强化类型
    /// </summary>
    RspStrong=204
}

/// <summary>
/// 服务器配置
/// </summary>
public class SrvCfg
{
    public const string sevAddress = "10.0.117.84";
    public const int sevPort = 17666;
}
