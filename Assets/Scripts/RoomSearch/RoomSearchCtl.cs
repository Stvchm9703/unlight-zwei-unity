using System.Collections;
using System.Collections.Generic;
using ULZAsset.Config;
using ULZAsset.ProtoMod;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
public class RoomSearchCtl : MonoBehaviour {
    public RoomServiceConn Connecter;
    public GameObject RoomPrefab;
    public GameObject ScrollcContent;
    public GameObject CreatePanel, LoadingPanel, WatchOnlyPanel;

    public Dropdown nvnOption;
    public RangeSlider TotalCost;
    public Text TTCostMax, TTCostMin;
    public RangeSlider CardCost;
    public Text CCMax, CCMin;

    public RoomCreateReq createReq;
    public CfServerSetting setting;
    Room reflect_room;
    void Start() {
        if (this.Connecter == null) {
            GameObject[] objs = GameObject.FindGameObjectsWithTag("room_connector");
            this.Connecter = objs[0].GetComponent<RoomServiceConn>();
        }
        this.TotalCost.OnValueChanged.AddListener((float min, float max) => {
            TTCostMax.text = "(Current:" + (Mathf.RoundToInt(max) * 10).ToString() + ")";
            TTCostMin.text = "(Current:" + (Mathf.RoundToInt(min) * 10).ToString() + ")";
        });
        this.CardCost.OnValueChanged.AddListener((float min, float max) => {
            CCMax.text = "(Current:" + (Mathf.RoundToInt(max)).ToString() + ")";
            CCMin.text = "(Current:" + (Mathf.RoundToInt(min)).ToString() + ")";
        });
        setting = ConfigContainer.LoadCfFile(ConfigPath.StreamingAsset).remote;
        this.Connecter.InitSetup(setting);
        this.RefetchRoomList();
    }
    public async void RefetchRoomList() {
        clearRoomList();
        var room_list = await this.Connecter.GetRoomList(createReq);
        foreach (Room rm in room_list) {
            if (rm.Status != RoomStatus.OnDestroy) {
                GameObject rmobj = (GameObject)Instantiate(
                    this.RoomPrefab, this.ScrollcContent.transform);
                rmobj.transform.Find("ID").GetComponent<Text>().text = "ID:" + rm.Id;
                rmobj.transform.Find("Host/name").GetComponent<Text>().text = "Host:" + rm.Host.Name;
                rmobj.transform.Find("Host/lv").GetComponent<Text>().text = "Lv:" + rm.Host.Level.ToString();
                rmobj.transform.Find("Dueler/name").GetComponent<Text>().text =
                    "Dueler:" + rm.Dueler.Name;
                rmobj.transform.Find("Dueler/lv").GetComponent<Text>().text =
                    "Lv:" + rm.Dueler.ToString();
                rmobj.transform.Find("nvn/Text").GetComponent<Text>().text = rm.CharCardNvn + "VS" + rm.CharCardNvn;
                string status_str = "";
                switch (rm.Status) {
                    case RoomStatus.OnWait:
                        status_str = "Available";
                        break;
                    case RoomStatus.OnStart:
                    case RoomStatus.OnEnd:
                        status_str = "On Running";
                        break;
                }
                rmobj.transform.Find("status/Value").GetComponent<Text>().text = status_str;
                rmobj.GetComponent<Button>().onClick.AddListener(
                    () => {
                        this.reflect_room = rm;
                        if (rm.Status == RoomStatus.OnWait) {
                            GoToRoom(rm.Key);
                        } else {
                            this.WatchOnlyPanel.SetActive(true);
                        }
                    }
                );
            }
        }
        return;
    }
    void clearRoomList() {
        for (int i = 0; i < this.ScrollcContent.transform.childCount; i++)
            Destroy(this.ScrollcContent.transform.GetChild(i).gameObject);
    }

    // CreateRoomPanel : 
    public void UndoCreate() {
        this.CreatePanel.SetActive(false);
        // this.CreatePanel;
    }

    public async void CreateRoom() {
        var create_req = new RoomCreateReq {
            // Password = 
            CostLimitMax = Mathf.RoundToInt(TotalCost.MaxValue) * 10,
            CostLimitMin = Mathf.RoundToInt(TotalCost.MinValue) * 10,
            CharCardLimitMax = new RmCharCardInfo {
            Cost = Mathf.RoundToInt(CardCost.MaxValue)
            },
            CharCardLimitMin = new RmCharCardInfo {
            Cost = Mathf.RoundToInt(CardCost.MinValue),
            },
            CharCardNvn = nvnOption.value,
        };
        var v = await this.Connecter.CreateRoom(create_req);
        if (v != null) {

        }
    }
    public void WatchOnlyRoom(string roomkey) {

    }
    public void GoToRoom(string roomKey) {

    }
    public async void BackToMenu() {
        await this.Connecter.Kill();
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
}