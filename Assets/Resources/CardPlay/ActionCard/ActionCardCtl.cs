using System.Collections;
using System.Collections.Generic;
using System.Threading;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public enum type_opt { attack, defence, move, star, gun, nil }
public enum type_pos { block, inside, outside, destroy }

public class ActionCardCtl : MonoBehaviour {

    //  Card Value
    public type_opt up_option;
    public int up_val;
    public type_opt down_option;
    public int down_val;

    // Postion
    public bool isOutSide;

    public bool m_isflip = false;
    private bool isflipping = false;

    private bool m_isInvert = false;
    public bool isInvert {
        get { return m_isInvert; }
        set {
            m_isInvert = value;
        }
    }

    public Vector3 _outside_pos;
    public Vector3 OutsidePos {
        get { return _outside_pos; }
        set {
            this._outside_pos = value;
            if (this.m_pos == type_pos.outside) {
                this.transform.DOLocalMove(value, 0.1f);
            }
        }
    }

    public Vector3 _inside_pos;
    public Vector3 InsidePos {
        get { return _inside_pos; }
        set {
            this._inside_pos = value;
            if (this.m_pos == type_pos.inside) {
                this.transform.DOLocalMove(value, 0.1f);
            }
        }
    }

    public Vector3 InitialPos;
    public Quaternion InitialRotate;
    public Vector3 DestroyPos;

    private type_pos m_pos = type_pos.block;
    public type_pos Pos {
        get { return m_pos; }
        set {
            m_pos = value;
            switch (value) {
                case type_pos.block:
                    this.transform.DOLocalMove(InitialPos, 0.3f);
                    break;
                case type_pos.inside:
                    // this.transform.DOLocalMove (InsidePos, 0.3f);
                    break;
                case type_pos.outside:
                    // this.transform.DOLocalMove (OutsidePos, 0.3f);
                    break;
                case type_pos.destroy:
                    this.transform.DOLocalMove(DestroyPos, 0.3f);
                    break;
            }
        }
    }

    private GameObject face;
    private GameObject back;

    public Text up_val_go, dw_val_go;
    public RawImage up_type_ri, dw_type_ri;
    public RectTransform up_type_rt;
    public IEnumerator FlipCard() {
        yield return (flip_card());
    }
    private IEnumerator flip_card() {
        isflipping = true;
        if (m_isflip) {
            var sqe = DOTween.Sequence();
            yield return
            sqe.Append(
                    this.face.transform.DORotate(
                        new Vector3(0, 90, 0),
                        0.1f, RotateMode.LocalAxisAdd)
                ).Append(
                    this.back.transform.DORotate(
                        new Vector3(0, -90, 0),
                        0.1f, RotateMode.LocalAxisAdd)
                )
                .WaitForCompletion();
        } else {
            // Close
            var sqe = DOTween.Sequence();
            yield return
            sqe.Append(
                    this.back.transform.DORotate(
                        new Vector3(0, 90, 0),
                        0.1f, RotateMode.LocalAxisAdd)
                ).Append(
                    this.face.transform.DORotate(
                        new Vector3(0, -90, 0),
                        0.1f, RotateMode.LocalAxisAdd)
                )
                .WaitForCompletion();
        }
        isflipping = false;
        this.m_isflip = !this.m_isflip;
        yield return new WaitUntil(() => !isflipping);
    }
    public void CardRotate() {
        this.m_isInvert = !this.m_isInvert;
        StartCoroutine(card_rotate());
    }

    private IEnumerator card_rotate() {
        Tween t = this.transform.DORotate(
            new Vector3(180, 0, 0),
            0.3f, RotateMode.LocalAxisAdd);
        yield return t.WaitForCompletion();
    }
    public void CardOut() {
        isOutSide = true;
        this.m_pos = type_pos.outside;
        if (this.transform.parent.gameObject.GetComponent<CardDeckControl>() != null) {
            this.transform.parent.gameObject.GetComponent<CardDeckControl>().CardOutTrigger();
        } else if (this.transform.parent.gameObject.GetComponent<OpsdCardControl>() != null) {
            this.transform.parent.gameObject.GetComponent<OpsdCardControl>().CardOutTrigger();
        }
        this.transform.DOLocalMove(OutsidePos, 0.1f);
    }

    public void CardBack() {
        isOutSide = false;
        this.m_pos = type_pos.inside;
        if (this.transform.parent.gameObject.GetComponent<CardDeckControl>() != null) {
            this.transform.parent.gameObject.GetComponent<CardDeckControl>().CardOutTrigger();
        } else if (this.transform.parent.gameObject.GetComponent<OpsdCardControl>() != null) {
            this.transform.parent.gameObject.GetComponent<OpsdCardControl>().CardOutTrigger();
        }
        this.transform.DOLocalMove(InsidePos, 0.1f);
    }

    public IEnumerator SetToInit(int card_index) {
        yield return new WaitUntil(
            () => this.InitialPos != null
        );
        this.InitialPos.x += 0.25f * card_index;
        this.transform.localPosition = this.InitialPos;
        this.transform.localRotation = this.InitialRotate;
        this.face.SetActive(false);
        yield return true;
    }

    public IEnumerator SetToOpInit(int card_index, Vector3 pos, Quaternion rotate) {
        yield return new WaitUntil(
            () => this.InitialPos != null
        );
        this.InitialPos = pos;
        this.InitialPos.x += 0.25f * card_index;
        this.InitialRotate = rotate * Quaternion.Euler(0, 90, 0);
        this.transform.localPosition = this.InitialPos;
        this.transform.localRotation = this.InitialRotate;
        this.face.SetActive(false);
        yield return true;
    }
    public IEnumerator MoveToHand() {
        yield return false;
        Tween t = this.transform.DORotate(new Vector3(0, 90, 0), 0.3f);
        yield return t.WaitForCompletion();
        this.m_pos = type_pos.inside;
        this.transform.DOLocalMove(InsidePos, 0.3f);
        yield return true;
        this.face.SetActive(true);
    }
    public IEnumerator MoveToDestroy() {
        this.m_pos = type_pos.destroy;
        this.transform.DOLocalMove(DestroyPos, 0.3f);
        yield return new WaitForSeconds(0.3f);
        this.face.SetActive(false);
        yield return true;
    }
    void Start() {
        this.face = transform.Find("face").gameObject;
        this.back = transform.Find("back").gameObject;
        var deck_block = transform.parent.parent.Find("DeckBlock");
        this.InitialPos = deck_block.localPosition;
        this.InitialRotate = deck_block.localRotation;
        this.InitialRotate *= Quaternion.Euler(0, 90, 0);

        if (this.down_option != type_opt.nil) {
            this.up_type_ri.texture =
                Resources.Load<Texture2D>("CardPlay/ActionCard/Image/" + up_option.ToString())as Texture2D;
            this.dw_type_ri.texture =
                Resources.Load<Texture2D>("CardPlay/ActionCard/Image/" + down_option.ToString())as Texture2D;
            this.up_val_go.text = (this.up_val).ToString();
            this.dw_val_go.text = (this.down_val).ToString();
        } else {
            this.up_type_ri.texture =
                Resources.Load<Texture2D>("CardPlay/ActionCard/Image/sp/" + up_option.ToString())as Texture2D;
            this.up_type_rt.sizeDelta = new Vector2(61f, 100f);
            this.dw_type_ri.color =
                new Color(0f, 0f, 0f, 0f);
            this.up_val_go.text = (this.up_val).ToString();
            this.dw_val_go.text = "";
        }
        // StartCoroutine (testing ());
    }

    public IEnumerator SetCardValue(type_opt _up_type, int _upval, type_opt _dw_type, int _dwval) {
        this.up_option = _up_type;
        this.up_val = _upval;
        this.down_option = _dw_type;
        this.down_val = _dwval;

        if (this.down_option != type_opt.nil) {
            this.up_type_ri.texture =
                Resources.Load<Texture2D>("CardPlay/ActionCard/Image/" + up_option.ToString())as Texture2D;
            this.dw_type_ri.texture =
                Resources.Load<Texture2D>("CardPlay/ActionCard/Image/" + down_option.ToString())as Texture2D;
            this.up_val_go.text = (this.up_val).ToString();
            this.dw_val_go.text = (this.down_val).ToString();
            yield return true;
        } else {
            this.up_type_ri.texture =
                Resources.Load<Texture2D>("CardPlay/ActionCard/Image/sp/" + up_option.ToString())as Texture2D;
            this.up_type_rt.sizeDelta = new Vector2(61f, 100f);
            this.dw_type_ri.color =
                new Color(0f, 0f, 0f, 0f);
            this.up_val_go.text = (this.up_val).ToString();
            this.dw_val_go.text = "";
            yield return true;
        }
    }
    private IEnumerator testing() {
        // Testing 
        Queue<IEnumerator> i = new Queue<IEnumerator>();
        // StartCoroutine (SetToInit (1));
        i.Enqueue(SetToInit(1));
        i.Enqueue(MoveToHand());
        i.Enqueue(FlipCard());
        // i.Enqueue (FlipCard ());

        Debug.Log("i testing queue:" + i);
        while (i.Count > 0) {
            Debug.Log(i.Count);
            var fa = i.Dequeue();
            // Thread.Sleep (150000);
            yield return new WaitForSeconds(10f);
            Debug.Log("start");
            yield return (fa);
        }
    }
}