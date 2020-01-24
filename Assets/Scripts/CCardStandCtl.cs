using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using ULZAsset;
public class CCardStandCtl: MonoBehaviour,IPointerClickHandler {

    public int is_self = 1;
    public CCardSetUp main_ctl;

    public GameObject MainStand;
    public Vector2 MainStand_size;
    public ImgSet MainStand_raw;
    public GameObject Shadow;
    public Vector2 Shadow_size;
    public ImgSet Shadow_raw;
    
    void Start() {
        if (MainStand == null) {
            MainStand = this.transform.Find("Stand_Img").gameObject;
        }
        if (Shadow == null) {
            Shadow = this.transform.Find("shadow").gameObject;
        }
        if (main_ctl == null) {
            main_ctl = this.transform.root.Find("EventSystem").gameObject.GetComponent<CCardSetUp>();
        }
    }
    public IEnumerator InitCCImg(AssetBundle ab, int level) {
        if (ab == null) {
            yield return false;
        }
        TextAsset ta = ab.LoadAsset("card_set.json")as TextAsset;
        var json = JsonConvert.DeserializeObject < ULZAsset.CardObject > (ta.text);


        for (int i = 0; i < json.card_set.Count; i ++) {
            if (json.card_set[i].level == level) {
                MainStand_raw = json.card_set[i].stand_image;
                Shadow_raw = json.card_set[i].bg_image;
                break;
            }
        }
        float ratio = 300f / 210f;
        MainStand_size = new Vector2( (float)(MainStand_raw.width) * ratio, (float)(MainStand_raw.height) * ratio);
        Shadow_size = new Vector2( (float)(Shadow_raw.width) * ratio, (float)(Shadow_raw.height) * ratio);
        
        if (MainStand == null) {
            yield return false;
        } else {
            Texture2D tas = ab.LoadAsset(MainStand_raw.name)as Texture2D;
            MainStand.GetComponent<RawImage>().texture = tas;
            RectTransform rt = (RectTransform)(MainStand.transform);
            rt.sizeDelta = MainStand_size;
        }

        if (Shadow == null) {
            yield return false;
        } else {
            Texture2D tas = ab.LoadAsset(Shadow_raw.name)as Texture2D;
            Shadow.GetComponent<RawImage>().texture = tas;
            RectTransform rt = (RectTransform)(Shadow.transform);
            rt.sizeDelta = Shadow_size;
        }

        yield return true;
    }

    // OnPointerClick : Click event trigger
    public void OnPointerClick(PointerEventData eventData) {
        if (main_ctl != null) {
            main_ctl.OpenCCInfoPanel(is_self);
        }
    }

}
