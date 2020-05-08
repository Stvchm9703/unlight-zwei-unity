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
  private static CCardSetUp instance;
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
  // public 
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

  // other UI Unit
  public PhaseTurnCtl phaseTurn;
  public RangeCtl rangeCtl;

  private void Awake() {
    GameObject[] tmp = GameObject.FindGameObjectsWithTag("GameController");
    Debug.Log(tmp.Length);
    if (tmp.Length > 1) {
      Destroy(this.gameObject);
    } else {
      DontDestroyOnLoad(this.gameObject);
      Debug.Log("CCardSetUp Start");
    }
  }
  public IEnumerator SelfCCImplement() {

    SelfSkillObject = SelfDataCardSet.skill_obj;

    if (SelfCCSetBlock == null) {
      SelfCCSetBlock = transform.root.parent.Find("Canvas/BlockLayout/CardBlock").gameObject.GetComponent<CCardBockCtl>();
    }

    if (SelfCCSetStand == null) {
      SelfCCSetStand = transform.root.parent.Find("StandLayer/SelfStand").gameObject.GetComponent<CCardStandCtl>();
    }

    SelfCCSetBlock.level = SelfCC_Level;

    SelfCCSetBlock.is_self = 1;
    SelfCCSetStand.is_self = 1;
    // SelfCCSetStand_v2.is_self = 1;

    StartCoroutine(SelfCCSetBlock.InitCCLvFrame());
    StartCoroutine(SelfCCSetBlock.InitEquSetting(self_atk_equ, self_def_equ));
    StartCoroutine(SelfCCSetBlock.InitCCImg(SelfDataSet, SelfDataCardSet));
    StartCoroutine(SelfCCSetStand.InitCCImg(SelfDataCardSet));
    StartCoroutine(SelfCCSetSkill.InitCCImg2(SelfSkillObject));
    StartCoroutine(SelfCCSetPhase.InitCCImg2(SelfDataCardSet));
    yield return true;

  }
  public IEnumerator DuelCCImplement() {

    DuelSkillObject = DuelDataCardSet.skill_obj;

    if (DuelCCSetBlock == null) {
      DuelCCSetBlock = transform.root.parent.Find("Canvas/BlockLayout/DuelCardBlock").gameObject.GetComponent<CCardBockCtl>();
    }

    if (DuelCCSetStand == null) {
      DuelCCSetStand = transform.root.parent.Find("StandLayer/DuelStand").gameObject.GetComponent<CCardStandCtl>();
    }

    DuelCCSetBlock.level = DuelCC_Level;

    DuelCCSetBlock.is_self = 0;
    DuelCCSetStand.is_self = 0;

    StartCoroutine(DuelCCSetBlock.InitCCImg(DuelDataSet, DuelDataCardSet));
    StartCoroutine(DuelCCSetBlock.InitCCLvFrame());
    StartCoroutine(DuelCCSetBlock.InitEquSetting(duel_atk_equ, duel_def_equ));
    StartCoroutine(DuelCCSetStand.InitCCImg(DuelDataCardSet));
    StartCoroutine(DuelCCSetSkill.InitCCImg2(DuelSkillObject));
    StartCoroutine(DuelCCSetPhase.InitCCImg2(DuelDataCardSet));

    yield return true;

  }
  public IEnumerator StartResxLoad() {

    ABPreloaded = new List<CardSetPack>();
    List<int> used_cc_id = new List<int> {
      SelfCC_ID,
      DuelCC_ID
    }.Distinct().ToList();

    List<int> used_cc_set_id = new List<int> { SelfCardSet_ID, DuelCardSet_ID };
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

          // ABPreloaded.Add(csp);
          if (csp.id == SelfCardSet_ID) {
            SelfDataSet = co;
            SelfDataCardSet = csp;
          } else if (csp.id == DuelCardSet_ID) {
            DuelDataSet = co;
            DuelDataCardSet = csp;
          }
          ABPreloaded.Add(csp);
        }
      }

      abs.Unload(false);
    };

    yield return true;
  }
  public void InitGameCtlSetup(
    bool isHost,
    int hostCharcardId, int hostCardLevel, int hostCardsetId,
    int duelCharcardId, int duelCardLevel, int duelCardsetId
  ) {
    Debug.Log("InitGameCtlSetup");

    if (!isHost) {

      var phase_layer = GameObject.FindGameObjectWithTag("phase_layer").transform;

      SelfCCSetPhase =
        phase_layer.Find("duelphase").GetComponent<CCPhaseRender>();
      DuelCCSetPhase =
        phase_layer.Find("selfphase").GetComponent<CCPhaseRender>();

      var skill_layer = GameObject.FindGameObjectWithTag("skill_layer").transform;

      SelfCCSetSkill =
        skill_layer.Find("DuelSkillRender").GetComponent<CCSkillRender>();
      DuelCCSetSkill =
        skill_layer.Find("SelfSkillRender").GetComponent<CCSkillRender>();

      DuelCC_ID = hostCharcardId;
      DuelCC_Level = hostCardLevel;
      DuelCardSet_ID = hostCardsetId;

      SelfCC_ID = duelCharcardId;
      SelfCC_Level = duelCardLevel;
      SelfCardSet_ID = duelCardsetId;

    } else {
      SelfCC_ID = hostCharcardId;
      SelfCC_Level = hostCardLevel;
      SelfCardSet_ID = hostCardsetId;

      DuelCC_ID = duelCharcardId;
      DuelCC_Level = duelCardLevel;
      DuelCardSet_ID = duelCardsetId;
    }

    StartCoroutine(StartResxLoad());

    StartCoroutine(SelfCCImplement());
    StartCoroutine(DuelCCImplement());
  }
  // CC Stand Setup
  public void TestLoad() {

    StartCoroutine(StartResxLoad());
    StartCoroutine(SelfCCImplement());
    StartCoroutine(DuelCCImplement());

  }

  // open panel
  public IEnumerator OpenCCInfoPanel(int self_or_duel) {
    if (InfoPanel.active == false && is_panel_open.AnyPanel == false) {
      is_panel_open.AnyPanel = true;
      InfoPanel.SetActive(true);
      if (self_or_duel == 1) {
        StartCoroutine(
          InfoPanel.GetComponent<CCInfoPanel>().Init(
            SelfDataSet, SelfDataCardSet,
            SelfSkillObject,
            true, SelfCC_ID, SelfCC_Level
          ));
      } else {
        StartCoroutine(
          InfoPanel.GetComponent<CCInfoPanel>().Init(
            DuelDataSet, DuelDataCardSet,
            DuelSkillObject,
            true, DuelCC_ID, DuelCC_Level
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