using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.UI;
using Photon.Pun;
using HashTable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;

public class GameSceneManager : MonoBehaviourPunCallbacks
{
    public GameObject panelStartMessage;
    
    //public GameObject prefabGM;
    //public GameObject prefabPlayer
    private PhotonView pvGSM;
    
    private GM myGM;
    private PhotonView pvGM;

    private PlayerController myPlayer;
    private PhotonView pvPlayer;
    
    void Start(){
        pvGSM = this.GetComponent<PhotonView>();

    // photon need 
        if(PhotonNetwork.CurrentRoom == null){
            SceneManager.LoadScene("scene_lobby");
            return;
        }
        ShowStartMessage();  

        InitGame();
    

    /* game scene test */
    /*
        myGM = GameObject.Instantiate(prefabGM, new Vector3(0, 0, 0), Quaternion.identity).GetComponent<GM>();
        myGM.SetUpDatabase();
        myGM.SetUpBoard();
        myGM.DisplayBoard();

        myPlayer = GameObject.Instantiate(prefabPlayer, new Vector3(0, 0, 0), Quaternion.identity).GetComponent<PlayerController>();
        myPlaye.player = new Player_("Bot");
        myPlayer.ShowPlayer();
    */
    }
    
    public void InitGame(){
    /* Instantiate GM, set up game data and display board */
        GameObject g = PhotonNetwork.Instantiate("GM", new Vector3(0, 0, 0), Quaternion.identity);
        
        myGM = g.GetComponent<GM>();
        pvGM = g.GetPhotonView();

        myGM.SetUpDatabase();

        if(PhotonNetwork.LocalPlayer.IsMasterClient){
            ExitGames.Client.Photon.Hashtable table = new ExitGames.Client.Photon.Hashtable();

            myGM.SetUpBoard();

            table.Add("rd", ListToString(myGM.randDev));
            table.Add("rn", ListToString(myGM.randNoble));
            PhotonNetwork.LocalPlayer.SetCustomProperties(table);

            myGM.DisplayBoard();  
        }

    /* Instantiate Player, set up player data and display */
        GameObject p = PhotonNetwork.Instantiate("PlayerPrefab", new Vector3(0, 0, 0), Quaternion.identity);
        
        myPlayer = p.GetComponent<PlayerController>();
        pvPlayer = p.GetPhotonView();

        myPlayer.player = new Player_(PhotonNetwork.LocalPlayer.NickName);
        myPlayer.ShowMyPlayer();

        CallRpcShowOtherPlayer(myPlayer.GetDataToOther()); /////////
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, HashTable changedProps)
    {
        List<int> rd = StringToList( (string)changedProps["rd"] );
        List<int> rn = StringToList( (string)changedProps["rn"] );
        myGM.SetUpBoard(rd, rn);

        myGM.DisplayBoard();  
    }

    private string ListToString(List<int> list){
        string s = "";
        foreach(var i in list){
            s += (i.ToString() + ' ');
        }

        return s;
    }

    private List<int> StringToList(string s){
        List<int> list = new List<int>();
        string[] values = s.Split(' ');

        foreach(string v in values){
            if(v != "") list.Add( int.Parse(v) );
        }

        return list;
    }

    public void CallRpcShowOtherPlayer(string playerData){
        pvGSM.RPC("RpcShowOtherPlayer", RpcTarget.Others, playerData);
    }

    [PunRPC]
    void RpcShowOtherPlayer(string playerData, PhotonMessageInfo info){
        int index = info.Sender.ActorNumber - PhotonNetwork.LocalPlayer.ActorNumber;
        index = (index + PhotonNetwork.PlayerList.Length) % PhotonNetwork.PlayerList.Length;
        index += 1;
        
        myPlayer.ShowOtherPlayer(playerData, index);
    }


    private void ShowStartMessage(){
        panelStartMessage.SetActive(true);

        Invoke("SwitchStartMessage", 2f);
        Invoke("HideStartMessage", 5.5f);
    }

    private void SwitchStartMessage(){
        string s = "玩家順序: ";

        for(int i=0; i<PhotonNetwork.PlayerList.Length; ++i){
            s += (i+1).ToString() + "." + PhotonNetwork.PlayerList[i].NickName + " ";
        }

        panelStartMessage.GetComponentInChildren<Text>().text = s;
    }

    private void HideStartMessage(){
        panelStartMessage.SetActive(false);
    }

}


// PhotonNetwork.PlayerListOthers