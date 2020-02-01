using System;
using System.Collections.Generic;
using TMPro;
using ULZAsset;
using UnityEngine;
using UnityEngine.UI;
public class IP_SkillBox : MonoBehaviour {
    public TextMeshProUGUI Title;
    public Text desp, phase_text;
    // 2 >> 1 >> 0 
    public List<RawImage> range_icon;
    public GameObject game_cond;
    public Color long_mid { get { return new Color (0f, 0.7f, 0f, 1f); } }
    public Color short_range { get { return new Color (0.7f, 0f, 0f, 1f); } }
    public void init (SkillObject skill_info) {
        string name_string = "", desp_string = "";
        switch (Application.systemLanguage) {
            case SystemLanguage.Chinese:
            case SystemLanguage.ChineseTraditional:
                name_string = skill_info.name.tcn;
                desp_string = skill_info.caption.tcn;
                break;
            case SystemLanguage.ChineseSimplified:
                name_string = skill_info.name.scn;
                desp_string = skill_info.caption.scn;
                break;
            case SystemLanguage.Japanese:
                name_string = skill_info.name.jp;
                desp_string = skill_info.caption.jp;

                break;
            case SystemLanguage.Korean:
                name_string = skill_info.name.kr;
                desp_string = skill_info.caption.kr;

                break;
            case SystemLanguage.Indonesian:
                name_string = skill_info.name.ina;
                desp_string = skill_info.caption.ina;

                break;
            case SystemLanguage.Thai:
                name_string = skill_info.name.thai;
                desp_string = skill_info.caption.thai;

                break;
            case SystemLanguage.English:
            case SystemLanguage.Unknown:
            default:
                name_string = skill_info.name.en;
                desp_string = skill_info.caption.en;

                break;
        }
        if (name_string == "") {
            name_string = skill_info.name.jp;
        }
        if (desp_string == "") {
            desp_string = skill_info.caption.jp;
        }
        Title.SetText (name_string);

        if (desp_string.Contains ("[移動:__CONDITION__]")) {
            phase_text.text = "MOV";
            phase_text.color = new Color (0.4f, 0f, 0.8f, 1f);
            desp_string = desp_string.Replace ("[移動:__CONDITION__]", "");
        }
        if (desp_string.Contains ("[攻撃:__CONDITION__]")) {
            phase_text.text = "MOV";
            phase_text.color = new Color (0.7f, 0f, 0f, 1f);
            desp_string = desp_string.Replace ("[攻撃:__CONDITION__]", "");

        }
        if (desp_string.Contains ("[防御:__CONDITION__]")) {
            phase_text.text = "MOV";
            phase_text.color = new Color (0f, 0f, 0.7f, 1f);
            desp_string = desp_string.Replace ("[防御:__CONDITION__]", "");
        }

        desp_string = desp_string.Replace ("__POW__", skill_info.pow.ToString ());
        desp_string = desp_string.Replace ("|", "\n");
        desp.text = desp_string;

        var range_ptn = skill_info.condition.Split (':');
        if (range_ptn[0].Contains ("L")) {
            range_icon[2].color = long_mid;
        }
        if (range_ptn[0].Contains ("M")) {
            range_icon[1].color = long_mid;
        }
        if (range_ptn[0].Contains ("S")) {
            range_icon[0].color = short_range;
        }

        var card_cond = range_ptn[1].Split (',');
        List<GameObject> cond_list = new List<GameObject> ();
        foreach (var cc_c in card_cond) {
            var cond = GameObject.Instantiate (game_cond);
            var tmp = cc_c;
            if (cc_c.Contains ("A")) {
                cond.GetComponent<RawImage> ().color = long_mid;
                tmp = cc_c.Replace ("A", "");
            } else if (cc_c.Contains ("S")) {
                cond.GetComponent<RawImage> ().color = short_range;
                tmp = cc_c.Replace ("S", "");
            } else if (cc_c.Contains ("M")) {
                cond.GetComponent<RawImage> ().color = new Color (0.4f, 0f, 0.8f, 1f);
                tmp = cc_c.Replace ("M", "");
            } else if (cc_c.Contains ("E")) {
                cond.GetComponent<RawImage> ().color = new Color (0.93f, 0.76f, 0.16f, 1f);
                tmp = cc_c.Replace ("E", "");
            }
            if (tmp.Contains ("+")) {
                cond.transform.Find ("Text").GetComponent<Text> ().text =
                    tmp;
            } else if (tmp.Contains ("*")) {
                var tmpa = tmp.Split ('*');
                cond.transform.Find ("Text").GetComponent<Text> ().text =
                    tmpa[0] + "=";
                for (int y = 1; y < Int32.Parse (tmpa[1]); y++) {
                    var con_clo = GameObject.Instantiate (cond);
                    cond_list.Add (con_clo);
                }
            }
            cond_list.Add (cond);
        }
        for (int i = 0; i < cond_list.Count; i++) {
            cond_list[i].transform.position += new Vector3 (
                55f * i, 0, 0
            );
        }
        GameObject.Destroy(game_cond);
    }

}