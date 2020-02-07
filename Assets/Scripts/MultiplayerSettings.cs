using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerSettings : MonoBehaviour
{
    public bool delayStart;
    public int maxPlayers;
    public int menuScene;
    public int multiplayerScene;
    
    
    private static MultiplayerSettings instance;
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public static MultiplayerSettings GetInstance()
    {
        return instance;
    }
    
    
}
