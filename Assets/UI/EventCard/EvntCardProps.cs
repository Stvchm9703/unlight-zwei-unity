using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
public enum type_opt { attack, defence, move, star, gun }
public enum type_pos { block, inside, outside, destroy }

[RequireComponent (typeof (Animator))]
public class EvntCardProps : MonoBehaviour {
    public type_opt up_option;
    public int up_val;
    public type_opt down_option;
    public int down_val;

    public bool isInvert;
    public bool isOutSide;
    private bool m_isflip = false;
    public bool is_flipover {
        get { return m_isflip; }
        set {
            m_isflip = value;
            if (anim) {
                anim.SetBool ("is_fliped", m_isflip);
            }
        }
    }
    // private pointer 
    private Animator anim;
    public Vector3 OutsidePos;
    public Vector3 InsidePos;
    public Vector3 InitialPos;
    public Vector3 DestroyPos;

    private type_pos m_pos = type_pos.block;
    public type_pos Pos {
        get { return m_pos; }
        set {
            m_pos = value;
            switch (value) {
                case type_pos.block:
                    move_pos (InitialPos);
                    break;
                case type_pos.inside:
                    move_pos (InsidePos);
                    break;
                case type_pos.outside:
                    move_pos (OutsidePos);
                    break;
                case type_pos.destroy:
                    move_pos (DestroyPos);
                    break;
            }
        }
    }

    private float _speed = 1200;
    void Start () {
        anim = GetComponent<Animator> ();
        // Debug.Log (transform.parent.parent.parent.parent);
        Vector3 deckBlock = transform.parent.parent.parent.parent.Find ("DeckBlock").gameObject.GetComponent<RectTransform> ().position;
        Vector3 deckDestroy = transform.parent.parent.parent.parent.Find ("DeckDestroy").gameObject.GetComponent<RectTransform> ().position;
        deckBlock.x -= GetComponent<RectTransform> ().rect.width / 2;
        deckDestroy.x += GetComponent<RectTransform> ().rect.width / 2;
        OutsidePos = deckBlock;
        InsidePos = deckBlock;
        InitialPos = deckBlock;
        DestroyPos = deckDestroy;

        // transform.Rotate (0, 90, 0);
        // Thread.Sleep (7000);
        // flip_card ();
        is_flipover = !is_flipover;
        // Pos = type_pos.block;
    }
    void Update () { }
    private void move_pos (Vector3 pos) {
        float step = _speed * Time.deltaTime;
        while (Vector3.Distance (transform.position, pos) > 0) {
            transform.position = Vector3.MoveTowards (transform.position, pos, step);
        }
    }
    public void RotateCard () {
        if (anim) {
            anim.SetBool ("is_rotate", !isInvert);
            isInvert = !isInvert;
        }
    }

    public void CardOut () {
        isOutSide = true;
        // transform.parent.parent.parent.gameObject.GetComponent<EventCardComp> ().PullOutCard (this.gameObject);
    }

    public void CardBack () {
        isOutSide = false;
        // transform.parent.parent.parent.gameObject.GetComponent<EventCardComp> ().PushInCard (this.gameObject);
    }

    public void InitialProps (type_opt up_opt, int up_val) { }

    public void DestroyCard () { }

}