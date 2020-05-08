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

public class MainCtrlComp : MonoBehaviour {
    // Start is called before the first frame update
    public CCardSetUp cCardResx;

    public PhaseTurnCtl phaseTurn;
    public RangeCtl rangeCtl;
    public CCSkillRender HostSkillRender, DuelSkillRender;
    public CCPhaseRender HostPhaseRender, DuelPhaseRender;

    // Room Service Conn
    public GDConnControl GDConn;
    public RoomServiceConn RoomConn;
    async void Start() {
        await FullSetup();
    }

    // Update is called once per frame
    void Update() {

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
    async Task<bool> FullSetup() {
        Debug.Log("MyScript.Start " + GetInstanceID(), this);
        var objs = GameObject.FindGameObjectWithTag("room_connector");
        var v = objs.GetComponent<RoomServiceConn>();
        this.RoomConn = v;
        this.GDConn.InitConnSetup(this.RoomConn.config, this.RoomConn.CurrentRoom.Key);
        this.GDConn.AddSubMsgHandle(OnSubMsgHandle);
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

            await this.GDConn.CreateGameSet(
                this.RoomConn.CurrentRoom.CharCardNvn,
                this.RoomConn.CurrentRoom.Host.Id, this.RoomConn.CurrentRoom.Dueler.Id,
                hostPackList, duelPackList
            );
            // this.SendMsg("Host:CreatedGame,GetGameSet");
        }
        return true;
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
                    await this.GDConn.GetGameData(this.RoomConn.CurrentRoom.Key, this.RoomConn.IsWatcher);
                }
                Debug.Log(this.GDConn.CurrentGS.HostCardDeck);
                Debug.Log(this.GDConn.CurrentGS.DuelCardDeck);
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
}