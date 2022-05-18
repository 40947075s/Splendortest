using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private List<GameObject> showCards = new List<GameObject>();
    public GameObject prefabDev;

    public Player_ player;

    public void ShowMyPlayer(){
        GameObject.Find("TextScore").GetComponent<Text>().text = player.GetPrestige().ToString() + " 分";
        
        GameObject mytok = GameObject.Find("MyToken") as GameObject;
        Component[] tokens = mytok.GetComponentsInChildren<Image>();
        foreach(var t in tokens){
            t.GetComponentInChildren<Text>().text = player.GetToken(t.name.ToLower()).ToString();
        }

        GameObject myDev = GameObject.Find("MyCard") as GameObject;
        Component[] devs = myDev.GetComponentsInChildren<Image>();
        foreach(var d in devs){
            d.GetComponentInChildren<Text>().text = player.GetDevCard(d.name.ToLower()).ToString();
        }

        showCards.Clear();
        List<DevCard> rCard = player.GetRemainCards();
        for(int i=1; i<=rCard.Count; ++i){
            GameObject pos = GameObject.Find("keepPlace_" + i.ToString()) as GameObject;
            GameObject newCard = GameObject.Instantiate(prefabDev, pos.transform.position, Quaternion.identity, GameObject.Find("MyKeep").transform);
            
            newCard.GetComponent<DevCardDisplay>().ShowCard(rCard[i-1]);
            showCards.Add(newCard);
        }
    }

    public void ShowOtherPlayer(string data, int index){
        string[] values = data.Split(',');
        int vi = 0;

        GameObject pp = GameObject.Find("Player" + index.ToString());
        pp.transform.GetChild(0).GetComponent<Text>().text = values[vi++] + "\n" + values[vi++] + "分";

        Component[] comp = pp.GetComponentsInChildren<Image>();
        foreach(var v in comp){
            v.GetComponentInChildren<Text>().text = values[vi++];
        }
    }



    public string GetDataToOther(){
    // name,prestige,tokens,devards 
        string data = "";
        data += player.GetName() + ",";
        data += player.GetPrestige().ToString() + ",";
        
        Dictionary<string, int> tokens = player.GetAllTokens();
        foreach(var i in tokens){
            data += i.Value.ToString() + ",";
        }

        Dictionary<string, int> devs = player.GetAllDevCards();
        foreach(var i in devs){
            data += i.Value.ToString() + ",";
        }

        data = data.Remove(data.LastIndexOf(","), 1);
        return data;
    }
    
}
