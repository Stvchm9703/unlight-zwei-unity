using System.Collections;
using System.Collections.Generic;
using TMPro;
using ULZAsset;
using UnityEngine;
using UnityEngine.UI;
public class IP_StatusBox : MonoBehaviour {
    public RawImage icon;
    public TextMeshProUGUI Title;
    public Text desp;
    public StatusEffectMainViewCtl base_asset;
    private void Start() {
        if (base_asset == null) {
            base_asset = this.transform.root.parent.Find("EventSystem").GetComponent<StatusEffectMainViewCtl>();
        }

    }
    public void init(StatusObject so) {
        string name_string = "", desp_string = "";
        switch (Application.systemLanguage) {
            case SystemLanguage.Chinese:
            case SystemLanguage.ChineseTraditional:
                name_string = so.name.tcn;
                desp_string = so.caption.tcn;
                break;
            case SystemLanguage.ChineseSimplified:
                name_string = so.name.scn;
                desp_string = so.caption.scn;
                break;
            case SystemLanguage.Japanese:
                name_string = so.name.jp;
                desp_string = so.caption.jp;

                break;
            case SystemLanguage.Korean:
                name_string = so.name.kr;
                desp_string = so.caption.kr;

                break;
            case SystemLanguage.Indonesian:
                name_string = so.name.ina;
                desp_string = so.caption.ina;

                break;
            case SystemLanguage.Thai:
                name_string = so.name.thai;
                desp_string = so.caption.thai;

                break;
            case SystemLanguage.English:
            case SystemLanguage.Unknown:
            default:
                name_string = so.name.en;
                desp_string = so.caption.en;

                break;
        }
        if (name_string == "") {
            name_string = so.name.jp;
        }
        if (desp_string == "") {
            desp_string = so.caption.jp;
        }
        Title.SetText(name_string);
        desp.text = desp_string;

        var t2d = base_asset.MainEffectAB.LoadAsset(so.img)as Texture2D;
        icon.texture = t2d;

    }
}