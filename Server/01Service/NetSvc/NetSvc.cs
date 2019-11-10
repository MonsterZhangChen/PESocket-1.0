/****************************************************
	文件：NetSvc.cs
	作者：章校长
	邮箱: 1728722243@qq.com
	日期：2019/11/01 14:33   	
	功能：网络服务
*****************************************************/

using PENet;
using System.Collections.Generic;


/// <summary>
/// 数据包 它包含数据和要处理它的Session
/// 这里需要传递Settion的原因：服务器端与客户端是一对多的关系，而这里将消息统一分发到单线程处理；
/// 必须获得对应的Settion才能发送与之对应的回应；如果直接在多线程处理，实际是不需要的；
/// 但多线程有线程争用资源问题，需要锁定对应的资源，也是逻辑的一种消耗
/// </summary>
public class MsgPack
{
    public ServerSession session;
    public GameMsg msg;
    public MsgPack(ServerSession session, GameMsg msg)
    {
        this.session = session;
        this.msg = msg;
    }
}

public class NetSvc
{
    private static NetSvc instance = null;
    public static NetSvc Instance
    {
        get
        {
            if (instance == null) instance = new NetSvc();
            return instance;
        }
    }

    public void Init()
    {
        PESocket<ServerSession, GameMsg> server = new PESocket<ServerSession, GameMsg>();
        server.StartAsServer(SrvCfg.sevAddress, SrvCfg.sevPort);
        PECommon.Log("NetSvc Init Done!");
    }

  
    /// <summary>
    /// 网络接收的消息队列
    /// </summary>
    private Queue<MsgPack> msgPacQue = new Queue<MsgPack>();
    private readonly object obj = new object();
    /// <summary>
    /// 将网络消息添加到队列当中
    /// 这里饶了一个弯子将消息数据集中到单线程处理，而不是在接收到消息时直接处理，
    /// 根据Plane老师的解释：是防止多个Socket的异步接收回调线程访问同一段逻辑代码而出现意想不到的bug。
    /// 个人猜想：如果处理消息System的方法体内本身就不包含共用的字段，是否可以避免争用问题，而且还能提高效率？
    /// 但是回想这种方式仍然不安全，因为业务逻辑可能与其他类仍然耦合，所以单线程仍然是保险的方式
    /// </summary>
    /// <param name="msg"></param>
    public void AddPackQue(MsgPack pack)
    {
        lock(obj)
        {
            msgPacQue.Enqueue(pack);
        }
    }

    /// <summary>
    /// 需要不断更新的方法
    /// </summary>
    public void Update()
    {
        if (msgPacQue.Count > 0)
        {
            //PETool.LogMsg("msgCount:" + msgPacQue.Count);
            MsgPack pack;
            lock (obj)
            {
                pack = msgPacQue.Dequeue();
            }
            HandleOutMsgPack(pack);
        }
    }

    /// <summary>
    /// 处理取出的消息，它是一个消息分发器，依据消息cmd字段所指示的不同类型，将消息分发到不同逻辑系统处理
    /// </summary>
    /// <param name="pack"></param>
    public void HandleOutMsgPack(MsgPack pack)
    {
        switch ((CMD)pack.msg.cmd)
        {
            case CMD.None:
                break;
            case CMD.ReqLogin:
                LoginSys.Instance.ReqLogin(pack);
                break;
            case CMD.ReqRename:
                LoginSys.Instance.ReqRename(pack);
                break;
            case CMD.ReqGuide:
                GuideSys.Instance.ReqGuide(pack);
                break;
            default:
                break;
        }
    }
}
