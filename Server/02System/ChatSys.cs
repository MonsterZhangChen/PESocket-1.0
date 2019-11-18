/****************************************************
	文件：ChatSys.cs
	作者：章校长
	邮箱: 1728722243@qq.com
	日期：2019/11/17 14:51   	
	功能：聊天系统
*****************************************************/

public class ChatSys
{
    private CacheSvc cacheSvc = null;
    private CfgSvc cfgSvc = null;
    private static ChatSys instance = null;
    public static ChatSys Instance
    {
        get
        {
            if (instance == null) instance = new ChatSys();
            return instance;
        }
    }

    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
        cfgSvc = CfgSvc.Instance;
        PECommon.Log("ChatSys Init Done!");
    }

    public void SndChat(MsgPack msgPack)
    {
        SndChat data = msgPack.msg.sndChat;
        PlayerData pd = cacheSvc.GetPlayerDataBySettion(msgPack.session);
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.PshChat,
            pshChat = new PshChat
            {
                name = pd.name,
                chat = data.chat,
            },
        };
        //广播消息
        var lst = cacheSvc.GetOnlineSvrSessions();
        byte[] msgData = PENet.PETool.PackNetMsg(msg);//广播前提前序列化，减少次数
        foreach(var svrsin in lst)
        {
            svrsin.SendMsg(msgData);
        }

    }
}
