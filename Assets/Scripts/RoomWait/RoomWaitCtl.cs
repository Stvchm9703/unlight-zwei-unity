using System.Collections;
using System.Collections.Generic;
using Grpc.Core;
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
    public GameObject ChangeDeck, ChangeSetting;
    public bool isReady;

    public CfCardVersion card_setting;

    public List<CardSet> Available;
    void Start() {
        init_char_card_load();
        // 
        room_connector_load();
    }
    void init_char_card_load(){
        card_setting = ConfigContainer.LoadCardVersion(ConfigPath.StreamingAsset);
        foreach(var t in card_setting.Available){
            
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
            ChangeDeck.SetActive(false);
            ChangeSetting.SetActive(false);
        } else {
            if (!this.Connecter.IsHost) {
                ChangeSetting.SetActive(false);
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

}