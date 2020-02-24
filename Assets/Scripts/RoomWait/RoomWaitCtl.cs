using System.Collections;
using System.Collections.Generic;
using System.IO;
using Grpc.Core;
using Newtonsoft.Json;
using ULZAsset;
using ULZAsset.Config;
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
    public Dictionary<string, List<SkillObject>> SkillObjectDict;
    public CCardCtl picked;
    public AssetBundle picked_ab;
    public CardObject picked_cardObj;
    public CardSet picked_cardSet;

    // Update Panel related
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
        // AvailableCard = new List<CardSet>();
        this.SkillObjectDict = new Dictionary<string, List<SkillObject>>();
        foreach (var t in card_setting.Available) {
            AssetBundle ptmp = AssetBundle.LoadFromFile(
                Path.Combine(
                    ConfigPath.Asset_path,
                    "CC" + (t.ToString()).PadLeft(2, '0') + ".ab")
            );
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
                card_face.CC_id = cs.id;
                card_face.level = cs.level;
                StartCoroutine(card_face.InitCCImg(ptmp, Dataset, cs));
                StartCoroutine(card_face.InitCCLvFrame());
                StartCoroutine(card_face.InitEquSetting(0, 0));
                Button gotmp_btn = gotmp.AddComponent<Button>();
                gotmp_btn.onClick.AddListener(
                    () => ChangePanel_OnCardClick(ptmp, Dataset, cs)
                );

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
                    t.ToString() +
                    "R" + cs.id.ToString(),
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
                if (inst_msg.MsgType == RoomMsg.Types.MsgType.SystemInfo) {
                    if (inst_msg.Message == "Dueler:GameSet:Ready") {
                        isReady = true;
                        this.DuelerStatus.text = "Ready!";
                    } else if (inst_msg.Message == "Host:GameSet:Ready" && isReady) {
                        SceneManager.LoadScene("CardPlay", LoadSceneMode.Single);
                    }
                }
            }
        } catch (RpcException e) {
            Debug.LogError(e);
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
        picked.CC_id = cardSet.id;
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
    public void ChangePanel_OkBtn() {
        if (!this.Connecter.IsWatcher) {
            if (this.Connecter.IsHost) {
                this.HostCard.CC_id = this.picked.CC_id;
                this.HostCard.level = this.picked.level;
                StartCoroutine(this.HostCard.InitCCImg(
                    picked_ab, picked_cardObj, picked_cardSet));
                StartCoroutine(this.HostCard.InitCCLvFrame());
                StartCoroutine(this.HostCard.InitEquSetting(0, 0));
            } else {
                this.DuelerCard.CC_id = this.picked.CC_id;
                this.DuelerCard.level = this.picked.level;
                StartCoroutine(this.DuelerCard.InitCCImg(
                    picked_ab, picked_cardObj, picked_cardSet));
                StartCoroutine(this.DuelerCard.InitCCLvFrame());
                StartCoroutine(this.DuelerCard.InitEquSetting(0, 0));
            }
        }
        this.InfoPanel.gameObject.SetActive(false);
    }

    // Updated Panel related
    void updatePanel_init(Room roomInfo) {
        Debug.Log("in update-panel-init");
        this.UP_updateReq = new RoomCreateReq();
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

        // TotalCardDeckCost Slider
        var totalCardDeckCost = this.UP_totalCardDeckCost.transform.Find("Range Slider").GetComponent<RangeSlider>();
        totalCardDeckCost.MinValue = roomInfo.CostLimitMin / 10;
        totalCardDeckCost.MaxValue = roomInfo.CostLimitMax / 10;
        totalCardDeckCost.OnValueChanged.AddListener((float min, float max) => {
            this.UP_totalCardDeckCost.transform.Find("MaxVal").GetComponent<Text>().text =
                "(Current:" + (Mathf.RoundToInt(max) * 10).ToString() + ")";
            this.UP_totalCardDeckCost.transform.Find("MinVal").GetComponent<Text>().text =
                "(Current:" + (Mathf.RoundToInt(min) * 10).ToString() + ")";

            this.UP_updateReq.CostLimitMax = Mathf.RoundToInt(max) * 10;
            this.UP_updateReq.CostLimitMin = Mathf.RoundToInt(min) * 10;
        });

        // TotalCardDeckCost Slider
        var CharCardCost = this.UP_CharecterCardLimit.transform.Find("Range Slider").GetComponent<RangeSlider>();
        CharCardCost.MinValue = roomInfo.CostLimitMin / 10;
        CharCardCost.MaxValue = roomInfo.CostLimitMax / 10;
        CharCardCost.OnValueChanged.AddListener((float min, float max) => {
            this.UP_CharecterCardLimit.transform.Find("MaxVal").GetComponent<Text>().text =
                "(Current:" + (Mathf.RoundToInt(max) * 10).ToString() + ")";
            this.UP_CharecterCardLimit.transform.Find("MinVal").GetComponent<Text>().text =
                "(Current:" + (Mathf.RoundToInt(min) * 10).ToString() + ")";
            this.UP_updateReq.CharCardLimitMax = new RmCharCardInfo {
                Cost = Mathf.RoundToInt(max)
            };
            this.UP_updateReq.CharCardLimitMin = new RmCharCardInfo {
                Cost = Mathf.RoundToInt(min)
            };
        });

    }
    public void UpdatePanel_OnUpdateClicked() {
        // send update via room-conn
        Debug.Log(this.UP_updateReq);
        // this.Connecter.UpdateRoom(UP_updateReq);
    }

}