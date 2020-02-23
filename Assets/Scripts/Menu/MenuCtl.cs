using System.Collections;
using System.Collections.Generic;
using ULZAsset.Config;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuCtl : MonoBehaviour {

    public void GotoRoomSearch() {
        SceneManager.LoadScene("RoomSearch", LoadSceneMode.Single);
    }
    public void GotoDeckEditing() {
        SceneManager.LoadScene("", LoadSceneMode.Single);
    }
    public void GotoSetting() {
        SceneManager.LoadScene("AccountCreate", LoadSceneMode.Single);
    }
    public void CloseProg() {
        Application.Quit();
    }
}