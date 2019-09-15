using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvntCdUpText : MonoBehaviour {
    // Start is called before the first frame update
    void Start () {
        // Debug.Log ("<EventCdUpText");
        GameObject pgo = transform.parent.parent.gameObject;
        string text = "";
        if (pgo.GetComponent<EvntCardProps> () != null) {
            text = pgo.GetComponent<EvntCardProps> ().up_val.ToString ();
        } else if (pgo.GetComponent<EvntCardPrev> () != null) {
            text = pgo.GetComponent<EvntCardPrev> ().up_val.ToString ();
        }
        GetComponent<Text> ().text = text;
    }
}