using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using ULZAsset;
using UnityEditor;
using UnityEngine;
public class CCardSetUp : MonoBehaviour {
  // ----------------------------------------------------
  // CC Card Prefab
  // AssetBundle SelfCC_AB, DuelCC_AB;
  /// <summary>
  /// Self CC
  /// </summary>
  public GameObject CCardPrefab;
  public List<CardSetPack> ABPreloaded;

  public int SelfCC_ID, SelfCC_Level, SelfCardSet_ID;
  public CardObject SelfDataSet;
  public CardSetPack SelfDataCardSet;
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
  [MinAttribute(1)]
  public int DuelCC_ID, DuelCC_Level, DuelCardSet_ID;
  public CardObject DuelDataSet;
  public CardSetPack DuelDataCardSet;
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

  public IEnumerator SelfCCImplement() {

    this.SelfSkillObject = this.SelfDataCardSet.skill_obj;

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
    StartCoroutine(this.SelfCCSetBlock.InitCCImg(this.SelfDataSet, this.SelfDataCardSet));
    StartCoroutine(this.SelfCCSetStand.InitCCImg(this.SelfDataCardSet));
    StartCoroutine(this.SelfCCSetSkill.InitCCImg2(this.SelfSkillObject));
    StartCoroutine(this.SelfCCSetPhase.InitCCImg2(this.SelfDataCardSet));
    yield return true;

  }
  public IEnumerator DuelCCImplement() {

    this.DuelSkillObject = this.DuelDataCardSet.skill_obj;

    if (this.DuelCCSetBlock == null) {
      this.DuelCCSetBlock = this.transform.root.parent.Find("Canvas/BlockLayout/DuelCardBlock").gameObject.GetComponent<CCardBockCtl>();
    }

    if (this.DuelCCSetStand == null) {
      this.DuelCCSetStand = this.transform.root.parent.Find("StandLayer/DuelStand").gameObject.GetComponent<CCardStandCtl>();
    }

    this.DuelCCSetBlock.level = DuelCC_Level;

    this.DuelCCSetBlock.is_self = 0;
    this.DuelCCSetStand.is_self = 0;

    StartCoroutine(this.DuelCCSetBlock.InitCCImg(this.DuelDataSet, this.DuelDataCardSet));
    StartCoroutine(this.DuelCCSetBlock.InitCCLvFrame());
    StartCoroutine(this.DuelCCSetBlock.InitEquSetting(duel_atk_equ, duel_def_equ));
    StartCoroutine(this.DuelCCSetStand.InitCCImg(this.DuelDataCardSet));
    StartCoroutine(this.DuelCCSetSkill.InitCCImg2(this.DuelSkillObject));
    StartCoroutine(this.DuelCCSetPhase.InitCCImg2(this.DuelDataCardSet));

    yield return true;

  }
  public IEnumerator StartResxLoad() {
    // yield return false;
    // this.SelfDataSet = new CardObject();
    // this.
    this.ABPreloaded = new List<CardSetPack>();
    List<int> used_cc_id = new List<int> {
      this.SelfCC_ID,
      this.DuelCC_ID
    }.Distinct().ToList();

    List<int> used_cc_set_id = new List<int> { this.SelfCardSet_ID, this.DuelCardSet_ID };
    foreach (var tmp in used_cc_id) {
      var abs = AssetBundle.LoadFromFile(
        Path.Combine(_asset_path, "CC" + (
          tmp.ToString()
        ).PadLeft(2, '0') + ".ab")
      );

      if (abs == null) {
        yield return null;
      }
      TextAsset ta = abs.LoadAsset("card_set.json")as TextAsset;
      TextAsset ska = abs.LoadAsset("skill.json")as TextAsset;

      CardObject co = JsonConvert.DeserializeObject<CardObject>(ta.text);
      List<SkillObject> tmp_sk = JsonConvert.DeserializeObject<List<SkillObject>>(ska.text);

      foreach (var cs in co.card_set) {
        if (used_cc_set_id.Contains(cs.id)) {
          CardSetPack csp = new CardSetPack();
          csp = cs;
          csp.chara_image_t2 = abs.LoadAsset(csp.chara_image.name.Replace(".png", ""))as Texture2D;
          csp.stand_image_t2 = abs.LoadAsset(csp.stand_image.name.Replace(".png", ""))as Texture2D;
          csp.bg_image_t2 = abs.LoadAsset(csp.bg_image.name.Replace(".png", ""))as Texture2D;
          csp.artifact_image_t2 = abs.LoadAsset(csp.artifact_image.name.Replace(".png", ""))as Texture2D;

          List<int> sumd = new List<int>();
          foreach (int d in csp.skill_pointer) {
            if (!sumd.Contains(d)) {
              sumd.Add(d);
            }
          }
          List<SkillObject> skj = new List<SkillObject>();
          foreach (var y in tmp_sk) {
            if (sumd.Contains(y.id) && !skj.Contains(y)) {
              skj.Add(y);
            }
          }

          foreach (var y in skj) {
            y.effect_image_t2 = abs.LoadAsset(y.effect_image.name.Replace(".png", ""))as Texture2D;
          }

          csp.skill_obj = skj;

          // this.ABPreloaded.Add(csp);
          if (csp.id == this.SelfCardSet_ID) {
            this.SelfDataSet = co;
            this.SelfDataCardSet = csp;
          } else if (csp.id == this.DuelCardSet_ID) {
            this.DuelDataSet = co;
            this.DuelDataCardSet = csp;
          }
          this.ABPreloaded.Add(csp);
        }
      }

      abs.Unload(false);
    };


    yield return true;
  }

  // CC Stand Setup
  public void TestLoad() {

    StartCoroutine(StartResxLoad());
    StartCoroutine(SelfCCImplement());
    StartCoroutine(DuelCCImplement());

  }

  public IEnumerator OpenCCInfoPanel(int self_or_duel) {
    if (InfoPanel.active == false && is_panel_open.AnyPanel == false) {
      is_panel_open.AnyPanel = true;
      InfoPanel.SetActive(true);
      if (self_or_duel == 1) {
        StartCoroutine(
          InfoPanel.GetComponent<CCInfoPanel>().Init(
            this.SelfDataSet, this.SelfDataCardSet,
            this.SelfSkillObject,
            true, this.SelfCC_ID, this.SelfCC_Level
          ));
      } else {
        StartCoroutine(
          InfoPanel.GetComponent<CCInfoPanel>().Init(
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
    EditorGUILayout.Space(20);
    if (GUILayout.Button("Test Load Asset ")) {
      d.TestLoad();
    }
    EditorGUILayout.Space(20);
    if (GUILayout.Button("Test Open Self CC Info Panel")) {
      d.OpenCCInfoPanel(1);
    }
    if (GUILayout.Button("Test Open Duel CC Info Panel")) {
      d.OpenCCInfoPanel(0);
    }
  }
}

#endif