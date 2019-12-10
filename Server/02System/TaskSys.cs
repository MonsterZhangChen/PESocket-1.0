/****************************************************
	文件：TaskSys.cs
	作者：章校长
	邮箱: 1728722243@qq.com
	日期：2019/12/02 19:40   	
	功能：任务奖励系统
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
    /// 获取一个任务的完成进度，通过玩家数据的id号
    /// </summary>
    /// <param name="pd"></param>
    /// <param name="rid"></param>
    /// <returns></returns>
    public TaskRewardData CalcTaskRewardData(PlayerData pd, int rid)
    {
        TaskRewardData trd = null;
        for (int i = 0; i < pd.taskStrArr.Length; i++)
        {
            string[] taskinfo = pd.taskStrArr[i].Split('|');
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
    /// 存储一个玩家的任务进度信息，通过玩家数据和任务id
    /// </summary>
    /// <param name="pd"></param>
    /// <param name="trd"></param>
    public void CalcTaskArr(PlayerData pd, TaskRewardData trd)
    {
        string result = trd.ID + "|" + trd.prgs + '|' + (trd.taked ? 1 : 0);
        int index = -1;
        for (int i = 0; i < pd.taskStrArr.Length; i++)
        {
            string[] taskinfo = pd.taskStrArr[i].Split('|');
            if (int.Parse(taskinfo[0]) == trd.ID)
            {
                index = i;
                break;
            }
        }
        pd.taskStrArr[index] = result;
    }

    //public void CalcTaskPrgs(PlayerData pd, int tid)
    //{
    //    TaskRewardData trd = CalcTaskRewardData(pd, tid);
    //    TaskRewardCfg trc = cfgSvc.GetTaskRewardCfg(tid);

    //    if (trd.prgs < trc.count)
    //    {
    //        trd.prgs += 1;
    //        //更新任务进度
    //        CalcTaskArr(pd, trd);

    //        ServerSession session = cacheSvc.GetOnlineServersession(pd.id);
    //        if (session != null)
    //        {
    //            session.SendMsg(new GameMsg
    //            {
    //                cmd = (int)CMD.PshTaskPrgs,
    //                pshTaskPrgs = new PshTaskPrgs
    //                {
    //                    taskArr = pd.taskArr
    //                }
    //            });
    //        }
    //    }
    //}

    //public PshTaskPrgs GetTaskPrgs(PlayerData pd, int tid)
    //{
    //    TaskRewardData trd = CalcTaskRewardData(pd, tid);
    //    TaskRewardCfg trc = cfgSvc.GetTaskRewardCfg(tid);

    //    if (trd.prgs < trc.count)
    //    {
    //        trd.prgs += 1;
    //        //更新任务进度
    //        CalcTaskArr(pd, trd);

    //        return new PshTaskPrgs
    //        {
    //            taskArr = pd.taskArr
    //        };
    //    }
    //    else
    //    {
    //        return null;
    //    }
    //}
}
