using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, IHealth
{
    public PhotonView PV_Owner;
    private PhotonView PV;
    private CharacterController movement;
    private Camera mainCamera;
    public HealthComponent HealthComp;
    public float moveSpeed;
    public float jumpForce = 5;
    public float rotationSpeed;

    public bool HunterWaiting = false;

    public Vector3 moveVector = new Vector3();


    private void Start()
    {
        PV = GetComponent<PhotonView>();
        movement = GetComponent<CharacterController>();
        mainCamera = FindObjectOfType<Camera>();
        HealthComp.Health = HealthComp.MaxHealth;
    }

    private void Update()
    {
        if (!PV.IsMine)
            return;
        
        if(HunterWaiting)
            return;
        
        BasicMovement();

        if (Input.GetKeyDown(KeyCode.T))
            TakeDamage(10f);
        
        if(HealthComp.Health <= 0)
            Death();
    }

    [PunRPC]
    void SendHealthToOthers(int pv, float currentHp)
    {
        PhotonView temp = PhotonNetwork.GetPhotonView(pv);

        if (temp)
        {
            PlayerMovement x = temp.GetComponent<PlayerMovement>();
            if (x)
                x.HealthComp.Health = currentHp;
        }

    }
    
    public void BasicMovement()
    {
        moveVector = new Vector3(0, moveVector.y, 0);
        
        if (Input.GetKey(KeyCode.W))
        {
            moveVector += transform.forward * moveSpeed;
            BasicRotation();
        }

        else if (Input.GetKey(KeyCode.S))
        {
            moveVector += -transform.forward * moveSpeed;
            BasicRotation();
        }

        if (Input.GetKey(KeyCode.D))
        {
            moveVector += transform.right * moveSpeed;

            BasicRotation();
        }

        else if (Input.GetKey(KeyCode.A))
        {
            moveVector += -transform.right * moveSpeed;
            BasicRotation();
        }
        
        if(movement.isGrounded && Input.GetKeyDown(KeyCode.Space))
            moveVector = new Vector3(moveVector.x, jumpForce, moveVector.z);
        else if (!movement.isGrounded)
            moveVector.y += Physics.gravity.y * Time.deltaTime;
        
        movement.Move(moveVector * Time.deltaTime);
    }

    private void BasicRotation()
    {
        transform.rotation = Quaternion.Euler(0, mainCamera.transform.eulerAngles.y, 0);
    }

    public bool TakeDamage(float amount)
    {
        HealthComp.TakeDamage(amount);

            PV.RPC("SendHealthToOthers", RpcTarget.Others, PV.ViewID, HealthComp.Health);
        return false; 
    }

    public void Death()
    {
        if (PV.IsMine)
        {
            PV.RPC("SendDeath", RpcTarget.MasterClient, Room.GetInstance().myPlayerID);
        
            if(gameObject != null)
                PhotonNetwork.Destroy(gameObject);
        }
        
    }

    [PunRPC]
    void SendDeath(int id)
    {
        if(PhotonNetwork.IsMasterClient)
            RoundSystem.GetInstance().PlayerKilled(id);
    }

    /// <summary>
    /// <param name="Info">Use this when transforming the prop player into a new mesh. </param>
    /// <param name="newMesh">newMesh - the mesh the prop will turn into. </param>
    /// <param name="MeshBonusHealth">MeshBonusHealth - number to multiply with health. </param>
    /// <param name="HealthRegen"> HealthRegen - Bigger object, bigger healthregen?</param>
    /// </summary>
    
    public void UpdateMeshHealth(Renderer newMesh, float MeshBonusHealth = 1, float HealthRegen = 1)
    {
        float TempMissingHealth = 100;
        TempMissingHealth = (HealthComp.MaxHealth / HealthComp.Health); //Ta % hp kvar, om du har 50 hp, 100 max, så vill vi ha 50% hp ifall vi byter med lågt hp inte få fullt hp.

        HealthComp.MaxHealth = (newMesh.bounds.size.magnitude * MeshBonusHealth);
        HealthComp.Health = (HealthComp.MaxHealth * TempMissingHealth);
        HealthComp.HealthRegain = HealthRegen;
        PV.RPC("SendHealthToOthers", RpcTarget.Others, PV.ViewID, HealthComp.Health);
    }

    public void Heal(float amount)
    {

    }

    public void ActivateHunters()
    {
        if (!PV.IsMine && !HunterWaiting)
            return;

        HunterWaiting = false;
        Camera.main.cullingMask = -1; //magic number for everthing
    }

}













































