﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
public class CCardSetUp : MonoBehaviour {
    // ----------------------------------------------------
    // CC Card Prefab
    AssetBundle SelfCC_AB, DuelCC_AB;
    /// <summary>
    /// Self CC
    /// </summary>
    public GameObject CCardPrefab;

    public int SelfCC_ID;
    [MinAttribute (1)]
    public int SelfCC_Level;
    public CCardBockCtl SelfCCSetBlock;
    public CCardStandCtl SelfCCSetStand;
    // public CCardStandCtl SelfCCSetStand_v2;
    public int self_atk_equ = 0, self_def_equ = 0;
    public CCSkillRender SelfCCSetSkill;
    public CCPhaseRender SelfCCSetPhase;

    /// <summary>
    /// Duel CC
    /// </summary>
    public int DuelCC_ID;
    [MinAttribute (1)]
    public int DuelCC_Level;
    public CCardBockCtl DuelCCSetBlock;
    // public CCardStandCtl DuelCCSetStand;
    public CCardStandCtl DuelCCSetStand;
    public int duel_atk_equ = 0, duel_def_equ = 0;
    public CCSkillRender DuelCCSetSkill;
    public CCPhaseRender DuelCCSetPhase;
    
    
    
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

    public GameObject InfoPanel;
    
    public IEnumerator StartSelfCCImplement () {
        // Debug.Log ("start : " + _asset_path);
        this.SelfCC_AB = AssetBundle.LoadFromFile (Path.Combine (_asset_path, "CC" + (
            SelfCC_ID.ToString ()
        ).PadLeft (2, '0') + ".ab"));

        if (this.SelfCC_AB == null) {
            yield return null;
        }

        if (this.SelfCCSetBlock == null) {
            this.SelfCCSetBlock = this.transform.root.parent.Find ("Canvas/BlockLayout/CardBlock").gameObject.GetComponent<CCardBockCtl> ();
        }

        // if (this.SelfCCSetStand == null) {
        //     this.SelfCCSetStand = this.transform.root.parent.Find ("Canvas/StandImgLayout/SelfStand").gameObject.GetComponent<CCardStandCtl> ();
        // }

        if (this.SelfCCSetStand == null) {
            this.SelfCCSetStand = this.transform.root.parent.Find ("StandLayer/SelfStand").gameObject.GetComponent<CCardStandCtl> ();
        }

        this.SelfCCSetBlock.level = this.SelfCC_Level;

        this.SelfCCSetBlock.is_self = 1;
        this.SelfCCSetStand.is_self = 1;
        // this.SelfCCSetStand_v2.is_self = 1;

        StartCoroutine (this.SelfCCSetBlock.InitCCLvFrame ());
        StartCoroutine (this.SelfCCSetBlock.InitEquSetting (self_atk_equ, self_def_equ));
        StartCoroutine (this.SelfCCSetBlock.InitCCImg (this.SelfCC_AB));
        StartCoroutine (this.SelfCCSetStand.InitCCImg (this.SelfCC_AB, this.SelfCC_Level));
        StartCoroutine (this.SelfCCSetSkill.InitCCImg (this.SelfCC_AB, this.SelfCC_Level));
        StartCoroutine (this.SelfCCSetPhase.InitCCImg (this.SelfCC_AB, this.SelfCC_Level));
        yield return true;

    }
    public IEnumerator StartDuelCCImplement () {
        // Debug.Log ("start : " + _asset_path);
        this.DuelCC_AB = AssetBundle.LoadFromFile (Path.Combine (_asset_path, "CC" + (
            DuelCC_ID.ToString ()
        ).PadLeft (2, '0') + ".ab"));
        if (this.DuelCC_AB == null) {
            yield return null;
        }

        if (this.DuelCCSetBlock == null) {
            this.DuelCCSetBlock = this.transform.root.parent.Find ("Canvas/BlockLayout/DuelCardBlock").gameObject.GetComponent<CCardBockCtl> ();
        }

        if (this.DuelCCSetStand == null) {
            this.DuelCCSetStand = this.transform.root.parent.Find ("StandLayer/DuelStand").gameObject.GetComponent<CCardStandCtl> ();
        }

        this.DuelCCSetBlock.level = DuelCC_Level;

        this.DuelCCSetBlock.is_self = 0;
        this.DuelCCSetStand.is_self = 0;

        StartCoroutine (this.DuelCCSetBlock.InitCCLvFrame ());
        StartCoroutine (this.DuelCCSetBlock.InitEquSetting (duel_atk_equ, duel_def_equ));
        StartCoroutine (this.DuelCCSetBlock.InitCCImg (this.DuelCC_AB));
        StartCoroutine (this.DuelCCSetStand.InitCCImg (this.DuelCC_AB, this.DuelCC_Level));
        StartCoroutine (this.DuelCCSetSkill.InitCCImg (this.DuelCC_AB, this.DuelCC_Level));
        StartCoroutine (this.DuelCCSetPhase.InitCCImg (this.DuelCC_AB, this.DuelCC_Level));

        yield return true;

    }

    // CC Stand Setup
    void Start () {
        StartCoroutine (StartSelfCCImplement ());
        StartCoroutine (StartDuelCCImplement ());
    }

    public void OpenCCInfoPanel (int self_or_duel) {
        Debug.Log ("is hello," + self_or_duel);
        if (self_or_duel == 1) { 
            
        }
    }

}

#if (UNITY_EDITOR) 
[CustomEditor (typeof (CCardSetUp))]
public class CCardSetUp_Editor : Editor {
    int CD = 0;
    int sider_phase = 10;
    public override void OnInspectorGUI () {
        DrawDefaultInspector ();
        CCardSetUp d = (CCardSetUp) target;
        EditorGUILayout.Space (50);
        if (GUILayout.Button ("Test Open Self CC Info Panel")) {
            d.OpenCCInfoPanel (1);
        }

        if (GUILayout.Button ("Test Open Duel CC Info Panel")) {
            d.OpenCCInfoPanel (0);
        }
    }
}

#endif