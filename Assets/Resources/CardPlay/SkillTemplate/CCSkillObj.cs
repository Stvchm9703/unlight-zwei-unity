using System.Collections;
using System.Collections.Generic;
using ULZAsset;
using UnityEngine;
public class CCSkillObj : MonoBehaviour {
    public int id, feat_no;
    public bool is_ex;
    public void import_info (SkillObject tmp) {
        this.id = tmp.id;
        this.feat_no = tmp.feat_no;
        this.is_ex = (tmp.name.jp).Contains ("EX");
    }
}