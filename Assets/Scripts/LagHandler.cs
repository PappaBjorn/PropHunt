using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class LagHandler : MonoBehaviour
{
    public GameObject playerAvatar;
    public PlayerMovement playerMoveComp;
    public Vector3 networkPosition;
    public Quaternion networkRotation;

    private PhotonView PV;

    private void Start()
    {
        PV = GetComponent<PhotonView>();
        playerAvatar = GameObject.FindWithTag("Player");
        playerMoveComp = playerAvatar.GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (!PV.IsMine)
        {
            playerAvatar.transform.position = Vector3.MoveTowards(playerAvatar.transform.position, networkPosition,
                Time.deltaTime * playerMoveComp.moveSpeed);
            playerAvatar.transform.rotation = Quaternion.RotateTowards(playerAvatar.transform.rotation, networkRotation,
                Time.deltaTime * 100);
            return;
        }
        
        Vector3 oldPosition = playerAvatar.transform.position;
        Vector3 movement = playerAvatar.transform.position - oldPosition;
    }

    public void OnPhotonSerilizeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(playerAvatar.transform.position);
            stream.SendNext(playerAvatar.transform.rotation);
            stream.SendNext(playerMoveComp.moveVector);
        }
        else
        {
            networkPosition = (Vector3) stream.ReceiveNext();
            networkRotation = (Quaternion) stream.ReceiveNext();
            playerMoveComp.moveVector = (Vector3) stream.ReceiveNext();

            float lag = Mathf.Abs((float) (PhotonNetwork.Time - info.timestamp));
            networkPosition += (playerMoveComp.moveVector * lag);
        }
    }
}
