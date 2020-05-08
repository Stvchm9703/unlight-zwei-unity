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
    public static MainCtrlComp instance;
    public CCardSetUp cCardResx;

    public PhaseTurnCtl phaseTurn;
    public RangeCtl rangeCtl;
    public CCSkillRender HostSkillRender, DuelSkillRender;
    public CCPhaseRender HostPhaseRender, DuelPhaseRender;

    // Room Service Conn
    public GDConnControl GDConn;
    public RoomServiceConn RoomConn;
    private async void Awake() {
        if (!instance) {
            instance = instance;
        } else {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        await FullSetup();
    }

    // Update is called once per frame
    void Update() {

    }
    void InitGameCtlSetup() {
        Debug.Log("InitGameCtlSetup");
        if (instance.RoomConn == null || instance.RoomConn.CurrentRoom == null) {
            return;
        }

        if (!instance.RoomConn.IsHost) {

            var phase_layer = GameObject.FindGameObjectWithTag("phase_layer").transform;

            instance.HostPhaseRender =
                phase_layer.Find("duelphase").GetComponent<CCPhaseRender>();
            instance.DuelPhaseRender =
                phase_layer.Find("selfphase").GetComponent<CCPhaseRender>();

            var skill_layer = GameObject.FindGameObjectWithTag("skill_layer").transform;

            instance.HostSkillRender =
                skill_layer.Find("DuelSkillRender").GetComponent<CCSkillRender>();
            instance.DuelSkillRender =
                skill_layer.Find("SelfSkillRender").GetComponent<CCSkillRender>();

            instance.cCardResx.DuelCC_ID = instance.RoomConn.CurrentRoom.HostCharcardId;
            instance.cCardResx.DuelCC_Level = instance.RoomConn.CurrentRoom.HostCardlevel;
            instance.cCardResx.DuelCardSet_ID = instance.RoomConn.CurrentRoom.HostCardsetId;

            instance.cCardResx.SelfCC_ID = instance.RoomConn.CurrentRoom.DuelCharcardId;
            instance.cCardResx.SelfCC_Level = instance.RoomConn.CurrentRoom.DuelCardlevel;
            instance.cCardResx.SelfCardSet_ID = instance.RoomConn.CurrentRoom.DuelCardsetId;

        } else {
            instance.cCardResx.SelfCC_ID = instance.RoomConn.CurrentRoom.HostCharcardId;
            instance.cCardResx.SelfCC_Level = instance.RoomConn.CurrentRoom.HostCardlevel;
            instance.cCardResx.SelfCardSet_ID = instance.RoomConn.CurrentRoom.HostCardsetId;

            instance.cCardResx.DuelCC_ID = instance.RoomConn.CurrentRoom.DuelCharcardId;
            instance.cCardResx.DuelCC_Level = instance.RoomConn.CurrentRoom.DuelCardlevel;
            instance.cCardResx.DuelCardSet_ID = instance.RoomConn.CurrentRoom.DuelCardsetId;
        }

        StartCoroutine(instance.cCardResx.StartResxLoad());

        StartCoroutine(instance.cCardResx.SelfCCImplement());
        StartCoroutine(instance.cCardResx.DuelCCImplement());
    }
    async Task<bool> FullSetup() {
        Debug.Log("MyScript.Start " + GetInstanceID(), instance);
        var objs = GameObject.FindGameObjectWithTag("room_connector");
        var v = objs.GetComponent<RoomServiceConn>();
        instance.RoomConn = v;
        instance.GDConn.InitConnSetup(instance.RoomConn.config, instance.RoomConn.CurrentRoom.Key);
        instance.GDConn.AddSubMsgHandle(OnSubMsgHandle);
        // yield return true;
        instance.InitGameCtlSetup();

        if (RoomConn.IsHost && !RoomConn.IsWatcher) {
            Debug.Log("Host-Create");
            CharCardSet hostPack = new CharCardSet();
            CharCardSet duelPack = new CharCardSet();
            List<CharCardSet> hostPackList = new List<CharCardSet>();
            List<CharCardSet> duelPackList = new List<CharCardSet>();
            if (instance.cCardResx.SelfDataCardSet != null) {
                hostPack = instance.cCardResx.SelfDataCardSet.ToCharCardSetRaw();
            }
            if (instance.cCardResx.DuelDataCardSet != null) {
                duelPack = instance.cCardResx.DuelDataCardSet.ToCharCardSetRaw();
            }
            hostPackList.Add(hostPack);
            duelPackList.Add(duelPack);

            await instance.GDConn.CreateGameSet(
                instance.RoomConn.CurrentRoom.CharCardNvn,
                instance.RoomConn.CurrentRoom.Host.Id, instance.RoomConn.CurrentRoom.Dueler.Id,
                hostPackList, duelPackList
            );
            // instance.SendMsg("Host:CreatedGame,GetGameSet");
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
                if (inst_msg.Msg == "Host:CreatedGame,GetGameSet" && (instance.RoomConn.IsWatcher || !instance.RoomConn.IsHost)) {
                    await instance.GDConn.GetGameData(instance.RoomConn.CurrentRoom.Key, instance.RoomConn.IsWatcher);
                }
                Debug.Log(instance.GDConn.CurrentGS.HostCardDeck);
                Debug.Log(instance.GDConn.CurrentGS.DuelCardDeck);
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