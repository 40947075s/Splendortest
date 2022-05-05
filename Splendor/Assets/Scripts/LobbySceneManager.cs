using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;


public class LobbySceneManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        if(PhotonNetwork.IsConnected == false){
            SceneManager.LoadScene("scene_start");
        }else{
            if(PhotonNetwork.CurrentLobby == null){
                PhotonNetwork.JoinLobby();
            }

        }
    }

    public override void OnConnectedToMaster()
    {
        print("connect to master");
        PhotonNetwork.JoinLobby();

    }

    public void OnClickBuild(){
        SceneManager.LoadScene("scene_buildroom");
    }

    public void OnClickEnter(){
        SceneManager.LoadScene("scene_enterroom");
    }
}
