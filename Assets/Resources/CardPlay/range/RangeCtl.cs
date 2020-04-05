using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
public class RangeCtl : MonoBehaviour {
    public string RangeDisplay;
    public SpriteRenderer RgLong, RgMiddle, RgShort;
    public GameObject SelfStand, DuelStand;
    public float Long, Middle, Short;

    void Start() {
        this.RgShort.DOFade(0, 0f);
        this.RgLong.DOFade(0, 0);
    }
    public void MoveToShortRange() {
        var tmpshort = DOTween.Sequence()
            .Append(this.RgMiddle.DOFade(0, 0.1f))
            .Join(this.RgShort.DOFade(0, 0.1f))
            .Join(this.RgLong.DOFade(0, 0.1f))
            .Append(this.SelfStand.transform.DOLocalMoveX(-1 * Short, 0.2f))
            .Join(this.DuelStand.transform.DOLocalMoveX(Short, 0.2f))
            .Append(this.RgShort.DOFade(1, 0.1f))
            .Play();
    }
    public void MoveToMiddleRange() {
        var tmpmid = DOTween.Sequence()
            .Append(this.RgMiddle.DOFade(0, 0.1f))
            .Join(this.RgShort.DOFade(0, 0.1f))
            .Join(this.RgLong.DOFade(0, 0.1f))
            .Append(this.SelfStand.transform.DOLocalMoveX(-1 * Middle, 0.2f))
            .Join(this.DuelStand.transform.DOLocalMoveX(Middle, 0.2f))
            .Append(this.RgMiddle.DOFade(1, 0.1f))
            .Play();
    }
    public void MoveToLongRange() {
        var tmplong = DOTween.Sequence()
            .Append(this.RgMiddle.DOFade(0, 0.1f))
            .Join(this.RgShort.DOFade(0, 0.1f))
            .Join(this.RgLong.DOFade(0, 0.1f))
            .Append(this.SelfStand.transform.DOLocalMoveX(-1 * Long, 0.2f))
            .Join(this.DuelStand.transform.DOLocalMoveX(Long, 0.2f))
            .Append(this.RgLong.DOFade(1, 0.1f))
            .Play();
    }
}

#if (UNITY_EDITOR) 
[CustomEditor(typeof(RangeCtl))]
public class RangeCtl_Editor : Editor {
    int CD = 0;
    int sider_phase = 10;
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        RangeCtl d = (RangeCtl)target;
        EditorGUILayout.Space(50);
        if (GUILayout.Button("Test Move to Long-Range")) {
            d.MoveToLongRange();
        }
        if (GUILayout.Button("Test Move to Middle-Range")) {
            d.MoveToMiddleRange();
        }
        if (GUILayout.Button("Test Move to  Short-Range")) {
            d.MoveToShortRange();
        }

        // if (GUILayout.Button("Test Open Duel CC Info Panel")) {
        //     d.OpenCCInfoPanel(0);
        // }
    }
}

#endif