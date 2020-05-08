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
    instance.GDServiceConfig = setting.GameDuelService;
    instance.main_ch = new Channel(
      setting.GameDuelService.Host, setting.GameDuelService.Port,
      ChannelCredentials.Insecure
    );

    instance.GDSClient = new GameDuelService.GameDuelServiceClient(instance.main_ch);

    var streamSet = setting.GameDuelService.StreamSetting;
    instance.natOpt = ConnectionFactory.GetDefaultOptions();
    instance.natOpt.Url = $"{streamSet.Connector}://{streamSet.Host}:{streamSet.Port}";
    Debug.Log(instance.natOpt.Url);
    instance.RoomKey = RoomKey;
    instance.natConn = new ConnectionFactory().CreateConnection(instance.natOpt);
    // instance.natConn.SubscribeAsync($"ULZ.GDSvc/{RoomKey}", instance.OnSubMsgHandle);
  }

  public bool AddSubMsgHandle(System.EventHandler<MsgHandlerEventArgs> handler) {
    if (instance.natConn == null)return false;
    instance.natConn.SubscribeAsync($"ULZ.GDSvc/{RoomKey}", handler);
    return true;
  }

  public bool SendMsg(string msg, bool IsHost) {
    if (instance.natConn == null)throw new System.Exception("Null_Conn");
    if (instance.RoomKey == null)throw new System.Exception("No_Current_Room");

    var msgPack = new GDBroadcastResp {
      RoomKey = instance.RoomKey,
      Msg = msg,
      Command = CastCmd.GetGamesetResult,
      Side = IsHost? PlayerSide.Host : PlayerSide.Dueler,
    };

    instance.natConn.Publish($"ULZ.GDSvc/{instance.RoomKey}", msgPack.ToByteArray());
    return true;
  }

  public async Task<GameDataSet> CreateGameSet(
    int nvn, string HostId, string DuelId,
    List<CharCardSet> HostCardList = null, List<CharCardSet> DuelerCardList = null,
    List<EventCard> HostECList = null, List<EventCard> DuelerECList = null) {
    // if (instance.RoomConn.CurrentRoom == null)throw new System.Exception("NO_ROOM_EXIST");
    if (instance.RoomKey == null)throw new System.Exception("NO_ROOM_KEY");
    if (instance.GDSClient == null)throw new System.Exception("NO_CLIENT_EXIST");
    if (instance.CurrentGS != null)return instance.CurrentGS;
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

      var tmpGameSet = await instance.GDSClient.CreateGameAsync(req);
      Debug.Log($"{tmpGameSet.CurrPhase}");
      instance.CurrentGS = tmpGameSet;
      return instance.CurrentGS;
    } catch (RpcException) {
      throw;
    }
  }
  public async Task<GameDataSet> GetGameData(string roomKey, bool isWatcher) {
    if (instance.GDSClient == null)throw new System.Exception("NO_CLIENT_EXIST");
    // if (instance.RoomKey == null || instance.RoomKey == "")throw new System.Exception("NO_ROOM_KEY");

    try {
      var req = new GDGetInfoReq {
        RoomKey = roomKey == null ? instance.RoomKey : roomKey,
        IsWatcher = isWatcher,
      };
      var tmpGameSet = await instance.GDSClient.GetGameDataAsync(req);
      instance.CurrentGS = tmpGameSet;
      return instance.CurrentGS;
    } catch (RpcException) {
      throw;
    }
  }

  public async Task<bool> QuitGame() {
    if (instance.CurrentGS == null)throw new System.Exception("NO_ROOM_EXIST");
    if (instance.GDSClient == null)throw new System.Exception("NO_CLIENT_EXIST");

    try {
      var req = new GDCreateReq {
        RoomKey = CurrentGS.RoomKey,
      };
      var tmpGameSet = await instance.GDSClient.QuitGameAsync(req);
      return true;
    } catch (RpcException) {
      throw;
    }
  }

  public async Task<bool> InstSetEventCard(List<EventCard> eclist, bool isHost) {
    if (instance.CurrentGS == null)throw new System.Exception("NO_ROOM_EXIST");
    if (instance.GDSClient == null)throw new System.Exception("NO_CLIENT_EXIST");

    try {
      var req = new GDInstanceDT {
        RoomKey = CurrentGS.RoomKey,
        Side = isHost? PlayerSide.Host : PlayerSide.Dueler,
        CurrentPhase = instance.CurrentGS.EventPhase,
      };
      foreach (var ec in eclist) {
        req.UpdateCard.Add(GDMsg.ConvertToEC(ec));
      }
      await instance.GDSClient.InstSetEventCardAsync(req);
      return true;
    } catch (RpcException) {
      throw;
    }
  }

}