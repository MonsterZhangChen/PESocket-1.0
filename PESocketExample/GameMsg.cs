﻿using System;
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

    public SndChat sndChat;
    public PshChat pshChat;

    public ReqBuy reqBuy;
    public RspBuy RspBuy;

    public PshPower pshPower;

    public ReqTakeTaskReward reqTakeTaskReward;
    public RspTakeTaskReward rspTakeTaskReward;
    public PshTaskPrgs pshTaskPrgs;

    public ReqFBFight reqFBFight;
    public RspFBFight rspFBFight;
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
    /// <summary>
    /// 表示玩家登陆的时间
    /// </summary>
    public long time;
    /// <summary>
    /// 代表完成任务的数据，字符串格式：0|1|0，代表id|完成进度|未完成
    /// </summary>
    public string[] taskStrArr;
    /// <summary>
    /// 代表当前副本的关卡
    /// </summary>
    public int fuben;
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

#region 聊天相关
/// <summary>
/// 接收聊天数据
/// </summary>
[Serializable]
public class SndChat
{
    public string chat;
}
/// <summary>
/// 推送聊天数据
/// </summary>
[Serializable]
public class PshChat
{
    public string name;
    public string chat;
}
#endregion

#region 购买交易相关
/// <summary>
/// 请求购买数据
/// </summary>
[Serializable]
public class ReqBuy
{
    /// <summary>
    /// 购买物品的类型；
    /// 0，体力；1，金币
    /// </summary>
    public int type;
    /// <summary>
    /// 购买消耗的钻石数目
    /// </summary>
    public int cost;
}
[Serializable]
public class RspBuy
{
    public int type;
    public int coin;
    public int diamond;
    public int power;
}
#endregion

#region 体力恢复相关
/// <summary>
/// 推送体力恢复数据
/// </summary>
[Serializable]
public class PshPower
{
    public int power;
}
#endregion

#region 副本战斗相关
/// <summary>
/// 请求副本战斗类型
/// </summary>
[Serializable]
public class ReqFBFight
{
    public int fbid;
}
/// <summary>
/// 响应请求副本战斗类型
/// </summary>
[Serializable]
public class RspFBFight
{
    public int fbid;
    public int power;
}
#endregion

#region 任务奖励相关
/// <summary>
/// 请求任务完成数据
/// </summary>
[Serializable]
public class ReqTakeTaskReward
{
    public int rid;
}
/// <summary>
/// 响应任务完成数据
/// </summary>
[Serializable]
public class RspTakeTaskReward
{
    public int coin;
    public int exp;
    public int lv;
    public string[] taskArr;
}

/// <summary>
/// 推送任务进度数据
/// </summary>
[Serializable]
public class PshTaskPrgs
{
    public string[] taskArr;
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
    /// 客户端数据异常
    /// </summary>
    ClientDataError,
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
    LackCrystal,
    LackPower
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
    ReqStrong=301,
    /// <summary>
    /// 响应强化类型
    /// </summary>
    RspStrong=302,
    //聊天相关
    /// <summary>
    /// 发送聊天消息类型
    /// </summary>
    SndChat=401,
    /// <summary>
    /// 广播聊天消息类型
    /// </summary>
    PshChat=402,
    //购买交易相关
    /// <summary>
    /// 请求购买类型
    /// </summary>
    ReqBuy=501,
    /// <summary>
    /// 响应购买类型
    /// </summary>
    RspBuy=502,
    //体力恢复相关
    /// <summary>
    /// 推送体力恢复类型
    /// </summary>
    PshPower=601,
    //任务奖励相关
    /// <summary>
    /// 请求完成任务类型
    /// </summary>
    ReqTakeTaskReward=701,
    /// <summary>
    /// 响应完成任务类型
    /// </summary>
    RspTakeTaskReward=702,
    /// <summary>
    /// 推送进度任务类型
    /// </summary>
    PshTaskPrgs=703,
    /// <summary>
    /// 请求副本战斗类型
    /// </summary>
    ReqFBFight=801,
    /// <summary>
    /// 响应副本战斗类型
    /// </summary>
    RspFBFight=802
}

/// <summary>
/// 服务器配置
/// </summary>
public class SrvCfg
{
    public const string sevAddress = "127.0.0.1";
    public const int sevPort = 17666;
}
