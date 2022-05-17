using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DevCardDisplay : MonoBehaviour
{

    public Image back, reward;
    public Image[] token = new Image[4];
    public Text prestige;
    
    //public DevCard card;

    public void ShowCard(DevCard card){
        back.sprite = Resources.Load("Card/level" + card.level.ToString(), typeof(Sprite)) as Sprite;
        reward.sprite = Resources.Load("Card/" + card.reward_token, typeof(Sprite)) as Sprite;

        int iToken = 0;

        iToken = setTokenImage(iToken, "black", card.black_cost);
        iToken = setTokenImage(iToken, "white", card.white_cost);
        iToken = setTokenImage(iToken, "red", card.red_cost);
        iToken = setTokenImage(iToken, "blue", card.blue_cost);
        iToken = setTokenImage(iToken, "green", card.green_cost);

        while(iToken < 4){
            iToken = setTokenImage(iToken, "emptyToken", -1);
        }

        prestige.text = card.prestige.ToString();
    }

    private int setTokenImage(int i, string color, int cost){
        if(cost != 0){
            token[i].sprite = Resources.Load("Card/" + color, typeof(Sprite)) as Sprite;
            token[i].GetComponentInChildren<Text>().text = ((cost != -1) ? cost.ToString() : "");
            return i + 1;
        }
        else 
            return i;
    }
}
