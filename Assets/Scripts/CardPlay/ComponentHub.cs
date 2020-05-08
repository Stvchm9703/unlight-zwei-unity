using UnityEngine;
using UnityEngine.SceneManagement;
public class ComponentHub : MonoBehaviour {
    // Start is called before the first frame update
    protected CachedComponent<CCardSetUp> _ccardResx = new CachedComponent<CCardSetUp>();
    protected CCardSetUp cCardResx {
        get { return _ccardResx.instance(this); }
    }

    // Room Service Conn
    protected CachedComponent<GDConnControl> _gdConn = new CachedComponent<GDConnControl>();
    protected GDConnControl GDConn {
        get { return _gdConn.instance(this); }
    }

    protected CachedComponent<RoomServiceConn> _roomConn = new CachedComponent<RoomServiceConn>();
    protected RoomServiceConn RoomConn {
        get { return _roomConn.instance(this); }
    }

    void OnEnable() {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable() {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {
        _ccardResx.clear();
        _gdConn.clear();
        _roomConn.clear();
    }
    protected virtual void Start() {
        Debug.Log("Central Call");
    }
    // async Task<bool> FullSetup() {
    //     // Debug.Log("MyScript.Start " + GetInstanceID(), instance);

    //     GDConn.InitConnSetup(RoomConn.config, RoomConn.CurrentRoom.Key);
    //     GDConn.AddSubMsgHandle(OnSubMsgHandle);
    //     // yield return true;
    //     cCardResx.InitGameCtlSetup(
    //         RoomConn.IsWatcher? true : RoomConn.IsHost,
    //         RoomConn.CurrentRoom.HostCharcardId, RoomConn.CurrentRoom.HostCardlevel, RoomConn.CurrentRoom.HostCardsetId,
    //         RoomConn.CurrentRoom.DuelCharcardId, RoomConn.CurrentRoom.DuelCardlevel, RoomConn.CurrentRoom.DuelCardsetId
    //     );

    //     if (RoomConn.IsHost && !RoomConn.IsWatcher) {
    //         Debug.Log("Host-Create");
    //         CharCardSet hostPack = new CharCardSet();
    //         CharCardSet duelPack = new CharCardSet();
    //         List<CharCardSet> hostPackList = new List<CharCardSet>();
    //         List<CharCardSet> duelPackList = new List<CharCardSet>();
    //         if (cCardResx.SelfDataCardSet != null) {
    //             hostPack = cCardResx.SelfDataCardSet.ToCharCardSetRaw();
    //         }
    //         if (cCardResx.DuelDataCardSet != null) {
    //             duelPack = cCardResx.DuelDataCardSet.ToCharCardSetRaw();
    //         }
    //         hostPackList.Add(hostPack);
    //         duelPackList.Add(duelPack);

    //         await GDConn.CreateGameSet(
    //             RoomConn.CurrentRoom.CharCardNvn,
    //             RoomConn.CurrentRoom.Host.Id, RoomConn.CurrentRoom.Dueler.Id,
    //             hostPackList, duelPackList
    //         );
    //         // SendMsg("Host:CreatedGame,GetGameSet");
    //     }
    //     return true;
    // }

    // async void OnSubMsgHandle(object caller, NATS.Client.MsgHandlerEventArgs income) {
    //     Debug.Log(caller);
    //     var inst_msg = GDBroadcastResp.Parser.ParseFrom(income.Message.Data);
    //     Debug.Log(inst_msg);
    //     Debug.Log(inst_msg.Command.ToString());
    //     switch (inst_msg.Command) {
    //         case CastCmd.GetGamesetResult:
    //             Debug.Log(inst_msg.Msg);
    //             // GameDataSet task;
    //             if (inst_msg.Msg == "Host:CreatedGame,GetGameSet" && (RoomConn.IsWatcher || !RoomConn.IsHost)) {
    //                 await GDConn.GetGameData(RoomConn.CurrentRoom.Key, RoomConn.IsWatcher);
    //             }
    //             Debug.Log(GDConn.CurrentGS.HostCardDeck);
    //             Debug.Log(GDConn.CurrentGS.DuelCardDeck);
    //             break;
    //         case CastCmd.GetEffectResult:
    //         case CastCmd.GetDrawPhaseResult:
    //         case CastCmd.GetMovePhaseResult:
    //         case CastCmd.GetAtkPhaseResult:
    //         case CastCmd.GetDefPhaseResult:

    //         case CastCmd.InstanceDamage:
    //         case CastCmd.InstanceStatusChange:
    //             break;
    //     }
    // }
}