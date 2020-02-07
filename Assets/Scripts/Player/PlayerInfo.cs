using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    private static PlayerInfo instance;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
    }

    public static PlayerInfo GetInstance()
    {
        return instance;
    }
    
}
