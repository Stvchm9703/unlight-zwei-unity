using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using ULZAsset;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class CCardSetObj : MonoBehaviour {
    [MinAttribute(1)]
    public int ID, Level;
    public CardObject DataSet;
    public CardSet DataCardSet;
    public List<SkillObject> SkillObject;

    /// <summary>
    /// Ctrl component
    /// </summary>
    public CCardCtl Info;
    public CCardBockCtl Block;
    public CCardStandCtl StandCtl;
    public CCSkillRender Skill;
    public CCPhaseRender Phase;
    public int atk_equ = 0, def_equ = 0;
    public bool is_self = true, is_hidden = true;
    public IEnumerator StartSelfCCImplement(AssetBundle abs) {
        if (abs == null) {
            yield return null;
        }
        TextAsset ta = abs.LoadAsset("card_set.json")as TextAsset;
        TextAsset ska = abs.LoadAsset("skill.json")as TextAsset;

        this.DataSet = JsonConvert.DeserializeObject<CardObject>(ta.text);
        for (int i = 0; i < this.DataSet.card_set.Count; i++) {
            if (this.DataSet.card_set[i].level == this.Level) {
                // this.DataCardSet = this.DataSet.card_set[i];
                break;
            }
        }
        List<SkillObject> tmp_sk = JsonConvert.DeserializeObject<List<SkillObject>>(ska.text);

        List<int> sumd = new List<int>();
        foreach (int d in DataCardSet.skill_pointer) {
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

        SkillObject = skj;
        if (this.Block == null) {
            this.Block = this.transform.root.parent.Find(
                "Canvas/BlockLayout/CardBlock"
            ).gameObject.GetComponent<CCardBockCtl>();
        }

        if (this.StandCtl == null) {
            this.StandCtl = this.transform.root.parent.Find(
                "Canvas/StandImgLayout/SelfStand"
            ).gameObject.GetComponent<CCardStandCtl>();
        }

        this.Block.level = this.Level;

        this.Block.is_self = 1;
        this.StandCtl.is_self = 1;

        StartCoroutine(this.Block.InitCCLvFrame());
        StartCoroutine(this.Block.InitEquSetting(atk_equ, def_equ));
        StartCoroutine(this.Block.InitCCImg(abs, this.DataSet, this.DataCardSet));
        StartCoroutine(this.StandCtl.InitCCImg(abs, this.DataCardSet));
        StartCoroutine(this.Skill.InitCCImg(abs, this.SkillObject));
        StartCoroutine(this.Phase.InitCCImg(abs, this.DataCardSet));
        yield return true;
    }
}
public class CCardSetUpV3 : MonoBehaviour {
    // ----------------------------------------------------
    // CC Card Prefab
    List<AssetBundle> SelfCC_ABs, DuelCC_ABs;
    // / <summary>
    // / Self CC
    // / </summary>
    public GameObject CCardPrefab;

    public List<CCardSetObj> SelfCardDeck;
    public List<CCardSetObj> DuelCardDeck;
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

    // CC Stand Setup
    void Start() {
        // StartCoroutine(StartSelfCCImplement());
        // StartCoroutine(StartDuelCCImplement());
    }

    public void OpenCCInfoPanel(int self_or_duel) {
        if (self_or_duel == 1) {
            // self 
        }
    }

}