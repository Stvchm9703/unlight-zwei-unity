using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using ULZAsset;
using UnityEngine;
using UnityEngine.UI;
public class CCInfoPanel : MonoBehaviour {
    // public CCardSetUp main_asset;
    // AssetBundle main_asset;
    public CCardCtl card_face;
    public GameObject status_list;
    public GameObject status_prefab;
    public TextMeshProUGUI CC_Title;
    public Text CC_Desp;
    public GameObject Skill_Info_Prefab, Status_Info_Prefab;
    public GameObject ScrollParent;
    public SystemLanguage sys_lang;
    public List<GameObject> WaitForDestory;

    public StatusEffectMainViewCtl status_ab;
    public IEnumerator Init(
        AssetBundle tar_asset,
        CardObject json, CardSet cs, List<SkillObject> sko,
        bool isSelf, int cc_id, int level
    ) {
        // yield return null;
        card_face.CC_id = cc_id;
        card_face.level = level;
        StartCoroutine(card_face.InitCCImg(tar_asset, json, cs));
        StartCoroutine(card_face.InitCCLvFrame());
        StartCoroutine(card_face.InitEquSetting(0, 0));
        string name_string = "", desp_string = "";
        switch (sys_lang) {
            case SystemLanguage.Chinese:
            case SystemLanguage.ChineseTraditional:
                name_string = json.name.tcn;
                desp_string = json.caption.tcn;
                break;
            case SystemLanguage.ChineseSimplified:
                name_string = json.name.scn;
                desp_string = json.caption.scn;

                break;
            case SystemLanguage.Japanese:
                name_string = json.name.jp;
                desp_string = json.caption.jp;

                break;
            case SystemLanguage.Korean:
                name_string = json.name.kr;
                desp_string = json.caption.kr;
                break;
            case SystemLanguage.Indonesian:
                name_string = json.name.ina;
                desp_string = json.caption.ina;
                break;
            case SystemLanguage.Thai:
                name_string = json.name.thai;
                desp_string = json.caption.thai;
                break;
            case SystemLanguage.English:
            case SystemLanguage.Unknown:
            default:
                name_string = json.name.en;
                desp_string = json.caption.en;
                break;
        }
        if (name_string == "") {
            name_string = json.name.jp;
            desp_string = json.caption.jp;
        }

        CC_Title.text = name_string;
        CC_Desp.text = desp_string;

        foreach (SkillObject t in sko) {
            GameObject ff = (GameObject)Instantiate(
                Skill_Info_Prefab, ScrollParent.transform);
            ff.GetComponent<IP_SkillBox>().init(t);
            this.WaitForDestory.Add(ff);
        }
        List<GameObject> status_icon_tmp = new List<GameObject>();
        var Status_list_sample = isSelf? status_ab.SelfList : status_ab.DuelList;
        for (int i = 0; i < Status_list_sample.Count; i++) {
            var st_id = Status_list_sample[i].GetComponent<StatusEffectViewSetting>().rawSet;
            int cd = Status_list_sample[i].GetComponent<StatusEffectViewSetting>().Turns;

            GameObject ff = (GameObject)Instantiate(status_prefab, status_list.transform);
            ff.GetComponent<IP_Status>().base_asset = status_ab;
            ff.GetComponent<IP_Status>().init(st_id, cd);
            status_icon_tmp.Add(ff);
            this.WaitForDestory.Add(ff);

            GameObject fv = (GameObject)Instantiate(
                Status_Info_Prefab, ScrollParent.transform);
            fv.GetComponent<IP_StatusBox>().base_asset = status_ab;
            fv.GetComponent<IP_StatusBox>().init(st_id);
            this.WaitForDestory.Add(fv);
        }
        var hdiff = (RectTransform)status_prefab.transform;
        for (int k = 0; k < status_icon_tmp.Count; k++) {
            float xt = (float)Math.Round((float)(k / 6), 1);
            float yt = (float)(k % 6);

            status_icon_tmp[k].transform.position += new Vector3(
                (hdiff.rect.width + 2) * xt,
                (hdiff.rect.height + 2) * -yt,
                0
            );

        }

        // StartCoroutine();
        yield return true;
    }

    public IEnumerator InitSkipStatus(
        AssetBundle tar_asset,
        CardObject json, CardSet cs, List<SkillObject> sko,
        int cc_id, int level
    ) {
        // yield return null;
        card_face.CC_id = cc_id;
        card_face.level = level;
        StartCoroutine(card_face.InitCCImg(tar_asset, json, cs));
        StartCoroutine(card_face.InitCCLvFrame());
        StartCoroutine(card_face.InitEquSetting(0, 0));
        string name_string = "", desp_string = "";
        switch (sys_lang) {
            case SystemLanguage.Chinese:
            case SystemLanguage.ChineseTraditional:
                name_string = json.name.tcn;
                desp_string = json.caption.tcn;
                break;
            case SystemLanguage.ChineseSimplified:
                name_string = json.name.scn;
                desp_string = json.caption.scn;

                break;
            case SystemLanguage.Japanese:
                name_string = json.name.jp;
                desp_string = json.caption.jp;

                break;
            case SystemLanguage.Korean:
                name_string = json.name.kr;
                desp_string = json.caption.kr;
                break;
            case SystemLanguage.Indonesian:
                name_string = json.name.ina;
                desp_string = json.caption.ina;
                break;
            case SystemLanguage.Thai:
                name_string = json.name.thai;
                desp_string = json.caption.thai;
                break;
            case SystemLanguage.English:
            case SystemLanguage.Unknown:
            default:
                name_string = json.name.en;
                desp_string = json.caption.en;
                break;
        }
        if (name_string == "") {
            name_string = json.name.jp;
            desp_string = json.caption.jp;
        }

        CC_Title.text = name_string;
        CC_Desp.text = desp_string;

        // Skill Related 
        foreach (SkillObject t in sko) {
            GameObject ff = (GameObject)Instantiate(
                Skill_Info_Prefab, ScrollParent.transform);
            ff.GetComponent<IP_SkillBox>().init(t);
            this.WaitForDestory.Add(ff);
        }

        // StartCoroutine();
        yield return true;
    }
    public void Clean() {
        foreach (var go in this.WaitForDestory)
            Destroy(go);
    }

}