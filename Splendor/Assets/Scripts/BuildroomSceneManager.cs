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
        SceneManager.LoadScene("scene_loby");
    }

    public override void OnJoinedRoom()
    {
        print("build room");
        SceneManager.LoadScene("scene_room");
    }

}