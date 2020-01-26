using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using ULZAsset;
using UnityEditor;
using UnityEngine;
public class StatusEffectMainViewCtl : MonoBehaviour {
    public AssetBundle MainEffectAB;
    public GameObject StatusPrefab;

    public GameObject SelfStatusViewList, DuelStatusViewList;
    List<GameObject> SelfList, DuelList;
    public List<ULZAsset.StatusObject> StatusOpt;
    public string _asset_path {
        get {
            var tmp = "";
            switch (Application.platform) {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    tmp = Path.Combine ("win", "x86");
                    break;
                case RuntimePlatform.Android:
                    tmp = "android";
                    break;
                case RuntimePlatform.IPhonePlayer:
                    tmp = "ios";
                    break;
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    tmp = "mac";
                    break;
            }
            return Path.Combine (Application.streamingAssetsPath, tmp);
        }
    }
    void Start () {
        MainEffectAB = AssetBundle.LoadFromFile (Path.Combine (_asset_path, "status.ab"));
        if (MainEffectAB != null) {
            TextAsset ta = MainEffectAB.LoadAsset ("status.json") as TextAsset;
            StatusOpt = JsonConvert.DeserializeObject<List<ULZAsset.StatusObject>> (ta.text);
        }
        if (SelfList == null) {
            SelfList = new List<GameObject> ();
        }
        if (DuelList == null) {
            DuelList = new List<GameObject> ();
        }
    }
    public bool InsertStatusToSelf (int status_id, int CD) {
        foreach (var t in SelfList) {
            if (t != null) {
                var y = t.GetComponent<StatusEffectViewSetting> ();
                if (y.st_id == status_id) {
                    y.Turns += CD;
                    return true;
                }
            }
        }
        var targ = new ULZAsset.StatusObject ();
        foreach (var sobj in StatusOpt) {
            if (sobj.id == status_id) {
                targ = sobj;
            }
        }
        GameObject ff = (GameObject) Instantiate (StatusPrefab, new Vector3 (0, 0, 0), Quaternion.identity);
        var t2d = MainEffectAB.LoadAsset (targ.img) as Texture2D;
        ff.name = "status_ins" + this.SelfList.Count.ToString ();
        ff.transform.SetParent (SelfStatusViewList.transform);
        ff.GetComponent<StatusEffectViewSetting> ().st_id = status_id;
        ff.GetComponent<StatusEffectViewSetting> ().Icon.sprite = Sprite.Create (
            t2d,
            new Rect (-0.2f, 0, t2d.width, t2d.height),
            new Vector2 (0.5f, 0.5f)
        );

        ff.GetComponent<StatusEffectViewSetting> ().Turns = CD;
        var r = SelfStatusViewList.transform.position;
        r.y -= SelfList.Count * 0.3f;
        ff.transform.position = r;

        SelfList.Add (ff);
        return true;
    }
    public bool InsertStatusToDuel (int status_id, int CD) {
        foreach (var t in DuelList) {
            if (t != null) {
                var y = t.GetComponent<StatusEffectViewSetting> ();
                if (y.st_id == status_id) {
                    y.Turns += CD;
                    return true;
                }
            }
        }
        var targ = new ULZAsset.StatusObject ();
        foreach (var sobj in StatusOpt) {
            if (sobj.id == status_id) {
                targ = sobj;
            }
        }

        GameObject ff = (GameObject) Instantiate (StatusPrefab, new Vector3 (0, 0, 0), Quaternion.identity);
        var t2d = MainEffectAB.LoadAsset (targ.img) as Texture2D;
        ff.name = "status_ins" + this.DuelList.Count.ToString ();
        ff.transform.SetParent (DuelStatusViewList.transform);
        ff.GetComponent<StatusEffectViewSetting> ().st_id = status_id;
        ff.GetComponent<StatusEffectViewSetting> ().Icon.sprite = Sprite.Create (
            t2d,
            new Rect (-0.2f, 0, t2d.width, t2d.height),
            new Vector2 (0.5f, 0.5f)
        );
        ff.GetComponent<StatusEffectViewSetting> ().Turns = CD;
        var r = DuelStatusViewList.transform.position;
        r.y -= DuelList.Count * 0.3f;
        ff.transform.position = r;

        DuelList.Add (ff);
        return true;
    }
    public void RemoveCDToSelf (int CD) {
        List<GameObject> tmp_rm = new List<GameObject> ();
        for (int sts = 0; sts < SelfList.Count; sts++) {
            if (SelfList[sts] != null) {
                var stmp = (SelfList[sts]).GetComponent<StatusEffectViewSetting> ();
                if (stmp.Turns - CD <= 0) {
                    tmp_rm.Add (SelfList[sts]);
                } else {
                    stmp.Turns -= CD;
                }
            }
        }
        foreach (var d in tmp_rm) {
            d.GetComponent<StatusEffectViewSetting> ().Remove ();
            SelfList.Remove (d);
        }
    }
    public void RemoveCDToDuel (int CD) {
        List<GameObject> tmp_rm = new List<GameObject> ();
        for (int sts = 0; sts < DuelList.Count; sts++) {
            if (DuelList[sts] != null) {
                var stmp = (DuelList[sts]).GetComponent<StatusEffectViewSetting> ();
                if (stmp.Turns - CD <= 0) {
                    tmp_rm.Add (DuelList[sts]);
                } else {
                    stmp.Turns -= CD;
                }
            }
        }
        foreach (var d in tmp_rm) {
            d.GetComponent<StatusEffectViewSetting> ().Remove ();
            DuelList.Remove (d);
        }
    }
    // public void RemoveCD 
    public void OpenInfoPanel (int sts_id, int turns) { }
}

#if (UNITY_EDITOR) 
[CustomEditor (typeof (StatusEffectMainViewCtl))]
public class StatusEffectMVCEditor : Editor {

    int TestID = 1, CD = 0;
    public override void OnInspectorGUI () {
        DrawDefaultInspector ();
        StatusEffectMainViewCtl d = (StatusEffectMainViewCtl) target;
        TestID = EditorGUILayout.IntField ("Test ID", TestID);
        CD = EditorGUILayout.IntField ("Test CD", CD);
        if (GUILayout.Button ("Insert Status into Self ")) {
            d.InsertStatusToSelf (TestID, CD);
        }

        if (GUILayout.Button ("Insert Status into Duel")) {
            d.InsertStatusToDuel (TestID, CD);
        }

        if (GUILayout.Button ("Remove Status into Self ")) {
            d.RemoveCDToSelf (CD);
        }

        if (GUILayout.Button ("Remove Status into Duel")) {
            d.RemoveCDToDuel (CD);
        }

    }
}

#endif