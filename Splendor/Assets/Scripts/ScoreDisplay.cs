using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ScoreDisplay : MonoBehaviour
{
    public Image bg;
    public Text Name, Score;
    public GameObject crown;
    
    private int AN_ = 0;
    public string name_ = "";
    public int score_ = 0;
    Vector3 pos_;

    public void initScore(int AN, string name, int score, Vector3 pos){
        AN_ = AN;
        name_ = name;
        score_ = score;
        pos_ = pos;
    }

    public void ShowScore(){
        //Destroy(p);
        this.gameObject.transform.position = pos_;
        //p = PhotonNetwork.Instantiate("ScorePrefab", pos, Quaternion.identity);
        bg.sprite = Resources.Load("6_player_background", typeof(Sprite)) as Sprite;
        Name.text = "  玩家" + AN_ + " " + name_;
        Score.text = score_.ToString() + "分";

        crown.SetActive(false);
    }

    public void ShowWin(){
        //Destroy(p);
        this.gameObject.transform.position = pos_;
        //p = PhotonNetwork.Instantiate("ScorePrefab", pos, Quaternion.identity);
        bg.sprite = Resources.Load("winnerBG", typeof(Sprite)) as Sprite;
        Name.text = "  玩家" + AN_ + " " + name_;
        Score.text = score_.ToString() + "分";

        crown.SetActive(true);
    }
    
    
}
