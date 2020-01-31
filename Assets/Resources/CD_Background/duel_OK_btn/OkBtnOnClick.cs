using UnityEngine;

public class OkBtnOnClick : MonoBehaviour {
    public OKBtnCtl main_ctl;
    private void Start () {
        if (main_ctl == null) {
            main_ctl = this.transform.parent.GetComponent<OKBtnCtl>();
        }
    }
    private void OnMouseDown() {
        main_ctl.OKOnClick();
    }
}