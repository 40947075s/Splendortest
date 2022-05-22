using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TakeCardController : MonoBehaviour
{
    public GameObject prefabToggle, prefabDev;
    public GameObject window;
    public Transform displaWindow;
    public Button btnSubmit;

    private Toggle[,] toggles = new Toggle[3, 4];
    
    private Toggle[] keepToggles = new Toggle[3];
    List<GameObject> showKeep = new List<GameObject>();
    
    private Vector3 offset = new Vector3(0, 48, 0);
    private int selectI = 0, selectJ = 0;

    void Start() {
        SetUpTakeCard();
    }

    void SetUpTakeCard(){
    // instantiate toggle of dev
        for(int i=1; i<=3; ++i){
            for(int j=1; j<=4; ++j){
                Toggle t = GameObject.Instantiate(prefabToggle, new Vector3(0, 0, 0), Quaternion.identity, window.transform).GetComponent<Toggle>();
                t.group = window.GetComponent<ToggleGroup>();

                int x=i-1, y=j-1;
                t.onValueChanged.AddListener(delegate{ EventClick(x, y); });

            // set position
                Transform pos = GameObject.Find("Lv" + i.ToString() + "Place_" + j.ToString()).transform;

                if(j <= 2){ t.transform.position = pos.position + offset; }
                else{ t.transform.position = pos.position - offset; }

                toggles[i-1, j-1] = t;
            }
        }

    // instance toggle of keepCard
        for(int i=1; i<=3; ++i){
            Toggle t = GameObject.Instantiate(prefabToggle, new Vector3(0, 0, 0), Quaternion.identity, window.transform).GetComponent<Toggle>();
            t.group = window.GetComponent<ToggleGroup>();

            int x = i-1;
            t.onValueChanged.AddListener(delegate{ EventClick(x, -1); });

        // set position
            Transform pos = GameObject.Find("keepPlace_" + i.ToString()).transform;
            t.transform.position = pos.position - offset; 

            keepToggles[i-1] = t;
        }

        ClearToggles();

        btnSubmit.interactable = false;
    }

    public void DisplayDev(){
    // display keep
        foreach(var c in showKeep){
            Destroy(c);
        }
        showKeep.Clear();

        List<DevCard> keepCards = GameSceneManager.Instance.myPlayer.player.GetRemainCards();
        for(int i=1; i<=keepCards.Count; ++i){
            GameObject pos = GameObject.Find("keepPlace_" + i.ToString()) as GameObject;
            GameObject newCard = GameObject.Instantiate(prefabDev, pos.transform.position, Quaternion.identity, displaWindow);
            
            newCard.GetComponent<DevCardDisplay>().ShowCard(keepCards[i-1]);
            showKeep.Add(newCard);
        }

    // display dev
        GameSceneManager.Instance.myGM.DisplayDevCard(displaWindow); 
    }


    void EventClick(int indI, int indJ){
        bool isOnCheck;

        if(indJ == -1) isOnCheck = keepToggles[indI].isOn;
        else isOnCheck = toggles[indI, indJ].isOn;

        if(isOnCheck){
            selectI = indI;
            selectJ = indJ;
        // is submit allow
            btnSubmit.interactable = IsBtnAllow();
        }
    }

    public void OnClickSubmit(){
        GameSceneManager.Instance.ActTakeCard(selectI, selectJ);
        ClearToggles();
    }

    private bool IsBtnAllow(){
        bool al = false;
        if(selectJ == -1){
            List<DevCard> c = GameSceneManager.Instance.myPlayer.player.GetRemainCards();
            if(selectI+1 > c.Count) al = false;
            else{
                al = GameSceneManager.Instance.myPlayer.player.IsPayableDevCard(c[selectI]);
            }
        }
        else{
            DevCard take = GameSceneManager.Instance.myGM.GetDevCardOnBoard(selectI, selectJ);        
            al = GameSceneManager.Instance.myPlayer.player.IsPayableDevCard(take);
        }

        return al;
    }

    void ClearToggles(){
        foreach(Toggle t in toggles){
            t.isOn = false;
        }

        foreach(Toggle t in keepToggles){
            t.isOn = false;
        }
    }
   
   
}
