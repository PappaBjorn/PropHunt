using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetup : MonoBehaviour
{
   [SerializeField] public Transform[] SpawnPoints;

   private static GameSetup instance;

   private void Awake()
   {
      instance = this;
   }

   public static GameSetup GetInstance()
   {
      return instance;
   }
}
