using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class GM : MonoBehaviourPunCallbacks
{
    public TextAsset devData, nobleData, tokenData;
    public GameObject prefabDev;

    private PhotonView pv;

    private Database DB;
    private Board BB;
    private List<int> randDev, randNoble;  /*private*/
    private int turnIndex;
    private int winCondition = 15;
    private bool someOneWin = false;

    private List<GameObject> showCards;

    void Start(){
        InitGM();
    }

/* setting method */
    public void DisplayBoard(){
        BB.DisplayBoard(ref DB);
    }

    public void SetUpDatabase(){
        DB = new Database();
        DB.LoadInData(devData, nobleData, tokenData);
    }

    public void SetUpBoard(){
    /* init board */
    // init rand seed
        randDev = new List<int>(DB.GetDevIdList());
        randNoble = new List<int>(DB.GetNobleIdList());        

        Shuffle(randDev);
        Shuffle(randNoble);

        while(randNoble.Count > 5) randNoble.RemoveAt(0);

        BB = new Board(randDev, randNoble, ref DB);
    }

    public void SetUpBoard(List<int> rd, List<int> rn){
        randDev = new List<int>(rd);
        randNoble = new List<int>(rn);

        BB = new Board(randDev, randNoble, ref DB);
    }

    private void InitGM(){
        pv = this.GetComponent<PhotonView>();
        turnIndex = 0;
        showCards = new List<GameObject>();
    }

    public int GetTokenNum(string color){ 
        if(BB == null) return 1;
        return BB.GetTokenNum(color); 
    }

/* Action take/return tokens */
    public bool IsTokensAllowToTake(Dictionary<string, int> takes){
    // take rule 
        int totalCount = 0;
        bool takeRule = false;
        bool singleCheck = true, doubleCheck = true;

        foreach(var t in takes){
            totalCount += t.Value;

            if(t.Value != 0 && t.Value != 1) singleCheck = false;
            if(t.Value != 0 && t.Value != 2) doubleCheck = false;
        }

        takeRule = ((singleCheck && totalCount==3) || (doubleCheck && totalCount==2));


    // board enough
        bool enough = false;

        foreach(var t in takes){
            if(t.Value == 0) continue;
            else if(t.Value == 1){
                enough = ( BB.GetTokenNum(t.Key) >= t.Value );
            }
            else if(t.Value == 2){
                enough = ( BB.GetTokenNum(t.Key) >= 4 );
            }
            else enough = false;
        }

        return takeRule && enough;
    } 

    public void GiveOutToken(string color, int amount){
        if(BB.GiveOutToken(color, amount) != amount) return;
    }

    public void ReturnToken(string color, int amount){
        if(BB.ReturnToken(color, amount) != amount) return;
    }

/*Action buy/keep card */
    
    public void DisplayDevCard(Transform parentObj){
        ClearShowCards();
        for(int i=1; i<=3; ++i){
            for(int j=1; j<=4; ++j){
                GameObject pos = GameObject.Find("Lv" + i.ToString() + "Place_" + j.ToString()) as GameObject;
                GameObject newCard = GameObject.Instantiate(prefabDev, pos.transform.position, Quaternion.identity, parentObj);
                
                //newCard.GetComponent<DevCardDisplay>().card = DB.getDevCard(i, devPlace[i-1, j-1]);
                newCard.GetComponent<DevCardDisplay>().ShowCard(DB.GetDevCard(i, BB.GetDevCardByPlace(i-1, j-1)));

                showCards.Add(newCard);
            }
        }
    }

    public DevCard GetDevCardOnBoard(int indI, int indJ){
        return DB.GetDevCard(indI+1, BB.GetDevCardByPlace(indI, indJ));
    }

    public DevCard PopDevCardFromBoard(int indI, int indJ){
        DevCard card = DB.GetDevCard(indI+1, BB.GetDevCardByPlace(indI, indJ));
        
        BB.Replenish(indI+1, indJ+1);

        return card;
    }

/* Noble related method */
    public bool isPlayerCanTakeNoble(PlayerController pc){
        for(int i=1; i<=5; ++i){
            int nobleId = BB.GetNobleIdAt(i);
            NobleCard nc = DB.GetNobleCard(nobleId);

            if(pc.player.IsTakeableNobleCard(nc))
                return true;
        }

        return false;
    }

    public List<int> GetTakeableNoblePlaceList(PlayerController pc){
        List<int> list = new List<int>();

        for(int i=1; i<=5; ++i){
            int nobleId = BB.GetNobleIdAt(i);
            NobleCard nc = DB.GetNobleCard(nobleId);

            if(pc.player.IsTakeableNobleCard(nc))
                list.Add(i);
        }

        return list;
    }

    public void TakeNobleCard(PlayerController pc, List<int> placeList){
        foreach(var i in placeList){
            int nobleId = BB.GetNobleIdAt(i);
            NobleCard nc = DB.GetNobleCard(nobleId);

            BB.Replenish(0, i);
            
            if(pc != null)
                pc.player.AddPrestige(nc.prestige);
        }
    }

/* victory related */
    
    public bool isSomeOneWin(bool os = false){ 
        someOneWin = someOneWin || os;        
        return someOneWin; 
    }


    public bool isPlayerAchieveWin(PlayerController pc){
        someOneWin = (pc.player.GetPrestige() >= winCondition || someOneWin);
        return pc.player.GetPrestige() >= winCondition; 
    }


/* some others */
    public int GetTurnIndex(){ return turnIndex; }
    
    public void NextTurn(){ 
        turnIndex  = (turnIndex + 1) % PhotonNetwork.PlayerList.Length;
    }

    public bool IsMyTurn(){ return (turnIndex + 1) == PhotonNetwork.LocalPlayer.ActorNumber; }

/* functional method */

    public List<int> GetBoardDevSeed(){ return randDev; }

    public List<int> GetBoardNobleSeed(){ return randNoble; }

    private void Shuffle<T>(List<T> inputList){
        for(int i = 0; i < inputList.Count - 1; ++i){
            T temp = inputList[i];
            int rand = Random.Range(i, inputList.Count);
            inputList[i] = inputList[rand];
            inputList[rand] = temp;
        }
    }

    private void ClearShowCards(){
        foreach(var card in showCards){
            Object.Destroy(card);
        }
        showCards.Clear();
    }
}
