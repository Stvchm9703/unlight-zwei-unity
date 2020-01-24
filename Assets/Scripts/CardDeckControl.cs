﻿using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CardDeckControl : MonoBehaviour {
    public GameObject CardPrefab;

    // public ActionCardCtl ACTest;
    public int cardNumber;

    public List<GameObject> Cards;

    public GameObject InsideArea, OutsideArea, DestroyArea;
    public Text CardRemainDisplay;
    public void GenerateCardRun () {
        StartCoroutine (GenerateCard ());
    }
    public IEnumerator GenerateCard () {
        int y = 0;
        while (y < 150) {
            GameObject ff = (GameObject) Instantiate (CardPrefab, new Vector3 (0, 0, 0), Quaternion.identity);
            ff.name = "AC" + y;
            ff.transform.SetParent (this.transform);
            ff.transform.position = new Vector3 (-10000, -10000, -10000);

            var t = InsideArea.transform.localPosition;
            t.y += InsideArea.GetComponent<RectTransform> ().rect.height / 2;
            ff.GetComponent<ActionCardCtl> ().InsidePos = t;

            t = OutsideArea.transform.localPosition;
            ff.GetComponent<ActionCardCtl> ().OutsidePos = t;

            t = DestroyArea.transform.localPosition;
            ff.GetComponent<ActionCardCtl> ().DestroyPos = t;

            yield return ff.GetComponent<ActionCardCtl> ().SetToInit (y + 1);
            y++;
            cardNumber++;
            Cards.Add (ff);
        }
        if (CardRemainDisplay != null) {
            CardRemainDisplay.text = (cardNumber).ToString ();
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
        if (CardRemainDisplay != null) {
            CardRemainDisplay.text = (cardNumber).ToString ();
        }
        yield return UpdateInsidePosition ();
    }

    public void FlipOpen () {
        StartCoroutine (FlipInsideCard ());
    }
    public IEnumerator FlipCard () {
        GameObject p = this.transform.Find ("AC149").gameObject;
        yield return p.GetComponent<ActionCardCtl> ().FlipCard ();
        // yield return ACTest.FlipCard();
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
        float cardlistwidth = 66 * (InCd.Count - 1);
        for (int f = 0; f < InCd.Count; f++) {
            var yy = InCd[f].GetComponent<ActionCardCtl> ().InsidePos;
            yy.x = InsideArea.transform.localPosition.x - cardlistwidth / 2 + f * 66;
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

    public IEnumerator FlipInsideCard () {
        foreach (var p in Cards) {
            if (p.GetComponent<ActionCardCtl> ().Pos == type_pos.inside) {
                StartCoroutine (p.GetComponent<ActionCardCtl> ().FlipCard ());
            }
        }
        yield return true;
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
[CustomEditor (typeof (CardDeckControl))]
public class CardDeckControlEditor : Editor {
    public override void OnInspectorGUI () {
        DrawDefaultInspector ();
        CardDeckControl d = (CardDeckControl) target;
        if (GUILayout.Button ("Generate Card Test")) {
            d.GenerateCardRun ();
        }

        if (GUILayout.Button ("Draw Card Test")) {
            // Debug.Log (ACTest);
            d.DrawCardTest ();
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