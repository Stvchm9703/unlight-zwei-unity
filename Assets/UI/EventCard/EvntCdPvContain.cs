using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EvntCdPvContain : MonoBehaviour {
    void Start () {
        GameObject pgo = transform.parent.parent.gameObject;
        string total = "";
        string remainer = "";
        if (pgo.GetComponent<SpEvtCardPrev> () != null) {
            total = pgo.GetComponent<SpEvtCardPrev> ().Total.ToString ();
            remainer = pgo.GetComponent<SpEvtCardPrev> ().Remainer.ToString ();
        } else if (pgo.GetComponent<EvntCardPrev> () != null) {
            total = pgo.GetComponent<EvntCardPrev> ().Total.ToString ();
            remainer = pgo.GetComponent<EvntCardPrev> ().Remainer.ToString ();
        }
        GetComponent<Text> ().text = remainer + "/" + total;
    }

}