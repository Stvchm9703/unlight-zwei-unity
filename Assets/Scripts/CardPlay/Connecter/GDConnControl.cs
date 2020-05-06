using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
    public Room CurrentRoom;
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
    async void Start() {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("room_connector");
        foreach (var t in objs) {
            Debug.Log(t.name);
            var v = t.GetComponent<RoomServiceConn>();
            // this.CurrentRoom = v.CurrentRoom;
            this.RoomConn = v;
        }
        await this.InitConnSetup(this.RoomConn.config);
        if (RoomConn.IsHost && !RoomConn.IsWatcher) {
            await this.CreateGameSet(this.RoomConn.CurrentRoom.CharCardNvn);
        } else {
            await this.GetGameData(this.RoomConn.CurrentRoom.Key, RoomConn.IsWatcher);
        }
        InitGameCtlSetup();
    }
    private void Update() {
        if (UpdateFlag) {
            UpdateFlag = false;
        }
    }
    // Custom Code 
    public async Task<bool> InitConnSetup(CfServerSetting setting) {
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
        natConn.SubscribeAsync($"ULZ.GDSvc/{this.CurrentRoom.Key}", OnSubMsgHandle);
        return false;
    }

    public void InitGameCtlSetup() {
        if (this.RoomConn == null || this.RoomConn.CurrentRoom == null) {
            return;
        }
        if (!this.RoomConn.IsHost) {
            this.HostPhaseRender =
                this.transform.parent.Find("PhaseLayer/duelphase").GetComponent<CCPhaseRender>();
            this.DuelPhaseRender =
                this.transform.parent.Find("PhaseLayer/selfphase").GetComponent<CCPhaseRender>();

            this.HostSkillRender =
                this.transform.parent.Find("SkillRender/DuelSkillRender").GetComponent<CCSkillRender>();
            this.DuelSkillRender =
                this.transform.parent.Find("SkillRender/SelfSkillRender").GetComponent<CCSkillRender>();

            this.cCardResx.DuelCC_ID = this.RoomConn.CurrentRoom.HostCharcardId;
            this.cCardResx.DuelCC_Level = this.RoomConn.CurrentRoom.HostCardlevel;
            this.cCardResx.DuelCardSet_ID = this.RoomConn.CurrentRoom.HostCardsetId;

            this.cCardResx.SelfCC_ID = this.RoomConn.CurrentRoom.DuelCharcardId;
            this.cCardResx.SelfCC_Level = this.RoomConn.CurrentRoom.DuelCardlevel;
            this.cCardResx.SelfCardSet_ID = this.RoomConn.CurrentRoom.DuelCardsetId;

        } else {
            this.cCardResx.DuelCC_ID = this.RoomConn.CurrentRoom.DuelCharcardId;
            this.cCardResx.DuelCC_Level = this.RoomConn.CurrentRoom.DuelCardlevel;
            this.cCardResx.DuelCardSet_ID = this.RoomConn.CurrentRoom.DuelCardsetId;

            this.cCardResx.SelfCC_ID = this.RoomConn.CurrentRoom.HostCharcardId;
            this.cCardResx.SelfCC_Level = this.RoomConn.CurrentRoom.HostCardlevel;
            this.cCardResx.SelfCardSet_ID = this.RoomConn.CurrentRoom.HostCardsetId;
        }

        StartCoroutine(this.cCardResx.StartResxLoad());
        StartCoroutine(this.cCardResx.SelfCCImplement());
        StartCoroutine(this.cCardResx.DuelCCImplement());
    }

    void OnSubMsgHandle(object caller, NATS.Client.MsgHandlerEventArgs income) {
        Debug.Log(caller);
        var inst_msg = GDBroadcastResp.Parser.ParseFrom(income.Message.Data);
        Debug.Log(inst_msg);
        Debug.Log(inst_msg.Command.ToString());
        switch (inst_msg.Command) {
            case CastCmd.GetGamesetResult:
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

    public async Task<GameDataSet> CreateGameSet(int nvn,
        List<CharCardSet> HostCardList = null, List<CharCardSet> DuelerCardList = null,
        List<EventCard> HostECList = null, List<EventCard> DuelerECList = null) {
        if (this.CurrentRoom == null) {
            throw new System.Exception("NO_ROOM_EXIST");
        }
        if (this.GDSClient == null) {
            throw new System.Exception("NO_CLIENT_EXIST");
        }

        try {
            var req = new GDCreateReq {
                RoomKey = CurrentRoom.Key,
                HostId = CurrentRoom.Host.Id,
                DuelerId = CurrentRoom.Dueler.Id,
                Nvn = nvn,
            };
            req.HostCardDeck.AddRange(HostCardList);
            req.DuelCardDeck.AddRange(DuelerCardList);
            req.HostExtraEc.AddRange(HostECList);
            req.DuelExtraEc.AddRange(DuelerECList);

            var tmpGameSet = await this.GDSClient.CreateGameAsync(req);
            this.CurrentGS = tmpGameSet;
            return this.CurrentGS;
        } catch (RpcException) {
            throw;
        }
    }
    public async Task<GameDataSet> GetGameData(string roomKey, bool isWatcher) {
        if (this.CurrentRoom == null) {
            throw new System.Exception("NO_ROOM_EXIST");
        }
        if (this.GDSClient == null) {
            throw new System.Exception("NO_CLIENT_EXIST");
        }

        try {
            var req = new GDGetInfoReq {
                RoomKey = CurrentRoom.Key,
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
        if (this.CurrentRoom == null) {
            throw new System.Exception("NO_ROOM_EXIST");
        }
        if (this.GDSClient == null) {
            throw new System.Exception("NO_CLIENT_EXIST");
        }

        try {
            var req = new GDCreateReq {
                RoomKey = CurrentRoom.Key,
            };
            var tmpGameSet = await this.GDSClient.QuitGameAsync(req);
            return true;
        } catch (RpcException) {
            throw;
        }
    }

    public async Task<bool> InstSetEventCard(List<EventCard> eclist) {
        if (this.CurrentRoom == null) {
            throw new System.Exception("NO_ROOM_EXIST");
        }
        if (this.GDSClient == null) {
            throw new System.Exception("NO_CLIENT_EXIST");
        }

        try {
            var req = new GDInstanceDT {
                RoomKey = CurrentRoom.Key,
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