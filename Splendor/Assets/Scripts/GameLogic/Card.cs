
public class DevCard
{   
    public readonly int level;
    public readonly int id;
    public readonly int prestige;
    public readonly string reward_token;
    public readonly int black_cost;
    public readonly int white_cost;
    public readonly int red_cost;
    public readonly int blue_cost;
    public readonly int green_cost;

    
    public DevCard(string dataRow){
        
        string[] values = dataRow.Split(',');

        this.level = int.Parse(values[0]);
        this.id = int.Parse(values[1]);
        this.prestige = int.Parse(values[2]);
        this.reward_token = values[3];
        this.black_cost = int.Parse(values[4]);
        this.white_cost = int.Parse(values[5]);
        this.red_cost = int.Parse(values[6]);
        this.blue_cost = int.Parse(values[7]);
        this.green_cost = int.Parse(values[8]);
    }
}

public class NobleCard
{
    public readonly int id;
    public readonly int prestige;
    public readonly int black_cost;
    public readonly int white_cost;
    public readonly int red_cost;
    public readonly int blue_cost;
    public readonly int green_cost;

    public NobleCard(string dataRow){
        string[] values = dataRow.Split(',');

        this.id = int.Parse(values[0]);
        this.prestige = int.Parse(values[1]);
        this.black_cost = int.Parse(values[2]);
        this.white_cost = int.Parse(values[3]);            
        this.red_cost = int.Parse(values[4]);
        this.blue_cost = int.Parse(values[5]);
        this.green_cost = int.Parse(values[6]);
    }
}