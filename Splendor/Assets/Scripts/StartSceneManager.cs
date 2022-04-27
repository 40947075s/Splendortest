using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSceneManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    InputField inputPlayerName;

    public void OnClickStart(){
        string playerName = GetPlayerName();

        if(playerName.Length > 0){
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.LocalPlayer.NickName = playerName;
            print("Start");  
        }
        else{
            print("No player name.");
        }
    }

    public string GetPlayerName(){
        string playerName = inputPlayerName.text;
        return playerName.Trim();
    }

    public override void OnConnectedToMaster()
    {
       print("connected");
       SceneManager.LoadScene("scene_loby");
    }


}
