using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SpEvtCdTexture : MonoBehaviour {
    // Start is called before the first frame update
    void Start () {
        GameObject pgo = transform.parent.gameObject;
        string texture_name = "";
        if (pgo.GetComponent<SpEvtCardPrev> ()) {
            texture_name = pgo.GetComponent<SpEvtCardPrev> ().card_type.ToString ();
        } else if (pgo.GetComponent<SpEvtCardProps> ()) {
            texture_name = pgo.GetComponent<SpEvtCardProps> ().card_type.ToString ();
        }
        Texture2D ts = Resources.Load<Texture2D> ("EventCard/Image/sp/" + texture_name) as Texture2D;
        GetComponent<RawImage> ().texture = ts;
    }
}