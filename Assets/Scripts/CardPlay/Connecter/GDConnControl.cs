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
  public CCardSetUp cCardResx;
  public PhaseTurnCtl phaseTurn;
  public RangeCtl rangeCtl;
  public CCSkillRender HostSkillRender,
  DuelSkillRender;
  public CCPhaseRender HostPhaseRender,
  DuelPhaseRender;

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
  private bool isInited = false;
  public bool UpdateFlag;
  // Life-cycle of the process
  protected virtual async void Start() {
    if (this.cCardResx == null)this.cCardResx = GameObject.Find("EventSystem").GetComponent<CCardSetUp>();
    if (this.phaseTurn == null)this.phaseTurn = GameObject.Find("VisualEffectLayer/phase_turn").GetComponent<PhaseTurnCtl>();
    if (this.rangeCtl == null)this.rangeCtl = GameObject.Find("StandLayer").GetComponent<RangeCtl>();
    if (this.HostSkillRender == null)this.HostSkillRender = GameObject.Find("SkillLayer/SelfSkillRender").GetComponent<CCSkillRender>();
    if (this.DuelSkillRender == null)this.DuelSkillRender = GameObject.Find("SkillLayer/DuelSkillRender").GetComponent<CCSkillRender>();
    if (this.HostPhaseRender == null)this.HostPhaseRender = GameObject.Find("PhaseLayer/selfphase").GetComponent<CCPhaseRender>();
    if (this.DuelPhaseRender == null)this.DuelPhaseRender = GameObject.Find("PhaseLayer/duelphase").GetComponent<CCPhaseRender>();

    if (!isInited)
      isInited = await FullSetup();
    // StartCoroutine( InitGameCtlSetup());
  }

  protected virtual void Update() {
    // if (UpdateFlag) {
    //   UpdateFlag = false;
    // }
  }
  async Task<bool> FullSetup() {
    Debug.Log("MyScript.Start " + GetInstanceID(), this);
    var objs = GameObject.FindGameObjectWithTag("room_connector");
    var v = objs.GetComponent<RoomServiceConn>();
    this.RoomConn = v;
    this.InitConnSetup(this.RoomConn.config);
    // yield return true;
    this.InitGameCtlSetup();

    if (RoomConn.IsHost && !RoomConn.IsWatcher) {
      Debug.Log("Host-Create");
      CharCardSet hostPack = new CharCardSet();
      CharCardSet duelPack = new CharCardSet();
      List<CharCardSet> hostPackList = new List<CharCardSet>();
      List<CharCardSet> duelPackList = new List<CharCardSet>();
      if (this.cCardResx.SelfDataCardSet != null) {
        hostPack = this.cCardResx.SelfDataCardSet.ToCharCardSetRaw();
      }
      if (this.cCardResx.DuelDataCardSet != null) {
        duelPack = this.cCardResx.DuelDataCardSet.ToCharCardSetRaw();
      }
      hostPackList.Add(hostPack);
      duelPackList.Add(duelPack);

      await this.CreateGameSet(
        this.RoomConn.CurrentRoom.CharCardNvn,
        hostPackList, duelPackList
      );
      // this.SendMsg("Host:CreatedGame,GetGameSet");
    }
    return true;
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

  void InitGameCtlSetup() {
    Debug.Log("InitGameCtlSetup");
    if (this.RoomConn == null || this.RoomConn.CurrentRoom == null) {
      return;
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

    StartCoroutine(this.cCardResx.StartResxLoad());

    StartCoroutine(this.cCardResx.SelfCCImplement());
    StartCoroutine(this.cCardResx.DuelCCImplement());
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