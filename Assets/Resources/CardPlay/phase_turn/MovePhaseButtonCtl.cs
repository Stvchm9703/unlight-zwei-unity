using System.Collections;
using System.Collections.Generic;
using ULZAsset.ProtoMod.GameDuelService;
using UnityEngine;
public class MovePhaseButtonCtl : MonoBehaviour {
    public MovePhaseOpt current_opt;
    public MovePhaseButton Forward;
    public MovePhaseButton Stay;
    public MovePhaseButton Backward;
    public MovePhaseButton Change;
    public void OptionClick(MovePhaseOpt opt) {
        this.current_opt = opt;
        switch (current_opt) {
            case MovePhaseOpt.Forward:
                Forward.ForceOn();
                Backward.ForceOff();
                Stay.ForceOff();
                if (Change != null) {
                    Change.ForceOff();
                }
                Debug.Log(current_opt + ",in fw");

                break;
            case MovePhaseOpt.Backward:
                Backward.ForceOn();
                Forward.ForceOff();
                Stay.ForceOff();
                if (Change != null) {
                    Change.ForceOff();
                }
                Debug.Log(current_opt + ",in bw");

                break;
            case MovePhaseOpt.Stay:
                Stay.ForceOn();
                Forward.ForceOff();
                Backward.ForceOff();
                if (Change != null) {
                    Change.ForceOff();
                }
                Debug.Log(current_opt + ",in stay");

                break;
            case MovePhaseOpt.Change:
                Stay.ForceOff();
                Forward.ForceOff();
                Backward.ForceOff();
                if (Change != null) {
                    Change.ForceOn();
                }
                Debug.Log(current_opt + ",in change");

                break;
            default:
                break;
        }
    }
    public void NewTurn() {
        current_opt = MovePhaseOpt.NoMove;
        Forward.Reset();
        Backward.Reset();
        Stay.Reset();
        if (Change != null) {
            Change.Reset();
        }
    }

    private void Start() {
        if (Forward == null) {
            Forward = this.transform.Find("forward").GetComponent<MovePhaseButton>();
        }
        if (Stay == null) {
            Stay = this.transform.Find("stay").GetComponent<MovePhaseButton>();
        }
        if (Backward == null) {
            Backward = this.transform.Find("back").GetComponent<MovePhaseButton>();
        }
    }

}