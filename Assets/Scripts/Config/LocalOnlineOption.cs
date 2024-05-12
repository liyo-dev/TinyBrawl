using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum TypePlayer
{
    Local,
    Online
}

public class LocalOnlineOption : MonoBehaviour
{
    public static TypePlayer SelectedTypePlayer;

    public static LocalOnlineOption instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); 
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); 
    }

    public void SetOnlineGame()
    {
        SelectedTypePlayer = TypePlayer.Online;
    }
}