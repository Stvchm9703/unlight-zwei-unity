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
  private bool isInited = false, UpdateFlag;
  public string RoomKey;
  // Life-cycle of the process

  // Custom Code 
  void Await() {
    GameObject[] tmp = GameObject.FindGameObjectsWithTag("gd_connector");
    Debug.Log(tmp.Length);
    if (tmp.Length > 1 ) {
      Destroy(this.gameObject);
      // return;
    } else {
      DontDestroyOnLoad(this.gameObject);
      Debug.Log("GDConn Start");
    }
  }
  public void InitConnSetup(CfServerSetting setting, string roomKey) {
    Debug.Log(setting);
    GDServiceConfig = setting.GameDuelService;
    main_ch = new Channel(
      setting.GameDuelService.Host, setting.GameDuelService.Port,
      ChannelCredentials.Insecure
    );

    GDSClient = new GameDuelService.GameDuelServiceClient(main_ch);

    var streamSet = setting.GameDuelService.StreamSetting;
    natOpt = ConnectionFactory.GetDefaultOptions();
    natOpt.Url = $"{streamSet.Connector}://{streamSet.Host}:{streamSet.Port}";
    Debug.Log(natOpt.Url);
    RoomKey = roomKey;
    natConn = new ConnectionFactory().CreateConnection(natOpt);
    // natConn.SubscribeAsync($"ULZ.GDSvc/{RoomKey}", OnSubMsgHandle);
  }

  public bool AddSubMsgHandle(System.EventHandler<MsgHandlerEventArgs> handler) {
    if (natConn == null)return false;
    natConn.SubscribeAsync($"ULZ.GDSvc/{RoomKey}", handler);
    return true;
  }

  public bool SendMsg(string msg, bool IsHost) {
    if (natConn == null)throw new System.Exception("Null_Conn");
    if (RoomKey == null)throw new System.Exception("No_Current_Room");

    var msgPack = new GDBroadcastResp {
      RoomKey = RoomKey,
      Msg = msg,
      Command = CastCmd.GetGamesetResult,
      Side = IsHost? PlayerSide.Host : PlayerSide.Dueler,
    };

    natConn.Publish($"ULZ.GDSvc/{RoomKey}", msgPack.ToByteArray());
    return true;
  }

  public async Task<GameDataSet> CreateGameSet(
    int nvn, string HostId, string DuelId,
    List<CharCardSet> HostCardList = null, List<CharCardSet> DuelerCardList = null,
    List<EventCard> HostECList = null, List<EventCard> DuelerECList = null) {
    // if (RoomConn.CurrentRoom == null)throw new System.Exception("NO_ROOM_EXIST");
    if (RoomKey == null)throw new System.Exception("NO_ROOM_KEY");
    if (GDSClient == null)throw new System.Exception("NO_CLIENT_EXIST");
    if (CurrentGS != null)return CurrentGS;
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

      var tmpGameSet = await GDSClient.CreateGameAsync(req);
      Debug.Log($"{tmpGameSet.CurrPhase}");
      CurrentGS = tmpGameSet;
      return CurrentGS;
    } catch (RpcException) {
      throw;
    }
  }
  public async Task<GameDataSet> GetGameData(string roomKey, bool isWatcher) {
    if (GDSClient == null)throw new System.Exception("NO_CLIENT_EXIST");
    // if (RoomKey == null || RoomKey == "")throw new System.Exception("NO_ROOM_KEY");

    try {
      var req = new GDGetInfoReq {
        RoomKey = roomKey == null ? RoomKey : roomKey,
        IsWatcher = isWatcher,
      };
      var tmpGameSet = await GDSClient.GetGameDataAsync(req);
      CurrentGS = tmpGameSet;
      return CurrentGS;
    } catch (RpcException) {
      throw;
    }
  }

  public async Task<bool> QuitGame() {
    if (CurrentGS == null)throw new System.Exception("NO_ROOM_EXIST");
    if (GDSClient == null)throw new System.Exception("NO_CLIENT_EXIST");

    try {
      var req = new GDCreateReq {
        RoomKey = CurrentGS.RoomKey,
      };
      var tmpGameSet = await GDSClient.QuitGameAsync(req);
      return true;
    } catch (RpcException) {
      throw;
    }
  }

  public async Task<bool> InstSetEventCard(List<EventCard> eclist, bool isHost) {
    if (CurrentGS == null)throw new System.Exception("NO_ROOM_EXIST");
    if (GDSClient == null)throw new System.Exception("NO_CLIENT_EXIST");

    try {
      var req = new GDInstanceDT {
        RoomKey = CurrentGS.RoomKey,
        Side = isHost? PlayerSide.Host : PlayerSide.Dueler,
        CurrentPhase = CurrentGS.EventPhase,
      };
      foreach (var ec in eclist) {
        req.UpdateCard.Add(GDMsg.ConvertToEC(ec));
      }
      await GDSClient.InstSetEventCardAsync(req);
      return true;
    } catch (RpcException) {
      throw;
    }
  }

}