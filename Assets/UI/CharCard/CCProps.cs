using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CCProps : MonoBehaviour {
    public string Card_Id;
    public string Card_Name;
    public string resx_path;
    public int Card_Lv;
    public int Hp_val;
    public int Atk_val;
    public int Def_val;

    public int atk_adj_val;
    public int def_adj_val;

    public List<CC_Skill> SkillList;

    // Buff / Debuff
    public List<Status> Status;
    public Animator anim;

    private bool m_isflipover = false;
    public bool is_flipover {
        get { return m_isflipover; }
        set {
            m_isflipover = value;
            if (anim) {
                anim.SetBool ("is_fliped", m_isflipover);
            }
        }
    }
    void Awake () {
        anim = GetComponent<Animator> ();
    }
    void Start () { }

}