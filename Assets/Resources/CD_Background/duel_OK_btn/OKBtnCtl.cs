using UnityEditor;
using UnityEngine;
public class OKBtnCtl : MonoBehaviour {
    // public mainCtl
    public GameObject Lock;

    public bool is_lock;

    public GameObject click_btn;
    public Color able_color;
    public Color disable_color;
    Material ok_shad;
    private void Start () {
        if (Lock == null) {
            Lock = this.transform.Find ("lock").gameObject;
        }
        if (click_btn == null) {
            click_btn = this.transform.Find ("ok_btn").gameObject;
        }
        ok_shad = click_btn.GetComponent<SpriteRenderer> ().material;
        SetAble (false);
    }
    public void OKOnClick () {
        if (!is_lock) {
            // do
        }
    }
    public void SetAble (bool able) {
        if (able) {
            Lock.SetActive (false);
            Debug.Log (ok_shad.name);
            ok_shad.SetColor ("_MaskColor", able_color);
            ok_shad.SetInt ("_DisableFlash", 0);
        } else {
            Lock.SetActive (true);
            ok_shad.SetColor ("_MaskColor", disable_color);
            ok_shad.SetInt ("_DisableFlash", 1);
        }

        is_lock = !able;

    }
}

#if (UNITY_EDITOR) 
[CustomEditor (typeof (OKBtnCtl))]
public class OKBtnCtl_Editor : Editor {
    public override void OnInspectorGUI () {
        DrawDefaultInspector ();
        OKBtnCtl d = (OKBtnCtl) target;

        if (GUILayout.Button ("Test OK on")) {
            d.SetAble (true);
        }
        if (GUILayout.Button ("Test OK off")) {
            d.SetAble (false);
        }

    }
}

#endif