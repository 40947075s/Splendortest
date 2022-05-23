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
            SceneManager.LoadScene("scene_lobby");
        }
        else{
            textRoomName.text = "房號\t" + PhotonNetwork.CurrentRoom.Name;
            UpdatePlayerList();
        }

        buttonStart.interactable = false;
        buttonStart.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        //buttonStart.interactable = PhotonNetwork.IsMasterClient;
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        buttonStart.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        //buttonStart.interactable = PhotonNetwork.IsMasterClient;
    }

    public void UpdatePlayerList(){
        int i=0;
        foreach(var kvp in PhotonNetwork.PlayerList){
            playerName[i].text = "  玩家" + (++i) + "    " + kvp.NickName;
        }

        while(i<4){
            playerName[i].text = "  玩家" + (++i) + "    " + "尚未加入";
        }   
        
        if(PhotonNetwork.PlayerList.Length > 1)
            buttonStart.interactable = true;
        else
            buttonStart.interactable = false;

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
        SceneManager.LoadScene("scene_lobby");
    }


//test
    public void OnClickStartGame(){
        SceneManager.LoadScene("scene_game");
    }
}
