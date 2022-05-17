using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
    private GameObject prefabDev, prefabNoble;
    private GameObject tableDev, tableNoble;
    private List<GameObject> showCards;

    private int[,] devPlace;
    private int[] noblePlace;
    private int[] tokenPlace;
    private List<int> randDev, randNoble;


    public Board(List<int> rd, List<int> rn){
        prefabDev = (GameObject)Resources.Load("DevPrefab");
        prefabNoble = (GameObject)Resources.Load("NoblePrefab");
        tableDev = GameObject.Find("TableDevCard");
        tableNoble = GameObject.Find("TableNoble");

        showCards = new List<GameObject>();

        devPlace = new int[3,4];
        noblePlace = new int[5];
        tokenPlace = new int[6];

        randDev = new List<int>(rd);
        randNoble = new List<int>(rn);

        for(int i=1; i<=3; ++i){
            for(int j=1; j<=4; ++j){
                this.Replenish(i, j);
            }
        }
        for(int i=1; i<=5; ++i){
            this.Replenish(0, i);
        }
    }

    public void DisplayBoard(ref Database DB){
        ClearShowCards();

    // display dev card
        for(int i=1; i<=3; ++i){
            for(int j=1; j<=4; ++j){
                GameObject pos = GameObject.Find("Lv" + i.ToString() + "Place_" + j.ToString()) as GameObject;
                GameObject newCard = GameObject.Instantiate(prefabDev, pos.transform.position, Quaternion.identity, tableDev.transform);
                
                //newCard.GetComponent<DevCardDisplay>().card = DB.getDevCard(i, devPlace[i-1, j-1]);
                newCard.GetComponent<DevCardDisplay>().ShowCard(DB.GetDevCard(i, devPlace[i-1, j-1]));

                showCards.Add(newCard);
            }
        }

    // display noble card
        for(int i=1; i<=5; ++i){
            GameObject pos = GameObject.Find("Place_" + i.ToString()) as GameObject;
            GameObject newCard = GameObject.Instantiate(prefabNoble, pos.transform.position, Quaternion.identity, tableNoble.transform);
            
            newCard.GetComponent<NobleCardDisplay>().card = DB.GetNobleCard(noblePlace[i-1]);
            newCard.GetComponent<NobleCardDisplay>().ShowCard();

            showCards.Add(newCard);
        }
    }

    private void Replenish(int level, int place){  // 0:noble , 1~3:dev
        if(level == 0 && randNoble.Count == 0) return;
        if(level !=0 && randDev.Count == 0) return;

        place -= 1;
        if(level == 0){
            noblePlace[place] = randNoble[0];
            randNoble.RemoveAt(0);
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
        }

        return;
    }

    private void ClearShowCards(){
        foreach(var card in showCards){
            Object.Destroy(card);
        }
        showCards.Clear();
    }

}