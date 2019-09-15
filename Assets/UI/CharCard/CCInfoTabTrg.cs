using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class CCInfoTabTrg : MonoBehaviour, IPointerClickHandler {
    // Start is called before the first frame update
    public cctab_page TabPage;
    public void OnPointerClick (PointerEventData eventData) {
        transform.parent.gameObject.GetComponent<CCInfoTab> ().TabPage = this.TabPage;
    }
}