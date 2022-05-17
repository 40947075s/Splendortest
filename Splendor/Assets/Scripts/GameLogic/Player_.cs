using System.Collections.Generic;

public class Player_
{
    public string name = "no name";
    public int prestige = 0;
    public int black_in_hand = 0;        
    public int white_in_hand = 0;
    public int red_in_hand = 0;
    public int blue_in_hand = 0;
    public int green_in_hand = 0;
    public int golden_in_hand = 0;
    public List<DevCard> cards_in_hand = new List<DevCard>();
    public DevCard[] remained_cards = new DevCard[3];
    public Dictionary<string, int> token_reward = new Dictionary<string, int>();

}
