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
public class GDConnControl : MonoBehaviour {
  // Game other Controller
  public CCardSetUp cCardResx;
  public PhaseTurnCtl phaseTurn;
  public RangeCtl rangeCtl;
  public CCSkillRender HostSkillRender, DuelSkillRender;
  public CCPhaseRender HostPhaseRender, DuelPhaseRender;

  // Room Service Conn
  public RoomServiceConn RoomConn;

  // service-connecter related
  public CfServiceClientSetting GDServiceConfig;
  public GameDuelService.GameDuelServiceClient GDSClient;
  public Grpc.Core.Channel main_ch;
  // broadcast-receiver 
  public NATS.Client.Options natOpt;
  public NATS.Client.IConnection natConn;

  // monitered data
  public GameDataSet CurrentGS;

  public bool UpdateFlag;
  // Life-cycle of the process
  void Start() {
    StartCoroutine(FullSetup());
    // StartCoroutine( InitGameCtlSetup());
  }
  private void Update() {
    if (UpdateFlag) {
      UpdateFlag = false;
    }
  }
  IEnumerator FullSetup() {
    // yield return new WaitForSeconds(2);
    GameObject[] objs;
    do {
      objs = GameObject.FindGameObjectsWithTag("room_connector");
    } while (objs.Length != 1);
    Debug.Log(objs[0].name);
    var v = objs[0].GetComponent<RoomServiceConn>();
    this.RoomConn = v;
    this.InitConnSetup(this.RoomConn.config);
    // yield return true;

    if (RoomConn.IsHost && !RoomConn.IsWatcher) {
      Debug.Log("Host-Create");
      var task = this.CreateGameSet(this.RoomConn.CurrentRoom.CharCardNvn).Result;
      this.SendMsg("Host:CreatedGame,GetGameSet");
    }
    yield return InitGameCtlSetup();
    yield return true;
  }
  // Custom Code 
  void InitConnSetup(CfServerSetting setting) {
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
    this.natConn = new ConnectionFactory().CreateConnection(this.natOpt);
    this.natConn.SubscribeAsync($"ULZ.GDSvc/{this.RoomConn.CurrentRoom.Key}", this.OnSubMsgHandle);
  }

  IEnumerator InitGameCtlSetup() {
    if (this.RoomConn == null || this.RoomConn.CurrentRoom == null) {
      yield break;
    }

    if (!this.RoomConn.IsHost) {

      var phase_layer = GameObject.FindGameObjectWithTag("phase_layer").transform;

      this.HostPhaseRender =
        phase_layer.Find("duelphase").GetComponent<CCPhaseRender>();
      this.DuelPhaseRender =
        phase_layer.Find("selfphase").GetComponent<CCPhaseRender>();

      var skill_layer = GameObject.FindGameObjectWithTag("skill_layer").transform;

      this.HostSkillRender =
        skill_layer.Find("DuelSkillRender").GetComponent<CCSkillRender>();
      this.DuelSkillRender =
        skill_layer.Find("SelfSkillRender").GetComponent<CCSkillRender>();

      this.cCardResx.DuelCC_ID = this.RoomConn.CurrentRoom.HostCharcardId;
      this.cCardResx.DuelCC_Level = this.RoomConn.CurrentRoom.HostCardlevel;
      this.cCardResx.DuelCardSet_ID = this.RoomConn.CurrentRoom.HostCardsetId;

      this.cCardResx.SelfCC_ID = this.RoomConn.CurrentRoom.DuelCharcardId;
      this.cCardResx.SelfCC_Level = this.RoomConn.CurrentRoom.DuelCardlevel;
      this.cCardResx.SelfCardSet_ID = this.RoomConn.CurrentRoom.DuelCardsetId;

    } else {
      this.cCardResx.SelfCC_ID = this.RoomConn.CurrentRoom.HostCharcardId;
      this.cCardResx.SelfCC_Level = this.RoomConn.CurrentRoom.HostCardlevel;
      this.cCardResx.SelfCardSet_ID = this.RoomConn.CurrentRoom.HostCardsetId;

      this.cCardResx.DuelCC_ID = this.RoomConn.CurrentRoom.DuelCharcardId;
      this.cCardResx.DuelCC_Level = this.RoomConn.CurrentRoom.DuelCardlevel;
      this.cCardResx.DuelCardSet_ID = this.RoomConn.CurrentRoom.DuelCardsetId;
    }

    yield return (this.cCardResx.StartResxLoad());

    yield return (this.cCardResx.SelfCCImplement());
    yield return (this.cCardResx.DuelCCImplement());
  }

  async void OnSubMsgHandle(object caller, NATS.Client.MsgHandlerEventArgs income) {
    Debug.Log(caller);
    var inst_msg = GDBroadcastResp.Parser.ParseFrom(income.Message.Data);
    Debug.Log(inst_msg);
    Debug.Log(inst_msg.Command.ToString());
    switch (inst_msg.Command) {
      case CastCmd.GetGamesetResult:
        Debug.Log(inst_msg.Msg);
        // GameDataSet task;
        if (inst_msg.Msg == "Host:CreatedGame,GetGameSet" && (this.RoomConn.IsWatcher || !this.RoomConn.IsHost)) {
          this.GetGameData(this.RoomConn.CurrentRoom.Key, this.RoomConn.IsWatcher).Wait();
        }
        Debug.Log(this.CurrentGS.HostCardDeck);
        Debug.Log(this.CurrentGS.DuelCardDeck);
        break;
      case CastCmd.GetEffectResult:
      case CastCmd.GetDrawPhaseResult:
      case CastCmd.GetMovePhaseResult:
      case CastCmd.GetAtkPhaseResult:
      case CastCmd.GetDefPhaseResult:

      case CastCmd.InstanceDamage:
      case CastCmd.InstanceStatusChange:
        break;
    }
  }
  bool SendMsg(string msg) {
    if (this.natConn == null)throw new System.Exception("Null_Conn");
    if (this.RoomConn.CurrentRoom == null || this.RoomConn.CurrentRoom.Key == "")throw new System.Exception("No_Current_Room");

    var msgPack = new GDBroadcastResp {
      RoomKey = this.RoomConn.CurrentRoom.Key,
      Msg = msg,
      Command = CastCmd.GetGamesetResult,
      Side = this.RoomConn.IsHost? PlayerSide.Host : PlayerSide.Dueler,
    };

    this.natConn.Publish($"ULZ.GDSvc/{this.RoomConn.CurrentRoom.Key}", msgPack.ToByteArray());
    return true;
  }

  public async Task<GameDataSet> CreateGameSet(int nvn,
    List<CharCardSet> HostCardList = null, List<CharCardSet> DuelerCardList = null,
    List<EventCard> HostECList = null, List<EventCard> DuelerECList = null) {
    if (this.RoomConn.CurrentRoom == null)throw new System.Exception("NO_ROOM_EXIST");
    if (this.GDSClient == null)throw new System.Exception("NO_CLIENT_EXIST");

    try {
      var req = new GDCreateReq {
        RoomKey = this.RoomConn.CurrentRoom.Key,
        HostId = this.RoomConn.CurrentRoom.Host.Id,
        DuelerId = this.RoomConn.CurrentRoom.Dueler.Id,
        Nvn = nvn,
      };
      req.HostCardDeck.AddRange(HostCardList);
      req.DuelCardDeck.AddRange(DuelerCardList);
      req.HostExtraEc.AddRange(HostECList);
      req.DuelExtraEc.AddRange(DuelerECList);

      var tmpGameSet = await this.GDSClient.CreateGameAsync(req);
      Debug.Log($"{tmpGameSet.CurrPhase}");
      this.CurrentGS = tmpGameSet;
      return this.CurrentGS;
    } catch (RpcException) {
      throw;
    }
  }
  public async Task<GameDataSet> GetGameData(string roomKey, bool isWatcher) {
    if (this.RoomConn.CurrentRoom == null)throw new System.Exception("NO_ROOM_EXIST");
    if (this.GDSClient == null)throw new System.Exception("NO_CLIENT_EXIST");

    try {
      var req = new GDGetInfoReq {
        RoomKey = this.RoomConn.CurrentRoom.Key,
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

  public async Task<bool> InstSetEventCard(List<EventCard> eclist) {
    if (this.CurrentGS == null)throw new System.Exception("NO_ROOM_EXIST");
    if (this.GDSClient == null)throw new System.Exception("NO_CLIENT_EXIST");

    try {
      var req = new GDInstanceDT {
        RoomKey = CurrentGS.RoomKey,
        Side = this.RoomConn.IsHost? PlayerSide.Host : PlayerSide.Dueler,
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