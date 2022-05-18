using System.Collections.Generic;

public class Player_
{
    public readonly string name_;
    private int prestige;
    private Dictionary<string, int> tokens;
    private Dictionary<string, int> devCards;
    private List<DevCard> remainCards;

    public readonly static int tokenLimit = 10;
    private int tokenCount = 0;

    public Player_(string playerName){
        name_ = playerName;
        prestige = playerName.Length;
        tokenCount = 0;
        tokens = new Dictionary<string, int>()
        {
            { "black", 1 }, { "white", 2 }, { "red", 3 }, 
            { "blue", 4 }, { "green", 5 }, { "gold", 6 }
        };
        devCards = new Dictionary<string, int>()
        {
            { "black", 5 }, { "white", 4 }, { "red", 3 }, 
            { "blue", 2 }, { "green", 1 }
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

    public int AddToken(string color, int amount){ 
        if(amount < 0) return -1;
        if( !tokens.ContainsKey(color) ) return -2;

        tokens[color] += amount;
        tokenCount += amount;
        return amount;
    }
    public int PayToken(string color, int amount){
        if(amount < 0) return -1;
        if( !tokens.ContainsKey(color) ) return -2;
        if( tokens[color] < amount ) return -3;
      
        tokens[color] -= amount;
        tokenCount -= amount;
        return amount;
    }
    public int GetToken(string color){ 
        if( !tokens.ContainsKey(color) ) return -2;
        else return tokens[color];
    }
    public Dictionary<string, int> GetAllTokens(){ return tokens; }

    public bool IsTokensExceed(){ return tokenCount > tokenLimit; }
    
    public bool IsPayableDevCard(DevCard card){
        int count = 0;
        count += PayCount("black", card.black_cost);
        count += PayCount("white", card.black_cost);
        count += PayCount("red", card.black_cost);
        count += PayCount("blue", card.black_cost);
        count += PayCount("green", card.black_cost);

        return count <= tokens["gold"];
    }

    public void AddDevCard(DevCard card){ 
        prestige += card.prestige;
        devCards[card.reward_token] += 1;
    }
    public int GetDevCard(string color){ 
        if( !devCards.ContainsKey(color) ) return -2;
        else return devCards[color];
    }
    public Dictionary<string, int> GetAllDevCards(){ return devCards; }

    public bool IsTakeableNobleCard(NobleCard card){
        return (
            devCards["black"] >= card.black_cost 
            && devCards["white"] >= card.white_cost
            && devCards["red"] >= card.red_cost
            && devCards["blue"] >= card.blue_cost
            && devCards["green"] >= card.green_cost
        );
    }

    public int AddRemainCard(DevCard card){
        if(remainCards.Count == 3) return -1; // full

        remainCards.Add(card);
        return card.id;
    }
    public List<DevCard> GetRemainCards(){ return remainCards; }
    public int RemoveRemainCard(DevCard card){
        int index = remainCards.FindIndex(x => x.id == card.id);      
        if(index == -1) return -1;
        else{
            remainCards.RemoveAt(index);
            return card.id;
        }
    }

    private int PayCount(string color, int cost){
        if(tokens[color] + devCards[color] >= cost) return 0;
        else return (cost - tokens[color] - devCards[color]);
    }

}
