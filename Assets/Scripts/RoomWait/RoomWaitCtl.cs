using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Google.Protobuf;
using Grpc.Core;
using NATS.Client;
using Newtonsoft.Json;
using ULZAsset;
using ULZAsset.Config;
using ULZAsset.MsgExtension;
using ULZAsset.ProtoMod.RoomService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
public class RoomWaitCtl : MonoBehaviour {
    public RoomServiceConn Connecter;
    // base layout
    public Text HostName, HostLv, HostStatus;
    public Text DuelerName, DuelerLv, DuelerStatus;
    public CCardCtl HostCard, DuelerCard;
    public GameObject ChangeDeckBtn, ChangeSettingBtn;
    public bool isReady, selfReady;

    // Card info
    public CfCardVersion card_setting;

    // Change Panel related
    public GameObject CCardPrefab;
    public GameObject ChangeDeckContent;

    public CCInfoPanel InfoPanel;
    public GameObject ChangeDeckPanel;
    public List<GameObject> CardOption;
    // public Dictionary<string, AssetBundle> CardAB;
    // public List<AssetBundle> CardAB;
    public Dictionary<string, CardSetPack> CardAssetPack;

    public Dictionary<string, CardObject> CardObjectDic;
    public Dictionary<string, List<SkillObject>> SkillObjectDict;
    public CCardCtl picked;
    public AssetBundle picked_ab;

    public CardObject picked_cardObj;
    public CardSetPack picked_cardSet;

    // Update Panel related
    public GameObject UpdatePanel;
    public Dropdown UP_deckType;
    public GameObject UP_totalCardDeckCost, UP_CharecterCardLimit;
    public RoomCreateReq UP_updateReq;

    public CardObject HostCardObj, DuelCardObj;
    public CardSetPack HostCardAB, DuelCardAB, PickedCardAB;
    public bool isHostUpdated = false, isDuelUpdated = false;
    public IConnection NatsConn;
    // public base function
    void Start() {
        init_char_card_load();
        room_connector_load();
    }

    void Update() {
        if (isHostUpdated) {
            Debug.Log("host updating");
            StartCoroutine(this.HostCard.InitCCImg2(this.HostCardObj, this.HostCardAB));
            StartCoroutine(this.HostCard.InitCCLvFrame());
            StartCoroutine(this.HostCard.InitEquSetting(0, 0));
            isHostUpdated = false;
        }

        if (isDuelUpdated) {
            Debug.Log("duel updating");

            StartCoroutine(this.DuelerCard.InitCCImg2(this.DuelCardObj, this.DuelCardAB));
            StartCoroutine(this.DuelerCard.InitCCLvFrame());
            StartCoroutine(this.DuelerCard.InitEquSetting(0, 0));
            isDuelUpdated = false;
        }

        if (this.isReady && this.selfReady) {
            SceneManager.LoadScene("CardPlay", LoadSceneMode.Single);
        }
    }

    // privated init loading
    void init_char_card_load() {
        card_setting = ConfigContainer.LoadCardVersion(ConfigPath.StreamingAsset);
        this.CardAssetPack = new Dictionary<string, CardSetPack>();

        // this.DuelCardSet = new CardSet();
        this.DuelCardObj = new CardObject();
        // this.HostCardSet = new CardSet();
        this.HostCardObj = new CardObject();

        this.CardOption = new List<GameObject>();
        this.SkillObjectDict = new Dictionary<string, List<SkillObject>>();
        this.CardObjectDic = new Dictionary<string, CardObject>();

        foreach (var t in card_setting.Available) {
            AssetBundle ptmp = AssetBundle.LoadFromFile(
                Path.Combine(ConfigPath.Asset_path, "CC" + (t.ToString()).PadLeft(2, '0') + ".ab")
            );

            // AvailableCCAsset.Add(ptmp);
            TextAsset ta = ptmp.LoadAsset("card_set.json")as TextAsset;
            TextAsset ska = ptmp.LoadAsset("skill.json")as TextAsset;

            CardObject Dataset = JsonConvert.DeserializeObject<CardObject>(ta.text);
            this.CardObjectDic.Add($"cc_{t.ToString()}", Dataset);

            List<SkillObject> tmp_sk = JsonConvert.DeserializeObject<List<SkillObject>>(ska.text);

            // CardSet
            // AvailableCard.AddRange(Dataset.card_set);

            foreach (var cs in Dataset.card_set) {
                GameObject gotmp = (GameObject)Instantiate(CCardPrefab, ChangeDeckContent.transform);
                var card_face = gotmp.GetComponent<CCardCtl>();
                card_face.CC_id = Dataset.id;
                card_face.level = cs.level;

                Texture2D tas = ptmp.LoadAsset(cs.chara_image.name.Replace(".png", ""))as Texture2D;
                cs.chara_image_t2 = tas;

                this.CardAssetPack.Add(
                    $"cc_{Dataset.id}_{cs.id}",
                    cs
                );

                StartCoroutine(card_face.InitCCImg2(Dataset, cs));
                StartCoroutine(card_face.InitCCLvFrame());
                StartCoroutine(card_face.InitEquSetting(0, 0));
                Button gotmp_btn = gotmp.AddComponent<Button>();
                gotmp_btn.onClick.AddListener(() => ChangePanel_OnCardClick(Dataset, cs));
                this.CardOption.Add(gotmp);
                List<SkillObject> skj = new List<SkillObject>();
                foreach (var y in tmp_sk) {
                    if (cs.skill_pointer.Contains(y.id) && !skj.Contains(y)) {
                        skj.Add(y);
                    }
                }
                this.SkillObjectDict.Add(t.ToString() + "R" + cs.id.ToString(), skj);
            }
            // this.CardAB.Add(ptmp);
            ptmp.Unload(false);
        }

    }

    void room_connector_load() {
        // room-connector
        Debug.Log("RoomWaitCtl");
        GameObject[] objs = GameObject.FindGameObjectsWithTag("room_connector");
        Debug.Log("objs:" + objs.Length);
        foreach (var t in objs) {
            Debug.Log(t.name);
            this.Connecter = t.GetComponent<RoomServiceConn>();
        }
        Debug.Log("this.Connecter:" + this.Connecter == null);
        if (this.Connecter.IsWatcher) {
            ChangeDeckBtn.SetActive(false);
            ChangeSettingBtn.SetActive(false);
        } else {
            if (!this.Connecter.IsHost) {
                ChangeSettingBtn.SetActive(false);
            }
        }

        Debug.Log(this.Connecter.CurrentRoom.Key);
        Debug.Log(this.Connecter.CurrentRoom);

        if (this.Connecter.CurrentRoom.Host != null) {
            HostName.text = this.Connecter.CurrentRoom.Host.Name;
            HostLv.text = this.Connecter.CurrentRoom.Host.Level.ToString();
            HostStatus.text = "";
        }
        if (this.Connecter.CurrentRoom.Dueler != null) {
            DuelerName.text = this.Connecter.CurrentRoom.Dueler.Name;
            DuelerLv.text = this.Connecter.CurrentRoom.Dueler.Level.ToString();
            DuelerStatus.text = "";
        }
        updatePanel_init(this.Connecter.CurrentRoom);

        OnConnecterUpdate();
    }
    void OnConnecterUpdate() {
        Debug.Log("start to connect Broadcast");
        Debug.Log(this.Connecter.config.RoomService);
        var natsSetting = this.Connecter.config.RoomService.StreamSetting;
        var natsOpt = ConnectionFactory.GetDefaultOptions();
        natsOpt.Url = $"{natsSetting.Connector}://{natsSetting.Host}:{natsSetting.Port}";
        this.NatsConn = new ConnectionFactory().CreateConnection(natsOpt);

        NatsConn.SubscribeAsync($"ULZ.RmSvc/{this.Connecter.CurrentRoom.Key}", msgSystMsg);
    }

    async void msgSystMsg(object caller, NATS.Client.MsgHandlerEventArgs income) {
        Debug.Log(caller);
        var inst_msg = RoomMsg.Parser.ParseFrom(income.Message.Data);
        Debug.Log(inst_msg.Message);
        Debug.Log(inst_msg.MsgType.ToString());
        if (inst_msg.MsgType == RoomMsg.Types.MsgType.SystemInfo) {
            if (inst_msg.Message == "Dueler:GameSet:Ready" && this.Connecter.IsHost) {
                isReady = true;
                this.DuelerStatus.text = "Ready!";
                // if (this.isReady && this.selfReady) {
                //     SceneManager.LoadScene("CardPlay", LoadSceneMode.Single);
                // }
            } else if (inst_msg.Message == "Host:GameSet:Ready" && !this.Connecter.IsHost) {
                isReady = true;
                this.HostStatus.text = "Ready";
                // if (this.isReady && this.selfReady) {
                //     SceneManager.LoadScene("CardPlay", LoadSceneMode.Single);
                // }
            } else if (inst_msg.Message.Contains("UPDATE_ROOM:pw::") && !this.Connecter.IsHost) {
                string _pw = inst_msg.Message.Replace("UPDATE_ROOM:pw::", "");
                var roomInfo = await this.Connecter.GetRoom(this.Connecter.CurrentRoom.Key, _pw, !this.Connecter.IsWatcher);
                updatePanel_init(roomInfo);
            } else if (inst_msg.Message.Contains("CardChange::")) {
                OnConnecterUpdate_CardChange(inst_msg);
            }
        }
    }

    public void OnConnecterUpdate_CardChange(RoomMsg msg) {
        string js_str = msg.Message.Replace("CardChange::", "");
        Debug.Log(js_str);

        RoomUpdateCardReq msg_blk = RoomUpdateCardReq.Parser.ParseJson(js_str);
        Debug.Log($"{msg_blk.CharcardId}, {msg_blk.CardsetId}, {msg_blk.Level}");

        var Dataset = this.CardObjectDic[$"cc_{msg_blk.CharcardId.ToString()}"];
        Debug.Log($"Dataset.name{Dataset.name.jp}");

        var tmpAssetPack = this.CardAssetPack[$"cc_{msg_blk.CharcardId.ToString()}_{msg_blk.CardsetId.ToString()}"];

        if (
            (msg_blk.Side == RoomUpdateCardReq.Types.PlayerSide.Host && !this.Connecter.IsHost) ||
            this.Connecter.IsWatcher
        ) {
            this.HostCard.CC_id = Dataset.id;
            this.HostCard.level = tmpAssetPack.level;
            this.HostCardObj = Dataset;
            this.HostCardAB = tmpAssetPack;
            this.isHostUpdated = true;
        } else if (
            (msg_blk.Side == RoomUpdateCardReq.Types.PlayerSide.Dueler && this.Connecter.IsHost) ||
            this.Connecter.IsWatcher
        ) {
            this.DuelerCard.CC_id = Dataset.id;
            this.DuelerCard.level = tmpAssetPack.level;
            this.DuelCardObj = Dataset;
            this.DuelCardAB = tmpAssetPack;
            this.isDuelUpdated = true;
        }

    }
    public async void QuitRoom() {
        if (this.Connecter.CurrentRoom != null) {
            await this.Connecter.QuitRoom();
        }
        SceneManager.LoadScene("RoomSearch", LoadSceneMode.Single);
    }

    public async void ReadyBtn() {

        if (!this.Connecter.IsHost) {
            await this.Connecter.SystemSendMessage("Dueler:GameSet:Ready");
            this.DuelerStatus.text = "Ready!";
        } else {
            await this.Connecter.SystemSendMessage("Host:GameSet:Ready");
            this.HostStatus.text = "Ready!";
        }
        // // this.Connecter.ClearPendingEventFunc();
        this.selfReady = true;
    }

    // ChangePanel related
    public void ChangePanel_OnCardClick(CardObject cardObject, CardSetPack cardSet) {
        // picked_ab = this.CardAB[cardObject.id - 1];

        picked_cardObj = cardObject;
        picked_cardSet = cardSet;
        picked.CC_id = cardObject.id;
        picked.level = cardSet.level;
        // PickedCardAB = this.CardAssetPack[$"cc_{cardObject.id}_{cardSet.id}"];
        PickedCardAB = cardSet;
        StartCoroutine(picked.InitCCImg2(cardObject, cardSet));
        StartCoroutine(picked.InitCCLvFrame());
        StartCoroutine(picked.InitEquSetting(0, 0));
    }

    public void ChangePanel_OpenInfoPanel() {
        InfoPanel.gameObject.SetActive(true);
        StartCoroutine(
            InfoPanel.InitSkipStatus(
                picked_cardObj, PickedCardAB,
                this.SkillObjectDict[
                    picked_cardObj.id.ToString() +
                    "R" + picked_cardSet.id.ToString()
                ],
                picked_cardObj.id, picked_cardSet.level
            ));
    }
    public async void ChangePanel_OkBtn() {
        if (!this.Connecter.IsWatcher) {
            if (this.Connecter.IsHost) {
                this.HostCard.CC_id = this.picked.CC_id;
                this.HostCard.level = this.picked.level;
                this.HostCardObj = this.picked_cardObj;
                this.HostCardAB = this.PickedCardAB;
                StartCoroutine(this.HostCard.InitCCImg2(
                    picked_cardObj, PickedCardAB));
                StartCoroutine(this.HostCard.InitCCLvFrame());
                StartCoroutine(this.HostCard.InitEquSetting(0, 0));
            } else {
                this.DuelerCard.CC_id = this.picked.CC_id;
                this.DuelerCard.level = this.picked.level;
                this.DuelCardObj = this.picked_cardObj;
                this.DuelCardAB = this.PickedCardAB;
                StartCoroutine(this.DuelerCard.InitCCImg2(
                    picked_cardObj, PickedCardAB));
                StartCoroutine(this.DuelerCard.InitCCLvFrame());
                StartCoroutine(this.DuelerCard.InitEquSetting(0, 0));
            }
            await this.Connecter.ChangeCharCard(
                this.picked.CC_id,
                this.PickedCardAB.id,
                this.picked.level
            );

        }
        this.ChangeDeckPanel.SetActive(false);
    }

    // Updated Panel related
    void updatePanel_init(Room roomInfo) {

        Debug.Log("in update-panel-init");
        this.UP_updateReq = new RoomCreateReq();
        this.UP_updateReq.Key = this.Connecter.CurrentRoom.Key;
        switch (roomInfo.CharCardNvn) {
            case 3:
                this.UP_deckType.value = 1;
                break;
            default:
            case 1:
                this.UP_deckType.value = 0;
                break;
        }
        this.UP_deckType.onValueChanged.AddListener((int v) => {
            switch (v) {
                default:
                    case 0:
                    this.UP_updateReq.CharCardNvn = 1;
                break;
                case 1:
                        this.UP_updateReq.CharCardNvn = 3;
                    break;
            }
        });

        Debug.Log("CostMin::" + roomInfo.CostLimitMin.ToString());
        Debug.Log("CostMax::" + roomInfo.CostLimitMax.ToString());
        // TotalCardDeckCost Slider
        var totalCardDeckCost = this.UP_totalCardDeckCost.transform.Find("Range Slider").GetComponent<RangeSlider>();

        totalCardDeckCost.LowValue = roomInfo.CostLimitMin / 10;
        if (roomInfo.CharCardLimitMax != null) {
            totalCardDeckCost.HighValue = roomInfo.CostLimitMax / 10;
        }

        totalCardDeckCost.OnValueChanged.AddListener((float min, float max) => {
            this.UP_totalCardDeckCost.transform.Find("MaxVal").GetComponent<Text>().text =
                "(Current:" + (Mathf.RoundToInt(max) * 10).ToString() + ")";
            this.UP_totalCardDeckCost.transform.Find("MinVal").GetComponent<Text>().text =
                "(Current:" + (Mathf.RoundToInt(min) * 10).ToString() + ")";

            this.UP_updateReq.CostLimitMax = Mathf.RoundToInt(max) * 10;
            this.UP_updateReq.CostLimitMin = Mathf.RoundToInt(min) * 10;
        });

        Debug.Log("CC-CostMin::" + roomInfo.CharCardLimitMin.Cost.ToString());
        Debug.Log("CC-CostMax::" + roomInfo.CharCardLimitMax.Cost.ToString());
        // CharacterCard Slider
        var CharCardCost = this.UP_CharecterCardLimit.transform.Find("Range Slider").GetComponent<RangeSlider>();
        if (roomInfo.CharCardLimitMin != null) {
            CharCardCost.LowValue = roomInfo.CharCardLimitMin.Cost;
        }
        if (roomInfo.CharCardLimitMax != null) {
            CharCardCost.HighValue = roomInfo.CharCardLimitMax.Cost;
        }
        CharCardCost.OnValueChanged.AddListener((float min, float max) => {
            this.UP_CharecterCardLimit.transform.Find("MaxVal").GetComponent<Text>().text =
                "(Current:" + (Mathf.RoundToInt(max)).ToString() + ")";
            this.UP_CharecterCardLimit.transform.Find("MinVal").GetComponent<Text>().text =
                "(Current:" + (Mathf.RoundToInt(min)).ToString() + ")";
            this.UP_updateReq.CharCardLimitMax = new RmCharCardInfo {
                Cost = Mathf.RoundToInt(max)
            };
            this.UP_updateReq.CharCardLimitMin = new RmCharCardInfo {
                Cost = Mathf.RoundToInt(min)
            };
        });
        on_requirement_update();
    }
    public async void UpdatePanel_OnUpdateClicked() {
        // send update via room-conn
        await this.Connecter.UpdateRoom(UP_updateReq);
        this.UpdatePanel.SetActive(false);
    }

    void on_requirement_update() {
        foreach (var gobj in this.CardOption) {
            var tmp = gobj.GetComponent<CCardCtl>().original.deck_cost;
            if (
                tmp < this.Connecter.CurrentRoom.CharCardLimitMin.Cost ||
                tmp > this.Connecter.CurrentRoom.CharCardLimitMax.Cost
            ) {
                gobj.SetActive(false);
            } else {
                gobj.SetActive(true);
            }
        }
    }

    private void OnDestroy() {
        if (this.NatsConn != null) {
            this.NatsConn.DrainAsync();
            this.NatsConn.Close();
        }
    }

}