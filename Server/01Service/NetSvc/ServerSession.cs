using PENet;
using MySql.Data.MySqlClient;
public class ServerSession : PESession<GameMsg>
{
    public int sessionID;

    protected override void OnConnected()
    {
        sessionID = ServerRoot.Instance.GetSettionID();
        PECommon.Log($"sessionID:{sessionID},Client Connected");
    }

    protected override void OnReciveMsg(GameMsg msg)
    {
        PECommon.Log($"sessionID:{sessionID},RecPack CMD:{((CMD)msg.cmd).ToString()}");
        NetSvc.Instance.AddPackQue(new MsgPack(this,msg));
    }

    protected override void OnDisConnected()
    {
        PECommon.Log($"sessionID:{sessionID},Client DisConnected");
        LoginSys.Instance.ClearOffLineData(this);
    }


}
