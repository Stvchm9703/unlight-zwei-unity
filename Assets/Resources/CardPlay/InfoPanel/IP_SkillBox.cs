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
    public Color long_mid { get { return new Color(0f, 0.7f, 0f, 1f); } }
    public Color short_range { get { return new Color(0.7f, 0f, 0f, 1f); } }
    public void init(SkillObject skill_info) {
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
        Title.SetText(name_string);

        if (desp_string.Contains("[移動:__CONDITION__]")) {
            phase_text.text = "MOV";
            phase_text.color = new Color(0.4f, 0f, 0.8f, 1f);
            desp_string = desp_string.Replace("[移動:__CONDITION__]", "");
        } else if (desp_string.Contains("[攻撃:__CONDITION__]")) {
            phase_text.text = "ATK";
            phase_text.color = new Color(0.7f, 0f, 0f, 1f);
            desp_string = desp_string.Replace("[攻撃:__CONDITION__]", "");

        } else if (desp_string.Contains("[防御:__CONDITION__]")) {
            phase_text.text = "DEF";
            phase_text.color = new Color(0f, 0f, 0.7f, 1f);
            desp_string = desp_string.Replace("[防御:__CONDITION__]", "");
        }

        desp_string = desp_string.Replace("__POW__", skill_info.pow.ToString());
        desp_string = desp_string.Replace("|", "\n");
        desp.text = desp_string;

        var range_ptn = skill_info.condition.Split(':');
        if (range_ptn[0].Contains("L")) {
            range_icon[2].color = long_mid;
        } else if (range_ptn[0].Contains("M")) {
            range_icon[1].color = long_mid;
        } else if (range_ptn[0].Contains("S")) {
            range_icon[0].color = short_range;
        }

        var card_cond = range_ptn[1].Split(',');
        List<GameObject> cond_list = new List<GameObject>();
        for (int i = 0; i < card_cond.Length; i++) {
            GameObject cond_init = (GameObject)Instantiate(
                this.game_cond,
                this.transform.Find("condition"));
            var tmp = card_cond[i];
            if (card_cond[i].Contains("A")) {
                cond_init.GetComponent<Image>().color = long_mid;
                tmp = card_cond[i].Replace("A", "");
            } else if (card_cond[i].Contains("S")) {
                cond_init.GetComponent<Image>().color = short_range;
                tmp = card_cond[i].Replace("S", "");
            } else if (card_cond[i].Contains("M")) {
                cond_init.GetComponent<Image>().color = new Color(0.4f, 0f, 0.8f, 1f);
                tmp = card_cond[i].Replace("M", "");
            } else if (card_cond[i].Contains("E")) {
                cond_init.GetComponent<Image>().color = new Color(0.93f, 0.76f, 0.16f, 1f);
                tmp = card_cond[i].Replace("E", "");
            } else if (card_cond[i].Contains("D")) {
                cond_init.GetComponent<Image>().color = new Color(0f, 0f, 0.7f, 1f);
                tmp = card_cond[i].Replace("D", "");
            }
            if (tmp.Contains("+")) {
                cond_init.transform.Find("Text").GetComponent<Text>().text =
                    tmp;
            } else if (tmp.Contains("*")) {
                var tmpa = tmp.Split('*');
                cond_init.transform.Find("Text").GetComponent<Text>().text =
                    tmpa[0] + "=";
                for (int y = 1; y < Int32.Parse(tmpa[1]); y++) {
                    GameObject con_clo = (GameObject)Instantiate(
                        cond_init, 
                        this.transform.Find("condition"));
                    cond_list.Add(con_clo);
                }
            }
            cond_list.Add(cond_init);
        }
        for (int i = 0; i < cond_list.Count; i++) {
            cond_list[i].transform.position += new Vector3(
                55f * i, 0, 0
            );
        }
    }

}