using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class GetPlayerPanel : MonoBehaviour
{
    public Image playerPanel;
    public Font font;

    public PlayerServerInfo[] _players = new PlayerServerInfo[0];
    private PhotonView PV;
    private static GetPlayerPanel instance;

    private void Awake()
    {
        instance = this;
    }

    public static GetPlayerPanel GetInstance()
    {
        return instance;
    }

    void Start()
    {
        playerPanel = transform.GetChild(0).gameObject.GetComponent<Image>();
       
        //if (_players.Length > (playerPanel.transform.childCount - 1))
        //{
        //    for (int i = 0; i < _players.Length; i++)
        //    {
        //        if (_players.Length > (playerPanel.transform.childCount - 1))
        //        {
        //            GameObject addPlayer = new GameObject();
        //            Text nameOfPlayer = addPlayer.AddComponent<Text>();
        //            nameOfPlayer.text = PhotonNetwork.PlayerList[i].NickName;
        //            nameOfPlayer.fontSize = 14;
        //            nameOfPlayer.font = font;
        //            if (_players[i].Prop)
        //                nameOfPlayer.color = Color.cyan;
        //            else
        //                nameOfPlayer.color = Color.red;
        //            Instantiate(addPlayer, playerPanel.transform);


        //            nameOfPlayer.rectTransform.position = playerPanel.transform.GetChild(playerPanel.transform.childCount - 1).transform.position;
        //            nameOfPlayer.rectTransform.position = new Vector3(addPlayer.transform.position.x, addPlayer.transform.position.y - 15, addPlayer.transform.position.z);
        //        }
        //        else
        //            break;
        //    }
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (_players.Length > (playerPanel.transform.childCount - 1))
            {
                for (int i = 0; i < _players.Length; i++)
                {
                    if (_players.Length > (playerPanel.transform.childCount - 1))
                    {
                        GameObject addPlayer = new GameObject();
                        
                        Text nameOfPlayer = addPlayer.AddComponent<Text>();
                        
                        nameOfPlayer.text = PhotonNetwork.PlayerList[i].NickName;
                        nameOfPlayer.fontSize = 14;
                        nameOfPlayer.font = font;
                        
                        if(_players[i].Prop)
                            nameOfPlayer.color = Color.cyan;
                        else
                            nameOfPlayer.color = Color.red;
                        
                        Instantiate(addPlayer, playerPanel.transform);


                        nameOfPlayer.rectTransform.position = playerPanel.transform.GetChild(playerPanel.transform.childCount - 1).transform.position;
                        nameOfPlayer.rectTransform.position = new Vector3(addPlayer.transform.position.x, addPlayer.transform.position.y - 15, addPlayer.transform.position.z);
                    }
                    else
                        break;
                }
            }
            playerPanel.gameObject.SetActive(true);
         
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            playerPanel.gameObject.SetActive(false);
        }
    }
}
