using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Google.Protobuf;
using Newtonsoft.Json;
using TMPro;
using ULZAsset;
using ULZAsset.ProtoMod.GameDuelService;
using UnityEditor;
using UnityEngine;

public class MainCtrlComp : MonoBehaviour {
    // Start is called before the first frame update
    public GDConnControl gDConn;
    public RoomServiceConn roomServiceConn;
    public StatusEffectMainViewCtl StatusCtl;
    public PhaseTurnCtl phaseTurn;
    public RangeCtl rangeCtl;

    public MovePhaseButtonCtl moveOptCtl;
    public OKBtnCtl oKBtnCtl;

    void Start() {
        GameObject[] tmp = GameObject.FindGameObjectsWithTag("GameController");
        Debug.Log(tmp.Length);

        var g = GameObject.FindGameObjectsWithTag("gd_connector");
        Debug.Log($"gd_connect:{g.Length}");

        var v = GameObject.FindGameObjectsWithTag("room_connector");
        Debug.Log($"room_connecter:{v.Length}");

        var ev = GameObject.FindGameObjectsWithTag("GameController");
        Debug.Log($"game_connecter:{ev.Length}");

        SelfPhaseNumber.text = 0. ToString();
        DuelPhaseNumber.text = 0. ToString();

        // Debug.Log(cCardSetUp == null);
    }
    // ----------------------------------------------------
    // CCardSetUp
    // ----------------------------------------------------
    // CC Card Prefab
    // AssetBundle SelfCC_AB, DuelCC_AB;
    /// <summary>
    /// Self CC
    /// </summary>
    // private static CCardSetUp instance;
    public GameObject CCardPrefab;
    public List<CardSetPack> ABPreloaded;

    public int SelfCC_ID, SelfCC_Level, SelfCardSet_ID;
    public CardObject SelfDataSet;
    public CardSetPack SelfDataCardSet;
    public List<SkillObject> SelfSkillObject;

    public CCardBockCtl SelfCCSetBlock;
    public CCardStandCtl SelfCCSetStand;
    // public CCardStandCtl SelfCCSetStand_v2;
    public int self_atk_equ = 0, self_def_equ = 0;
    public CCSkillRender SelfCCSetSkill;
    public CCPhaseRender SelfCCSetPhase;
    public TextMeshPro SelfPhaseNumber;
    /// <summary>
    /// Duel CC
    /// </summary>
    [MinAttribute(1)]
    public int DuelCC_ID, DuelCC_Level, DuelCardSet_ID;
    public CardObject DuelDataSet;
    public CardSetPack DuelDataCardSet;
    public List<SkillObject> DuelSkillObject;

    public CCardBockCtl DuelCCSetBlock;
    // public CCardStandCtl DuelCCSetStand;
    public CCardStandCtl DuelCCSetStand;
    public int duel_atk_equ = 0, duel_def_equ = 0;
    public CCSkillRender DuelCCSetSkill;
    public CCPhaseRender DuelCCSetPhase;
    public TextMeshPro DuelPhaseNumber;

    public ULVisualCtl is_panel_open;
    // public 
    public string _asset_path {
        get {
            var tmp = "";
            switch (Application.platform) {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    tmp = Path.Combine("win", "x86");
                    break;
                case RuntimePlatform.Android:
                    tmp = "android";
                    break;
                case RuntimePlatform.IPhonePlayer:
                    tmp = "ios";
                    break;
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    tmp = "mac";
                    break;
            }
            return Path.Combine(Application.streamingAssetsPath, tmp);
        }
    }

    /// <summary>
    /// InfoPanel
    /// </summary>
    public GameObject InfoPanel;
    public PlayerSide CurrentAtkPlayer {
        get {
            if (this.gDConn != null) {
                return this.gDConn.CurrentGS.CurrPhase;
            }
            return PlayerSide.Host;
        }
    }
    public RangeType CurrentRange {
        get {
            if (this.gDConn != null) {
                return this.gDConn.CurrentGS.Range;
            }
            return RangeType.Middle;
        }
    }

    public IEnumerator SelfCCImplement() {

        SelfSkillObject = SelfDataCardSet.skill_obj;

        if (SelfCCSetBlock == null) {
            SelfCCSetBlock = transform.root.parent.Find("Canvas/BlockLayout/CardBlock").gameObject.GetComponent<CCardBockCtl>();
        }

        if (SelfCCSetStand == null) {
            SelfCCSetStand = transform.root.parent.Find("StandLayer/SelfStand").gameObject.GetComponent<CCardStandCtl>();
        }

        SelfCCSetBlock.level = SelfCC_Level;

        SelfCCSetBlock.is_self = 1;
        SelfCCSetStand.is_self = 1;
        // SelfCCSetStand_v2.is_self = 1;

        StartCoroutine(SelfCCSetBlock.InitCCLvFrame());
        StartCoroutine(SelfCCSetBlock.InitEquSetting(self_atk_equ, self_def_equ));
        StartCoroutine(SelfCCSetBlock.InitCCImg(SelfDataSet, SelfDataCardSet));
        StartCoroutine(SelfCCSetStand.InitCCImg(SelfDataCardSet));
        StartCoroutine(SelfCCSetSkill.InitCCImg2(SelfSkillObject));
        StartCoroutine(SelfCCSetPhase.InitCCImg2(SelfDataCardSet));
        yield return true;

    }
    public IEnumerator DuelCCImplement() {

        DuelSkillObject = DuelDataCardSet.skill_obj;

        if (DuelCCSetBlock == null) {
            DuelCCSetBlock = transform.root.parent.Find("Canvas/BlockLayout/DuelCardBlock").gameObject.GetComponent<CCardBockCtl>();
        }

        if (DuelCCSetStand == null) {
            DuelCCSetStand = transform.root.parent.Find("StandLayer/DuelStand").gameObject.GetComponent<CCardStandCtl>();
        }

        DuelCCSetBlock.level = DuelCC_Level;

        DuelCCSetBlock.is_self = 0;
        DuelCCSetStand.is_self = 0;

        StartCoroutine(DuelCCSetBlock.InitCCImg(DuelDataSet, DuelDataCardSet));
        StartCoroutine(DuelCCSetBlock.InitCCLvFrame());
        StartCoroutine(DuelCCSetBlock.InitEquSetting(duel_atk_equ, duel_def_equ));
        StartCoroutine(DuelCCSetStand.InitCCImg(DuelDataCardSet));
        StartCoroutine(DuelCCSetSkill.InitCCImg2(DuelSkillObject));
        StartCoroutine(DuelCCSetPhase.InitCCImg2(DuelDataCardSet));

        yield return true;

    }
    public IEnumerator StartResxLoad() {

        ABPreloaded = new List<CardSetPack>();
        List<int> used_cc_id = new List<int> {
            SelfCC_ID,
            DuelCC_ID
        }.Distinct().ToList();

        List<int> used_cc_set_id = new List<int> { SelfCardSet_ID, DuelCardSet_ID };
        foreach (var tmp in used_cc_id) {
            var abs = AssetBundle.LoadFromFile(
                Path.Combine(_asset_path, "CC" + (
                    tmp.ToString()
                ).PadLeft(2, '0') + ".ab")
            );

            if (abs == null) {
                yield return null;
            }
            TextAsset ta = abs.LoadAsset("card_set.json")as TextAsset;
            TextAsset ska = abs.LoadAsset("skill.json")as TextAsset;

            CardObject co = JsonConvert.DeserializeObject<CardObject>(ta.text);
            List<SkillObject> tmp_sk = JsonConvert.DeserializeObject<List<SkillObject>>(ska.text);

            foreach (var cs in co.card_set) {
                if (used_cc_set_id.Contains(cs.id)) {
                    CardSetPack csp = new CardSetPack();
                    csp = cs;
                    csp.chara_image_t2 = abs.LoadAsset(csp.chara_image.name.Replace(".png", ""))as Texture2D;
                    csp.stand_image_t2 = abs.LoadAsset(csp.stand_image.name.Replace(".png", ""))as Texture2D;
                    csp.bg_image_t2 = abs.LoadAsset(csp.bg_image.name.Replace(".png", ""))as Texture2D;
                    csp.artifact_image_t2 = abs.LoadAsset(csp.artifact_image.name.Replace(".png", ""))as Texture2D;

                    List<int> sumd = new List<int>();
                    foreach (int d in csp.skill_pointer) {
                        if (!sumd.Contains(d)) {
                            sumd.Add(d);
                        }
                    }
                    List<SkillObject> skj = new List<SkillObject>();
                    foreach (var y in tmp_sk) {
                        if (sumd.Contains(y.id) && !skj.Contains(y)) {
                            skj.Add(y);
                        }
                    }

                    foreach (var y in skj) {
                        y.effect_image_t2 = abs.LoadAsset(y.effect_image.name.Replace(".png", ""))as Texture2D;
                    }

                    csp.skill_obj = skj;

                    // ABPreloaded.Add(csp);
                    if (csp.id == SelfCardSet_ID) {
                        SelfDataSet = co;
                        SelfDataCardSet = csp;
                    } else if (csp.id == DuelCardSet_ID) {
                        DuelDataSet = co;
                        DuelDataCardSet = csp;
                    }
                    ABPreloaded.Add(csp);
                }
            }

            // abs.Unload(false);
        };

        yield return true;
    }

    public void InitGameCtlSetup(
        bool isHost,
        int hostCharcardId, int hostCardLevel, int hostCardsetId,
        int duelCharcardId, int duelCardLevel, int duelCardsetId
    ) {
        Debug.Log("InitGameCtlSetup");

        if (!isHost) {

            var phase_layer = GameObject.FindGameObjectWithTag("phase_layer").transform;

            SelfCCSetPhase =
                phase_layer.Find("duelphase").GetComponent<CCPhaseRender>();
            DuelCCSetPhase =
                phase_layer.Find("selfphase").GetComponent<CCPhaseRender>();

            var skill_layer = GameObject.FindGameObjectWithTag("skill_layer").transform;

            SelfCCSetSkill =
                skill_layer.Find("DuelSkillRender").GetComponent<CCSkillRender>();
            DuelCCSetSkill =
                skill_layer.Find("SelfSkillRender").GetComponent<CCSkillRender>();

            // SelfPhaseNumber =
            //     DuelCCSetPhase.transform.Find("set_b/number").GetComponent<TextMeshPro>();
            // DuelPhaseNumber =
            //     SelfCCSetPhase.transform.Find("set_b/number").GetComponent<TextMeshPro>();

            DuelCC_ID = hostCharcardId;
            DuelCC_Level = hostCardLevel;
            DuelCardSet_ID = hostCardsetId;

            SelfCC_ID = duelCharcardId;
            SelfCC_Level = duelCardLevel;
            SelfCardSet_ID = duelCardsetId;

        } else {
            SelfCC_ID = hostCharcardId;
            SelfCC_Level = hostCardLevel;
            SelfCardSet_ID = hostCardsetId;

            DuelCC_ID = duelCharcardId;
            DuelCC_Level = duelCardLevel;
            DuelCardSet_ID = duelCardsetId;
        }

        StartCoroutine(StartResxLoad());

        StartCoroutine(SelfCCImplement());
        StartCoroutine(DuelCCImplement());
    }
    // CC Stand Setup
    public void TestLoad() {

        StartCoroutine(StartResxLoad());
        StartCoroutine(SelfCCImplement());
        StartCoroutine(DuelCCImplement());

    }

    // open panel
    public IEnumerator OpenCCInfoPanel(int self_or_duel) {
        if (InfoPanel.active == false && is_panel_open.AnyPanel == false) {
            is_panel_open.AnyPanel = true;
            InfoPanel.SetActive(true);
            if (self_or_duel == 1) {
                StartCoroutine(
                    InfoPanel.GetComponent<CCInfoPanel>().Init(
                        SelfDataSet, SelfDataCardSet,
                        SelfSkillObject,
                        true, SelfCC_ID, SelfCC_Level
                    ));
            } else {
                StartCoroutine(
                    InfoPanel.GetComponent<CCInfoPanel>().Init(
                        DuelDataSet, DuelDataCardSet,
                        DuelSkillObject,
                        true, DuelCC_ID, DuelCC_Level
                    ));
            }
        }
        yield return true;
    }
    public void CloseCCInfoPanel() {
        if (InfoPanel.active == true && is_panel_open.AnyPanel == true) {
            InfoPanel.SetActive(false);
            is_panel_open.AnyPanel = false;
            InfoPanel.GetComponent<CCInfoPanel>().Clean();
        }
    }
    // ----------------------------------------------------

    // ----------------------------------------------------
    // Event-Card-block
    public CardDeckControl SelfEventSet;
    public void SelfEventCardSetTest() { StartCoroutine(SelfEventSet.GenerateCard()); }
    // public void SelfEventCardSetTest(List<EventCard> DeckEC) { StartCoroutine(SelfEventSet.GenerateCardDt(DeckEC)); }
    public void SelfDrawCardTest() { StartCoroutine(SelfEventSet.DrwCard()); }

    public void SelfPushCardTest(int numOf) {
        var tyu = SelfEventSet.GetInsideCard();
        var num_list = new List<int>();
        for (int i = numOf > tyu.Count || numOf == 0 ? tyu.Count : numOf; i > 0; i--) {
            num_list.Add(tyu[i].OrignalSet.Id);
        }
        SelfEventSet.CardPush(num_list);
    }

    public void SelfMoveOpt(MovePhaseOpt opt) {
        moveOptCtl.OptionClick(opt);
    }

    public void SelfFlipOpenTest() { StartCoroutine(SelfEventSet.FlipInsideCard()); }
    public void SelfCardToDestroy() { StartCoroutine(SelfEventSet.CdToDestroy()); }
    // ----------------------------------------------------
    public OpsdCardControl DuelEventSet;
    public void DuelEventCardSetTest() { StartCoroutine(DuelEventSet.GenerateCard()); }
    public void DuelDrawCardTest() { StartCoroutine(DuelEventSet.DrwCard()); }
    public void DuelRamdomPush() { DuelEventSet.RandomPush(); }
    public void DuelPushCardTest(int numOf) {
        var tyu = DuelEventSet.GetInsideCard();
        // var tyu = SelfEventSet.GetInsideCard();
        var num_list = new List<int>();
        for (int i = (numOf > tyu.Count || numOf == 0 ? tyu.Count : numOf); i > 0; i--) {
            num_list.Add(tyu[i].OrignalSet.Id);
        }
        DuelEventSet.CardPush(num_list);

    }
    public void DuelFlipOpenTest() { StartCoroutine(DuelEventSet.FlipOutsideCard()); }
    public void DuelCardToDestroy() { StartCoroutine(DuelEventSet.CdToDestroy()); }

    // ----------------------------------------------------
    // Test Gen Card
    List<EventCard> Test_HostEC, Test_DuelEC;
    public void TestGenerateCardData() {
        string test_data_path = Path.Combine(Application.dataPath, "Data", "test");
        Test_HostEC = new List<EventCard>();
        var dict_Event = File.ReadAllText(Path.Combine(test_data_path, "HostTmp.json"));
        dict_Event = dict_Event.Trim(new Char[] { '[', ']' });
        var dict_Event_list = dict_Event.Split(new string[] { "},{" }, StringSplitOptions.None);
        foreach (var ec in dict_Event_list) {
            var tec = ec;
            if (tec.Length == 0) {
                continue;
            }
            if (tec[0] != '{') {
                tec = '{' + tec;
            }
            if (tec[tec.Length - 1] != '}') {
                tec = tec + '}';
            }

            var ert = EventCard.Parser.ParseJson(tec);
            Test_HostEC.Add(ert);
        }

        Test_DuelEC = new List<EventCard>();
        var dict_EventDl = File.ReadAllText(Path.Combine(test_data_path, "DuelTmp.json"));
        dict_EventDl = dict_EventDl.Trim(new Char[] { '[', ']' });
        var dict_EventDl_list = dict_EventDl.Split(new string[] { "},{" }, StringSplitOptions.None);
        foreach (var ec in dict_EventDl_list) {
            var tec = ec;
            if (tec.Length == 0) {
                continue;
            }
            if (tec[0] != '{') {
                tec = '{' + tec;
            }
            if (tec[tec.Length - 1] != '}') {
                tec = tec + '}';
            }
            var ert = EventCard.Parser.ParseJson(tec);
            Test_DuelEC.Add(ert);
        }
        Debug.Log(Test_HostEC.Count);
        Debug.Log(Test_DuelEC.Count);
        StartCoroutine(SelfEventSet.GenerateCardDt(Test_HostEC));
        StartCoroutine(DuelEventSet.GenerateCardDt(Test_DuelEC));
    }

    // ----------------------------------------------------
    // Status Ctl 
    public void InsertStatusToSelf(int status_id, int CD) { StatusCtl.InsertStatusToSelf(status_id, CD); }
    public void InsertStatusToDuel(int status_id, int CD) { StatusCtl.InsertStatusToDuel(status_id, CD); }

    public void RemoveStatusToSelf(int CD) { StatusCtl.RemoveCDToSelf(CD); }
    public void RemoveStatusToDuel(int CD) { StatusCtl.RemoveCDToDuel(CD); }
    // ----------------------------------------------------
    // ----------------------------------------------------
    // Range Turn Ctl
    public void PlayChangeRange(RangeType range) {
        switch (range) {
            case RangeType.Long:
                rangeCtl.MoveToLongRange();
                break;
            case RangeType.Short:
                rangeCtl.MoveToShortRange();
                break;
            case RangeType.Middle:
                rangeCtl.MoveToMiddleRange();
                break;

        }
    }
    // ---------------------------------------------------
    // ---------------------------------------------------
    // Self Phase Option Anim 
    // Phase Turn Ctl (Merged to Player Phase-Turn)
    public void PlayerPhaseTurn(EventHookPhase HookPhase, EventHookType type) {
        if (HookPhase == EventHookPhase.MoveCardDropPhase && type == EventHookType.Before) {
            StartCoroutine(SelfCCSetPhase.MovePhaseAnimation(true));
            return;
        } else if (HookPhase == EventHookPhase.MoveCardDropPhase && type == EventHookType.After) {
            StartCoroutine(SelfCCSetPhase.MovePhaseAnimation(false));
            return;
        }
        // self is atker 
        if (this.CurrentAtkPlayer == PlayerSide.Host) {
            StartCoroutine(SelfCCSetPhase.AttackerPhaseAnimation(HookPhase, type, CurrentRange));
            StartCoroutine(DuelCCSetPhase.DeferencePhaseAnimation(HookPhase, type, CurrentRange));
        } else {
            // dueler is atker
            StartCoroutine(DuelCCSetPhase.AttackerPhaseAnimation(HookPhase, type, CurrentRange));
            StartCoroutine(SelfCCSetPhase.DeferencePhaseAnimation(HookPhase, type, CurrentRange));
        }
    }

    public void PlayerPhaseTurn(EventHookPhase hookPhase, EventHookType type, PlayerSide CurrentAtker, RangeType range) {
        phaseTurn.PlayAnim(hookPhase, type);

        if (hookPhase == EventHookPhase.MoveCardDropPhase && type == EventHookType.Before) {
            StartCoroutine(SelfCCSetPhase.MovePhaseAnimation(true));
            return;
        } else if (hookPhase == EventHookPhase.MoveCardDropPhase && type == EventHookType.After) {
            StartCoroutine(SelfCCSetPhase.MovePhaseAnimation(false));
            return;
        }

        // self is atker 
        if (CurrentAtker == PlayerSide.Host) {
            StartCoroutine(SelfCCSetPhase.AttackerPhaseAnimation(hookPhase, type, range));
            StartCoroutine(DuelCCSetPhase.DeferencePhaseAnimation(hookPhase, type, range));
        } else {
            // dueler is atker
            StartCoroutine(DuelCCSetPhase.AttackerPhaseAnimation(hookPhase, type, range));
            StartCoroutine(SelfCCSetPhase.DeferencePhaseAnimation(hookPhase, type, range));
        }
    }

    public IEnumerator IPlayerPhaseTurn(EventHookPhase hookPhase, EventHookType type, PlayerSide CurrentAtker, RangeType range) {
        phaseTurn.PlayAnim(hookPhase, type);

        if (hookPhase == EventHookPhase.MoveCardDropPhase && type == EventHookType.Before) {
            yield return (SelfCCSetPhase.MovePhaseAnimation(true));
        } else if (hookPhase == EventHookPhase.MoveCardDropPhase && type == EventHookType.After) {
            yield return (SelfCCSetPhase.MovePhaseAnimation(false));
        }

        if (CurrentAtker == PlayerSide.Host) {
            //      self is atker 
            StartCoroutine(SelfCCSetPhase.AttackerPhaseAnimation(hookPhase, type, range));
            StartCoroutine(DuelCCSetPhase.DeferencePhaseAnimation(hookPhase, type, range));
        } else {
            // dueler is atker
            StartCoroutine(DuelCCSetPhase.AttackerPhaseAnimation(hookPhase, type, range));
            StartCoroutine(SelfCCSetPhase.DeferencePhaseAnimation(hookPhase, type, range));
        }
        yield return new WaitForSeconds(1.2f);
        yield return true;

    }

    // ------------------------------------------------------------
    // Ok Btn Ctl
    public void OkBtnOnOff(bool on_off) {
        oKBtnCtl.SetAble(on_off);
    }
    // ------------------------------------------------------------
    // Skill-Play

    public void TestSkillPlay(PlayerSide side, int index) {
        if (side == PlayerSide.Host) {
            int SkillFeatNo = SelfCCSetSkill.debug_skl_id[index];
            StartCoroutine(SelfCCSetSkill.PlayAnim(SkillFeatNo));
        } else if (side == PlayerSide.Dueler) {
            int SkillFeatNo = DuelCCSetSkill.debug_skl_id[index];
            StartCoroutine(DuelCCSetSkill.PlayAnim(SkillFeatNo));
        }
    }
    public IEnumerator ITestSkillPlay(PlayerSide side, int index) {
        if (side == PlayerSide.Host) {
            int SkillFeatNo = SelfCCSetSkill.debug_skl_id[index];
            yield return (SelfCCSetSkill.PlayAnim(SkillFeatNo));
        } else if (side == PlayerSide.Dueler) {
            int SkillFeatNo = DuelCCSetSkill.debug_skl_id[index];
            yield return (DuelCCSetSkill.PlayAnim(SkillFeatNo));
        }
    }

    // ---------------------------------------------------------------------------
    // Full Flow
    public void TestGameInitPhase() {
        Debug.Log("Start GameInit Phase");
        TestLoad();
        TestGenerateCardData();
        Debug.Log("End GameInit Phase");
    }

    public IEnumerator ITestDrawPhaseUI() {
        yield return IPlayerPhaseTurn(
            EventHookPhase.RefillActionCardPhase,
            EventHookType.Before,
            PlayerSide.Host,
            RangeType.Middle
        );

        for (int i = 0; i < 7; i++) {
            StartCoroutine(SelfEventSet.DrwCard());
            StartCoroutine(DuelEventSet.DrwCard());
        }
        yield return new WaitForSeconds(2.5f);
        yield return SelfEventSet.FlipInsideCard();
        yield return IPlayerPhaseTurn(
            EventHookPhase.RefillActionCardPhase,
            EventHookType.After,
            PlayerSide.Host,
            RangeType.Middle);
    }
    public void TestGameDrawPhaseUI() {
        StartCoroutine(ITestDrawPhaseUI());
    }

    public IEnumerator ITestMovePhaseUI() {
        yield return IPlayerPhaseTurn(
            EventHookPhase.MoveCardDropPhase,
            EventHookType.Before,
            PlayerSide.Host,
            RangeType.Middle);

        OkBtnOnOff(true);
        yield return new WaitForSeconds(1);
        // ---- fake user move
        SelfMoveOpt(MovePhaseOpt.Backward);
        yield return new WaitForSeconds(1.2f);
        SelfPushCardTest(1);
        yield return new WaitForSeconds(1.2f);
        OkBtnOnOff(false);
        yield return new WaitForSeconds(2f);
        // ---- fake user move

        DuelPushCardTest(1);
        yield return new WaitForSeconds(2f);

        // determine-move-
        DuelFlipOpenTest();
        yield return new WaitForSeconds(2f);

        StartCoroutine(SelfEventSet.CdToDestroy());
        StartCoroutine(DuelEventSet.CdToDestroy());
        yield return new WaitForSeconds(1.2f);
        // finish-move

        rangeCtl.MoveToLongRange();
        yield return IPlayerPhaseTurn(
            EventHookPhase.MoveCardDropPhase,
            EventHookType.After,
            PlayerSide.Host,
            RangeType.Middle);
    }

    public void TestMovePhaseUI() {
        StartCoroutine(ITestMovePhaseUI());
    }

    public IEnumerator ITestHostAttacker() {
        yield return IPlayerPhaseTurn(
            EventHookPhase.AttackCardDropPhase,
            EventHookType.Before,
            PlayerSide.Host,
            RangeType.Long
        );
        OkBtnOnOff(true);
        // Thread.Sleep(2000);
        yield return new WaitForSeconds(2f);
        // ---- fake user move
        SelfPushCardTest(2);
        yield return new WaitForSeconds(1.2f);
        OkBtnOnOff(false);
        yield return new WaitForSeconds(1.2f);
        // end Attack Phase
        yield return IPlayerPhaseTurn(
            EventHookPhase.AttackCardDropPhase,
            EventHookType.After,
            PlayerSide.Host,
            RangeType.Long
        );
        // Thread.Sleep(1200);
        yield return new WaitForSeconds(1.2f);

        // start Defence Phase
        yield return IPlayerPhaseTurn(
            EventHookPhase.DefenceCardDropPhase,
            EventHookType.Before,
            PlayerSide.Host,
            RangeType.Long
        );

        // OkBtnOnOff(true);
        // Thread.Sleep(2000);
        yield return new WaitForSeconds(2f);

        // ---- fake user move
        DuelPushCardTest(1);
        // Thread.Sleep(1200);
        yield return new WaitForSeconds(2f);
        // Def OK -ed 
        DuelFlipOpenTest();
        // Thread.Sleep(2000);
        yield return new WaitForSeconds(1.2f);

        // End of def 
        yield return IPlayerPhaseTurn(
            EventHookPhase.DefenceCardDropPhase,
            EventHookType.After,
            PlayerSide.Host,
            RangeType.Long
        );
        yield return new WaitForSeconds(2f);
        StartCoroutine(SelfEventSet.CdToDestroy());
        StartCoroutine(DuelEventSet.CdToDestroy());
        yield return new WaitForSeconds(1.2f);
        yield return IPlayerPhaseTurn(
            EventHookPhase.DetermineBattlePointPhase,
            EventHookType.Before,
            PlayerSide.Host,
            RangeType.Long
        );
        // Test for Skill-Animation 
        yield return new WaitForSeconds(2f);

        StartCoroutine(ITestSkillPlay(PlayerSide.Host, 0));
        StartCoroutine(ITestSkillPlay(PlayerSide.Dueler, 0));
        yield return true;
    }

    public void TestHostAtkPhaseUI() {
        StartCoroutine(ITestHostAttacker());
    }

    public void TestDuelAtkPhaseUI() {
        StartCoroutine(ITestDuelAtkPhaseUI());
    }
    public IEnumerator ITestDuelAtkPhaseUI() {
        yield return IPlayerPhaseTurn(
            EventHookPhase.AttackCardDropPhase,
            EventHookType.Before,
            PlayerSide.Dueler,
            RangeType.Long
        );

        // OkBtnOnOff(true);
        // Thread.Sleep(2000);
        yield return new WaitForSeconds(3f);

        // ---- fake user move
        DuelPushCardTest(1);
        // Thread.Sleep(1200);
        yield return new WaitForSeconds(2f);
        // Def OK -ed 
        DuelFlipOpenTest();
        // Thread.Sleep(2000);
        yield return new WaitForSeconds(1.2f);

        // end Attack Phase
        yield return IPlayerPhaseTurn(
            EventHookPhase.AttackCardDropPhase,
            EventHookType.After,
            PlayerSide.Dueler,
            RangeType.Long
        );
        // Thread.Sleep(1200);
        yield return new WaitForSeconds(1.2f);

        // start Defence Phase
        yield return IPlayerPhaseTurn(
            EventHookPhase.DefenceCardDropPhase,
            EventHookType.Before,
            PlayerSide.Dueler,
            RangeType.Long
        );

        OkBtnOnOff(true);
        // Thread.Sleep(2000);
        yield return new WaitForSeconds(2f);
        // ---- fake user move
        SelfPushCardTest(2);
        yield return new WaitForSeconds(1.2f);
        OkBtnOnOff(false);
        yield return new WaitForSeconds(1.2f);

        // End of def 
        yield return IPlayerPhaseTurn(
            EventHookPhase.DefenceCardDropPhase,
            EventHookType.After,
            PlayerSide.Dueler,
            RangeType.Long
        );
        yield return new WaitForSeconds(2f);
        StartCoroutine(SelfEventSet.CdToDestroy());
        StartCoroutine(DuelEventSet.CdToDestroy());
        yield return new WaitForSeconds(1.2f);
        yield return IPlayerPhaseTurn(
            EventHookPhase.DetermineBattlePointPhase,
            EventHookType.Before,
            PlayerSide.Dueler,
            RangeType.Long
        );
        // Test for Skill-Animation 
        yield return new WaitForSeconds(2f);

        StartCoroutine(ITestSkillPlay(PlayerSide.Host, 0));
        StartCoroutine(ITestSkillPlay(PlayerSide.Dueler, 0));
        yield return true;
    }

}