using System.Collections.Generic;
using UnityEngine;

public interface IAction
{
        void fulfillAction(GM gm, PlayerController player);
}

/* take token */
public class TakeToken : IAction
{
    public Dictionary<string, int> takeToken;

    public TakeToken(Dictionary<string, int> takes){
        this.takeToken = takes;
    }

    public void fulfillAction(GM gm, PlayerController pc){
        foreach(var t in takeToken){
            gm.GiveOutToken(t.Key, t.Value);

            if(pc != null)
                pc.player.AddToken(t.Key, t.Value);
        }
    }
}

public class GetTokenFactory
{
    public IAction createAction(Dictionary<string, int> takes){
        return new TakeToken(takes);
    }
}

/* return token */
public class ReturnToken : IAction
{
    public Dictionary<string, int> returnToken;

    public ReturnToken(Dictionary<string, int> returns){
        this.returnToken = returns;
    }

    public void fulfillAction(GM gm, PlayerController pc){
        foreach(var r in returnToken){
            gm.ReturnToken(r.Key, r.Value);

            if(pc != null)
                pc.player.PayToken(r.Key, r.Value);
        }
    }
}

public class ReturnTokenFactory
{
    public IAction createAction(Dictionary<string, int> returns){
        return new ReturnToken(returns);
    }
}

/* buy dev */
public class BuyDev : IAction
{
    public int I, J;

    public BuyDev(int selectI, int selectJ){
        this.I = selectI;        
        this.J = selectJ;
    }

    public void fulfillAction(GM gm, PlayerController pc){
    // pop card
        DevCard card = null;
        if(J == -1){ // from player
            card = pc.player.PopRemainCard(I);
        }
        else{ // from board
            card= gm.PopDevCardFromBoard(I, J);
        } 

    // add to player and return token
        if(pc != null){
            Dictionary<string, int> before = new Dictionary<string, int>(pc.player.GetAllTokens());
            pc.player.BuyDevCard(card);
            Dictionary<string, int> after = pc.player.GetAllTokens();
            
            foreach(var t in before){
                gm.ReturnToken(t.Key, t.Value - after[t.Key]);
            }
        }
    }
    
}

public class BuyDevFactory
{
    public IAction createAction(int selectI, int selectJ)
    {
        return new BuyDev(selectI, selectJ);
    }
}

/* keep dev */
public class KeepDev : IAction
{
    public int I, J;

    public KeepDev(int selectI, int selectJ){
        this.I = selectI;        
        this.J = selectJ;
    }

    public void fulfillAction(GM gm, PlayerController pc){
    // pop card from board
        DevCard card = gm.PopDevCardFromBoard(I, J);
        gm.GiveOutToken("gold", 1);

    // add to player
        if(pc != null){
            pc.player.AddRemainCard(card);
            pc.player.AddToken("gold", 1);
        }
    }
}

public class KeepDevFactory
{
    public IAction createAction(int selectI, int selectJ)
    {
        return new KeepDev(selectI, selectJ);
    }
}


