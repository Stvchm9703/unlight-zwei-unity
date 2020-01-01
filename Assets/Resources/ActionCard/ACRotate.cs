using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ACRotate : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
    public GameObject ACParent;
    private void Start () {
        this.ACParent = this.transform.parent.parent.gameObject;
    }
    public void OnPointerClick (PointerEventData eventData) {
        this.ACParent.GetComponent<ActionCardCtl> ().CardRotate ();
    }
    public void OnPointerEnter (PointerEventData eventData) {
        this.gameObject.GetComponent<RawImage> ().DOFade (1, 0.1f);
    }
    public void OnPointerExit (PointerEventData eventData) {
        this.gameObject.GetComponent<RawImage> ().DOFade (0, 0.1f);
    }
}