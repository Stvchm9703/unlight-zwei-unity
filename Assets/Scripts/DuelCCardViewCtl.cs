using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
public class DuelCCardViewCtl : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
    public Transform outside;
    public Transform inside;
    public void OnPointerClick (PointerEventData eventData) {
        this.gameObject.GetComponent<CCardCtl> ().OpenCCInfoPanel ();
    }
    public void OnPointerEnter (PointerEventData eventData) {
        this.transform.DOMoveY (outside.position.y, 0.1f);
    }
    public void OnPointerExit (PointerEventData eventData) {
        this.transform.DOMoveY (inside.position.y, 0.1f);
    }
}