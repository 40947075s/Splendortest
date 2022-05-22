using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class WindowsController : MonoBehaviourPunCallbacks
{
/* panle */
    public GameObject ActionChoose;
    public GameObject ActionTakeToken;
    public GameObject ActionReturnToken;
    public GameObject ActionTakeCard;
    public GameObject ActionKeepCard;

/* image */
    public GameObject actionMessage, TurnMessage;   
    public Button btnKeepCard;

    

    void Start()
    {
        GameSceneManager.Instance.eventShowMessage.AddListener(ShowActionMessage);
        GameSceneManager.Instance.eventTurnSet.AddListener(TurnSceneSet);
        GameSceneManager.Instance.eventRetrunToken.AddListener(OnClickReturnToken);

        OnClickReturn();
    }

    void ShowActionMessage(int ActionPlayer, string msg){
        string playerName = PhotonNetwork.PlayerList[ActionPlayer].NickName;
        ActionChoose.SetActive(false);
        
        actionMessage.SetActive(true);
        actionMessage.GetComponentInChildren<Text>().text = msg;
        
        TurnMessage.SetActive(true);
        TurnMessage.GetComponentInChildren<Text>().text = "玩家回合： " + playerName;
    }

    void TurnSceneSet(int ActionPlayer, bool isMyTurn){
        string playerName = PhotonNetwork.PlayerList[ActionPlayer].NickName;

        ActionChoose.SetActive(isMyTurn);
        
        actionMessage.SetActive(false);
        
        TurnMessage.SetActive(!isMyTurn);
        TurnMessage.GetComponentInChildren<Text>().text = "玩家回合： " + playerName;

        if( GameSceneManager.instance.myPlayer.player.IsRemainCardFull() 
            || GameSceneManager.instance.myGM.GetTokenNum("gold") <= 0){
            btnKeepCard.interactable = false;
        }
        else{
            btnKeepCard.interactable = true;
        }
    }

    public void OnClickTakeToken(){
        ActionChoose.SetActive(false);
        ActionTakeToken.SetActive(true);
    }

    public void OnClickReturnToken(){
        ActionChoose.SetActive(false);
        ActionReturnToken.SetActive(true);
    }

    public void OnClickBuyCard(){
        ActionChoose.SetActive(false);
        ActionTakeCard.SetActive(true);
    }

    public void OnClickKeepCard(){
        ActionChoose.SetActive(false);
        ActionKeepCard.SetActive(true);
    }

    public void OnClickReturn(){
        ActionTakeToken.SetActive(false);
        ActionReturnToken.SetActive(false);
        ActionTakeCard.SetActive(false);
        ActionKeepCard.SetActive(false);

        ActionChoose.SetActive(true);
    }

    public void OnClickSubmit(){
        ActionTakeToken.SetActive(false);
        ActionReturnToken.SetActive(false);
        ActionTakeCard.SetActive(false);
        ActionKeepCard.SetActive(false);

        ActionChoose.SetActive(false);
    }

}
