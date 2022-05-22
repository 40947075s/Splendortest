using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeepCardController : MonoBehaviour
{
    public GameObject prefabToggle;
    public GameObject window;
    public Transform displaWindow;
    public Button btnSubmit;

    private Toggle[,] toggles = new Toggle[3, 4];
    private Vector3 offset = new Vector3(0, 53, 0);
    private int selectI = 0, selectJ = 0;

    void Start() {
        SetUpTakeCard();
    }

    void SetUpTakeCard(){
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
        ClearToggles();
//
        btnSubmit.interactable = false;
    }

    public void DisplayDev(){
        GameSceneManager.Instance.myGM.DisplayDevCard(displaWindow);
    }
    
    void EventClick(int indI, int indJ){
        if(toggles[indI, indJ].isOn){
            selectI = indI;
            selectJ = indJ;
        // is submit allow
            btnSubmit.interactable = true;
        }
    }

    public void OnClickSubmit(){
        GameSceneManager.Instance.ActKeepCard(selectI, selectJ);
        ClearToggles();
    }

    void ClearToggles(){
        foreach(Toggle t in toggles){
            t.isOn = false;
        }
    }
   




}
