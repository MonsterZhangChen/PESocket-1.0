/****************************************************
	文件：TimerSvc.cs
	作者：章校长
	邮箱: 1728722243@qq.com
	日期：2019/11/24 14:13   	
	功能：计时服务
*****************************************************/
using System;
using System.Collections.Generic;

class TimerSvc
{
    private static TimerSvc instance = null;
    public static TimerSvc Instance
    {
        get
        {
            if (instance == null) instance = new TimerSvc();
            return instance;
        }
    }

    private PETimer timer;
    /// <summary>
    /// 任务队列
    /// </summary>
    private Queue<TaskPack> tpQue = new Queue<TaskPack>();
    private static readonly object tpQueLock = new object();
    public void Init()
    {
        PECommon.Log("TimerSvc Init Done!");
        timer = new PETimer(100);//表征100ms调用一次事件的计时器
        timer.SetLog(info => PECommon.Log(info));
        //设置Handle的逻辑体，即驱动函数在驱动至一个满足条件的回调时，不再执行回调，而是将回调加入队列当中
        timer.SetHandle((cb, tid) =>
        {
            lock (tpQueLock)
            {
                tpQue.Enqueue(new TaskPack(tid, cb));
            }
        });
    }

    /// <summary>
    /// 这个函数在主函数的循环中驱动；
    /// 注意：入队是通过PETimer自身的srvTimer异步驱动，而出对执行则仍然要依靠主函数循环驱动；
    /// Plane设计的思路是，既希望它异步驱动，但是不希望它异步的执行回调（因为异步存在资源争用的问题）
    /// </summary>
    public void Update()
    {
        //timer.Update();
        //单线程处理
        TaskPack tp;
        while (tpQue.Count > 0)
        {
            lock (tpQueLock)
            {
                tp = tpQue.Dequeue();
            }
            if (tp != null)
            {
                tp.cb(tp.tid);
            }
        }
    }

    public int AddTimeTask(Action<int> callback, double delay, PETimeUnit timeUnit = PETimeUnit.Millisecond, int count = 1)
    {
        return timer.AddTimeTask(callback, delay, timeUnit, count);
    }

    public long GetNowTime()
    {
        return (long)timer.GetMillisecondsTime();
}
}

/// <summary>
/// 任务包：包含任务id和任务回调，它被放置在队列中，在单线程中调用
/// </summary>
public class TaskPack
{
    public int tid;
    public Action<int> cb;
    public TaskPack(int tid, Action<int> cb)
    {
        this.tid = tid;
        this.cb = cb;
    }
}



