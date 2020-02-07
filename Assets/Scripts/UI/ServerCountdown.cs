using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon;
public class ServerCountdown : MonoBehaviour
{
    public Text CountdownButton;
    [SerializeField] float MaxServerTime = 120;
    public float CurrentTime = 0;
    private PhotonView PV;
    private PhotonView PV_Avatar;
    System.TimeSpan tempTime;

    void Start()
    {
        CountdownButton = GetComponent<Text>();
        CurrentTime = MaxServerTime;
        if (PhotonNetwork.IsMasterClient)
            PV = PhotonNetwork.GetPhotonView(1001);

    }
    void FixedUpdate()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            CurrentTime -= Time.deltaTime;
            tempTime = System.TimeSpan.FromSeconds(CurrentTime);
            CountdownButton.text = tempTime.ToString(@"mm\:ss");
            PV.RPC("ServerTime", RpcTarget.Others, CurrentTime);
        }
        else
        {
            tempTime = System.TimeSpan.FromSeconds(CurrentTime);
            CountdownButton.text = tempTime.ToString(@"mm\:ss");
        }

    }




}
