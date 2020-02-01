using UnityEditor;
using UnityEngine;

public class PhaseTurnCtl : MonoBehaviour {
    public Animator anim;
    void Start () {
        if (anim == null) {
            anim = this.GetComponent<Animator> ();
        }
    }
    public void PlayAnim (int phase_num, int in_out) {
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
        anim.Play (anim_name, -1);
    }

}

#if (UNITY_EDITOR) 
[CustomEditor (typeof (PhaseTurnCtl))]
public class PhaseTurnCtl_Editor : Editor {
    int phase_num = 0;
    int in_out = 0;
    public override void OnInspectorGUI () {
        DrawDefaultInspector ();
        PhaseTurnCtl d = (PhaseTurnCtl) target;
        phase_num = EditorGUILayout.IntSlider ("sider-phase", phase_num, 0, 3);
        in_out = EditorGUILayout.IntSlider ("Type", in_out, 0, 1);

        if (GUILayout.Button ("Test Move Phase")) {
            d.PlayAnim (phase_num, in_out);
        }

    }
}

#endif