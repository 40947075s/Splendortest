using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MonoSingleton<T> : MonoBehaviourPunCallbacks where T : MonoBehaviourPunCallbacks
{
    public static T instance;

    public static T Instance{
        get{
            if(instance == null) instance = FindObjectOfType<T>();
            return instance;
        }
    }

    private void Awake() {
        if(instance != null) Destroy(gameObject);
    }


}

