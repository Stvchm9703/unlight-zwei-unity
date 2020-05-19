using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using ULZAsset;
using UnityEditor;
using UnityEngine;
public class CCSkillRender : MonoBehaviour {
    public List<GameObject> Skill_preload;
    public GameObject Backgroud;
    private Material k;
    public Color EX_Color { get { return new Color(1f, 1f, 0f, 1f); } }
    public Color Nor_Color { get { return new Color(1f, 0f, 0f, 1f); } }
    public Animation SkillMask;
    public GameObject skill_prefab;
    public GameObject skl_ls;
    public GameObject MaskObj;
    public IEnumerator InitCCImg(AssetBundle abs, List<SkillObject> skobj) {
        if (abs == null) {
            yield return false;
        }

        foreach (var tmpsk in skobj) {
            if (skl_ls.transform.Find(tmpsk.effect_image.name) == null) {
                GameObject ff = GameObject.Instantiate(skill_prefab, skl_ls.transform);
                ff.name = tmpsk.effect_image.name;
                ff.GetComponent<CCSkillObj>().import_info(tmpsk);
                ff.transform.SetParent(skl_ls.transform);
                var ffsp = ff.GetComponent<SpriteRenderer>();
                Texture2D tas = abs.LoadAsset(tmpsk.effect_image.name)as Texture2D;

                ffsp.sprite = Sprite.Create(tas,
                    new Rect(0, 0, tas.width, tas.height),
                    new Vector2(0.5f, 0.5f)
                );

                float orig_ratio = ((float)tas.height / (float)tas.width) /
                    ((float)tmpsk.effect_image.height / (float)tmpsk.effect_image.width);

                ff.GetComponent<Transform>().localScale *= new Vector2(orig_ratio, 1);
                ffsp.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                ffsp.sortingLayerName = "skill_render";
                Skill_preload.Add(ff);
                ff.SetActive(false);
            }
        }
        yield return true;
    }
    public List<int> debug_skl_id;
    public IEnumerator InitCCImg2(List<SkillObject> skobj) {
        foreach (var tmpsk in skobj) {
            if (skl_ls.transform.Find(tmpsk.effect_image.name) == null) {
                GameObject ff = GameObject.Instantiate(skill_prefab, skl_ls.transform);
                ff.name = tmpsk.effect_image.name;
                ff.GetComponent<CCSkillObj>().import_info(tmpsk);
                ff.transform.SetParent(skl_ls.transform);
                var ffsp = ff.GetComponent<SpriteRenderer>();
                Texture2D tas = tmpsk.effect_image_t2;
                ffsp.sprite = Sprite.Create(tas,
                    new Rect(0, 0, tas.width, tas.height),
                    new Vector2(0.5f, 0.5f)
                );

                float orig_ratio = ((float)tas.height / (float)tas.width) /
                    ((float)tmpsk.effect_image.height / (float)tmpsk.effect_image.width);

                ff.GetComponent<Transform>().localScale *= new Vector2(orig_ratio, 1);
                ffsp.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                ffsp.sortingLayerName = "skill_render";
                Skill_preload.Add(ff);
                debug_skl_id.Add(tmpsk.id);
                ff.SetActive(false);
            }
        }
        yield return true;
    }

    public IEnumerator PlayAnim(int skill_id) {
        GameObject tmp = null;
        foreach (var tt in Skill_preload) {
            if (tt.GetComponent<CCSkillObj>().id == skill_id) {
                tt.SetActive(true);
                tmp = tt;
                break;
            }
        }
        SkillMask.Play();
        yield return new WaitForSeconds(2f);
        tmp.SetActive(false);
        yield return true;
    }
    public IEnumerator PlayAnimSort(int sort_no) {
        // Debug.Log (Skill_preload);
        GameObject tmp = Skill_preload[sort_no];
        tmp.SetActive(true);
        SkillMask.Play();
        yield return new WaitForSeconds(2f);
        tmp.SetActive(false);
        yield return true;

    }
    void Start() {
        if (Backgroud == null) {
            Backgroud = this.transform.Find("background").gameObject;
        }
        Backgroud.SetActive(false);
        k = Backgroud.GetComponent<SpriteRenderer>().material;
        if (SkillMask == null) {
            SkillMask = this.gameObject.GetComponent<Animation>();
            SkillMask.wrapMode = WrapMode.Once;
        }
        if (skl_ls == null) {
            skl_ls = this.transform.Find("skillObj").gameObject;
        }
    }

    public void testPlayAnim(int test_id) {
        StartCoroutine(PlayAnim(test_id));
    }

    public void testPlayAnimSort(int test_id) {
        StartCoroutine(PlayAnimSort(test_id));
    }

}

#if (UNITY_EDITOR) 
[CustomEditor(typeof(CCSkillRender))]
public class CCSkillRenderEditor : Editor {

    int TestID = 1;
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        CCSkillRender d = (CCSkillRender)target;
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Test Block");
        TestID = EditorGUILayout.IntField("Test ID", TestID);
        if (GUILayout.Button("Insert Status into Self ")) {
            d.testPlayAnim(TestID);
        }

        if (GUILayout.Button("Test Play Skiil obj")) {
            d.testPlayAnimSort(TestID);
        }
    }
}

#endif