using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ReturnTokenController : MonoBehaviourPunCallbacks
{
    public Button btnSubmit;

    private List<Text> views;
    private List<Button> plus, minus;
    private Dictionary<string, int> tokenNum;

    void Start() {
        SetUpReturnToken();
    }

    void SetUpReturnToken(){
        Text[] texts = this.GetComponentsInChildren<Text>();
        views = new List<Text>();
        foreach(Text t in texts){
            if(t.name == "Num") views.Add(t);
        }

        Button[] buttons = this.GetComponentsInChildren<Button>();
        plus = new List<Button>();
        minus = new List<Button>();
        
        int p = 0, n = 0;
        foreach(Button b in buttons){
            if(b.name == "Plus"){
                int x = p;
                b.onClick.AddListener(() => EventClickPlus(x));
                ++p;
                plus.Add(b);
            }

            if(b.name == "Minus"){ 
                int x = n;
                b.onClick.AddListener(() => EventClickMinus(x));
                ++n;
                minus.Add(b);
            }
        }

        ClearReturnToken();

        ShowTokens();
    }

    void ShowTokens(){
        foreach(var t in views){
            var parent = t.gameObject.transform.parent;
            t.text = tokenNum[parent.name].ToString();
        }
    }

    void EventClickPlus(int Index){
        string btnName = plus[Index].transform.parent.name;
        if(tokenNum[btnName] < 5)
            tokenNum[btnName] += 1;

        btnSubmit.interactable = IsBtnAllow();
        ShowTokens();
    }

    void EventClickMinus(int Index){
        string btnName = minus[Index].transform.parent.name;
        if(tokenNum[btnName] > 0)
            tokenNum[btnName] -= 1;

        btnSubmit.interactable = IsBtnAllow();
        ShowTokens();
    }

    private bool IsBtnAllow(){
        return GameSceneManager.Instance.myPlayer.player.IsReturnableTokens(tokenNum);
    }
    
    public void ClearReturnToken(){
        tokenNum = new Dictionary<string, int>()
        {
            { "black", 0 }, { "white", 0 }, { "red", 0 }, 
            { "blue", 0 }, { "green", 0 }
        };
        
        btnSubmit.interactable = false;
        ShowTokens();
    }

    public void OnClickSubmit(){
        GameSceneManager.Instance.ActReturnToken(tokenNum);
        ClearReturnToken();
    }
}
