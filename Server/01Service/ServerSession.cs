using PENet;

public class ServerSession : PESession<GameMsg>
{
    protected override void OnConnected()
    {
        PECommon.Log("Client Connected");
    }

    protected override void OnReciveMsg(GameMsg msg)
    {
        PECommon.Log("Msg Received"+((CMD)msg.cmd).ToString());
        NetSvc.Instance.AddMsgQue(new MsgPack(this,msg));
    }

    protected override void OnDisConnected()
    {
        PECommon.Log("Client DisConnected");
    }
}
