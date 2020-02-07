using UnityEngine;

[CreateAssetMenu(menuName = "RoundSystem/New Round")]
public class GameMode : ScriptableObject
{
    [Header("Round-Settings:")]
    public float RoundTimeLimit = 120f;
    public float TimeBetweenRounds = 10f;
    public float HunterWaitTime = 10f;
}
