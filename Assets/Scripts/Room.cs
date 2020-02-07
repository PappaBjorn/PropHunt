using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Room : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    private static Room instance;
    private PhotonView PV;
    private MultiplayerSettings multiplayerSettings;

    public bool isGameLoaded;
    public int currentScene;
    private Player[] players;
    public int playersInRoom;
    public int myPlayerID;
    public int playersInGame;
    private string MyNickname = "";
    public PlayerController MyPlayerController;
    private bool readyToCount;
    private bool readyToStart;
    public float startingTime;
    private float lessThanMaxPlayers;
    private float atMaxPlayers;
    private float timeToStart;

    private void Awake()
    {
        if (Room.GetInstance() == null)
            instance = this;
        else
        {
            if (Room.GetInstance() != this)
            {
                Destroy(Room.GetInstance().gameObject);
                instance = this;
            }
        }
        multiplayerSettings = MultiplayerSettings.GetInstance();
        DontDestroyOnLoad(this.gameObject);
    }
    public static Room GetInstance()
    {
        return instance;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        isGameLoaded = false;
        PhotonNetwork.AddCallbackTarget(this);
        multiplayerSettings = MultiplayerSettings.GetInstance();
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.buildIndex;

        multiplayerSettings = MultiplayerSettings.GetInstance();
        if (currentScene == multiplayerSettings.multiplayerScene)
        {
            isGameLoaded = true;

            if (multiplayerSettings.delayStart)
            {
                PV.RPC("RPC_LoadedGameScene", RpcTarget.MasterClient);
            }
            else
            {
                RPC_CreatePlayer();
            }
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }

    private void Start()
    {
        PV = GetComponent<PhotonView>();
        readyToStart = false;
        readyToCount = false;
        lessThanMaxPlayers = startingTime;
        timeToStart = startingTime;
        atMaxPlayers = 6;
    }
    public void ChangeNickName(string NewNickname)
    {
        MyNickname = NewNickname;
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        players = PhotonNetwork.PlayerList;
        playersInRoom = players.Length;
        myPlayerID = playersInRoom;
        if (MyNickname == "")
            PhotonNetwork.NickName = MyNickname;
        else
            PhotonNetwork.NickName = MyNickname;


        if (multiplayerSettings.delayStart)
        {
            Debug.Log("Displayed players in room out of max players possible (" + playersInRoom + ":" +
                      multiplayerSettings.maxPlayers + ")");

            if (playersInRoom > 1)
                readyToCount = true;

            if (playersInRoom == multiplayerSettings.maxPlayers)
            {
                readyToStart = true;

                if (!PhotonNetwork.IsMasterClient)
                    return;

                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
        }
        else
        {
            StartGame();
        }
    }

    private void Update()
    {
        if (multiplayerSettings.delayStart)
        {
            if (playersInGame == 1)
                RestartTimer();
        }

        if (!isGameLoaded)
        {
            if (readyToStart)
            {
                atMaxPlayers -= Time.deltaTime;
                lessThanMaxPlayers = atMaxPlayers;
                timeToStart = atMaxPlayers;
            }
            else if (readyToCount)
            {
                lessThanMaxPlayers -= Time.deltaTime;
                timeToStart = lessThanMaxPlayers;
            }

            if (timeToStart <= 0)
                StartGame();
        }
    }

    private void RestartTimer()
    {
        lessThanMaxPlayers = startingTime;
        timeToStart = startingTime;
        atMaxPlayers = 6;
        readyToCount = false;
        readyToStart = false;
    }

    private void StartGame()
    {
        isGameLoaded = true;
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (multiplayerSettings.delayStart)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
        PhotonNetwork.LoadLevel(multiplayerSettings.multiplayerScene);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log("New player connected!");
        players = PhotonNetwork.PlayerList;
        playersInGame++;

        if (multiplayerSettings.delayStart)
        {
            if (playersInRoom > 1)
                readyToCount = true;

            if (playersInRoom == multiplayerSettings.maxPlayers)
            {
                readyToStart = true;

                if (!PhotonNetwork.IsMasterClient)
                    return;

                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        Debug.Log("Player left the room");
        players = PhotonNetwork.PlayerList;
        playersInGame--;

    }

    [PunRPC]
    private void RPC_LoadedGameScene()
    {
        playersInGame++;
        if (playersInGame == PhotonNetwork.PlayerList.Length)
        {
            PV.RPC("RPC_CreatePlayer", RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPC_CreatePlayer()
    {
        MyPlayerController = PhotonNetwork.Instantiate(Path.Combine("Prefabs", "Player"), transform.position, Quaternion.identity, 0).GetComponent<PlayerController>();
    }
}