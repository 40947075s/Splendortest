using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextgameSceneManager : MonoBehaviour
{
    public void OnClickEnd(){
        Application.Quit();
    }

    public void OnClickNext(){
        SceneManager.LoadScene("scene_lobby");
    }

}
