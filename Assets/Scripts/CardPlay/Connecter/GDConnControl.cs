using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using NATS.Client;
using ULZAsset.Config;
using ULZAsset.MsgExtension;
using ULZAsset.ProtoMod.GameDuelService;
using ULZAsset.ProtoMod.RoomService;
using UnityEngine;

public class GDConnControl : Singleton<GDConnControl> {
  // Game other Controller

  // service-connecter related
  public CfServiceClientSetting GDServiceConfig;
  public GameDuelService.GameDuelServiceClient GDSClient;
  public Grpc.Core.Channel main_ch;
  // broadcast-receiver 
  public NATS.Client.Options natOpt;
  public NATS.Client.IConnection natConn;

  // monitered data
  public GameDataSet CurrentGS;
  private bool isInited = false,
  UpdateFlag;
  public string RoomKey;
  // Life-cycle of the process

  // Custom Code 
  public void InitConnSetup(CfServerSetting setting, string RoomKey) {
    Debug.Log(setting);
    this.GDServiceConfig = setting.GameDuelService;
    this.main_ch = new Channel(
      setting.GameDuelService.Host, setting.GameDuelService.Port,
      ChannelCredentials.Insecure
    );

    this.GDSClient = new GameDuelService.GameDuelServiceClient(this.main_ch);

    var streamSet = setting.GameDuelService.StreamSetting;
    this.natOpt = ConnectionFactory.GetDefaultOptions();
    this.natOpt.Url = $"{streamSet.Connector}://{streamSet.Host}:{streamSet.Port}";
    Debug.Log(this.natOpt.Url);
    this.RoomKey = RoomKey;
    this.natConn = new ConnectionFactory().CreateConnection(this.natOpt);
    // this.natConn.SubscribeAsync($"ULZ.GDSvc/{RoomKey}", this.OnSubMsgHandle);
  }

  public bool AddSubMsgHandle(System.EventHandler<MsgHandlerEventArgs> handler) {
    if (this.natConn == null)return false;
    this.natConn.SubscribeAsync($"ULZ.GDSvc/{RoomKey}", handler);
    return true;
  }

  public bool SendMsg(string msg, bool IsHost) {
    if (this.natConn == null)throw new System.Exception("Null_Conn");
    if (this.RoomKey == null)throw new System.Exception("No_Current_Room");

    var msgPack = new GDBroadcastResp {
      RoomKey = this.RoomKey,
      Msg = msg,
      Command = CastCmd.GetGamesetResult,
      Side = IsHost? PlayerSide.Host : PlayerSide.Dueler,
    };

    this.natConn.Publish($"ULZ.GDSvc/{this.RoomKey}", msgPack.ToByteArray());
    return true;
  }

  public async Task<GameDataSet> CreateGameSet(
    int nvn, string HostId, string DuelId,
    List<CharCardSet> HostCardList = null, List<CharCardSet> DuelerCardList = null,
    List<EventCard> HostECList = null, List<EventCard> DuelerECList = null) {
    // if (this.RoomConn.CurrentRoom == null)throw new System.Exception("NO_ROOM_EXIST");
    if (this.RoomKey == null)throw new System.Exception("NO_ROOM_KEY");
    if (this.GDSClient == null)throw new System.Exception("NO_CLIENT_EXIST");
    if (this.CurrentGS != null)return this.CurrentGS;
    try {
      var req = new GDCreateReq {
        RoomKey = RoomKey,
        HostId = HostId,
        DuelerId = DuelId,
        Nvn = nvn,
      };

      if (HostCardList != null)req.HostCardDeck.AddRange(HostCardList);
      if (DuelerCardList != null)req.DuelCardDeck.AddRange(DuelerCardList);
      if (HostECList != null)req.HostExtraEc.AddRange(HostECList);
      if (DuelerECList != null)req.DuelExtraEc.AddRange(DuelerECList);

      var tmpGameSet = await this.GDSClient.CreateGameAsync(req);
      Debug.Log($"{tmpGameSet.CurrPhase}");
      this.CurrentGS = tmpGameSet;
      return this.CurrentGS;
    } catch (RpcException) {
      throw;
    }
  }
  public async Task<GameDataSet> GetGameData(string roomKey, bool isWatcher) {
    if (this.GDSClient == null)throw new System.Exception("NO_CLIENT_EXIST");
    // if (this.RoomKey == null || this.RoomKey == "")throw new System.Exception("NO_ROOM_KEY");

    try {
      var req = new GDGetInfoReq {
        RoomKey = roomKey == null ? this.RoomKey : roomKey,
        IsWatcher = isWatcher,
      };
      var tmpGameSet = await this.GDSClient.GetGameDataAsync(req);
      this.CurrentGS = tmpGameSet;
      return this.CurrentGS;
    } catch (RpcException) {
      throw;
    }
  }

  public async Task<bool> QuitGame() {
    if (this.CurrentGS == null)throw new System.Exception("NO_ROOM_EXIST");
    if (this.GDSClient == null)throw new System.Exception("NO_CLIENT_EXIST");

    try {
      var req = new GDCreateReq {
        RoomKey = CurrentGS.RoomKey,
      };
      var tmpGameSet = await this.GDSClient.QuitGameAsync(req);
      return true;
    } catch (RpcException) {
      throw;
    }
  }

  public async Task<bool> InstSetEventCard(List<EventCard> eclist, bool isHost) {
    if (this.CurrentGS == null)throw new System.Exception("NO_ROOM_EXIST");
    if (this.GDSClient == null)throw new System.Exception("NO_CLIENT_EXIST");

    try {
      var req = new GDInstanceDT {
        RoomKey = CurrentGS.RoomKey,
        Side = isHost? PlayerSide.Host : PlayerSide.Dueler,
        CurrentPhase = this.CurrentGS.EventPhase,
      };
      foreach (var ec in eclist) {
        req.UpdateCard.Add(GDMsg.ConvertToEC(ec));
      }
      await this.GDSClient.InstSetEventCardAsync(req);
      return true;
    } catch (RpcException) {
      throw;
    }
  }

}