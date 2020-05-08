using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCtrlComp : MonoBehaviour {
    // Start is called before the first frame update
    public GDConnControl gDConn;
    public RoomServiceConn roomServiceConn;
    public CCardSetUp cCardSetUp;
    void Start() {
        GameObject[] tmp = GameObject.FindGameObjectsWithTag("GameController");
        Debug.Log(tmp.Length);
        if (tmp.Length > 1 && cCardSetUp == null) {
            Destroy(this.gameObject);
        } else {
            DontDestroyOnLoad(gameObject);
            Debug.Log("CCardSetUp Start");
        }

        var g = GameObject.FindGameObjectsWithTag("gd_connector");
        Debug.Log($"gd_connect:{g.Length}");

        var v = GameObject.FindGameObjectsWithTag("room_connector");
        Debug.Log($"room_connecter:{v.Length}");

        var ev = GameObject.FindGameObjectsWithTag("GameController");
        Debug.Log($"game_connecter:{ev.Length}");

        Debug.Log(cCardSetUp == null);
    }

}