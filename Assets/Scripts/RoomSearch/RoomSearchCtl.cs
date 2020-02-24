using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
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
    public List<Room> rmInSort;
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
        if (createReq == null)createReq = new RoomCreateReq();

        rmInSort = await this.Connecter.GetRoomList(createReq);
        foreach (Room rm in rmInSort) {
            Debug.Log(rm.Key);
            if (rm.Status != RoomStatus.OnDestroy) {
                GameObject rmobj = (GameObject)Instantiate(
                    this.RoomPrefab, this.ScrollcContent.transform);
                rmobj.transform.Find("ID").GetComponent<Text>().text = "ID:" + rm.Id;
                rmobj.transform.Find("Host/name").GetComponent<Text>().text = "Host:" + rm.Host.Name;
                rmobj.transform.Find("Host/lv").GetComponent<Text>().text = "Lv:" + rm.Host.Level.ToString();
                if (rm.Dueler != null) {
                    rmobj.transform.Find("Dueler/name").GetComponent<Text>().text = "Dueler:" + rm.Dueler.Name;
                    rmobj.transform.Find("Dueler/lv").GetComponent<Text>().text = "Lv:" + rm.Dueler.ToString();
                } else {
                    rmobj.transform.Find("Dueler/name").GetComponent<Text>().text = "Dueler:";
                    rmobj.transform.Find("Dueler/lv").GetComponent<Text>().text = "";
                }
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
        Debug.Log("nvnOption:" + nvnOption.value);
        var conv_nvnOpt = 1;
        switch (nvnOption.value) {
            default:
                case 0:
                conv_nvnOpt = 1;
            break;
            case 1:
                    conv_nvnOpt = 3;
                break;
        }
        var create_req = new RoomCreateReq {
            // Password = 
            Host = new RmUserInfo {
            Id = setting.UserInfo.Id,
            Name = setting.UserInfo.Name,
            Level = setting.UserInfo.Level,
            Rank = setting.UserInfo.Rank,
            },

            CostLimitMax = Mathf.RoundToInt(TotalCost.HighValue) * 10,
            CostLimitMin = Mathf.RoundToInt(TotalCost.LowValue) * 10,

            CharCardLimitMax = new RmCharCardInfo {
            Cost = Mathf.RoundToInt(CardCost.HighValue)
            },
            CharCardLimitMin = new RmCharCardInfo {
            Cost = Mathf.RoundToInt(CardCost.LowValue),
            },
            CharCardNvn = conv_nvnOpt,
        };
        // Debug.Log(JsonUtility.ToJson(createReq));
        var v = await this.Connecter.CreateRoom(create_req);
        Debug.Log(this.Connecter.CurrentRoom.Key);
        if (this.Connecter.CurrentRoom != null) {
            SceneManager.LoadScene("RoomWait", LoadSceneMode.Single);
        }
    }
    public void WatchOnlyRoom(string roomkey) {
        Debug.Log("key:" + roomkey);
    }
    public void GoToRoom(string roomKey) {
        Debug.Log("go key:" + roomKey);
        if (this.Connecter.CurrentRoom == null) {
            foreach (var rm in rmInSort) {
                if (rm.Key == roomKey) {
                    this.Connecter.CurrentRoom = rm;
                }
            }
        }
        SceneManager.LoadScene("RoomWait", LoadSceneMode.Single);
    }
    public async void BackToMenu() {
        // await this.Connecter.Kill();
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
}