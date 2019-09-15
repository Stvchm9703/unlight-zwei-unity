using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setting_btn : MonoBehaviour {
    // on_clicked event

    void OnClick () {
        Debug.Log ("Setting called");
        GameObject.Find("Menu").GetComponent<menu_detail_bhvr>().SettingBtnClick();
    }
}