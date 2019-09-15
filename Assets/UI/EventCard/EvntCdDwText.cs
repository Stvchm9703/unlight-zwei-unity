using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EvntCdDwText : MonoBehaviour {
    void Start () {
        GameObject pgo = transform.parent.parent.gameObject;
        string text = "";
        if (pgo.GetComponent<EvntCardProps> () != null) {
            text = pgo.GetComponent<EvntCardProps> ().down_val.ToString ();
        } else if (pgo.GetComponent<EvntCardPrev> () != null) {
            text = pgo.GetComponent<EvntCardPrev> ().down_val.ToString ();
        }
        GetComponent<Text> ().text = text;
    }
}