using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using ULZAsset;
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
    [MinAttribute(1)]
    public int SelfCC_Level;
    public CardObject SelfDataSet;
    public CardSet SelfDataCardSet;
    public List<SkillObject> SelfSkillObject;

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
    [MinAttribute(1)]
    public int DuelCC_Level;
    public CardObject DuelDataSet;
    public CardSet DuelDataCardSet;
    public List<SkillObject> DuelSkillObject;

    public CCardBockCtl DuelCCSetBlock;
    // public CCardStandCtl DuelCCSetStand;
    public CCardStandCtl DuelCCSetStand;
    public int duel_atk_equ = 0, duel_def_equ = 0;
    public CCSkillRender DuelCCSetSkill;
    public CCPhaseRender DuelCCSetPhase;

    public ULVisualCtl is_panel_open;

    public string _asset_path {
        get {
            var tmp = "";
            switch (Application.platform) {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    tmp = Path.Combine("win", "x86");
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
            return Path.Combine(Application.streamingAssetsPath, tmp);
        }
    }

    /// <summary>
    /// InfoPanel
    /// </summary>
    public GameObject InfoPanel;

    public IEnumerator StartSelfCCImplement() {
        // Debug.Log ("start : " + _asset_path);
        this.SelfCC_AB = AssetBundle.LoadFromFile(
            Path.Combine(_asset_path, "CC" + (
                SelfCC_ID.ToString()
            ).PadLeft(2, '0') + ".ab")
        );

        if (this.SelfCC_AB == null) {
            yield return null;
        }
        TextAsset ta = this.SelfCC_AB.LoadAsset("card_set.json")as TextAsset;
        TextAsset ska = this.SelfCC_AB.LoadAsset("skill.json")as TextAsset;

        this.SelfDataSet = JsonConvert.DeserializeObject<CardObject>(ta.text);
        for (int i = 0; i < this.SelfDataSet.card_set.Count; i++) {
            if (this.SelfDataSet.card_set[i].level == this.SelfCC_Level) {
                this.SelfDataCardSet = this.SelfDataSet.card_set[i];
                break;
            }
        }
        List<SkillObject> tmp_sk = JsonConvert.DeserializeObject<List<SkillObject>>(ska.text);

        List<int> sumd = new List<int>();
        foreach (int d in SelfDataCardSet.skill_pointer) {
            if (!sumd.Contains(d)) {
                sumd.Add(d);
            }
        }
        List<SkillObject> skj = new List<SkillObject>();
        foreach (var tt in sumd) {
            foreach (var y in tmp_sk) {
                if (y.id == tt && !skj.Contains(y)) {
                    skj.Add(y);
                }
            }
        }

        this.SelfSkillObject = skj;

        if (this.SelfCCSetBlock == null) {
            this.SelfCCSetBlock = this.transform.root.parent.Find("Canvas/BlockLayout/CardBlock").gameObject.GetComponent<CCardBockCtl>();
        }

        if (this.SelfCCSetStand == null) {
            this.SelfCCSetStand = this.transform.root.parent.Find("StandLayer/SelfStand").gameObject.GetComponent<CCardStandCtl>();
        }

        this.SelfCCSetBlock.level = this.SelfCC_Level;

        this.SelfCCSetBlock.is_self = 1;
        this.SelfCCSetStand.is_self = 1;
        // this.SelfCCSetStand_v2.is_self = 1;

        StartCoroutine(this.SelfCCSetBlock.InitCCLvFrame());
        StartCoroutine(this.SelfCCSetBlock.InitEquSetting(self_atk_equ, self_def_equ));
        StartCoroutine(this.SelfCCSetBlock.InitCCImg(this.SelfCC_AB, this.SelfDataSet, this.SelfDataCardSet));
        StartCoroutine(this.SelfCCSetStand.InitCCImg(this.SelfCC_AB, this.SelfDataCardSet));
        StartCoroutine(this.SelfCCSetSkill.InitCCImg(this.SelfCC_AB, this.SelfSkillObject));
        StartCoroutine(this.SelfCCSetPhase.InitCCImg(this.SelfCC_AB, this.SelfDataCardSet));
        yield return true;

    }
    public IEnumerator StartDuelCCImplement() {
        // Debug.Log ("start : " + _asset_path);
        this.DuelCC_AB = AssetBundle.LoadFromFile(Path.Combine(_asset_path, "CC" + (
            DuelCC_ID.ToString()
        ).PadLeft(2, '0') + ".ab"));

        if (this.DuelCC_AB == null) {
            yield return null;
        }
        TextAsset ta = this.DuelCC_AB.LoadAsset("card_set.json")as TextAsset;
        TextAsset ska = this.DuelCC_AB.LoadAsset("skill.json")as TextAsset;

        this.DuelDataSet = JsonConvert.DeserializeObject<CardObject>(ta.text);
        for (int i = 0; i < this.DuelDataSet.card_set.Count; i++) {
            if (this.DuelDataSet.card_set[i].level == this.DuelCC_Level) {
                this.DuelDataCardSet = this.DuelDataSet.card_set[i];
                break;
            }
        }
        List<SkillObject> tmp_sk = JsonConvert.DeserializeObject<List<SkillObject>>(ska.text);

        List<int> sumd = new List<int>();
        foreach (int d in DuelDataCardSet.skill_pointer) {
            if (!sumd.Contains(d)) {
                sumd.Add(d);
            }
        }
        List<SkillObject> skj = new List<SkillObject>();
        foreach (var tt in sumd) {
            foreach (var y in tmp_sk) {
                if (y.id == tt && !skj.Contains(y)) {
                    skj.Add(y);
                }
            }
        }

        DuelSkillObject = skj;

        if (this.DuelCCSetBlock == null) {
            this.DuelCCSetBlock = this.transform.root.parent.Find("Canvas/BlockLayout/DuelCardBlock").gameObject.GetComponent<CCardBockCtl>();
        }

        if (this.DuelCCSetStand == null) {
            this.DuelCCSetStand = this.transform.root.parent.Find("StandLayer/DuelStand").gameObject.GetComponent<CCardStandCtl>();
        }

        this.DuelCCSetBlock.level = DuelCC_Level;

        this.DuelCCSetBlock.is_self = 0;
        this.DuelCCSetStand.is_self = 0;

        StartCoroutine(this.DuelCCSetBlock.InitCCLvFrame());
        StartCoroutine(this.DuelCCSetBlock.InitEquSetting(duel_atk_equ, duel_def_equ));
        StartCoroutine(this.DuelCCSetBlock.InitCCImg(this.DuelCC_AB, this.DuelDataSet, this.DuelDataCardSet));
        StartCoroutine(this.DuelCCSetStand.InitCCImg(this.DuelCC_AB, this.DuelDataCardSet));
        StartCoroutine(this.DuelCCSetSkill.InitCCImg(this.DuelCC_AB, this.DuelSkillObject));
        StartCoroutine(this.DuelCCSetPhase.InitCCImg(this.DuelCC_AB, this.DuelDataCardSet));

        yield return true;

    }

    // CC Stand Setup
    void Start() {
        StartCoroutine(StartSelfCCImplement());
        StartCoroutine(StartDuelCCImplement());
    }

    public IEnumerator OpenCCInfoPanel(int self_or_duel) {
        Debug.Log("is hello," + self_or_duel);
        if (InfoPanel.active == false && is_panel_open.AnyPanel == false) {
            is_panel_open.AnyPanel = true;
            InfoPanel.SetActive(true);
            if (self_or_duel == 1) {
                StartCoroutine(
                    InfoPanel.GetComponent<CCInfoPanel>().Init(
                        this.SelfCC_AB,
                        this.SelfDataSet, this.SelfDataCardSet,
                        this.SelfSkillObject,
                        true, this.SelfCC_ID, this.SelfCC_Level
                    ));
            } else {
                StartCoroutine(
                    InfoPanel.GetComponent<CCInfoPanel>().Init(
                        this.DuelCC_AB,
                        this.DuelDataSet, this.DuelDataCardSet,
                        this.DuelSkillObject,
                        true, this.DuelCC_ID, this.DuelCC_Level
                    ));
            }
        }
        yield return true;
    }
    public void CloseCCInfoPanel() {
        if (InfoPanel.active == true && is_panel_open.AnyPanel == true) {
            InfoPanel.SetActive(false);
            is_panel_open.AnyPanel = false;
            InfoPanel.GetComponent<CCInfoPanel>().Clean();
        }
    }
}

#if (UNITY_EDITOR) 
[CustomEditor(typeof(CCardSetUp))]
public class CCardSetUp_Editor : Editor {
    int CD = 0;
    int sider_phase = 10;
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        CCardSetUp d = (CCardSetUp)target;
        EditorGUILayout.Space(50);
        if (GUILayout.Button("Test Open Self CC Info Panel")) {
            d.OpenCCInfoPanel(1);
        }

        if (GUILayout.Button("Test Open Duel CC Info Panel")) {
            d.OpenCCInfoPanel(0);
        }
    }
}

#endif