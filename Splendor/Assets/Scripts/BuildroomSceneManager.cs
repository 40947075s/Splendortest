using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class BuildroomSceneManager : MonoBehaviourPunCallbacks
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

    public void OnClickCreateRoom(){
        string roomName = GetRoomName();
        if(roomName.Length > 0){
            PhotonNetwork.CreateRoom(roomName);
        }else{
            print("please input name");        
        }
    }
    
     public void OnClickLeaveScene(){
        SceneManager.LoadScene("scene_lobby");
    }

    public override void OnJoinedRoom()
    {
        print("build room");
        SceneManager.LoadScene("scene_room");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        print("build faild");
        inputRoomName.text = "";
        inputRoomName.placeholder.GetComponent<Text>().text = "無法建立房間，請重新輸入";
    }
 
}