using ULZAsset.ProtoMod.GameDuelService;
using UnityEditor;
using UnityEngine;
public enum Test_EventHookPhase {
    Draw = EventHookPhase.RefillActionCardPhase,
    Move = EventHookPhase.MoveCardDropPhase,
    Attack = EventHookPhase.AttackCardDropPhase,
    Defence = EventHookPhase.DefenceCardDropPhase,
    Dice = EventHookPhase.DetermineBattlePointPhase,
}

public static class Test_EventHookPhaseCast {

    public static EventHookPhase ToEventHookPhase(Test_EventHookPhase testval) {
        switch (testval) {
            case Test_EventHookPhase.Draw:
                return EventHookPhase.RefillActionCardPhase;
            case Test_EventHookPhase.Move:
                return EventHookPhase.MoveCardDropPhase;
            case Test_EventHookPhase.Attack:
                return EventHookPhase.AttackCardDropPhase;
            case Test_EventHookPhase.Defence:
                return EventHookPhase.DefenceCardDropPhase;
            case Test_EventHookPhase.Dice:
                return EventHookPhase.DetermineBattlePointPhase;
            default:
                return EventHookPhase.GamesetEnd;
        }
    }
}
public class PhaseTurnCtl : MonoBehaviour {
    public Animator anim;
    void Start() {
        if (anim == null) {
            anim = this.GetComponent<Animator>();
        }
    }
    public void PlayAnim(int phase_num, int in_out) {
        string anim_name = in_out == 1 ? "in" : "out";
        switch (phase_num) {
            case 0:
                anim_name = "draw_" + anim_name;
                break;
            case 1:
                anim_name = "move_" + anim_name;
                break;
            case 2:
                anim_name = "atk_" + anim_name;
                break;
            case 3:
                anim_name = "def_" + anim_name;
                break;
        }
        anim.Play(anim_name, -1);
    }
    public void PlayAnim(EventHookPhase HookPhase, EventHookType type) {
        string anim_name = "";
        if (HookPhase == EventHookPhase.RefillActionCardPhase) {
            anim_name = "draw_";
        } else if (HookPhase == EventHookPhase.MoveCardDropPhase) {
            anim_name = "move_";
        } else if (HookPhase == EventHookPhase.AttackCardDropPhase) {
            anim_name = "atk_";
        } else if (HookPhase == EventHookPhase.DefenceCardDropPhase) {
            anim_name = "def_";
        }

        if (type == EventHookType.Before) {
            anim_name = anim_name + "in";
        } else if (type == EventHookType.After) {
            anim_name = anim_name + "out";
        }
        anim.Play(anim_name, -1);

    }
}

#if (UNITY_EDITOR) 
[CustomEditor(typeof(PhaseTurnCtl))]
public class PhaseTurnCtl_Editor : Editor {
    int phase_num = 0;
    int in_out = 0;
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        PhaseTurnCtl d = (PhaseTurnCtl)target;
        phase_num = EditorGUILayout.IntSlider("sider-phase", phase_num, 0, 3);
        in_out = EditorGUILayout.IntSlider("Type", in_out, 0, 1);

        if (GUILayout.Button("Test Move Phase")) {
            d.PlayAnim(phase_num, in_out);
        }

    }
}

#endif