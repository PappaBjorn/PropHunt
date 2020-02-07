using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public enum RoundState
{
    InRound,
    WaitingForNextRound,
}

[System.Serializable]
public class PlayerServerInfo
{
    public int Id;
    public bool Prop;
    public bool Alive;

    public PlayerServerInfo(int viewID, bool prop, bool alive)
    {
        Id = viewID;
        Prop = prop;
        Alive = alive;
    }
}


[RequireComponent(typeof(PhotonView))]
public class RoundSystem : MonoBehaviour
{
    [SerializeField] private GameMode _gameMode;
    [SerializeField] private bool _winCondtionActive = true;
    //dassd
    private static RoundSystem _instance;
    private static PhotonView PV;

    private int _numberOfProps = Int32.MaxValue;
    private int _numberOfHunters = Int32.MaxValue;
    private int _numberOfPropsAlive = Int32.MaxValue;
    private int _numberOfHuntersAlive = Int32.MaxValue;

    private bool _ran = false;

    private List<PlayerServerInfo> _playerInfo = new List<PlayerServerInfo>();
    private Dictionary<int, bool> _playerRoundEndDataUpdate = new Dictionary<int, bool>();

    private float _time;
    private RoundState _currentState = RoundState.WaitingForNextRound;
    private bool _isPlaying = false;
    private short _roundCount = 0;

    private void Awake()
    {
        PV = gameObject.GetComponent<PhotonView>();
        _instance = this;
        DontDestroyOnLoad(gameObject);
        enabled = false;
        _time = _gameMode.TimeBetweenRounds;
    }
    private void Start()
    {
        PV.RPC("UpdatePlayerList", RpcTarget.All);

    }
    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        PlayerAliveUpdate();

        switch (_currentState)
        {
            case RoundState.InRound:
                OnInRound();
                break;
            case RoundState.WaitingForNextRound:
                OnWaitingForNextRound();
                break;
                ;
        }
    }

    private void PlayerAliveUpdate()
    {
        int propsAlive = 0;
        int huntersAlive = 0;
        int hunters = 0;
        int props = 0;

        foreach (var x in _playerInfo)
        {
            if (x.Prop && x.Alive)
                propsAlive++;
            else if (x.Alive)
                huntersAlive++;

            if (x.Prop)
                props++;
            else
                hunters++;
        }

        _numberOfHunters = hunters;
        _numberOfProps = props;
        _numberOfPropsAlive = propsAlive;
        _numberOfHuntersAlive = huntersAlive;
        //Debug.Log("Props: " + _numberOfPropsAlive + " Hunters: " + _numberOfHuntersAlive);
    }

    private void OnInRound()
    {
        if (!_isPlaying)
            InitNewRound();


        _time -= Time.deltaTime;
        PV.RPC("ServerTime", RpcTarget.All, Convert.ToInt16(_time));

        if (_time < _gameMode.RoundTimeLimit - _gameMode.HunterWaitTime && !_ran)
        {
            _ran = true;
            PV.RPC("ActivateHunters", RpcTarget.AllBuffered);
        }
        
        

        if (_time < _gameMode.RoundTimeLimit * 0.9f)
        {
            if(_numberOfHuntersAlive <= 0 && _winCondtionActive)
                EndRound("Props");
            else if(_numberOfPropsAlive <= 0  && _winCondtionActive)
                EndRound("Hunter");
        }

        if (_time <= 0)
            EndRound("Props");
    }

    private void OnWaitingForNextRound()
    {
        _time -= Time.deltaTime;
        PV.RPC("ServerTime", RpcTarget.All, Convert.ToInt16(_time));
        if (_time <= 0)
            _currentState = RoundState.InRound;
    }

    private bool EnoughPlayersToStart()
    {


        return false;
    }

    private void InitNewRound()
    {
        _ran = false;
        _time = _gameMode.RoundTimeLimit;
        _isPlaying = true;
        _roundCount++;
        PV.RPC("ServerRound", RpcTarget.AllBuffered, _roundCount);
        PV.RPC("SpawnPlayers", RpcTarget.AllBuffered);
        foreach (var x in _playerInfo)
        {
            x.Alive = true;
        }

        foreach (var x in _playerRoundEndDataUpdate)
        {
            foreach (var playerData in _playerInfo)
            {
                if (x.Key == playerData.Id)
                    playerData.Prop = x.Value;
            }
        }
        _playerRoundEndDataUpdate.Clear();
        PV.RPC("UpdatePlayerList", RpcTarget.All);
    }

    private void EndRound(string winner)
    {
        _isPlaying = false;
        _currentState = RoundState.WaitingForNextRound;
        _time = _gameMode.TimeBetweenRounds;
        PV.RPC("DestroyPlayers", RpcTarget.AllBuffered);
        Debug.Log(winner);
    }

    public static RoundSystem GetInstance()
    {
        return _instance;
    }


    public void NewPlayerInfo(int id, bool prop)
    {
        _playerInfo.Add(new PlayerServerInfo(id, prop, false));
    }

    public void PlayerKilled(int id)
    {
        foreach (var x in _playerInfo)
        {
            if (x.Id == id)
            {
                x.Alive = false;
                return;
            }
        }
    }

    [PunRPC]
    void ServerTime(short time)
    {
        PlayerUIManager.GetInstance().SetServerTimeText(time);
    }

    [PunRPC]
    void ServerRound(short count)
    {
        PlayerUIManager.GetInstance().SetServerRoundCountText(count);
    }
    
    [PunRPC]
    void ActivateHunters()
    {
        if (Room.GetInstance().MyPlayerController.Avatar.GetComponent<PlayerMovement>())
        {
            Room.GetInstance().MyPlayerController.Avatar.GetComponent<PlayerMovement>().ActivateHunters();
        }
    }

    [PunRPC]
    void SpawnPlayers()
    {
        Room.GetInstance().MyPlayerController.SpawnPlayer();
    }

    [PunRPC]
    void DestroyPlayers()
    {
        if (Room.GetInstance().MyPlayerController.Avatar.GetComponent<PlayerMovement>())
        {
            Room.GetInstance().MyPlayerController.Avatar.GetComponent<PlayerMovement>().Death();
        }

        Room.GetInstance().MyPlayerController.characterSelected = false;
    }

    [PunRPC]
    public void PlayerRoleChange(int viewId, bool prop)
    {
        _playerRoundEndDataUpdate.Add(viewId, prop);
    }

    [PunRPC]
    void UpdatePlayerList()
    {
        if(GetPlayerPanel.GetInstance())
            GetPlayerPanel.GetInstance()._players = _playerInfo.ToArray();
        Debug.Log("Round: " + _playerInfo.Count);
    }
}