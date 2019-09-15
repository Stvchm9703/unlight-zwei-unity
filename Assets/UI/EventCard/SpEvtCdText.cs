using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SpEvtCdText : MonoBehaviour {
    void Start () {
        GameObject pgo = transform.parent.parent.gameObject;
        string text = "";
        if (pgo.GetComponent<SpEvtCardPrev> () != null) {
            text = pgo.GetComponent<SpEvtCardPrev> ().card_val.ToString ();
        } else if (pgo.GetComponent<SpEvtCardProps> () != null) {
            text = pgo.GetComponent<SpEvtCardProps> ().card_val.ToString ();
        }
        GetComponent<Text> ().text = text;
    }
}