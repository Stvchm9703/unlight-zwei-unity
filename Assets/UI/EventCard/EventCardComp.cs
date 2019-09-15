using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventCardComp : MonoBehaviour {

    public List<GameObject> CardBlock;
    public List<GameObject> outsideCard;
    public List<GameObject> insideCard;

    void Start () {

    }
    void Update () { }
    public void PullOutCard (GameObject Obj) {
        Debug.Log (Obj);
        
    }
    public void PushInCard (GameObject Obj) {
        Debug.Log (Obj);
    }
    public void AssignCard () { }
}