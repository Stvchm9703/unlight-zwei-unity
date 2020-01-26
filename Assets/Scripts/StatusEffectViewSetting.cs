using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class StatusEffectViewSetting : MonoBehaviour {
    public int st_id;
    public TextMeshPro Turn_Text;
    public SpriteRenderer Icon;
    int _turns = 0;
    public int Turns {
        get { return _turns; }
        set {
            _turns = value;
            if (Turn_Text != null) {
                Turn_Text.text = _turns.ToString ();
            }
        }
    }
    public void Remove () {
        Destroy (this.gameObject);
    }
    void Start () {
        if (Turn_Text == null) {
            Turn_Text = this.transform.Find ("turns").GetComponent<TextMeshPro> ();
        }
        if (Icon == null) {
            Icon = this.transform.Find ("icon").GetComponent<SpriteRenderer> ();
        }
    }
}