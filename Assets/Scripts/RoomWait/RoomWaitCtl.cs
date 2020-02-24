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
public class RoomWaitCtl : MonoBehaviour {
    public RoomServiceConn Connecter;
    public Text HostName, HostLv, HostStatus;
    public Text DuelerName, DuelerLv, DuelerStatus;
    public GameObject ChangeDeckPanel, ChangeSettingPanel;
    public bool isReady;

    public CfCardVersion card_setting;

    public GameObject CCardPrefab;
    public GameObject ChangeDeckContent;

    public CCInfoPanel InfoPanel;
    public Dictionary<string, List<SkillObject>> SkillObjectDict;
    public CCardCtl picked;
    public AssetBundle picked_ab;
    public CardObject picked_cardObj;
    public CardSet picked_cardSet;
    void Start() {
        init_char_card_load();
        // 
        room_connector_load();
    }
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
                    () => OnCardClick(ptmp, Dataset, cs)
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
            ChangeDeckPanel.SetActive(false);
            ChangeSettingPanel.SetActive(false);
        } else {
            if (!this.Connecter.IsHost) {
                ChangeSettingPanel.SetActive(false);
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

    public void OnCardClick(AssetBundle ab, CardObject cardObject, CardSet cardSet) {
        picked_ab = ab;
        picked_cardObj = cardObject;
        picked_cardSet = cardSet;
        picked.CC_id = cardSet.id;
        picked.level = cardSet.level;
        StartCoroutine(picked.InitCCImg(ab, cardObject, cardSet));
        StartCoroutine(picked.InitCCLvFrame());
        StartCoroutine(picked.InitEquSetting(0, 0));
    }

    public void OpenInfoPanel() {
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

}