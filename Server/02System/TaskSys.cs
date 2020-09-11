/****************************************************
	文件：TaskSys.cs
	作者：章晨
	邮箱: 1728722243@qq.com
	日期：2019/12/02 19:40   	
	功能：任务奖励系统
    描述：该模块与其他逻辑模块有耦合，因为一段逻辑处理可能
        引起任务进度的增加
*****************************************************/


public class TaskSys
{

    private CacheSvc cacheSvc = null;
    private CfgSvc cfgSvc = null;
    private static TaskSys instance = null;
    public static TaskSys Instance
    {
        get
        {
            if (instance == null) instance = new TaskSys();
            return instance;
        }
    }

    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
        cfgSvc = CfgSvc.Instance;
        PECommon.Log("TaskSys Init Done!");
    }

    public void ReqTakeTaskReward(MsgPack pack)
    {
        ReqTakeTaskReward data = pack.msg.reqTakeTaskReward;
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspTakeTaskReward,
        };
        PlayerData pd = cacheSvc.GetPlayerDataBySettion(pack.session);
        TaskRewardCfg trc = cfgSvc.GetTaskRewardCfg(data.rid);
        TaskRewardData trd = CalcTaskRewardData(pd, data.rid);
        if (trd.prgs == trc.count && !trd.taked)
        {
            pd.coin += trc.coin;
            PECommon.CaculExp(pd, trc.exp);
            trd.taked = true;
            //更新任务进度数据
            CalcTaskArr(pd, trd);

            if (!cacheSvc.UpdatePlayerData(pd.id, pd))
            {
                msg.err = (int)ErrorCode.UpdateDbError;
            }
            else
            {
                RspTakeTaskReward rspTakeTaskReward = new RspTakeTaskReward
                {
                    coin = pd.coin,
                    lv = pd.lv,
                    exp = pd.exp,
                    taskArr = pd.taskStrArr
                };
                msg.rspTakeTaskReward = rspTakeTaskReward;
            }
        }
        else
        {
            msg.err = (int)ErrorCode.ClientDataError;
        }
        pack.session.SendMsg(msg);
    }

    /// <summary>
    /// 获取一个任务的完成进度，通过玩家数据的奖励id
    /// </summary>
    /// <param name="pd"></param>
    /// <param name="rid"></param>
    /// <returns></returns>
    public TaskRewardData CalcTaskRewardData(PlayerData pd, int rid)
    {
        TaskRewardData trd = null;
        for (int i = 0; i < pd.taskStrArr.Length; i++)
        {
            string[] taskinfo = pd.taskStrArr[i].Split('#');
            //1|0|0
            if (int.Parse(taskinfo[0]) == rid)
            {
                trd = new TaskRewardData
                {
                    ID = int.Parse(taskinfo[0]),
                    prgs = int.Parse(taskinfo[1]),
                    taked = taskinfo[2].Equals("1")
                };
                break;
            }
        }
        return trd;
    }

    /// <summary>
    /// 存储一个玩家的任务进度，通过玩家数据和任务进度数据
    /// </summary>
    /// <param name="pd"></param>
    /// <param name="trd"></param>
    public void CalcTaskArr(PlayerData pd, TaskRewardData trd)
    {
        string result = trd.ID + "#" + trd.prgs + '#' + (trd.taked ? 1 : 0);
        int index = -1;
        for (int i = 0; i < pd.taskStrArr.Length; i++)
        {
            string[] taskinfo = pd.taskStrArr[i].Split('#');
            if (int.Parse(taskinfo[0]) == trd.ID)
            {
                index = i;
                break;
            }
        }
        pd.taskStrArr[index] = result;
    }

    /// <summary>
    /// 计算玩家的指定任务的任务进度，通过玩家数据和奖励任务id
    /// </summary>
    /// <param name="pd"></param>
    /// <param name="rid"></param>
    public void CalcTaskPrgs(PlayerData pd, int rid)
    {
        TaskRewardData trd = CalcTaskRewardData(pd, rid);
        TaskRewardCfg trc = cfgSvc.GetTaskRewardCfg(rid);
        if (trd.prgs < trc.count)
        {
            trd.prgs += 1;
            //更新任务进度
            CalcTaskArr(pd, trd);
            ServerSession session = cacheSvc.GetOnlineServersession(pd.id);
            if (session != null)
            {
                session.SendMsg(new GameMsg
                {
                    cmd = (int)CMD.PshTaskPrgs,
                    pshTaskPrgs = new PshTaskPrgs
                    {
                        taskArr = pd.taskStrArr
                    }
                });
            }
        }
    }

    /// <summary>
    /// 计算玩家的指定任务的任务进度，通过玩家数据和奖励任务id,并返回任务推送数据；
    /// 该方法用于放置在其他业务逻辑中，进行并包处理，优化网络性能
    /// </summary>
    /// <param name="pd"></param>
    /// <param name="rid"></param>
    public PshTaskPrgs GetTaskPrgs(PlayerData pd, int tid)
    {
        TaskRewardData trd = CalcTaskRewardData(pd, tid);
        TaskRewardCfg trc = cfgSvc.GetTaskRewardCfg(tid);

        if (trd.prgs < trc.count)
        {
            trd.prgs += 1;
            //更新任务进度
            CalcTaskArr(pd, trd);
            return new PshTaskPrgs
            {
                taskArr = pd.taskStrArr
            };
        }
        else
        {
            return null;
        }
    }
}
