using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvntCardPrev : MonoBehaviour {
    public type_opt up_option;
    public int up_val;
    public type_opt down_option;
    public int down_val;
    public int Total;
    public int Remainer;
    void Start () { }
}

// public class EvntCdPvDwText : MonoBehaviour {
//     void Start () {
//         // Debug.Log ("<EventCdUpText");
//         EvntCardProps CardProps = transform.parent.parent.gameObject.GetComponent<EvntCardProps> ();
//         // Debug.Log (CardProps.up_val.ToString ());
//         GetComponent<Text> ().text = CardProps.down_val.ToString ();
//     }
// }

// public class EvntCdPvDwTexture : MonoBehaviour {
//     void Start () {
//         // Debug.Log ("<EventCdUpTexture");
//         EvntCardProps CardProps = transform.parent.gameObject.GetComponent<EvntCardProps> ();
//         // Debug.Log(CardProps.down_option.ToString());
//         Texture2D ts = Resources.Load<Texture2D> ("CardDuel/Image/" + CardProps.down_option.ToString ()) as Texture2D;
//         GetComponent<RawImage> ().texture = ts;
//         // upPart.transform.Find ("val").gameObject.GetComponent<Text> ().text = up_val.ToString ();
//     }
// }

// public class EvntCdPvUpText : MonoBehaviour {
//     // Start is called before the first frame update
//     void Start () {
//         EvntCardPrev CardProps = transform.parent.parent.gameObject.GetComponent<EvntCardPrev> ();
//         GetComponent<Text> ().text = CardProps.up_val.ToString ();
//     }
// }

// public class EvntCdPvUpTexture : MonoBehaviour {
//     void Start () {
//         EvntCardPrev CardProps = transform.parent.gameObject.GetComponent<EvntCardPrev> ();
//         Texture2D up_ts = Resources.Load<Texture2D> ("CardDuel/Image/" + CardProps.up_option.ToString ()) as Texture2D;
//         GetComponent<RawImage> ().texture = up_ts;
//     }
// }