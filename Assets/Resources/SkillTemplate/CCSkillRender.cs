using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CCSkillRender: MonoBehaviour {
    public List<GameObject> Skill_preload;
    public GameObject Backgroud;
    private Material k;
    public Color EX_Color { get { return new Color(1f, 1f, 0f, 1f); } }
    public Color Nor_Color { get { return new Color(1f,0f,0f,1f); } }
    public float default_height_limit = 500f;
    void Start(){
        if (Backgroud == null){
            Backgroud  = this.transform.Find("background").gameObject;
        }
        k = Backgroud.GetComponent<Image>().material;
        default_height_limit =  Backgroud.GetComponent<RectTransform>().rect.height;
    }
    public IEnumerator LoadCCImg(AssetBundle abs , int level) {
        
        yield return true;
    }
}
