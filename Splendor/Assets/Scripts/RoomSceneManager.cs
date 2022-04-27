using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class RoomSceneManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    Text textRoomName;
    [SerializeField]
    Text[] playerName = new Text[4];
    [SerializeField]
    Button buttonStart;



    void Start() {
        if(PhotonNetwork.CurrentRoom == null){
            SceneManager.LoadScene("scene_loby");
        }
        else{
            textRoomName.text = "房號\t" + PhotonNetwork.CurrentRoom.Name;
            UpdatePlayerList();
        }

        buttonStart.interactable = PhotonNetwork.IsMasterClient;
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        buttonStart.interactable = PhotonNetwork.IsMasterClient;
    }

    public void UpdatePlayerList(){
        int i=0;
        foreach(var kvp in PhotonNetwork.CurrentRoom.Players){
            playerName[i].text = "玩家" + (++i) + "\t" + kvp.Value.NickName;
        }

        while(i<4){
            playerName[i].text = "玩家" + (++i) + "\t" + "尚未加入";
        }   
        
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }

    public void OnClickLeaveRoom(){
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("scene_loby");
    }


//test
    public void OnClickStartGame(){
        SceneManager.LoadScene("scene_game");
    }
}
