using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ACRotate : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
    public GameObject ACParent;
    public RawImage icon;
    private void Start () {
        if (this.ACParent == null)
            this.ACParent = this.transform.parent.parent.gameObject;
        if (this.icon == null)
            this.icon = this.transform.Find ("icon").gameObject.GetComponent<RawImage> ();
    }
    public void OnPointerClick (PointerEventData eventData) {
        this.ACParent.GetComponent<ActionCardCtl> ().CardRotate ();
    }
    public void OnPointerEnter (PointerEventData eventData) {
        icon.color = new Color (1, 1, 1, 1);
    }
    public void OnPointerExit (PointerEventData eventData) {
        icon.color = new Color (1, 1, 1, 0);
    }
}