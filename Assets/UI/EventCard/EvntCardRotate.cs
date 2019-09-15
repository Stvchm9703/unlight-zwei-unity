using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EvntCardRotate : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
    public float Rotate_Hide_Alp = 0f;
    public float Rotate_Show_Alp = 255f;
    public float Current_Alpha = 0f;

    public void OnPointerEnter (PointerEventData eventData) {
        GetComponent<RawImage> ().color = new Color (255f, 255f, 255f, Rotate_Show_Alp);
    }

    public void OnPointerExit (PointerEventData eventData) {
        GetComponent<RawImage> ().color = new Color (255f, 255f, 255f, Rotate_Hide_Alp);
    }

    public void OnPointerClick (PointerEventData eventData) {
        GameObject p = transform.parent.gameObject;
        
        if (p.GetComponent<EvntCardProps> () != null) {
            p.GetComponent<EvntCardProps> ().RotateCard ();

        } else if (p.GetComponent<SpEvtCardProps> () != null) {
            p.GetComponent<SpEvtCardProps> ().RotateCard ();
        }
    }

    //NOTE: Timefunc for "FadeIn" "FadeOut" 
}