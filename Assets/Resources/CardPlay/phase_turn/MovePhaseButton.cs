using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ULZAsset.ProtoMod.GameDuelService;
public enum move_phase_option { NULL = 0, FORWARD = 1, STAY = 2, BACKWARD = 3, CHANGE = 4, }
public class MovePhaseButton : MonoBehaviour {
    public MovePhaseButtonCtl mainctl;
    Material effect;
    public MovePhaseOpt option;
    public void ForceOn () {
        if (effect != null) {
            effect.SetInt ("ForceOn", 1);
            effect.SetInt ("IsGrow", 1);
        }

    }
    public void ForceOff () {
        if (effect != null) {
            effect.SetInt ("ForceOn", 0);
            effect.SetInt ("IsGrow", 0);
        }
    }

    public void Reset () {
        if (effect != null) {
            effect.SetInt ("ForceOn", 0);
            effect.SetInt ("IsGrow", 1);
        }

    }
    private void Start () {
        if (mainctl == null) {
            mainctl = this.transform.parent.GetComponent<MovePhaseButtonCtl> ();
        }
        if (effect == null) {
            effect = this.GetComponent<SpriteRenderer> ().material;
        }
    }

    private void OnMouseDown () {
        mainctl.OptionClick (option);
    }
}