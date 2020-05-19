using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DG.Tweening;
using TMPro;
using ULZAsset.ProtoMod.GameDuelService;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
public class CardDeckControl : MonoBehaviour {
    public GameObject CardPrefab;

    // public ActionCardCtl ACTest;
    public int cardNumber;

    public List<ActionCardCtl> Cards;

    public TextMeshPro ValDisp;

    public GameObject InsideArea, OutsideArea, DestroyArea;
    public Text CardRemainDisplay;

    public void GenerateCardRun() {
        StartCoroutine(GenerateCard());
    }
    public IEnumerator GenerateCard() {
        int y = 0;
        while (y < 150) {
            GameObject ff = (GameObject)Instantiate(CardPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            ff.name = "AC" + y;
            ff.transform.SetParent(this.transform);
            ff.transform.position = new Vector3(-10000, -10000, -10000);

            var t = InsideArea.transform.localPosition;
            t.y += InsideArea.GetComponent<RectTransform>().rect.height / 2;
            var k = ff.GetComponent<ActionCardCtl>();
            k.InsidePos = t;

            t = OutsideArea.transform.localPosition;
            k.OutsidePos = t;

            t = DestroyArea.transform.localPosition;
            k.DestroyPos = t;

            yield return k.SetToInit(y + 1);
            y++;
            cardNumber++;
            Cards.Add(k);
        }
        if (CardRemainDisplay != null) {
            CardRemainDisplay.text = (cardNumber).ToString();
        }
        yield return (true);
    }

    public IEnumerator GenerateCardDt(List<EventCard> CardDeck) {
        for (int ind = 0; ind < CardDeck.Count; ind++) {
            GameObject ff = (GameObject)Instantiate(CardPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            ff.name = "AC" + ind;
            ff.transform.SetParent(this.transform);
            ff.transform.position = new Vector3(-10000, -10000, -10000);
            var k = ff.GetComponent<ActionCardCtl>();
            var ec = CardDeck[ind];

            k.OrignalSet = ec;
           
            var t = InsideArea.transform.localPosition;
            t.y += InsideArea.GetComponent<RectTransform>().rect.height / 2;
            k.InsidePos = t;
            t = OutsideArea.transform.localPosition;
            k.OutsidePos = t;
            t = DestroyArea.transform.localPosition;
            k.DestroyPos = t;
            yield return k.SetCardValue(ec);
            yield return k.SetToInit(ind);
            cardNumber++;
            Cards.Add(k);
        }
        if (CardRemainDisplay != null) {
            CardRemainDisplay.text = (cardNumber).ToString();
        }
        yield return (true);
    }
    public void DrawCardTest() {
        StartCoroutine(DrwCard());
    }
    public IEnumerator DrwCard() {
        cardNumber--;
        yield return Cards[cardNumber].MoveToHand();
        yield return new WaitForSeconds(0.3f);
        if (CardRemainDisplay != null) {
            CardRemainDisplay.text = (cardNumber).ToString();
        }
        yield return UpdateInsidePosition();
    }

    public IEnumerator DrwCard(List<int> income_card_id) {
        foreach (var ind_val in income_card_id) {
            var tmp = Cards.Find(cd => cd.OrignalSet.Id == ind_val);
            if (tmp != null) {
                cardNumber--;
                yield return tmp.MoveToHand();
            }
            yield return new WaitForSeconds(0.3f);
        }
        if (CardRemainDisplay != null) {
            CardRemainDisplay.text = (cardNumber).ToString();
        }
        yield return UpdateInsidePosition();
    }

    public void FlipOpen() {
        StartCoroutine(FlipInsideCard());
    }

    public void CardOutTrigger() {
        StartCoroutine(UpdateOutsidePosition());
        StartCoroutine(UpdateInsidePosition());
        CheckECOutSide();
    }

    // for watch / demo disp
    public void CardPush(List<int> CardId) {
        foreach (var cd in CardId) {
            var tmp = Cards.Find(ec => ec.OrignalSet.Id == cd);
            if (tmp != null && tmp.Pos == EventCardPos.Inside) {
                tmp.CardOut();
            }
        }
    }
    public List<ActionCardCtl> GetInsideCard() {
        return Cards.FindAll(ec => ec.Pos == EventCardPos.Inside);
    }
    public void CheckECOutSide() {
        var totalval = 0;
        foreach (var tec in Cards.FindAll(x => x.Pos == EventCardPos.Outside)) {
            var val = tec.up_val;
            if (tec.isInvert) {
                val = tec.down_val;
            }
            totalval += val;
        }
        ValDisp.text = totalval.ToString();
    }
    public void ResetValDisp(int val) {
        ValDisp.text = val.ToString();
    }
    public IEnumerator UpdateInsidePosition() {
        List<ActionCardCtl> InCd = new List<ActionCardCtl>();
        foreach (var p in Cards) {
            // if (p.GetComponent<ActionCardCtl>().Pos == type_pos.inside) {
            if (p.Pos == EventCardPos.Inside) {
                InCd.Add(p);
            }
        }
        float insideX = InsideArea.GetComponent<RectTransform>().rect.width;
        float cardlistwidth = 66 * (InCd.Count - 1);
        for (int f = 0; f < InCd.Count; f++) {
            var yy = InCd[f].InsidePos;
            yy.x = InsideArea.transform.localPosition.x - cardlistwidth / 2 + f * 66;
            InCd[f].InsidePos = yy;
            yield return f;
        }
        yield return true;
    }

    public IEnumerator UpdateOutsidePosition() {
        List<ActionCardCtl> InCd = new List<ActionCardCtl>();
        foreach (var p in Cards) {
            if (p.Pos == EventCardPos.Outside) {
                // if (p.GetComponent<ActionCardCtl>().Pos == type_pos.outside) {
                InCd.Add(p);
            }
        }
        float insideX = OutsideArea.GetComponent<RectTransform>().rect.width;
        float cardlistwidth = 66 * (InCd.Count - 1);
        if (66 * (InCd.Count) > insideX) {
            for (int f = 0; f < InCd.Count; f++) {
                float padd = insideX / (InCd.Count + 1);
                var yy = InCd[f].OutsidePos;
                yy.x = OutsideArea.transform.localPosition.x - insideX / 2 + f * padd + 20;
                InCd[f].OutsidePos = yy;
                yield return f;
            }
        } else {
            for (int f = 0; f < InCd.Count; f++) {
                var yy = InCd[f].OutsidePos;
                yy.x = OutsideArea.transform.localPosition.x - cardlistwidth / 2 + f * 66;
                InCd[f].OutsidePos = yy;
                yield return f;
            }
        }
        yield return true;
    }

    public IEnumerator FlipInsideCard() {
        foreach (var p in Cards) {
            if (p.Pos == EventCardPos.Inside) {
                // if (p.GetComponent<ActionCardCtl>().Pos == type_pos.inside) {
                StartCoroutine(p.FlipCard());
            }
        }
        yield return true;
    }

    public void SendToDestroy() {
        StartCoroutine(CdToDestroy());
    }
    public IEnumerator CdToDestroy() {
        foreach (var cd in Cards) {
            // if (cd.GetComponent<ActionCardCtl>().Pos == type_pos.outside) {
            if (cd.Pos == EventCardPos.Outside) {
                StartCoroutine(cd.MoveToDestroy());
            }
        }
        yield return true;
    }

}
#if (UNITY_EDITOR) 
[CustomEditor(typeof(CardDeckControl))]
public class CardDeckControlEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        CardDeckControl d = (CardDeckControl)target;
        if (GUILayout.Button("Generate Card Test")) {
            d.GenerateCardRun();
        }

        if (GUILayout.Button("Draw Card Test")) {
            // Debug.Log (ACTest);
            d.DrawCardTest();
        }

        if (GUILayout.Button("Flip Open !")) {
            d.FlipOpen();
        }

        if (GUILayout.Button("Send To Destroy")) {
            d.SendToDestroy();
        }
    }
}
#endif