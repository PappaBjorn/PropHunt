using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] private Text _serverTimeText;
    [SerializeField] private Text _serverRoundCount;
    [SerializeField] private Text _serverRole;
    
    private static PlayerUIManager _instance;

    private void Awake()
    {
        _instance = this;
    }
    
    public static PlayerUIManager GetInstance()
    {
        return _instance;
    }

    public void SetServerTimeText(short x)
    {
        if (_serverTimeText)
            _serverTimeText.text = x.ToString();
    }
    
    public void SetServerRoundCountText(short x)
    {
        if (_serverRoundCount)
            _serverRoundCount.text = x.ToString();
    }
    
    public void SetServerRoleText(bool x)
    {
        if (_serverRole)
            _serverRole.text = "Next Round: " + (x ? "Prop" : "Hunter");
    }
    
    
    

}
