using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using ULZAsset;
using UnityEngine;
using UnityEngine.UI;
public class CCardCtl : MonoBehaviour {

    public int CC_id = 0, Equ_id = -1;
    public int level;
    public int HP_defaut, ATK_defaut, DEF_defaut;
    public int ATK_equ, DEF_equ;

    public bool is_hidden;

    /// <summary>
    ///  int *_instant : the instant value; 
    /// </summary>
    private int HP_instant, ATK_instant, DEF_instant;

    /// <summary>
    /// private Color : colour setting  
    /// </summary>
    private Color _defaut_color { get { return new Color (1, 1, 1, 1); } }
    private Color _damage_color { get { return new Color (1, 1, (float) (54 / 255), 1); } }
    private Color _dead_color { get { return new Color (1, (float) (70 / 255), (float) (70 / 255), 1); } }

    private Color _equ_color_a { get { return new Color ((float) (86 / 255), 1f, (float) (81 / 255), 1); } }
    private Color _equ_color_b { get { return new Color (0, 0, 0, 1); } }

    /// <summary>
    ///  HP / ATK / DEF : the instant value of the CC
    /// </summary>
    public int HP {
        get { return this.HP_instant; }
        set {
            if (value > this.HP_defaut) {
                this.HP = this.HP_defaut;
            } else if (value < 0) {
                this.HP_instant = 0;
            } else {
                this.HP_instant = value;
            }

            if (value == this.HP_defaut) {
                //  white
                if (this.HPVal != null) {
                    this.HPVal.GetComponent<Text> ().color = _defaut_color;
                }
            } else if (value > (int) (this.HP_defaut / 2) && this.HP_defaut > value) {
                // yellow
                if (this.HPVal != null) {
                    this.HPVal.GetComponent<Text> ().color = _damage_color;
                }
            } else {
                // Red
                if (this.HPVal != null) {
                    this.HPVal.GetComponent<Text> ().color = _damage_color;
                }
            }
            this.HPVal.GetComponent<Text> ().text = value.ToString ();
        }
    }
    public int ATK {
        get { return this.ATK_instant; }
        set {
            if (value > this.ATK_defaut) {
                this.ATK_instant = this.ATK_defaut;
            } else if (value < 0) {
                this.ATK_instant = 0;
            } else {
                this.ATK_instant = value;
            }

            if (value == this.ATK_defaut) {
                //  white
                if (this.ATKVal != null) {
                    this.ATKVal.GetComponent<Text> ().color = _defaut_color;
                }
            } else if (value > (int) (this.ATK_defaut / 2) && this.ATK_defaut > value) {
                // yellow
                if (this.ATKVal != null) {
                    this.ATKVal.GetComponent<Text> ().color = _damage_color;
                }
            } else {
                // Red
                if (this.ATKVal != null) {
                    this.ATKVal.GetComponent<Text> ().color = _damage_color;
                }
            }
            this.ATKVal.GetComponent<Text> ().text = value.ToString ();
        }
    }
    public int DEF {
        get { return this.DEF_instant; }
        set {
            if (value > this.DEF_defaut) {
                this.DEF_instant = this.DEF_defaut;
            } else if (value < 0) {
                this.DEF_instant = 0;
            } else {
                this.DEF_instant = value;
            }

            if (value == this.DEF_defaut) {
                //  white
                if (this.DEFVal != null) {
                    this.DEFVal.GetComponent<Text> ().color = _defaut_color;
                }
            } else if (value > (int) (this.DEF_defaut / 2) && this.DEF_defaut > value) {
                // yellow
                if (this.DEFVal != null) {
                    this.DEFVal.GetComponent<Text> ().color = _damage_color;
                }
            } else {
                // Red
                if (this.DEFVal != null) {
                    this.DEFVal.GetComponent<Text> ().color = _damage_color;
                }
            }
            this.DEFVal.GetComponent<Text> ().text = value.ToString ();
        }
    }

    /// <summary>
    /// GameObject *Val : the GameObject Pointer for displaying 
    /// </summary>
    public GameObject HPVal, ATKVal, DEFVal;

    /// <summary>
    /// GameObject *Equ : the GameObject Pointer for display equiment val 
    /// </summary>
    public GameObject ATKEqu, DEFEqu;

    public SystemLanguage SL_setting;
    public void InitImageResx () { }
    public IEnumerator InitEquSetting (int atk, int def) {
        yield return null;
        this.ATK_equ = atk;
        if (atk > 0) {
            this.ATKEqu.GetComponent<Text> ().text = "+" + atk.ToString ();
            this.ATKEqu.GetComponent<Text> ().color = this._equ_color_a;
            this.ATKEqu.GetComponent<Outline> ().effectColor = this._equ_color_b;
        } else if (atk < 0) {
            this.ATKEqu.GetComponent<Text> ().text = atk.ToString ();
            this.ATKEqu.GetComponent<Text> ().color = this._equ_color_b;
            this.ATKEqu.GetComponent<Outline> ().effectColor = this._equ_color_a;
        } else {
            this.ATKEqu.GetComponent<Text> ().text = "";
        }
        this.DEF_equ = def;
        if (def > 0) {
            this.DEFEqu.GetComponent<Text> ().text = "+" + def.ToString ();
            this.DEFEqu.GetComponent<Text> ().color = this._equ_color_a;
            this.DEFEqu.GetComponent<Outline> ().effectColor = this._equ_color_b;

        } else if (def < 0) {
            this.DEFEqu.GetComponent<Text> ().text = def.ToString ();
            this.DEFEqu.GetComponent<Text> ().color = this._equ_color_b;
            this.DEFEqu.GetComponent<Outline> ().effectColor = this._equ_color_a;
        } else {
            this.DEFEqu.GetComponent<Text> ().text = "";
        }
        yield return true;
    }

    public IEnumerator InitCCLvFrame () {
        if (this.transform.Find ("CardFace/frame/lvtitle/num").gameObject) {
            this.transform.Find ("CardFace/frame/lvtitle/num").gameObject.GetComponent<TextMeshProUGUI> ().SetText (
                this.level.ToString ());
        } else {
            yield return false;
        }

        if (this.transform.Find ("CardFace/frame/lv").gameObject) {
            this.transform.Find ("CardFace/frame/lv").gameObject.GetComponent<RawImage> ().texture =
                Resources.Load<Texture2D> ("CCardFrame/Image/lv" + this.level.ToString ()) as Texture2D;
        } else {
            yield return false;
        }

        yield return true;
    }
    public IEnumerator InitCCImg (AssetBundle ab) {
        if (ab) {
            TextAsset ta = ab.LoadAsset ("card_set.json") as TextAsset;
            var json = JsonConvert.DeserializeObject<ULZAsset.CardObject> (ta.text);
            CardSet c_set = new CardSet ();
            string coverImgNm = "";
            for (int i = 0; i < json.card_set.Count; i++) {
                if (json.card_set[i].level == this.level) {
                    c_set = json.card_set[i];
                    coverImgNm = json.card_set[i].chara_image.Replace (".png", "");
                    break;
                }
            }

            Texture2D tas = ab.LoadAsset (coverImgNm) as Texture2D;

            this.transform.Find ("CardFace/char").gameObject.GetComponent<RawImage> ().texture = tas;

            string name_string = "";
            switch (SL_setting) {
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseTraditional:
                    name_string = json.name.tcn;
                    break;
                case SystemLanguage.ChineseSimplified:
                    name_string = json.name.scn;
                    break;
                case SystemLanguage.Japanese:
                    name_string = json.name.jp;
                    break;
                case SystemLanguage.Korean:
                    name_string = json.name.kr;
                    break;
                case SystemLanguage.Indonesian:
                    name_string = json.name.ina;
                    break;
                case SystemLanguage.Thai:
                    name_string = json.name.thai;
                    break;
                case SystemLanguage.English:
                case SystemLanguage.Unknown:
                default:
                    name_string = json.name.en;
                    break;
            }
            if (name_string == "") {
                name_string = json.name.en;
            }

            Debug.Log (c_set);

            if (this.transform.Find ("CardFace/frame/name/val").gameObject) {
                this.transform.Find ("CardFace/frame/name/val").gameObject.GetComponent<Text> ().text = name_string;
                this.HP_defaut = c_set.hp;
                this.HP = c_set.hp;
                this.ATK_defaut = c_set.ap;
                this.ATK = c_set.ap;
                this.DEF_defaut = c_set.dp;
                this.DEF = c_set.dp;
                yield return true;
            } else {
                yield return false;
            }
            // if (this.transform.Find())

        } else {
            yield return false;
        }
    }
    public void OpenCCInfoPanel () {

    }
    void Start () {
        if (this.HPVal == null) {
            this.HPVal = this.transform.Find ("CardFace/frame/value/HP").gameObject;
        }
        if (this.ATKVal == null) {
            this.ATKVal = this.transform.Find ("CardFace/frame/value/ATK").gameObject;
        }
        if (this.DEFVal == null) {
            this.DEFVal = this.transform.Find ("CardFace/frame/value/DEF").gameObject;
        }
        if (this.ATKEqu == null) {
            this.ATKEqu = this.transform.Find ("CardFace/frame/value/ATKplus").gameObject;
        }
        if (this.DEFEqu == null) {
            this.DEFEqu = this.transform.Find ("CardFace/frame/value/DEFplus").gameObject;
        }
    }

    void Update () {

    }
}