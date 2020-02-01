using ULZAsset;
using UnityEngine;
using UnityEngine.UI;
public class IP_Status : MonoBehaviour {
    public StatusEffectMainViewCtl base_asset;
    public Text cd_text;
    public RawImage icon;

    private void Start () {
        if (base_asset == null) {
            base_asset = this.transform.root.parent.Find ("EventSystem").GetComponent<StatusEffectMainViewCtl> ();
        }
        if (cd_text == null) {
            cd_text = this.transform.Find ("Text").GetComponent<Text> ();
        }
        if (icon == null) {
            icon = this.transform.Find ("RawImage").GetComponent<RawImage> ();
        }
    }
    public void init (int status_id, int cd) {
        var targ = new StatusObject ();
        foreach (var sobj in base_asset.StatusOpt) {
            if (sobj.id == status_id) {
                targ = sobj;
            }
        }
        var t2d = base_asset.MainEffectAB.LoadAsset (targ.img) as Texture2D;
        icon.texture = t2d;

        cd_text.text = cd.ToString ();
    }
    public void init (StatusObject targ) {
        var t2d = base_asset.MainEffectAB.LoadAsset (targ.img) as Texture2D;
        icon.texture = t2d;
    }
}