using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ULZAsset.Config;
public class MenuCtl : MonoBehaviour {
 
    public void GotoRoomSearch() {
        SceneManager.LoadScene("RoomSearch", LoadSceneMode.Single);
    }
    public void GotoDeckEditing() {
        SceneManager.LoadScene("", LoadSceneMode.Single);
    }
    public void GotoSetting() {
        SceneManager.LoadScene("Setting", LoadSceneMode.Single);
    }
    public void CloseProg() {
        Application.Quit();
    }
}