using System.Collections.Generic;
using UnityEngine;

public class Player_
{
    public readonly string name_;
    private int prestige;
    private Dictionary<string, int> tokens;
    private Dictionary<string, int> devCards;
    private List<DevCard> remainCards;

    public readonly static int tokenLimit = 10;
    //private int tokenCount = 0;

    public Player_(string playerName){
        name_ = playerName;
        prestige = 0;
        tokens = new Dictionary<string, int>()
        {
            { "black", 0 }, { "white", 0 }, { "red", 0 }, 
            { "blue", 0 }, { "green", 0 }, { "gold", 0 }
        };
        devCards = new Dictionary<string, int>()
        {
            { "black", 0 }, { "white", 0 }, { "red", 0 }, 
            { "blue", 0 }, { "green", 0 }
        };
        remainCards = new List<DevCard>();
    }

    public string GetName(){ return name_; }

    public int AddPrestige(int num){ 
        if(num >= 0){ 
            prestige += num;
            return prestige; 
        }
        else return -1;
    }
    public int GetPrestige(){ return prestige; }

/* about tokens */
    public int AddToken(string color, int amount){ 
        if(amount < 0) return -1;
        if( !tokens.ContainsKey(color) ) return -2;

        tokens[color] += amount;
        return amount;
    }
    public int PayToken(string color, int amount){
        if(amount < 0) return -1;
        if( !tokens.ContainsKey(color) ) return -2;
        if( tokens[color] < amount ) return -3;
      
        tokens[color] -= amount;
        return amount;
    }
    public int GetToken(string color){ 
        if( !tokens.ContainsKey(color) ) return -2;
        else return tokens[color];
    }
    public Dictionary<string, int> GetAllTokens(){ return tokens; }
    public int TokensExceedNum(){ 
        int count = CountTotalTokens(false) - tokenLimit; 
        return (count <= 0) ? 0 : count;
    }

/* about dev */
    public void BuyDevCard(DevCard card){ 
        prestige += card.prestige;
        devCards[card.reward_token] += 1;

        tokens["black"] -= card.black_cost;
        tokens["white"] -= card.white_cost;
        tokens["red"] -= card.red_cost;
        tokens["blue"] -= card.blue_cost;
        tokens["green"] -= card.green_cost;
        
        int count = 0;

        foreach(var t in tokens){
            if(t.Value < 0){ count -= t.Value;}
        }

        tokens["gold"] -= count;

        tokens["black"] = (tokens["black"] < 0) ? 0 : tokens["black"];
        tokens["white"] = (tokens["white"] < 0) ? 0 : tokens["white"];
        tokens["red"] = (tokens["red"] < 0) ? 0 : tokens["red"];
        tokens["blue"] = (tokens["blue"] < 0) ? 0 : tokens["blue"];
        tokens["green"] = (tokens["green"] < 0) ? 0 : tokens["green"];
    }
    public int GetDevCard(string color){ 
        if( !devCards.ContainsKey(color) ) return -2;
        else return devCards[color];
    }
    public Dictionary<string, int> GetAllDevCards(){ return devCards; }
    
    public bool IsReturnableTokens(Dictionary<string, int> rt){
    // enough to return
        int count = 0;
        count += PayCount("black", rt["black"]);
        count += PayCount("white", rt["white"]);
        count += PayCount("red", rt["red"]);
        count += PayCount("blue", rt["blue"]);
        count += PayCount("green", rt["green"]);

    // exceed or not (after return)
        int total = 0;
        foreach(var t in rt){
            total += t.Value;
        }

        return (count == 0) && (CountTotalTokens(false)-total == tokenLimit);
    }
    public bool IsPayableDevCard(DevCard card){
        int count = 0;
        count += PayCount("black", card.black_cost);
        count += PayCount("white", card.white_cost);
        count += PayCount("red", card.red_cost);
        count += PayCount("blue", card.blue_cost);
        count += PayCount("green", card.green_cost);

        return count <= tokens["gold"];
    }
    public bool IsTakeableNobleCard(NobleCard card){
        return (
            devCards["black"] >= card.black_cost 
            && devCards["white"] >= card.white_cost
            && devCards["red"] >= card.red_cost
            && devCards["blue"] >= card.blue_cost
            && devCards["green"] >= card.green_cost
        );
    }

/* about keep */
    public int AddRemainCard(DevCard card){
        if(remainCards.Count == 3) return -1; // full

        remainCards.Add(card);
        return card.id;
    }
    public List<DevCard> GetRemainCards(){ return remainCards; }
    public DevCard PopRemainCard(int index){
        Debug.Log("pop" + index);
        if(index < 0 || index >= remainCards.Count) return null;

        DevCard c = remainCards[index];
        remainCards.RemoveAt(index);

        return c;
    }
    public bool IsRemainCardFull(){ return remainCards.Count >= 3; }

    private int PayCount(string color, int cost){
        if(tokens[color] + devCards[color] >= cost) return 0;
        else return (cost - tokens[color] - devCards[color]);
    }
    private int CountTotalTokens(bool isIgnorGold){
        int count = 0;

        foreach(var t in tokens){
            count += t.Value;
        }

        if( isIgnorGold )
            count -= tokens["gold"];

        return count;
    }

}
