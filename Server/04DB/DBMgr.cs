/****************************************************
	文件：DBMgr.cs
	作者：章校长
	邮箱: 1728722243@qq.com
	日期：2019/11/02 15:50   	
	功能：数据库管理类
*****************************************************/
using MySql.Data.MySqlClient;
using System;
/// <summary>
/// 数据库管理类
/// </summary>
public class DBMgr
{
    private static DBMgr instance = null;
    public static DBMgr Instance
    {
        get
        {
            if (instance == null) instance = new DBMgr();
            return instance;
        }
    }


    private MySqlConnection conn;
    public void Init()
    {
        conn = new MySqlConnection("database=darkgod;datasource=127.0.0.1;port=3306;userid=root;pwd=root;charset=utf8");
        conn.Open();
        PECommon.Log("DBMgr Init Done!");
    }

    /// <summary>
    /// 从数据库中查询玩家信息；
    /// 1)如果账密匹配，则返回玩家数据；2）如果账密不匹配，则返回空；3）如果账号不存在，则创建一账号并生成信息
    /// </summary>
    /// <param name="acct">账号</param>
    /// <param name="pass">密码</param>
    /// <returns></returns>
    public PlayerData QueryPlayerData(string acct, string pass)
    {
        PlayerData playerData = null;
        bool isNewAcct = true;
        //todo Query
        try
        {
            MySqlCommand cmd = new MySqlCommand("select * from account where acct=@acct", conn);
            cmd.Parameters.AddWithValue("acct", acct);
            using(MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    isNewAcct = false;
                    string _pass = reader.GetString("pass");
                    //账密匹配，返回玩家数据
                    if (_pass == pass)
                    {
                        playerData = new PlayerData
                        {
                            id = reader.GetInt32("id"),
                            name = reader.GetString("name"),
                            lv = reader.GetInt32("level"),
                            exp = reader.GetInt32("exp"),
                            power = reader.GetInt32("power"),
                            coin = reader.GetInt32("coin"),
                            diamond = reader.GetInt32("diamond"),
                            crystal = reader.GetInt32("crystal"),
                            hp = reader.GetInt32("hp"),
                            ad = reader.GetInt32("ad"),
                            ap = reader.GetInt32("ap"),
                            addef = reader.GetInt32("addef"),
                            apdef = reader.GetInt32("apdef"),
                            dodge = reader.GetInt32("dodge"),
                            pierce = reader.GetInt32("pierce"),
                            critical = reader.GetInt32("critical"),
                            guideid = reader.GetInt32("guideid"),
                            strongArr = ParseStrongStr(reader.GetString("strong")),
                        };
                    };
                }
            }
        }
        catch (Exception e)
        {
            PECommon.Log("Query PlayerData By Acct&Pass Error:" + e, LogType.error);
        }
        finally
        {
            //账号不存在，生成新账号并插入数据库(这里应该读取配置文件，此处略)
            if (isNewAcct)
            {
                playerData = new PlayerData
                {
                    id = -1,//这里缓存的实际是个错误的id,数据库会自动生成
                    name = "",
                    lv = 1,
                    exp = 0,
                    power = 150,
                    coin = 5000,
                    diamond = 500,
                    crystal=600,
                    hp = 2000,
                    ad = 275,
                    ap = 265,
                    addef = 67,
                    apdef = 43,
                    dodge = 7,
                    pierce = 5,
                    critical = 2,
                    guideid = 1001,
                    strongArr=new int[6]
                    //to add
                };
                //调试：这里必须换上新id,否则会造成缓冲区的id错误
                playerData.id=InsertNewAcctData(acct, pass, playerData);
            }
        }
        return playerData;
    }

    #region strong
    /// <summary>
    /// 解析升级字符串数据：1#2#3#2#1#6#3#
    /// </summary>
    /// <param name="strongArrStr">升级字符串</param>
    /// <returns></returns>
    private int[] ParseStrongStr(string strongArrStr)
    {
        string[] strongStrArr = strongArrStr.Split('#');
        int[] _StrongArr = new int[6];
        for (int i = 0; i < strongStrArr.Length; i++)
        {
            if (strongStrArr[i] == "") continue;
            if (!int.TryParse(strongStrArr[i], out _StrongArr[i]))
            {
                PECommon.Log("Parse Strong Data Error", LogType.error);
            }
        }
        return _StrongArr;
    }

    /// <summary>
    /// 创建升级字符串数据
    /// </summary>
    /// <param name="strongArr"></param>
    /// <returns></returns>
    private string BulidStrongStr(int[] strongArr)
    {
        string strongStr = "";
        for (int i = 0; i < strongArr.Length; i++)
        {
            strongStr += strongArr[i].ToString() + "#";
        }
        return strongStr;
    }
    #endregion

    /// <summary>
    /// 插入玩家数据
    /// </summary>
    /// <param name="acct">账号</param>
    /// <param name="pass">密码</param>
    /// <param name="pd">玩家数据</param>
    /// <returns>插入数据返回的id值</returns>
    public int InsertNewAcctData(string acct,string pass,PlayerData pd)
    {
        int id = -1;
        try
        {
            MySqlCommand cmd = new MySqlCommand
                ("insert into account set acct=@acct,pass=@pass,name=@name,level=@level," +
                "exp=@exp,power=@power,coin=@coin,diamond=@diamond,crystal=@crystal,hp=@hp,ad=@ad,ap=@ap," +
                "addef=@addef,apdef=@apdef,dodge=@dodge,pierce=@pierce,critical=@critical," +
                "guideid=@guideid,strong=@strong", conn);
            cmd.Parameters.AddWithValue("acct", acct);
            cmd.Parameters.AddWithValue("pass", pass);
            cmd.Parameters.AddWithValue("name", pd.name);
            cmd.Parameters.AddWithValue("level", pd.lv);
            cmd.Parameters.AddWithValue("exp", pd.exp);
            cmd.Parameters.AddWithValue("power", pd.power);
            cmd.Parameters.AddWithValue("coin", pd.coin);
            cmd.Parameters.AddWithValue("diamond", pd.diamond);
            cmd.Parameters.AddWithValue("crystal", pd.crystal);
            cmd.Parameters.AddWithValue("hp", pd.hp);
            cmd.Parameters.AddWithValue("ad", pd.ad);
            cmd.Parameters.AddWithValue("ap", pd.ap);
            cmd.Parameters.AddWithValue("addef", pd.addef);
            cmd.Parameters.AddWithValue("apdef", pd.apdef);
            cmd.Parameters.AddWithValue("dodge", pd.dodge);
            cmd.Parameters.AddWithValue("pierce", pd.pierce);
            cmd.Parameters.AddWithValue("critical", pd.critical);
            cmd.Parameters.AddWithValue("guideid", pd.guideid);
            cmd.Parameters.AddWithValue("strong", BulidStrongStr(pd.strongArr));
            //to add
            cmd.ExecuteNonQuery();
            id = (int)cmd.LastInsertedId;
        }
        catch (Exception e)
        {
            PECommon.Log("Insert PlayerData Error:" + e, LogType.error);
        }
        return id;
    }

    /// <summary>
    /// 查询用户中是否存在name的成员
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool QueryNameExit(string name)
    {
        bool exist = true;
        try
        {
            using(MySqlCommand cmd = new MySqlCommand("select * from account where name=@name", conn))
            {
                cmd.Parameters.AddWithValue("name", name);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        exist = true;
                    }
                    else exist = false;
                }
            }
        }
        catch(Exception e)
        {
            PECommon.Log("Query Name Error:" + e, LogType.error);
        }
        return exist;
    }

    /// <summary>
    /// 将指定id玩家的数据存入数据库中
    /// </summary>
    /// <param name="id"></param>
    /// <param name="playerData">玩家数据</param>
    /// <returns></returns>
    public bool UpdatePlayerData(int id,PlayerData playerData)
    {
        bool isUpdate;
        try
        {
            //关于charset:字符编码，用Navicat时得加上不然会乱码
            MySqlCommand cmd = new MySqlCommand("update account set name=@name,level=@level,exp=@exp,power=@power,coin=@coin,diamond=@diamond,crystal=@crystal," +
                "hp=@hp,ad=@ad,ap=@ap,addef=@addef,apdef=@apdef,dodge=@dodge,pierce=@pierce,critical=@critical,guideid=@guideid,strong=@strong where id=@id", conn);
            cmd.Parameters.AddWithValue("id", playerData.id);
            cmd.Parameters.AddWithValue("name", playerData.name);
            cmd.Parameters.AddWithValue("level", playerData.lv);
            cmd.Parameters.AddWithValue("exp", playerData.exp);
            cmd.Parameters.AddWithValue("power", playerData.power);
            cmd.Parameters.AddWithValue("coin", playerData.coin);
            cmd.Parameters.AddWithValue("diamond", playerData.diamond);
            cmd.Parameters.AddWithValue("crystal", playerData.crystal);
            cmd.Parameters.AddWithValue("hp",playerData.hp);
            cmd.Parameters.AddWithValue("ad", playerData.ad);
            cmd.Parameters.AddWithValue("ap", playerData.ap);
            cmd.Parameters.AddWithValue("addef", playerData.ap);
            cmd.Parameters.AddWithValue("apdef", playerData.apdef);
            cmd.Parameters.AddWithValue("dodge", playerData.dodge);
            cmd.Parameters.AddWithValue("pierce", playerData.pierce);
            cmd.Parameters.AddWithValue("critical", playerData.critical);
            cmd.Parameters.AddWithValue("guideid", playerData.guideid);
            cmd.Parameters.AddWithValue("strong", BulidStrongStr(playerData.strongArr));
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            isUpdate = true;

            //MySqlCommand cmd1 = new MySqlCommand("select * from account where id=@id", conn);
            //cmd1.Parameters.AddWithValue("id",)

        }
        catch(Exception e)
        {
            isUpdate = false;
            Console.WriteLine("UpdatePlayerData Name Error:" + e, LogType.error);
        }
        return isUpdate;
    }
}
