using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DG.Tweening;
using TMPro;
using ULZAsset.ProtoMod.GameDuelService;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
public class OpsdCardControl : MonoBehaviour {
    public GameObject CardPrefab;

    // public ActionCardCtl ACTest;
    public int cardNumber;

    public List<ActionCardCtl> Cards;

    public GameObject InsideArea, OutsideArea, DestroyArea;
    public TextMeshPro ValDisp;
    public Text CardRemainDisplay;
    public void GenerateCardTestRun() {
        StartCoroutine(GenerateCard());
    }

    // public IEnumerator GenCardFromDt(List<ACard> CardList) {

    // }

    public IEnumerator GenerateCard() {
        int y = 0;
        while (y < 150) {
            GameObject ff = (GameObject)Instantiate(CardPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            ff.name = "AC" + y;
            ff.transform.SetParent(this.transform);
            ff.transform.position = new Vector3(-10000, -10000, -10000);

            var t = InsideArea.transform.localPosition;
            var k = ff.GetComponent<ActionCardCtl>();
            k.InsidePos = t;
            t = OutsideArea.transform.localPosition;
            k.OutsidePos = t;
            t = DestroyArea.transform.localPosition;
            k.DestroyPos = t;

            var block = this.transform.parent.Find("OpDeckBlock");
            yield return k.SetToOpInit(y + 1, block.localPosition, block.localRotation);
            y++;
            cardNumber++;
            Cards.Add(k);
        }
        if (CardRemainDisplay != null) {
            CardRemainDisplay.text = cardNumber.ToString();
        }
        yield return (true);
    }

    public IEnumerator GenerateCardDt(List<EventCard> CardDeck) {
        for (int ind = 0; ind < CardDeck.Count; ind++) {
            GameObject ff = (GameObject)Instantiate(CardPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            var ec = CardDeck[ind];
            ff.name = "AC" + ind;
            ff.transform.SetParent(this.transform);
            ff.transform.position = new Vector3(-10000, -10000, -10000);

            var t = InsideArea.transform.localPosition;
            var k = ff.GetComponent<ActionCardCtl>();
            k.OrignalSet = ec;

            k.InsidePos = t;
            t = OutsideArea.transform.localPosition;
            k.OutsidePos = t;
            t = DestroyArea.transform.localPosition;
            k.DestroyPos = t;

            var block = this.transform.parent.Find("OpDeckBlock");
            yield return k.SetCardValue(ec);
            yield return k.SetToOpInit(ind + 1, block.localPosition, block.localRotation);
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
            CardRemainDisplay.text = cardNumber.ToString();
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
            CardRemainDisplay.text = cardNumber.ToString();
        }
        yield return UpdateInsidePosition();
    }
    public void FlipOpen() {
        StartCoroutine(FlipOutsideCard());

    }

    public void CardOutTrigger() {
        StartCoroutine(UpdateOutsidePosition());
        StartCoroutine(UpdateInsidePosition());

    }
    public void CheckECOutSide() {
        var totalval = 0;
        foreach (var tec in Cards.FindAll(x => (x.Pos == EventCardPos.Outside && !x.m_isflip))) {
            var val = tec.up_val;
            if (tec.isInvert) {
                val = tec.down_val;
            }
            totalval += val;
        }
        ValDisp.text = totalval.ToString();
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
        float cardlistwidth = 20 * (InCd.Count - 1);
        for (int f = 0; f < InCd.Count; f++) {
            var yy = InCd[f].InsidePos;
            yy.x = InsideArea.transform.localPosition.x + insideX / 2 - f * 22 - 33;
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

    public IEnumerator FlipOutsideCard() {
        foreach (var p in Cards) {
            if (p.Pos == EventCardPos.Outside) {
                // if (p.GetComponent<ActionCardCtl>().Pos == type_pos.outside) {
                StartCoroutine(p.FlipCard());
            }
        }
        yield return true;
        CheckECOutSide();
    }

    public void RandomPush() {
        foreach (var cd in Cards) {
            if (cd.Pos == EventCardPos.Inside) {
                // if (cd.GetComponent<ActionCardCtl>().Pos == type_pos.inside) {
                cd.CardOut();
            }
        }
    }
    public void CardPush(List<int> CardId) {
        foreach (var cd in CardId) {
            var tmp = Cards.Find(ec => ec.OrignalSet.Id == cd && ec.Pos == EventCardPos.Inside);
            if (tmp != null) {
                tmp.CardOut();
            }
        }
    }
    public List<ActionCardCtl> GetInsideCard() {
        return Cards.FindAll(ec => ec.Pos == EventCardPos.Inside);
    }

    public void SendToDestroy() {
        StartCoroutine(CdToDestroy());
    }
    public IEnumerator CdToDestroy() {
        foreach (var cd in Cards) {
            if (cd.Pos == EventCardPos.Outside) {
                // if (cd.GetComponent<ActionCardCtl>().Pos == type_pos.outside) {
                StartCoroutine(cd.MoveToDestroy());
            }
        }
        yield return true;
    }

}

#if (UNITY_EDITOR) 
[CustomEditor(typeof(OpsdCardControl))]
public class OpsdCardControlEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        OpsdCardControl d = (OpsdCardControl)target;
        if (GUILayout.Button("Generate Card Test")) {
            d.GenerateCardTestRun();
        }

        if (GUILayout.Button("Draw Card Test")) {
            d.DrawCardTest();
        }

        if (GUILayout.Button("Random Push out")) {
            d.RandomPush();
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