using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class CCardSetUp : MonoBehaviour {
    /// <summary>
    /// Self CC
    /// </summary>
    public GameObject CCardPrefab;

    public int SelfCC_ID;
    [MinAttribute (1)]
    public int SelfCC_Level;
    // public CCardCtl SelfCCSet;

    public CCardBockCtl SelfCCSetBlock;
    public int self_atk_equ = 0, self_def_equ = 0;
    AssetBundle SelfCC_AB;

    /// <summary>
    /// Duel CC
    /// </summary>
    public int DuelCC_ID;
    public int DuelCC_Level;
    public CCardBockCtl DuelCCSet;
    public int duel_atk_equ = 0 , duel_def_equ = 0;
    AssetBundle DuelCC_AB;

    public string _asset_path {
        get {
            var tmp = "";
            switch (Application.platform) {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    tmp = Path.Combine ("win", "x86");
                    break;
                case RuntimePlatform.Android:
                    tmp = "android";
                    break;
                case RuntimePlatform.IPhonePlayer:
                    tmp = "ios";
                    break;
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    tmp = "mac";
                    break;
            }
            return Path.Combine (Application.streamingAssetsPath, tmp);
        }
    }
    public IEnumerator StartSelfCCImplement () {
        // Debug.Log ("start : " + _asset_path);
        this.SelfCC_AB = AssetBundle.LoadFromFile (
            Path.Combine (_asset_path, "CC" + (SelfCC_ID.ToString ()).PadLeft (2, '0') + ".ab")
        );
        if (this.SelfCC_AB == null) {
            yield return null;
        } else {
            this.SelfCCSetBlock.level = SelfCC_Level;
            StartCoroutine (this.SelfCCSetBlock.InitCCLvFrame ());
            StartCoroutine (this.SelfCCSetBlock.InitEquSetting (self_atk_equ, self_def_equ));
            StartCoroutine (this.SelfCCSetBlock.InitCCImg (this.SelfCC_AB));

            yield return true;
        }
    }
    public IEnumerator StartDuelCCImplement () {
        // Debug.Log ("start : " + _asset_path);
        this.DuelCC_AB = AssetBundle.LoadFromFile (
            Path.Combine (_asset_path, "CC" + (DuelCC_ID.ToString ()).PadLeft (2, '0') + ".ab")
        );
        if (this.DuelCC_AB == null) {
            yield return null;
        } else {
            this.DuelCCSet.level = DuelCC_Level;
            StartCoroutine (this.DuelCCSet.InitCCLvFrame ());
            StartCoroutine (this.DuelCCSet.InitEquSetting (duel_atk_equ, duel_def_equ));
            StartCoroutine (this.DuelCCSet.InitCCImg (this.DuelCC_AB));

            yield return true;
        }
    }
    void Start () {
        StartCoroutine (StartSelfCCImplement ());
        StartCoroutine (StartDuelCCImplement ());
    }

    // Update is called once per frame
    void Update () {

    }
}