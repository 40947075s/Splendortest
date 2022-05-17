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
    public GameObject prefabGM;
    //private PlayerDisplay myPlayer;
    private GM myGM;
    private PhotonView pvGM;

    void Start(){
        if(PhotonNetwork.CurrentRoom == null){
            SceneManager.LoadScene("scene_lobby");
            return;
        }
        ShowStartMessage();  

        InitGame();
    }
    
    void Update() {
        myGM.DisplayBoard();    
    }

    public void InitGame(){
        //GameObject p = PhotonNetwork.Instantiate("Player", new Vector3(0, 0, 0), Quaternion.identity);
        GameObject g = PhotonNetwork.Instantiate("GM", new Vector3(0, 0, 0), Quaternion.identity);        
        
        //p.GetComponent<PlayerDisplay>().player = new Player_();
        //p.GetComponent<PlayerDisplay>().player.name = PhotonNetwork.LocalPlayer.NickName;

        //myPlayer = p.GetComponent<PlayerDisplay>();
        myGM = g.GetComponent<GM>();
        pvGM = g.GetPhotonView();

        myGM.SetUpDatabase();

        if(PhotonNetwork.LocalPlayer.IsMasterClient){
            ExitGames.Client.Photon.Hashtable table = new ExitGames.Client.Photon.Hashtable();

            myGM.SetUpBoard();

            table.Add("rd", ListToString(myGM.randDev));
            table.Add("rn", ListToString(myGM.randNoble));
            PhotonNetwork.LocalPlayer.SetCustomProperties(table);

        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, HashTable changedProps)
    {
        List<int> rd = StringToList( (string)changedProps["rd"] );
        List<int> rn = StringToList( (string)changedProps["rn"] );
        myGM.SetUpBoard(rd, rn);
    }

    public string ListToString(List<int> list){
        string s = "";
        foreach(var i in list){
            s += (i.ToString() + ' ');
        }

        return s;
    }

    public List<int> StringToList(string s){
        List<int> list = new List<int>();
        string[] values = s.Split(' ');

        foreach(string v in values){
            if(v != "") list.Add( int.Parse(v) );
        }

        return list;
    }










    private void ShowStartMessage(){
        panelStartMessage.SetActive(true);

        Invoke("SwitchStartMessage", 2f);
        Invoke("HideStartMessage", 5.5f);
    }

    private void SwitchStartMessage(){
        panelStartMessage.GetComponentInChildren<Text>().text = "玩家順序";
    }

    private void HideStartMessage(){
        panelStartMessage.SetActive(false);
    }

}
