using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ExitGames.Client.Photon;

public class Enterroom_sceneManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    InputField inputRoomName;

    void Start(){
        if(PhotonNetwork.IsConnected == false){
            SceneManager.LoadScene("scene_start");
        }
    }

    public string GetRoomName(){
        string roomName = inputRoomName.text;
        return roomName.Trim();
    }

    public void OnClickEnterRoom(){
        string roomName = GetRoomName();
        if(roomName.Length > 0){
            PhotonNetwork.JoinRoom(roomName);
        }else{
            print("please input name");        
        }
    }

    public void OnClickLeaveScene(){
        SceneManager.LoadScene("scene_lobby");
    }

    public override void OnJoinedRoom()
    {
        print("enter room");
        SceneManager.LoadScene("scene_room");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        print("enter faild");
        inputRoomName.text = "";
        inputRoomName.placeholder.GetComponent<Text>().text = "無法加入房間，請重新輸入";
    }

}
