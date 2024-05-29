using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGamesSelection : MonoBehaviour
{
    private LocalOnlineOption _localOnlineOption;
   
    void Start()
    {
        _localOnlineOption = FindObjectOfType<LocalOnlineOption>();
    }

    public void Impostor()
    {
        _localOnlineOption.SetImpostorGame();
    }

    public void Burguer()
    {
        _localOnlineOption.SetBurguerGame();
    }

    public void Local()
    {
        _localOnlineOption.SetLocalGame();
    }

    public void Online()
    {
        _localOnlineOption.SetOnlineGame();
    }
    
}
