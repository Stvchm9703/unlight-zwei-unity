using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using ULZAsset;
using ULZAsset;
using UnityEngine;
public class CCardStandCtl : MonoBehaviour {
    public int is_self = 1;
    // public CCardSetUp main_ctl;
    public MainCtrlComp main_ctl;
    public GameObject MainStand;
    public Vector2 MainStand_size;
    public ImgSet MainStand_raw;
    public GameObject Shadow;
    public Vector2 Shadow_size;
    public ImgSet Shadow_raw;

    public Transform CCMainSet;

    public GameObject SkillList;

    public float ratio;
    void Start() {
        if (MainStand == null) {
            MainStand = this.transform.Find("cc/stand").gameObject;
        }
        if (Shadow == null) {
            Shadow = this.transform.Find("cc/shadow").gameObject;
        }
        if (CCMainSet == null) {
            CCMainSet = this.transform.Find("cc").gameObject.GetComponent<Transform>();
        }
        if (main_ctl == null) {
            // main_ctl = this.transform.root.Find("EventSystem").gameObject.GetComponent<CCardSetUp>();
            main_ctl = this.transform.root.Find("MainCtl").gameObject.GetComponent<MainCtrlComp>();
        }
    }
    public IEnumerator InitCCImg(AssetBundle ab, CardSet Cs) {
        if (ab == null) {
            yield return false;
        }

        MainStand_raw = Cs.stand_image;
        Shadow_raw = Cs.bg_image;

        if (MainStand == null) {
            yield return false;
        } else {
            Texture2D tas = ab.LoadAsset(MainStand_raw.name)as Texture2D;
            var ms_sprite = MainStand.GetComponent<SpriteRenderer>();
            ms_sprite.sprite = Sprite.Create(tas,
                new Rect(0, 0, tas.width, tas.height),
                new Vector2(0.5f, 0f)
            );
            MainStand.AddComponent<BoxCollider2D>();

        }

        if (Shadow == null) {
            yield return false;
        } else {
            Texture2D tas = ab.LoadAsset(Shadow_raw.name)as Texture2D;
            var ms_sprite = Shadow.GetComponent<SpriteRenderer>();
            ms_sprite.sprite = Sprite.Create(tas,
                new Rect(0, 0, tas.width, tas.height),
                new Vector2(0.5f, 1f)
            );
        }

        if (CCMainSet == null) {
            yield return false;
        } else {
            CCMainSet.localScale = new Vector3(ratio, ratio, 1);
        }

        yield return true;
    }

    public IEnumerator InitCCImg(CardSetPack Cs) {
        MainStand_raw = Cs.stand_image;
        Shadow_raw = Cs.bg_image;

        if (MainStand == null) {
            yield return false;
        } else {
            Texture2D tas = Cs.stand_image_t2;

            var income_img_ratio = (float)(tas.height / tas.width);
            var orig_img_ratio = (float)Cs.stand_image.height / Cs.stand_image.width;

            var ms_sprite = MainStand.GetComponent<SpriteRenderer>();
            ms_sprite.sprite = Sprite.Create(tas,
                new Rect(0, 0, tas.width, tas.height),
                new Vector2(0.5f, 0f)
            );
            MainStand.AddComponent<BoxCollider2D>();

            var mtrans = MainStand.GetComponent<RectTransform>();
            mtrans.localScale = new Vector3((float)(income_img_ratio / orig_img_ratio), 1, 1);
        }

        if (Shadow == null) {
            yield return false;
        } else {
            Texture2D tas = Cs.bg_image_t2;
            var income_img_ratio = (float)1 / (tas.width / tas.height);
            var orig_img_ratio = (float)Cs.bg_image.height / Cs.bg_image.width;

            var ms_sprite = Shadow.GetComponent<SpriteRenderer>();
            ms_sprite.sprite = Sprite.Create(tas,
                new Rect(0, 0, tas.width, tas.height),
                new Vector2(0.5f, 1f)
            );
            var mtrans = Shadow.GetComponent<RectTransform>();
            mtrans.localScale = new Vector3((float)(income_img_ratio / orig_img_ratio ), 1, 1);

        }

        if (CCMainSet == null) {
            yield return false;
        } else {
            this.transform.Find("cc").localScale = new Vector3(ratio, ratio, 1);
        }

        yield return true;
    }
    public void ClickTrig() {
        if (main_ctl != null) {
            StartCoroutine(
                main_ctl.OpenCCInfoPanel(is_self)
            );
        }
    }

}