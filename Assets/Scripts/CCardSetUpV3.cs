using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEditor;

public class CCardSet : MonoBehaviour {
    [MinAttribute(1)]
    public int ID, Level; 
    public CCardCtl Info;
    public CCardBockCtl Block;
    public CCardStandCtl StandCtl;
    public int atk_equ =0 , def_equ = 0;
    public bool is_self = true, is_hidden = true;
    public IEnumerator StartSelfCCImplement(AssetBundle abs) { 
        if (abs == null) {
            yield return null;
        }

        if (this.Block == null) {
            this.Block = this.transform.root.parent.Find("Canvas/BlockLayout/CardBlock").gameObject.GetComponent<CCardBockCtl>();
        }


        if (this.StandCtl == null) {
            this.StandCtl = this.transform.root.parent.Find("Canvas/StandImgLayout/SelfStand").gameObject.GetComponent<CCardStandCtl>();
        }

        this.Block.level = this.Level;

        this.Block.is_self = 1;
        this.StandCtl.is_self = 1;

        StartCoroutine(this.Block.InitCCLvFrame());
        StartCoroutine(this.Block.InitEquSetting(atk_equ, def_equ));
        StartCoroutine(this.Block.InitCCImg(abs));
        StartCoroutine(this.StandCtl.InitCCImg(abs, this.Level));

        yield return true;
    }
}
public class CCardSetUpV3: MonoBehaviour {
    // ----------------------------------------------------
    // CC Card Prefab
    List<AssetBundle> SelfCC_ABs , DuelCC_ABs;
    // / <summary>
    // / Self CC
    // / </summary>
    public GameObject CCardPrefab;

    public List<CCardSet> SelfCardDeck;
    public List<CCardSet> DuelCardDeck;
    public string _asset_path {
        get {
            var tmp = "";
            switch (Application.platform) {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer: tmp = Path.Combine("win", "x86");
                    break;
                case RuntimePlatform.Android: tmp = "android";
                    break;
                case RuntimePlatform.IPhonePlayer: tmp = "ios";
                    break;
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer: tmp = "mac";
                    break;
            }
            return Path.Combine(Application.streamingAssetsPath, tmp);
        }
    }
    
    // CC Stand Setup
    void Start() {
        // StartCoroutine(StartSelfCCImplement());
        // StartCoroutine(StartDuelCCImplement());
    }

    public void OpenCCInfoPanel(int self_or_duel) {
        if (self_or_duel == 1) {
            // self 
            
        }
    }

}
