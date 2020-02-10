using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePhaseButtonCtl : MonoBehaviour {
    public move_phase_option current_opt;
    public MovePhaseButton Forward;
    public MovePhaseButton Stay;
    public MovePhaseButton Backward;
    public MovePhaseButton Change;
    public void OptionClick (move_phase_option opt) {
        this.current_opt = opt;
        switch (current_opt) {
            case move_phase_option.FORWARD:
                Forward.ForceOn ();
                Backward.ForceOff ();
                Stay.ForceOff ();
                if (Change != null) {
                    Change.ForceOff ();
                }
                Debug.Log (current_opt + ",in fw");

                break;
            case move_phase_option.BACKWARD:
                Backward.ForceOn ();
                Forward.ForceOff ();
                Stay.ForceOff ();
                if (Change != null) {
                    Change.ForceOff ();
                }
                Debug.Log (current_opt + ",in bw");

                break;
            case move_phase_option.STAY:
                Stay.ForceOn ();
                Forward.ForceOff ();
                Backward.ForceOff ();
                if (Change != null) {
                    Change.ForceOff ();
                }
                Debug.Log (current_opt + ",in stay");

                break;
            case move_phase_option.CHANGE:
                Stay.ForceOff ();
                Forward.ForceOff ();
                Backward.ForceOff ();
                if (Change != null) {
                    Change.ForceOn ();
                }
                Debug.Log (current_opt + ",in change");

                break;
            default:
                break;

        }
    }
    public void NewTurn () {
        current_opt = move_phase_option.NULL;
        Forward.Reset ();
        Backward.Reset ();
        Stay.Reset ();
        if (Change != null) {
            Change.Reset ();
        }
    }

    private void Start () {
        if (Forward == null) {
            Forward = this.transform.Find ("forward").GetComponent<MovePhaseButton> ();
        }
        if (Stay == null) {
            Stay = this.transform.Find ("stay").GetComponent<MovePhaseButton> ();
        }
        if (Backward == null) {
            Backward = this.transform.Find ("back").GetComponent<MovePhaseButton> ();
        }
    }

}