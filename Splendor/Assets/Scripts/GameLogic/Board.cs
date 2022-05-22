using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board
{
    private GameObject prefabDev, prefabNoble;
    private GameObject tableDev, tableNoble, tableToken;
    private List<GameObject> showCards;

    private int[,] devPlace;
    private int[] noblePlace;
    private Dictionary<string, int> tokenPlace;
    private List<int> randDev, randNoble;


    public Board(List<int> rd, List<int> rn, ref Database DB){
        prefabDev = (GameObject)Resources.Load("DevPrefab");
        prefabNoble = (GameObject)Resources.Load("NoblePrefab");
        tableDev = GameObject.Find("TableDevCard");
        tableNoble = GameObject.Find("TableNoble");
        tableToken = GameObject.Find("TableToken");
        
        showCards = new List<GameObject>();

        devPlace = new int[3,4];
        noblePlace = new int[5];
        tokenPlace = new Dictionary<string, int>();

        randDev = new List<int>(rd);
        randNoble = new List<int>(rn);

        // dev
        for(int i=1; i<=3; ++i){
            for(int j=1; j<=4; ++j){
                this.Replenish(i, j);
            }
        }
        //noble
        for(int i=1; i<=5; ++i){
            this.Replenish(0, i);
        }

        //token
        tokenPlace = DB.GetTokens();

    }

    public void DisplayBoard(ref Database DB){
        ClearShowCards();
        
     // display dev card
        for(int i=1; i<=3; ++i){
            for(int j=1; j<=4; ++j){
                GameObject pos = GameObject.Find("Lv" + i.ToString() + "Place_" + j.ToString()) as GameObject;
                GameObject newCard = GameObject.Instantiate(prefabDev, pos.transform.position, Quaternion.identity, tableDev.transform);
                
                newCard.GetComponent<DevCardDisplay>().ShowCard(DB.GetDevCard(i, devPlace[i-1, j-1]));

                showCards.Add(newCard);
            }
        }
    
    // display noble card
        for(int i=1; i<=5; ++i){
            GameObject pos = GameObject.Find("Place_" + i.ToString()) as GameObject;
            GameObject newCard = GameObject.Instantiate(prefabNoble, pos.transform.position, Quaternion.identity, tableNoble.transform);
            
            newCard.GetComponent<NobleCardDisplay>().ShowCard(DB.GetNobleCard(noblePlace[i-1]));

            showCards.Add(newCard);
        }
    
    // display tokens
        Component[] tokens = tableToken.GetComponentsInChildren<Image>();
        foreach(var t in tokens){
            t.GetComponentInChildren<Text>().text = tokenPlace[t.name.ToLower()].ToString();
        }
        
    }

    public void Replenish(int level, int place){  // 0:noble , 1~3:dev

        place -= 1;
        if(level == 0){
            if(randNoble.Count == 0){
                noblePlace[place] = 0;
             }
             else{
                noblePlace[place] = randNoble[0];
                randNoble.RemoveAt(0); 
            }
            
        }
        else{
            int card;

            switch(level){
            case 1: card = randDev.FindIndex(x => x<=40); break;
            case 2: card = randDev.FindIndex(x => x>40 && x<=70); break;
            case 3: card = randDev.FindIndex(x => x>70); break;
            default: card = -1; break;
            }

            if(card != -1){
                devPlace[level-1, place] = randDev[card];
                randDev.RemoveAt(card);
            }
            else{
                devPlace[level-1, place] = 0;
            }
        }
        

        return;
    }

    private void ClearShowCards(){
        foreach(var card in showCards){
            Object.Destroy(card);
        }
        showCards.Clear();
    }

    public int GetTokenNum(string color){ 
        if(tokenPlace.ContainsKey(color)) return tokenPlace[color];
        else return -2;
    }

    public int GiveOutToken(string color, int amount){
        if(amount < 0) return -1;
        if(!tokenPlace.ContainsKey(color)) return -2;
        if(tokenPlace[color] < amount) return -3;

        tokenPlace[color] -= amount;
        return amount;
    }

    public int GetDevCardByPlace(int i, int j){
        return devPlace[i, j];
    }

    public int ReturnToken(string color, int amount){
        if(amount < 0) return -1;
        if(!tokenPlace.ContainsKey(color)) return -2;

        tokenPlace[color] += amount;
        return amount;
    }

    public int GetNobleIdAt(int place){
        if(place < 1 || place > 5) return 0;
        else return noblePlace[place-1]; 
    }

}