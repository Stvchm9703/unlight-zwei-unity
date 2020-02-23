using System.Collections;
using System.Collections.Generic;
using Grpc.Core;
using ULZAsset.ProtoMod;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ChatPanelCtl : MonoBehaviour {
    // Start is called before the first frame update
    public RoomServiceConn connecter;
    public GameObject ChatPanel;
    public GameObject NamePref, DespPref;
    public InputField ChatInput;
    public ULVisualCtl PanelPrevent;
    public GameObject PanelDisplay;
    void Start() {
        if (ChatPanel == null) {
            this.ChatPanel = this.transform.Find("Scroll View/Viewport/Content").gameObject;
        }
        GameObject[] objs = GameObject.FindGameObjectsWithTag("room_connector");
        foreach (var t in objs) {
            Debug.Log(t.name);
            this.connecter = t.GetComponent<RoomServiceConn>();
        }
        if (this.ChatInput == null) {
            this.ChatInput = this.transform.Find("InputField").GetComponent<InputField>();
        }
    }

    async void OnConnecterUpdate() {
        try {
            var t = this.connecter.InitChatRoomStream();
            while (await t.ResponseStream.MoveNext(connecter.CloseChatRoomToken.Token)) {
                GenMessage(t.ResponseStream.Current);
            }
        } catch (RpcException e) {
            Debug.LogError(e);
        }
    }
    public void OpenPanel() {
        if (PanelPrevent.AnyPanel == false && this.PanelDisplay.activeSelf == false) {
            PanelPrevent.AnyPanel = true;
            this.PanelDisplay.SetActive(true);
        }
    }

    public void ClosePanel() {
        if (PanelPrevent.AnyPanel == true && this.PanelDisplay.activeSelf == true) {
            PanelPrevent.AnyPanel = false;
            this.PanelDisplay.SetActive(false);

        }
    }

    public void GenMessage(RoomMsg msg) {
        GameObject title = (GameObject)Instantiate(NamePref, ChatPanel.transform);
        title.GetComponent<Text>().text = msg.FormId;

        if (msg.MsgType == RoomMsg.Types.MsgType.UserText) {
            GameObject info = (GameObject)Instantiate(DespPref, ChatPanel.transform);
            info.GetComponent<Text>().text = msg.Message;
        }
        return;
    }

    public async void SendMessage() {
        var msg = await this.connecter.SendMessage(ChatInput.text);
        GenMessage(msg);
    }

}

#if (UNITY_EDITOR) 
[CustomEditor(typeof(ChatPanelCtl))]
public class ChatPanelCtlEditor : Editor {
    string test_message;
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        ChatPanelCtl d = (ChatPanelCtl)target;

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Test Block");
        test_message = EditorGUILayout.TextField("Test text", test_message);

        if (GUILayout.Button("Test for Generate Text message")) {
            var msg = new RoomMsg {
                FormId = "test_id",
                Message = test_message,
                MsgType = RoomMsg.Types.MsgType.UserText,
            };
            d.GenMessage(msg);
        }
    }
}
#endif