using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using ULZAsset;
using UnityEditor;
using UnityEngine;
public class CCPhaseRender : MonoBehaviour {
    public GameObject IconBlock;
    public Animator cAnimator;

    // public 

    private void Start () {
        if (this.IconBlock == null) {
            this.IconBlock = this.transform.Find ("set_a/char_f").gameObject;
        }
        if (this.cAnimator == null) {
            this.cAnimator = this.gameObject.GetComponent<Animator> ();
        }
    }

    public IEnumerator InitCCImg (AssetBundle abs, int level) {
        if (abs == null) {
            yield return false;
        }
        TextAsset tb = abs.LoadAsset ("card_set.json") as TextAsset;

        CardObject crd = JsonConvert.DeserializeObject<CardObject> (tb.text);
        List<CardSet> tmp = new List<CardSet> ();
        foreach (var tt in (crd.card_set)) {
            if (tt.level == level) {
                tmp.Add (tt);
            }
        }
        ImgSet Imgs = new ImgSet ();
        if (tmp[0] != null) {
            Imgs = tmp[0].artifact_image;
        }
        if (this.IconBlock == null) {
            yield return false;
        } else {
            Texture2D tas = abs.LoadAsset (Imgs.name) as Texture2D;
            var sprit = IconBlock.GetComponent<SpriteRenderer> ();
            sprit.sprite = Sprite.Create (tas,
                new Rect (0, 0, tas.width, tas.height),
                new Vector2 (0.5f, 0.5f)
            );
            float orig_ratio = ((float) tas.height / (float) tas.width) /
                ((float) Imgs.height / (float) Imgs.width);
            IconBlock.GetComponent<Transform> ().localScale *= new Vector2 (orig_ratio * 1.2f, 1 * 1.2f);
        }
        yield return true;
    }

    public IEnumerator PlayAnimation (int phase, int event_phase) {
        yield return false;
    }

}