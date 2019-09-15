using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EvntCdUpOut : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
    public float Rotate_Hide_Alp = 0f;
    public float Rotate_Show_Alp = 255f;
    public float Current_Alpha = 0f;

    public void OnPointerEnter (PointerEventData eventData) {
        GameObject pgo = transform.parent.gameObject;
        bool isInvert = false;
        bool isOutSide = false;
        if (pgo.GetComponent<EvntCardProps> () != null) {
            isInvert = pgo.GetComponent<EvntCardProps> ().isInvert;
            isOutSide = pgo.GetComponent<EvntCardProps> ().isOutSide;
        } else if (pgo.GetComponent<SpEvtCardProps> () != null) {
            isInvert = pgo.GetComponent<SpEvtCardProps> ().isInvert;
            isOutSide = pgo.GetComponent<SpEvtCardProps> ().isOutSide;
        }
        if (!isInvert && !isOutSide) {
            GetComponent<RawImage> ().color = new Color (255f, 255f, 255f, Rotate_Show_Alp);
        } else if (isInvert && isOutSide) {
            GetComponent<RawImage> ().color = new Color (255f, 255f, 255f, Rotate_Show_Alp);
        }
    }

    public void OnPointerExit (PointerEventData eventData) {
        GetComponent<RawImage> ().color = new Color (255f, 255f, 255f, Rotate_Hide_Alp);
    }

    public void OnPointerClick (PointerEventData eventData) {
        GameObject pgo = transform.parent.gameObject;
        bool isInvert = false;
        bool isOutSide = false;
        if (pgo.GetComponent<EvntCardProps> () != null) {
            isInvert = pgo.GetComponent<EvntCardProps> ().isInvert;
            isOutSide = pgo.GetComponent<EvntCardProps> ().isOutSide;
        } else if (pgo.GetComponent<SpEvtCardProps> () != null) {
            isInvert = pgo.GetComponent<SpEvtCardProps> ().isInvert;
            isOutSide = pgo.GetComponent<SpEvtCardProps> ().isOutSide;
        }
        if (!isInvert && !isOutSide) {
            // p.CardOut ();
        } else if (isInvert && isOutSide) {
            // p.CardBack ();
        }
    }
    //NOTE: Timefunc for "FadeIn" "FadeOut" 
}