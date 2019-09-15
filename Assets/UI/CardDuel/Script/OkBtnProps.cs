using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OkBtnProps : MonoBehaviour {
    public bool isAble = true;

    void Update () {
        if (isAble) {
            transform.Find ("able").gameObject.SetActive (true);
        } else {
            transform.Find ("able").gameObject.SetActive (false);
        }
    }
}