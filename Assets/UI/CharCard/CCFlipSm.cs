using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CCFlipSm : MonoBehaviour,  IPointerClickHandler {
    public void OnPointerClick (PointerEventData eventData) {
        GameObject p = transform.parent.parent.gameObject;
        p.GetComponent<CCProps> ().is_flipover = !p.GetComponent<CCProps> ().is_flipover;
    }

    //NOTE: Timefunc for "FadeIn" "FadeOut" 
}