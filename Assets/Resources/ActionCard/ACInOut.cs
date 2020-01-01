using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ACInOut : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
    public ActionCardCtl ACParent;
    private void Start () {
        this.ACParent = this.transform.parent.parent.gameObject.GetComponent<ActionCardCtl> ();
    }
    public void OnPointerClick (PointerEventData eventData) {
        if (ACParent.isOutSide && ACParent.isInvert) {
            ACParent.CardBack ();
        } else if (!ACParent.isOutSide && !ACParent.isInvert) {
            ACParent.CardOut ();
        }
    }
    public void OnPointerEnter (PointerEventData eventData) {
        if (
            (ACParent.isOutSide && ACParent.isInvert) ||
            (!ACParent.isOutSide && !ACParent.isInvert)
        ) {
            this.gameObject.GetComponent<RawImage> ().DOFade (1, 0.1f);
        }
    }
    public void OnPointerExit (PointerEventData eventData) {
        this.gameObject.GetComponent<RawImage> ().DOFade (0, 0.1f);
    }
}