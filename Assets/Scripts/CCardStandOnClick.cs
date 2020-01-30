using UnityEngine;

public class CCardStandOnClick : MonoBehaviour {
    public CCardStandCtl mainCtl;
    private void Start() {
        if(mainCtl == null){
            mainCtl = this.transform.parent.parent.gameObject.GetComponent<CCardStandCtl>();
        }
    }
    private void OnMouseDown () {
        mainCtl.ClickTrig();
    }
}