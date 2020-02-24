using System.Collections;
using System.Collections.Generic;
using System.IO;
using Grpc.Core;
using Newtonsoft.Json;
using ULZAsset;
using ULZAsset.Config;
using ULZAsset.MsgExtension;
using ULZAsset.ProtoMod;
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
    public bool isReady;

    // Card info
    public CfCardVersion card_setting;

    // Change Panel related
    public GameObject CCardPrefab;
    public GameObject ChangeDeckContent;

    public CCInfoPanel InfoPanel;
    public GameObject ChangeDeckPanel;
    List<GameObject> CardOption;
    public Dictionary<string, AssetBundle> CCAsset;
    public Dictionary<string, List<SkillObject>> SkillObjectDict;
    public CCardCtl picked;
    public AssetBundle picked_ab;
    public CardObject picked_cardObj;
    public CardSet picked_cardSet;

    // Update Panel related
    public GameObject UpdatePanel;
    public Dropdown UP_deckType;
    public GameObject UP_totalCardDeckCost, UP_CharecterCardLimit;
    public RoomCreateReq UP_updateReq;

    // public base function
    void Start() {
        init_char_card_load();
        room_connector_load();
    }
    // privated init loading
    void init_char_card_load() {
        card_setting = ConfigContainer.LoadCardVersion(ConfigPath.StreamingAsset);
        // AvailableCCAsset = new List<AssetBundle>();
        this.CardOption = new List<GameObject>();
        this.SkillObjectDict = new Dictionary<string, List<SkillObject>>();
        this.CCAsset = new Dictionary<string, AssetBundle>();
        foreach (var t in card_setting.Available) {
            AssetBundle ptmp = AssetBundle.LoadFromFile(
                Path.Combine(
                    ConfigPath.Asset_path,
                    "CC" + (t.ToString()).PadLeft(2, '0') + ".ab")
            );
            this.CCAsset.Add("cc" + t.ToString(), ptmp);
            // AvailableCCAsset.Add(ptmp);
            TextAsset ta = ptmp.LoadAsset("card_set.json")as TextAsset;
            TextAsset ska = ptmp.LoadAsset("skill.json")as TextAsset;

            var Dataset = JsonConvert.DeserializeObject<CardObject>(ta.text);
            List<SkillObject> tmp_sk = JsonConvert.DeserializeObject<List<SkillObject>>(ska.text);

            // CardSet
            // AvailableCard.AddRange(Dataset.card_set);
            foreach (var cs in Dataset.card_set) {
                GameObject gotmp = (GameObject)Instantiate(CCardPrefab, ChangeDeckContent.transform);
                var card_face = gotmp.GetComponent<CCardCtl>();
                card_face.CC_id = Dataset.id;
                card_face.level = cs.level;
                StartCoroutine(card_face.InitCCImg(ptmp, Dataset, cs));
                StartCoroutine(card_face.InitCCLvFrame());
                StartCoroutine(card_face.InitEquSetting(0, 0));
                Button gotmp_btn = gotmp.AddComponent<Button>();
                gotmp_btn.onClick.AddListener(
                    () => ChangePanel_OnCardClick(ptmp, Dataset, cs)
                );
                this.CardOption.Add(gotmp);

                // skill-obk list for info-panel
                List<int> sumd = new List<int>();
                foreach (int d in cs.skill_pointer) {
                    if (!sumd.Contains(d)) {
                        sumd.Add(d);
                    }
                }
                List<SkillObject> skj = new List<SkillObject>();
                foreach (var tt in sumd) {
                    foreach (var y in tmp_sk) {
                        if (y.id == tt && !skj.Contains(y)) {
                            skj.Add(y);
                        }
                    }
                }
                this.SkillObjectDict.Add(
                    t.ToString() + "R" + cs.id.ToString(),
                    skj
                );

            }
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
    async void OnConnecterUpdate() {
        try {
            var t = this.Connecter.InitChatRoomStream();
            while (await t.ResponseStream.MoveNext(this.Connecter.CloseChatRoomToken.Token)) {
                var inst_msg = t.ResponseStream.Current;
                Debug.Log(inst_msg.Message);
                if (inst_msg.MsgType == RoomMsg.Types.MsgType.SystemInfo) {
                    if (inst_msg.Message == "Dueler:GameSet:Ready") {
                        isReady = true;
                        this.DuelerStatus.text = "Ready!";
                    } else if (inst_msg.Message == "Host:GameSet:Ready" && isReady) {
                        SceneManager.LoadScene("CardPlay", LoadSceneMode.Single);
                    } else if (inst_msg.Message.Contains("UPDATE_ROOM:pw::") && !this.Connecter.IsHost) {
                        string _pw = inst_msg.Message.Replace("UPDATE_ROOM:pw::", "");
                        var rm = await this.Connecter.GetRoom(this.Connecter.CurrentRoom.Key, _pw, !this.Connecter.IsWatcher);
                        updatePanel_init(rm);
                    } else if (inst_msg.Message.Contains("CardChange::")) {
                        OnConnecterUpdate_CardChange(inst_msg.Message);
                    }
                }
            }
        } catch (RpcException e) {
            Debug.LogError(e);
        }
    }
    public void OnConnecterUpdate_CardChange(string msg) {
        string js_str = msg.Replace("CardChange::", "");
        Debug.Log(js_str);
        RmChangeCharCard msg_blk = JsonConvert.DeserializeObject<RmChangeCharCard>(js_str);
        CardSet tmp = new CardSet();
        foreach (var gobj in CardOption) {
            var f = gobj.GetComponent<CCardCtl>();
            if (f.original.id == msg_blk.cardset_id &&
                f.CC_id == msg_blk.charcard_id) {
                tmp = f.original;
            }
        }
        var ab_tmp = CCAsset["cc" + msg_blk.charcard_id];
        var card_json_tmp = CCAsset["cc" + msg_blk.charcard_id].LoadAsset("card_set.json")as TextAsset;
        var Dataset = JsonConvert.DeserializeObject<CardObject>(card_json_tmp.text);
        if (msg_blk.side == "Host") {
            this.HostCard.CC_id = msg_blk.charcard_id;
            this.HostCard.level = msg_blk.level;
            StartCoroutine(this.HostCard.InitCCImg(
                ab_tmp, Dataset, tmp));
            StartCoroutine(this.HostCard.InitCCLvFrame());
            StartCoroutine(this.HostCard.InitEquSetting(0, 0));
        } else {
            this.DuelerCard.CC_id = msg_blk.charcard_id;
            this.DuelerCard.level = msg_blk.level;
            StartCoroutine(this.DuelerCard.InitCCImg(
                ab_tmp, Dataset, tmp));
            StartCoroutine(this.DuelerCard.InitCCLvFrame());
            StartCoroutine(this.DuelerCard.InitEquSetting(0, 0));
        }
    }
    public async void QuitRoom() {
        if (this.Connecter.CurrentRoom != null) {
            // await this.Connecter.QuitRoom(); 
        }
        SceneManager.LoadScene("RoomSearch", LoadSceneMode.Single);
    }
    public void ReadyBtn() {
        if (!this.Connecter.IsHost) {
            this.Connecter.SystemSendMessage("Dueler:GameSet:Ready");
        } else {
            this.Connecter.SystemSendMessage("Host:GameSet:Ready");
        }
        SceneManager.LoadScene("CardPlay", LoadSceneMode.Single);
    }

    // ChangePanel related
    public void ChangePanel_OnCardClick(AssetBundle ab, CardObject cardObject, CardSet cardSet) {
        picked_ab = ab;
        picked_cardObj = cardObject;
        picked_cardSet = cardSet;
        picked.CC_id = cardObject.id;
        picked.level = cardSet.level;
        StartCoroutine(picked.InitCCImg(ab, cardObject, cardSet));
        StartCoroutine(picked.InitCCLvFrame());
        StartCoroutine(picked.InitEquSetting(0, 0));
    }

    public void ChangePanel_OpenInfoPanel() {
        InfoPanel.gameObject.SetActive(true);
        StartCoroutine(
            InfoPanel.InitSkipStatus(
                picked_ab, picked_cardObj, picked_cardSet,
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
                StartCoroutine(this.HostCard.InitCCImg(
                    picked_ab, picked_cardObj, picked_cardSet));
                StartCoroutine(this.HostCard.InitCCLvFrame());
                StartCoroutine(this.HostCard.InitEquSetting(0, 0));
                // this.Connecter.SystemSendMessage();
            } else {
                this.DuelerCard.CC_id = this.picked.CC_id;
                this.DuelerCard.level = this.picked.level;
                StartCoroutine(this.DuelerCard.InitCCImg(
                    picked_ab, picked_cardObj, picked_cardSet));
                StartCoroutine(this.DuelerCard.InitCCLvFrame());
                StartCoroutine(this.DuelerCard.InitEquSetting(0, 0));
            }
            await this.Connecter.SystemSendMessage("CardChange::" +
                JsonConvert.SerializeObject(new RmChangeCharCard {
                    user_id = this.Connecter.CurrentUser.Id,
                        side = this.Connecter.IsHost ? "Host" : "Dueler",
                        charcard_id = this.picked.CC_id,
                        cardset_id = this.picked.original.id,
                        level = this.picked.level,
                })
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

}