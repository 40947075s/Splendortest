using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.UI;
using UnityEngine.Events;
using Photon.Pun;
using HashTable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;

public class GameSceneManager : MonoSingleton<GameSceneManager>
{
    public GameObject panelStartMessage;
    public GameObject[] scorePos = new GameObject[4];
    public GameObject gameOverScene, scorePrefab;
    private List<GameObject> scores = new List<GameObject>();

    private PhotonView pvGSM;
    
    public GM myGM;
    public PlayerController myPlayer;
    private PhotonView pvGM, pvPlayer;
    
    public UnityEvent<int, bool> eventTurnSet = new UnityEvent<int, bool>();
    public UnityEvent<int, string> eventShowMessage = new UnityEvent<int, string>();
    public UnityEvent eventRetrunToken = new UnityEvent();

    void Start(){
        pvGSM = this.GetComponent<PhotonView>();

        if(PhotonNetwork.CurrentRoom == null){
            SceneManager.LoadScene("scene_lobby");
            return;
        }  

        ShowStartMessage();

        InitGame();
        TurnSet();
    }
    
    void InitGame(){
        gameOverScene.SetActive(false);
    /* Instantiate GM, set up game data and display board */
        GameObject g = PhotonNetwork.Instantiate("GM", new Vector3(0, 0, 0), Quaternion.identity);
        
        myGM = g.GetComponent<GM>();
        pvGM = g.GetPhotonView();

        myGM.SetUpDatabase();

        if(PhotonNetwork.LocalPlayer.IsMasterClient){
            ExitGames.Client.Photon.Hashtable table = new ExitGames.Client.Photon.Hashtable();

            myGM.SetUpBoard(PhotonNetwork.PlayerList.Length);

            table.Add("rd", ListToString(myGM.GetBoardDevSeed()) );
            table.Add("rn", ListToString(myGM.GetBoardNobleSeed()) );
            PhotonNetwork.LocalPlayer.SetCustomProperties(table);
            
            myGM.DisplayBoard();  
        }

    /* Instantiate Player, set up player data and display */
        GameObject p = PhotonNetwork.Instantiate("PlayerPrefab", new Vector3(0, 0, 0), Quaternion.identity);
        
        myPlayer = p.GetComponent<PlayerController>();
        pvPlayer = p.GetPhotonView();

        myPlayer.player = new Player_(PhotonNetwork.LocalPlayer.NickName);
        myPlayer.ShowMyPlayer();

        CallRpcShowOtherPlayer(myPlayer.GetDataToOther());
    }

/* turn control */
    public void NextTurn(string msg){
        eventShowMessage.Invoke(myGM.GetTurnIndex(), msg);  // show action message
        //CallRpcShowMessage(msg);
    // if need return token 
        if(myPlayer.player.TokensExceedNum() > 0){  
            CallRpcShowMessage(msg);
            Invoke("SetReturnToken", 4);
            return;
        }

    // check noble
    
        if( myGM.isPlayerCanTakeNoble(myPlayer) ){
            Debug.Log("可以獲得貴族卡");
            CallRpcShowMessage(msg);
            Invoke("ActTakeNobleCard", 4);
            return;
        }
    

    // check victory condition
        if(!myGM.isSomeOneWin() && myGM.isPlayerAchieveWin(myPlayer)){
            Debug.Log("有人贏");

            CallRpcShowMessage(msg);
            Invoke("PlayerAchieveWin", 4);
            return;
        }

    // check game over
        if(myGM.isSomeOneWin() && myGM.GetTurnIndex() == (PhotonNetwork.PlayerList.Length-1)){
            Debug.Log("進入GameOver," + msg);
            
            CallRpcShowMessage(msg);
            Invoke("GameOver", 4);
            return;
        }
        
    // next turn
        myGM.NextTurn();
        Invoke("TurnSet", 4);
        CallRpcNextTurn(msg);
    }

    private void TurnSet(){
        eventTurnSet.Invoke(myGM.GetTurnIndex(), myGM.IsMyTurn());
    }

    private void SetReturnToken(){
        TurnSet();
        eventRetrunToken.Invoke();
    }

    private void PlayerAchieveWin(){
        CallRpcPlayerAchieveWin();

        string msg = "他達成了勝利條件 ! 本輪將是最後一輪。";
        
        // 這條要延遲
        NextTurn(msg);
    }

    private void GameOver(){
        CallRpcGameOver();

        string msg = "遊戲結束 !";
        eventShowMessage.Invoke(myGM.GetTurnIndex(), msg);

        Invoke("EndTurnScene", 4);
    }

    private void EndTurnScene(){
        gameOverScene.SetActive(true);
    
        ShowGameOver(PhotonNetwork.LocalPlayer.ActorNumber, myPlayer.player.GetPrestige());
        CallRpcShowScore(myPlayer.player.GetPrestige());
       
    }

    private void ShowGameOver(int AN, int score){

        Debug.Log("ps");
        
        GameObject p = GameObject.Instantiate(scorePrefab, scorePos[AN-1].transform.position, Quaternion.identity, gameOverScene.transform);
        p.GetComponent<ScoreDisplay>().initScore(AN, PhotonNetwork.PlayerList[AN-1].NickName, score, scorePos[AN-1].transform.position);

        scores.Add(p);
        
        int winner = 0;
        ScoreDisplay sw = scores[winner].GetComponent<ScoreDisplay>();

        for(int i=0; i<scores.Count; ++i){
            ScoreDisplay sd = scores[i].GetComponent<ScoreDisplay>();
  
            sd.ShowScore();
            if(sd.score_ >  sw.score_){
                winner = i;
                sw = sd;
            }
        }

        sw.ShowWin();
    }

    public void OnClickEndGame(){
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom(){
        SceneManager.LoadScene("scene_nextgame");
    }
    
/* action */
    public void ActTakeToken(Dictionary<string, int> takes){
        TakeTokenFactory factory = new TakeTokenFactory();
        IAction takeToken = factory.createAction(takes);
        takeToken.fulfillAction(myGM, myPlayer);

        myGM.DisplayBoard();

    // photon
        takes.Add("gold", 0);
        CallRpcTakeToken(PackTokenData(takes));
    
        myPlayer.ShowMyPlayer();
        CallRpcShowOtherPlayer(myPlayer.GetDataToOther());

    // build next turn message
        string msg = "他拿了";

        foreach(var t in takes){
            if(t.Value == 0) continue;

            msg += t.Value.ToString();
            msg += "個";
            msg += t.Key;
            msg += "寶石, ";
        }

        msg = msg.Remove(msg.LastIndexOf(","), 1);
        msg += " !"; 

        NextTurn(msg);
    }

    public void ActReturnToken(Dictionary<string, int> returns){
        ReturnTokenFactory factory = new ReturnTokenFactory();
        IAction returnToken = factory.createAction(returns);
        returnToken.fulfillAction(myGM, myPlayer);

        myGM.DisplayBoard();

    // photon
        returns.Add("gold", 0);
        CallRpcReturnToken(PackTokenData(returns));
    
        myPlayer.ShowMyPlayer();
        CallRpcShowOtherPlayer(myPlayer.GetDataToOther());

    // build next turn message
        string msg = "他歸還了";

        foreach(var r in returns){
            if(r.Value == 0) continue;

            msg += r.Value.ToString();
            msg += "個";
            msg += r.Key;
            msg += "寶石, ";
        }

        msg = msg.Remove(msg.LastIndexOf(","), 1);
        msg += " !"; 
        
        NextTurn(msg);
    }

    public void ActTakeCard(int selectI, int selectJ){
        Dictionary<string, int> before = new Dictionary<string, int>(myPlayer.player.GetAllTokens());

        BuyDevFactory factory = new BuyDevFactory();
        IAction buyDev = factory.createAction(selectI, selectJ);
        buyDev.fulfillAction(myGM, myPlayer);

        myGM.DisplayBoard();

    // photon
        Dictionary<string, int> after = myPlayer.player.GetAllTokens();
        Dictionary<string, int> temp = new Dictionary<string, int>();
        foreach(var t in before){
            temp.Add(t.Key, t.Value - after[t.Key]);
        }

        if(selectJ != -1){
            CallRpcBuyDev(selectI, selectJ);
        }

        CallRpcReturnToken(PackTokenData(temp));

     
        
        myPlayer.ShowMyPlayer();
        CallRpcShowOtherPlayer(myPlayer.GetDataToOther());

    // nextTurn
        string msg = "他買了1張發展卡 !";
        NextTurn(msg);
    }

    public void ActKeepCard(int selectI, int selectJ){
        KeepDevFactory factory = new KeepDevFactory();
        IAction KeepDev = factory.createAction(selectI, selectJ);
        KeepDev.fulfillAction(myGM, myPlayer);

        myGM.DisplayBoard();

    // photon
        CallRpcKeepDev(selectI, selectJ);

        myPlayer.ShowMyPlayer();
        CallRpcShowOtherPlayer(myPlayer.GetDataToOther());

    // nextTurn
        string msg = "他保留了1張發展卡 !";
        NextTurn(msg);
    }

    public void ActTakeNobleCard(){
        List<int> cardList = myGM.GetTakeableNoblePlaceList(myPlayer);
        myGM.TakeNobleCard(myPlayer, cardList);

        myGM.DisplayBoard();

    // photon
        CallRpcTakeNoble(ListToString(cardList));

        myPlayer.ShowMyPlayer();
        CallRpcShowOtherPlayer(myPlayer.GetDataToOther());

    // next turn
        string msg = "他獲得了貴族卡 !";
        NextTurn(msg);
    }
/* start message */
    void ShowStartMessage(){
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

/* Photon relate method */
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, HashTable changedProps)
    {
        if(!pvGSM.IsMine){
            List<int> rd = StringToList( (string)changedProps["rd"] );
            List<int> rn = StringToList( (string)changedProps["rn"] );
            myGM.SetUpBoard(rd, rn, PhotonNetwork.PlayerList.Length);

            myGM.DisplayBoard();  
        }
    }
    public void CallRpcShowOtherPlayer(string playerData){
        pvGSM.RPC("RpcShowOtherPlayer", RpcTarget.Others, playerData);
    }
    public void CallRpcNextTurn(string msg){
        pvGSM.RPC("RpcNextTurn", RpcTarget.Others, msg);
    }
    public void CallRpcTakeToken(string takes){
        pvGSM.RPC("RpcTakeToken", RpcTarget.Others, takes);
    }
    public void CallRpcReturnToken(string returns){
        pvGSM.RPC("RpcReturnToken", RpcTarget.Others, returns);
    }
    public void CallRpcBuyDev(int selectI, int selectJ){
        pvGSM.RPC("RpcBuyDev", RpcTarget.Others, selectI, selectJ);
    }
    public void CallRpcKeepDev(int selectI, int selectJ){
        pvGSM.RPC("RpcKeepDev", RpcTarget.Others, selectI, selectJ);
    }
    public void CallRpcShowMessage(string msg){
        pvGSM.RPC("RpcShowMessage", RpcTarget.Others, msg);
    }
    public void CallRpcTakeNoble(string pack){
        pvGSM.RPC("RpcTakeNoble", RpcTarget.Others, pack);
    }
    public void CallRpcPlayerAchieveWin(){
        pvGSM.RPC("RpcPlayerAchieveWin", RpcTarget.Others);
    }
    public void CallRpcGameOver(){
        pvGSM.RPC("RpcGameOver", RpcTarget.Others);
    }

    public void CallRpcShowScore(int score){
        pvGSM.RPC("RpcShowScore", RpcTarget.Others, score);
    }

    [PunRPC]
    void RpcShowOtherPlayer(string playerData, PhotonMessageInfo info){
        int index = info.Sender.ActorNumber - PhotonNetwork.LocalPlayer.ActorNumber;
        index = (index + PhotonNetwork.PlayerList.Length) % PhotonNetwork.PlayerList.Length;
        index += 1;
        
        myPlayer.ShowOtherPlayer(playerData, index);
    }

    [PunRPC]
    void RpcNextTurn(string msg){
        Debug.Log("RPC next");

        eventShowMessage.Invoke(myGM.GetTurnIndex(), msg);
        myGM.NextTurn();

        Invoke("TurnSet", 4);  
    }

    [PunRPC]
    void RpcTakeToken(string takes){
        TakeTokenFactory factory = new TakeTokenFactory();
        IAction takeToken = factory.createAction(UnPackTokenData(takes));
        takeToken.fulfillAction(myGM, null);

        myGM.DisplayBoard();
    }

    [PunRPC]
    void RpcReturnToken(string returns){
        ReturnTokenFactory factory = new ReturnTokenFactory();
        IAction returnToken = factory.createAction(UnPackTokenData(returns));
        returnToken.fulfillAction(myGM, null);

        myGM.DisplayBoard();
        TurnSet();
    }

    [PunRPC]
    void RpcBuyDev(int selectI, int selectJ){
    
        BuyDevFactory factory = new BuyDevFactory();
        IAction buyDev = factory.createAction(selectI, selectJ);
        buyDev.fulfillAction(myGM, null);

        myGM.DisplayBoard();
    }

    [PunRPC]
    void RpcKeepDev(int selectI, int selectJ){
        KeepDevFactory factory = new KeepDevFactory();
        IAction KeepDev = factory.createAction(selectI, selectJ);
        KeepDev.fulfillAction(myGM, null);

        myGM.DisplayBoard();
    }
    
    [PunRPC]
    void RpcShowMessage(string msg){
        Debug.Log("RPC Show msg");
        eventShowMessage.Invoke(myGM.GetTurnIndex(), msg);
        //Invoke("TurnSet", 4);
    }

    [PunRPC]
    void RpcTakeNoble(string pack){
        List<int> list = StringToList(pack);
        myGM.TakeNobleCard(null, list);

        myGM.DisplayBoard();
    }

    [PunRPC]
    void RpcPlayerAchieveWin(){
        myGM.isSomeOneWin(true);
    } 

    [PunRPC]
    void RpcGameOver(){
        Debug.Log("Rpc GO");

        string msg = "遊戲結束 !";
        eventShowMessage.Invoke(myGM.GetTurnIndex(), msg);

        Invoke("EndTurnScene", 4);
    }

    
    [PunRPC]
    void RpcShowScore(int score, PhotonMessageInfo info){
        Debug.Log("Return" + info.Sender.ActorNumber + "," + score);
        
        ShowGameOver(info.Sender.ActorNumber, score);
    }
/* Functional method */
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

    private string PackTokenData(Dictionary<string, int> dic){
        string s = "";

        foreach(var d in dic){
            s += d.Value.ToString() + ' ';
        }
        return s;
    }
    private Dictionary<string, int> UnPackTokenData(string s){

        string[] values = s.Split(' ');
        Dictionary<string, int> dic = new Dictionary<string, int>(){
            {"black", int.Parse(values[0])},
            {"white", int.Parse(values[1])}, 
            {"red", int.Parse(values[2])}, 
            {"blue", int.Parse(values[3])}, 
            {"green", int.Parse(values[4])},
            {"gold", int.Parse(values[5])}   
        };

        return dic;
    }
}
