/****************************************************
	文件：ServerRoot.cs
	作者：章校长
	邮箱: 1728722243@qq.com
	日期：2019/11/01 14:20   	
	功能：服务器初始化
*****************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        CacheSvc.Instance.Init();
        NetSvc.Instance.Init();
        //业务层
        LoginSys.Instance.Init();
    }

    /// <summary>
    /// 需要循环调用的逻辑
    /// </summary>
    public void Update()
    {
        NetSvc.Instance.Update();
    }
}
