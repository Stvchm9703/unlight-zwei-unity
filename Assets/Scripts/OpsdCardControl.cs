using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

public class OpsdCardControl : MonoBehaviour {
    public GameObject CardPrefab;

    // public ActionCardCtl ACTest;
    public int cardNumber;

    public List<GameObject> Cards;

    public GameObject InsideArea;
    public GameObject OutsideArea;
    public GameObject DestroyArea;
    public void GenerateCardTestRun () {
        StartCoroutine (GenerateCard ());
    }

    // public IEnumerator GenCardFromDt(List<ACard> CardList) {

    // }

    public IEnumerator GenerateCard () {
        int y = 0;
        while (y < 150) {
            GameObject ff = (GameObject) Instantiate (CardPrefab, new Vector3 (0, 0, 0), Quaternion.identity);
            ff.name = "AC" + y;
            ff.transform.SetParent (this.transform);
            ff.transform.position = new Vector3 (-10000, -10000, -10000);

            var t = InsideArea.transform.localPosition;
            ff.GetComponent<ActionCardCtl> ().InsidePos = t;
            t = OutsideArea.transform.localPosition;
            ff.GetComponent<ActionCardCtl> ().OutsidePos = t;
            t = DestroyArea.transform.localPosition;
            ff.GetComponent<ActionCardCtl> ().DestroyPos = t;

            var block = this.transform.parent.Find ("OpDeckBlock");
            yield return ff.GetComponent<ActionCardCtl> ().SetToOpInit (y + 1, block.localPosition, block.localRotation);
            y++;
            cardNumber++;
            Cards.Add (ff);
        }
        yield return (true);
    }
    public void DrawCardTest () {
        StartCoroutine (DrwCard ());
    }
    public IEnumerator DrwCard () {
        cardNumber--;
        yield return Cards[cardNumber].GetComponent<ActionCardCtl> ().MoveToHand ();
        yield return new WaitForSeconds (0.3f);
        yield return UpdateInsidePosition ();
    }

    public void FlipOpen () {
        StartCoroutine (FlipOutsideCard ());
    }

    public void CardOutTrigger () {
        StartCoroutine (UpdateOutsidePosition ());
        StartCoroutine (UpdateInsidePosition ());
    }
    public IEnumerator UpdateInsidePosition () {
        List<GameObject> InCd = new List<GameObject> ();
        foreach (var p in Cards) {
            if (p.GetComponent<ActionCardCtl> ().Pos == type_pos.inside) {
                InCd.Add (p);
            }
        }
        float insideX = InsideArea.GetComponent<RectTransform> ().rect.width;
        float cardlistwidth = 20 * (InCd.Count - 1);
        for (int f = 0; f < InCd.Count; f++) {
            var yy = InCd[f].GetComponent<ActionCardCtl> ().InsidePos;
            yy.x = InsideArea.transform.localPosition.x + insideX / 2 - f * 22 - 33;
            InCd[f].GetComponent<ActionCardCtl> ().InsidePos = yy;
            yield return f;
        }
        yield return true;
    }

    public IEnumerator UpdateOutsidePosition () {
        List<GameObject> InCd = new List<GameObject> ();
        foreach (var p in Cards) {
            if (p.GetComponent<ActionCardCtl> ().Pos == type_pos.outside) {
                InCd.Add (p);
            }
        }
        float insideX = OutsideArea.GetComponent<RectTransform> ().rect.width;
        float cardlistwidth = 66 * (InCd.Count - 1);
        if (66 * (InCd.Count) > insideX) {
            for (int f = 0; f < InCd.Count; f++) {
                float padd = insideX / (InCd.Count + 1);
                var yy = InCd[f].GetComponent<ActionCardCtl> ().OutsidePos;
                yy.x = OutsideArea.transform.localPosition.x - insideX / 2 + f * padd + 20;
                InCd[f].GetComponent<ActionCardCtl> ().OutsidePos = yy;
                yield return f;
            }
        } else {
            for (int f = 0; f < InCd.Count; f++) {
                var yy = InCd[f].GetComponent<ActionCardCtl> ().OutsidePos;
                yy.x = OutsideArea.transform.localPosition.x - cardlistwidth / 2 + f * 66;
                InCd[f].GetComponent<ActionCardCtl> ().OutsidePos = yy;
                yield return f;
            }
        }
        yield return true;
    }

    public IEnumerator FlipOutsideCard () {
        foreach (var p in Cards) {
            if (p.GetComponent<ActionCardCtl> ().Pos == type_pos.outside) {
                StartCoroutine (p.GetComponent<ActionCardCtl> ().FlipCard ());
            }
        }
        yield return true;
    }

    public void RandomPush () {
        foreach (var cd in Cards) {
            if (cd.GetComponent<ActionCardCtl> ().Pos == type_pos.inside) {
                cd.GetComponent<ActionCardCtl> ().CardOut ();
            }
        }
    }

    public void SendToDestroy () {
        StartCoroutine (CdToDestroy ());
    }
    public IEnumerator CdToDestroy () {
        foreach (var cd in Cards) {
            if (cd.GetComponent<ActionCardCtl> ().Pos == type_pos.outside) {
                StartCoroutine (cd.GetComponent<ActionCardCtl> ().MoveToDestroy ());
            }
        }
        yield return true;
    }

}


#if (UNITY_EDITOR) 
[CustomEditor (typeof (OpsdCardControl))]
public class OpsdCardControlEditor : Editor {
    public override void OnInspectorGUI () {
        DrawDefaultInspector ();
        OpsdCardControl d = (OpsdCardControl) target;
        if (GUILayout.Button ("Generate Card Test")) {
            d.GenerateCardTestRun ();
        }

        if (GUILayout.Button ("Draw Card Test")) {
            d.DrawCardTest ();
        }

        if (GUILayout.Button ("Random Push out")) {
            d.RandomPush ();
        }

        if (GUILayout.Button ("Flip Open !")) {
            d.FlipOpen ();
        }

        if (GUILayout.Button ("Send To Destroy")) {
            d.SendToDestroy ();
        }
    }
}

#endif