/****************************************************
	文件：ServerRoot.cs
	作者：章校长
	邮箱: 1728722243@qq.com
	日期：2019/11/01 14:20   	
	功能：服务器初始化
*****************************************************/

public class ServerRoot
{
    private static ServerRoot instance = null;
    public static ServerRoot Instance
    {
        get
        {
            if (instance == null) instance = new ServerRoot();
            return instance;
        }
    }

    public void Init()
    {
        //todo 数据层
        //服务层
        NetSvc.Instance.Init();
        DBMgr.Instance.Init();
        CacheSvc.Instance.Init();
        //业务层
        CfgSvc.Instance.Init();
        LoginSys.Instance.Init();
        GuideSys.Instance.Init();
        StrongSys.Instance.Init();
        ChatSys.Instance.Init();
        BuySys.Instance.Init();
    }

    /// <summary>
    /// 需要循环调用的逻辑
    /// </summary>
    public void Update()
    {
        NetSvc.Instance.Update();
    }

    /// <summary>
    /// 网络会话id生成种子
    /// </summary>
    private int sessionId = 0;
    public int GetSettionID()
    {
        if (sessionId == int.MaxValue)
        {
            sessionId=0;
        }
        return sessionId++;
    }
}
