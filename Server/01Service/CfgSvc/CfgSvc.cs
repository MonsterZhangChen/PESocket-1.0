/****************************************************
	文件：CfgSvc.cs
	作者：章校长
	邮箱: 1728722243@qq.com
	日期：2019/11/09 17:22   	
	功能：配置文件读取服务
*****************************************************/
using System;
using System.Collections.Generic;
using System.Xml;

/// <summary>
///  配置文件包含了关键的游戏中产生的奖励数据，它可以和客户端公用，但客户端只负责拿它显示，而关键的数据计算与修改必须在服务器
///  中进行，并将结果值返回给客户端用于显示；
///  对于同一份配置文件，由于对客户端和服务端侧重点不同，客户端注重显示过程，服务器注重存储数据，所有可以对配置文件有着不同的解释
/// </summary>
class CfgSvc
{
    private static CfgSvc instance = null;
    public static CfgSvc Instance
    {
        get
        {
            if (instance == null) instance = new CfgSvc();
            return instance;
        }
    }

    public void Init()
    {
        PECommon.Log("CfgSvc Init Done!");
        InitGuideTaskCfg(@"E:\U3dProject\DarkGod\Assets\Resources\ResCfgs\guide.xml");
    }

    #region 任务引导信息配置
    private Dictionary<int, GuideTaskCfg> guideTaskDic = new Dictionary<int, GuideTaskCfg>();
    /// <summary>
    /// 初始化读取任务配置;
    /// </summary>
    /// <param name="path"></param>
    public void InitGuideTaskCfg(string path)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(path);
        XmlNodeList nodeList = doc.SelectSingleNode("root").ChildNodes;
        for (int i = 0; i < nodeList.Count; i++)
        {
            XmlElement ele = nodeList[i] as XmlElement;
            if (ele.GetAttributeNode("ID") == null) continue;
            int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);//获得元素ele的属性ID的值，将它转换为整型
            GuideTaskCfg gtc = new GuideTaskCfg() { ID = ID };
            foreach (XmlElement e in ele.ChildNodes)
            {
                switch (e.Name)
                {
                    case "coin":
                        gtc.coin = int.Parse(e.InnerText);
                        break;
                    case "exp":
                        gtc.exp = int.Parse(e.InnerText);
                        break;
                }
            }
            guideTaskDic.Add(gtc.ID, gtc);
        }
    }

    /// <summary>
    /// 通过任务ID号，获取任务引导配置信息
    /// </summary>
    /// <param name="taskID">任务id</param>
    /// <returns></returns>
    public GuideTaskCfg GetGuideTaskCfgData(int taskID)
    {
        guideTaskDic.TryGetValue(taskID, out GuideTaskCfg gtc);
        return gtc;
    }
    #endregion
}




/// <summary>
/// 任务信息配置数据
/// </summary>
public class GuideTaskCfg : BaseData<GuideTaskCfg>
{
    public int coin;
    public int exp;
}



/// <summary>
/// 配置数据基类
/// </summary>
/// <typeparam name="T"></typeparam>
public class BaseData<T>
{
    public int ID;
}
