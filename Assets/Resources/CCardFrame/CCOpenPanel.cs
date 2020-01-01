using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class CCOpenPanel : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
    public CCardCtl main;
    private void Start () {
        if (!main) {
            main = this.transform.parent.parent.gameObject.GetComponent<CCardCtl> ();
        }
    }
    public void OnPointerClick (PointerEventData eventData) {
        main.OpenCCInfoPanel();
    }
    public void OnPointerEnter (PointerEventData eventData) {
        this.gameObject.GetComponent<RawImage> ().DOFade (1, 0.1f);
    }
    public void OnPointerExit (PointerEventData eventData) {
        this.gameObject.GetComponent<RawImage> ().DOFade (0, 0.1f);
    }
}