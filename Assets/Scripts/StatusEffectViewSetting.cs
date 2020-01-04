using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StatusEffectViewSetting : MonoBehaviour {
    public int st_id;
    public Text Turns_Text;
    public RawImage Icon_ri;
    int  _turns = 0;
    public int Turns {
        get { return _turns; }
        set {
            _turns = value;
            if (Turns_Text != null) {
                Turns_Text.text =  _turns.ToString ();
            }
        }
    }
    public void Remove() {
        Destroy(this.gameObject);
    }
    void Start () {
        if (Turns_Text == null) {
            Turns_Text = this.transform.Find ("turns").GetComponent<Text> ();
        }
        if (Icon_ri == null) {
            Icon_ri = this.transform.Find ("turns").GetComponent<RawImage> ();
        }
    }
}