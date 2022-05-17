using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class GM : MonoBehaviourPunCallbacks
{
    public TextAsset devData, nobleData, tokenData;
    
    private PhotonView pv;
    private Database DB;
    private Board BB;
    public List<int> randDev, randNoble;  /*private*/

    void Start(){
        pv = this.GetComponent<PhotonView>();
    }

    public void DisplayBoard(){
        BB.DisplayBoard(ref DB);
    }

    public void SetUpDatabase(){
        DB = new Database();
        DB.LoadInData(devData, nobleData, tokenData);
    }

    public void SetUpBoard(){
    /* init board */
        randDev = new List<int>();
        randNoble = new List<int>();

    // init rand seed
        for(int i=1; i<=DB.GetDevNum(); ++i){
            randDev.Add(i);
        }
        for(int i=1; i<=DB.GetNobleNum(); ++i){
            randNoble.Add(i);
        }

        Shuffle(randDev);
        Shuffle(randNoble);
    
        BB = new Board(randDev, randNoble);
    }

    public void SetUpBoard(List<int> rd, List<int> rn){
        Debug.Log("setBoard");

        randDev = new List<int>(rd);
        randNoble = new List<int>(rn);

        foreach(var i in randNoble){
            Debug.Log(i);
        }
    
        BB = new Board(randDev, randNoble);
    }















    public void Shuffle<T>(List<T> inputList){
        for(int i = 0; i < inputList.Count - 1; ++i){
            T temp = inputList[i];
            int rand = Random.Range(i, inputList.Count);
            inputList[i] = inputList[rand];
            inputList[rand] = temp;
        }
    }

    
}
