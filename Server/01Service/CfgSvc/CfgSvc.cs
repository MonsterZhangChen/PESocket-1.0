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
        InitStrongCfg(@"E:\U3dProject\DarkGod\Assets\Resources\ResCfgs\strong.xml");
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

    #region 强化升级配置
    /// <summary>
    /// 存储装备强化配置的字典；
    /// 它包含两个键，装备的位置何装备的星级
    /// </summary>
    private Dictionary<int, Dictionary<int, StrongCfg>> strongDic = new Dictionary<int, Dictionary<int, StrongCfg>>();
    private void InitStrongCfg(string path)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(path);

        XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;

        for (int i = 0; i < nodLst.Count; i++)
        {
            XmlElement ele = nodLst[i] as XmlElement;

            if (ele.GetAttributeNode("ID") == null)
            {
                continue;
            }
            int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
            StrongCfg sd = new StrongCfg
            {
                ID = ID
            };

            foreach (XmlElement e in nodLst[i].ChildNodes)
            {
                int val = int.Parse(e.InnerText);
                switch (e.Name)
                {
                    case "pos":
                        sd.pos = val;
                        break;
                    case "starlv":
                        sd.startlv = val;
                        break;
                    case "addhp":
                        sd.addhp = val;
                        break;
                    case "addhurt":
                        sd.addhurt = val;
                        break;
                    case "adddef":
                        sd.adddef = val;
                        break;
                    case "minlv":
                        sd.minlv = val;
                        break;
                    case "coin":
                        sd.coin = val;
                        break;
                    case "crystal":
                        sd.crystal = val;
                        break;
                }
            }

            Dictionary<int, StrongCfg> dic;
            if (strongDic.TryGetValue(sd.pos, out dic))
            {
                dic.Add(sd.startlv, sd);
            }
            else
            {
                dic = new Dictionary<int, StrongCfg>();
                dic.Add(sd.startlv, sd);
                strongDic.Add(sd.pos, dic);
            }
        }
    }

    /// <summary>
    /// 通过装备的位置和星级获得强化升级配置
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="starlv"></param>
    /// <returns></returns>
    public StrongCfg GetStrongCfg(int pos, int starlv)
    {
        StrongCfg sd = null;
        Dictionary<int, StrongCfg> dic = null;
        if (strongDic.TryGetValue(pos, out dic))
        {
            if (dic.ContainsKey(starlv))
            {
                sd = dic[starlv];
            }
        }
        return sd;
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
/// 强化升级配置
/// </summary>
public class StrongCfg : BaseData<StrongCfg>
{
    /// <summary>
    /// 装备标志位
    /// </summary>
    public int pos;
    /// <summary>
    /// 星级
    /// </summary>
    public int startlv;
    /// <summary>
    /// 增加血量值
    /// </summary>
    public int addhp;
    /// <summary>
    /// 增加伤害值
    /// </summary>
    public int addhurt;
    /// <summary>
    /// 增加防御值
    /// </summary>
    public int adddef;
    /// <summary>
    /// 升级最小等级
    /// </summary>
    public int minlv;
    /// <summary>
    /// 升级所需金币
    /// </summary>
    public int coin;
    /// <summary>
    /// 升级所需水晶
    /// </summary>
    public int crystal;
}

/// <summary>
/// 配置数据基类
/// </summary>
/// <typeparam name="T"></typeparam>
public class BaseData<T>
{
    public int ID;
}
