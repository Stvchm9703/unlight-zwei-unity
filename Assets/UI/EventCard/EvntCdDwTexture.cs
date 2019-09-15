using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvntCdDwTexture : MonoBehaviour {
    void Start () {
        GameObject pgo = transform.parent.gameObject;
        string text = "";
        if (pgo.GetComponent<EvntCardProps> () != null) {
            text = pgo.GetComponent<EvntCardProps> ().down_option.ToString ();
        } else if (pgo.GetComponent<EvntCardPrev> () != null) {
            text = pgo.GetComponent<EvntCardPrev> ().down_option.ToString ();
        }

        Texture2D ts = Resources.Load<Texture2D> ("EventCard/Image/" + text) as Texture2D;
        GetComponent<RawImage> ().texture = ts;
    }
}