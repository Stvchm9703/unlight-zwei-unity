using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using ULZAsset;
using UnityEngine;
// using UnityEngine.EventSystems;
// using UnityEngine.UI;
public class CCardStandCtl : MonoBehaviour {
    public int is_self = 1;
    public CCardSetUp main_ctl;

    public GameObject MainStand;
    public Vector2 MainStand_size;
    public ImgSet MainStand_raw;
    public GameObject Shadow;
    public Vector2 Shadow_size;
    public ImgSet Shadow_raw;

    public Transform CCMainSet;

    public GameObject SkillList;

    public float ratio;
    void Start () {
        if (MainStand == null) {
            MainStand = this.transform.Find ("cc/stand").gameObject;
        }
        if (Shadow == null) {
            Shadow = this.transform.Find ("cc/shadow").gameObject;
        }
        if (CCMainSet == null) {
            CCMainSet = this.transform.Find ("cc").gameObject.GetComponent<Transform> ();
        }
        if (main_ctl == null) {
            main_ctl = this.transform.root.Find ("EventSystem").gameObject.GetComponent<CCardSetUp> ();
        }
    }
    public IEnumerator InitCCImg (AssetBundle ab, int level) {
        if (ab == null) {
            yield return false;
        }
        TextAsset ta = ab.LoadAsset ("card_set.json") as TextAsset;
        var json = JsonConvert.DeserializeObject<CardObject> (ta.text);

        for (int i = 0; i < json.card_set.Count; i++) {
            if (json.card_set[i].level == level) {
                MainStand_raw = json.card_set[i].stand_image;
                Shadow_raw = json.card_set[i].bg_image;
                break;
            }
        }

        if (MainStand == null) {
            yield return false;
        } else {
            Texture2D tas = ab.LoadAsset (MainStand_raw.name) as Texture2D;
            var ms_sprite = MainStand.GetComponent<SpriteRenderer> ();
            ms_sprite.sprite = Sprite.Create (tas,
                new Rect (0, 0, tas.width, tas.height),
                new Vector2 (0.5f, 0f)
            );
            MainStand.AddComponent<BoxCollider2D>();
        }

        if (Shadow == null) {
            yield return false;
        } else {
            Texture2D tas = ab.LoadAsset (Shadow_raw.name) as Texture2D;
            var ms_sprite = Shadow.GetComponent<SpriteRenderer> ();
            ms_sprite.sprite = Sprite.Create (tas,
                new Rect (0, 0, tas.width, tas.height),
                new Vector2 (0.5f, 1f)
            );
        }

        if (CCMainSet == null) {
            yield return false;
        } else {
            CCMainSet.localScale = new Vector3 (ratio, ratio, 1);
        }

        yield return true;
    }

    public void ClickTrig () {
        Debug.Log("hi");
        if (main_ctl != null) {
            main_ctl.OpenCCInfoPanel (is_self);
        }
    }

}