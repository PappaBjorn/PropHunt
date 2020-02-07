using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using Random = UnityEngine.Random;

public class Lobby : MonoBehaviourPunCallbacks
{
   public event Action OnClickProps = () => { };
   public event Action OnClickHunters = () => { };

   public bool failedJoining = false;
   
   private static Lobby lobby;

   [SerializeField] private Transform playerListPanel;
   [SerializeField] private GameObject playerListPrefab;
   [SerializeField] private GameObject joinLobbyButton;
   [SerializeField] private GameObject cancelLobbyButton;
   [SerializeField] private GameObject propPlayerButton;
   [SerializeField] private GameObject hunterPlayerButton;
   [SerializeField] private int maxPlayers = 4;

   private void Awake()
   {
      lobby = this;
   }

   private void Start()
   {
      PhotonNetwork.ConnectUsingSettings();
   }

   public override void OnConnectedToMaster()
   {
      joinLobbyButton.SetActive(true);
      PhotonNetwork.AutomaticallySyncScene = true;
   }

   public void FindGamesButton()
   {
       joinLobbyButton.SetActive(false);
       propPlayerButton.SetActive(true);
       hunterPlayerButton.SetActive(true);
       cancelLobbyButton.SetActive(true);
   }
   
   public void CancelFindGamesButton()
   {
      cancelLobbyButton.SetActive(false);
      joinLobbyButton.SetActive(true);
      PhotonNetwork.LeaveRoom();
   }

   public override void OnJoinedRoom()
   {
      UpdatePlayerList();
      joinLobbyButton.SetActive(false);
      cancelLobbyButton.SetActive(true);
      Debug.Log("In room");
   }

   public void JoinPropsButton()
   {
      propPlayerButton.SetActive(false);
      hunterPlayerButton.SetActive(false);
      Debug.Log("Joining Props");
      PhotonNetwork.JoinRandomRoom();
      PlayerPrefs.SetString("PlayerType", "PlayerAvatar");
   }

   public void JoinHuntersButton()
   {
      propPlayerButton.SetActive(false);
      hunterPlayerButton.SetActive(false);
      Debug.Log("Joining Hunters");
      PhotonNetwork.JoinRandomRoom();
      PlayerPrefs.SetString("PlayerType", "PlayerHunter");
   }

   private void UpdatePlayerList()
   {
      Player[] players = PhotonNetwork.PlayerList;

      foreach (Transform child in playerListPanel)
      {
         Destroy(child);
      }

      foreach (Player player in players)
      {
         GameObject go = Instantiate(playerListPrefab, Vector3.zero, Quaternion.identity, playerListPanel);
         TextMeshProUGUI playerText = go.GetComponentInChildren<TextMeshProUGUI>();
         playerText.text = player.NickName;
      }
      
   }
   

   public override void OnJoinRandomFailed(short returnCode, string message)
   {

      Debug.Log("Failed to join a room");
      failedJoining = true;
      Debug.Log("Creating room");
      int randomRoomNumber = Random.Range(0, 10000);
      RoomOptions roomOptions = new RoomOptions() {IsVisible = true, IsOpen = true, MaxPlayers = (byte) MultiplayerSettings.GetInstance().maxPlayers};
      roomOptions.CleanupCacheOnLeave = true;
      PhotonNetwork.CreateRoom("Room" + randomRoomNumber, roomOptions);
      Debug.Log("Room Create: " + randomRoomNumber);
      failedJoining = false;
   }

   public static Lobby GetInstance()
   {
      return lobby;
   }
}
