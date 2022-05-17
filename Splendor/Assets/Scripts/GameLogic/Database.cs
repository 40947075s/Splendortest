using System.Collections.Generic;
using UnityEngine;

public class Database
{
    private List<DevCard>[] devCards;
    private List<NobleCard> nobleCards;
    private Dictionary<string, int> tokens;

    public Database(){
        devCards = new List<DevCard>[3];
        for(int i=0; i<3; ++i){
            devCards[i] = new List<DevCard>();
        }

        nobleCards = new List<NobleCard>();
        tokens = new Dictionary<string, int>();
    }

    public void LoadInData(TextAsset devCardData, TextAsset nobleCardData, TextAsset tokenData){
        this.ClearDatabase();

    // dev card
        string[] dataRow = devCardData.text.Split('\n');
        foreach(var row in dataRow){
            string[] values = row.Split(',');
            if(values[0] == "#") continue;
            if(values[0] == "") break;

            DevCard card = new DevCard(row);
            devCards[card.level - 1].Add(card);
        }

    // noble card
        dataRow = nobleCardData.text.Split('\n');
        foreach(var row in dataRow){
            string[] values = row.Split(',');
            if(values[0] == "#") continue;

            NobleCard card = new NobleCard(row);
            nobleCards.Add(card);
        }

    // token    
        dataRow = tokenData.text.Split('\n');
        foreach(var row in dataRow){
            string[] values = row.Split(',');
            tokens.Add(values[0], int.Parse(values[1]));
        }
    }

    public int GetDevNum(){
        int count = 0;
        for(int i=0; i<3; ++i){
            count += devCards[i].Count;
        }
        return count;
    }

    public int GetNobleNum(){ return nobleCards.Count; }

    public DevCard GetDevCard(int level, int id){
        return devCards[level-1].Find(x => x.id == id);
    }

    public NobleCard GetNobleCard(int id){
        return nobleCards.Find(x => x.id == id);
    }

    private void ClearDatabase(){
        for(int i=0; i<3; ++i){
            devCards[i].Clear();
        }

        nobleCards.Clear();
        tokens.Clear();
    }



}

