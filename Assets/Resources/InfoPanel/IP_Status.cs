using ULZAsset;
using UnityEngine;
using UnityEngine.UI;
public class IP_Status : MonoBehaviour {
    public StatusEffectMainViewCtl base_asset;
 
    public void init(StatusObject targ, int cd) {
        Debug.Log(targ.img);
        Texture2D t2d = base_asset.MainEffectAB.LoadAsset(targ.img)as Texture2D;
        this.transform.Find("RawImage").GetComponent<RawImage>().texture = t2d;
        this.transform.Find("Text").GetComponent<Text>().text = cd.ToString();
    }
}