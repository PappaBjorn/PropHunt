using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private string defaultObject;
    public GameObject Avatar;
    public GameObject Prop;
    public bool characterSelected = false;

    private float testTimer = 3;
    private PhotonView PV;
    public PhotonView PV_Avatar;
    private string myRole;
    private Camera MyCam;


    private void Start()
    {
        PV = GetComponent<PhotonView>();
        if (PhotonNetwork.IsMasterClient)
            RoundSystem.GetInstance().enabled = true;
        
        if (PV.IsMine)
        {
            myRole = PlayerPrefs.GetString("PlayerType");

            if (myRole == "PlayerAvatar")
            {
                PlayerUIManager.GetInstance().SetServerRoleText(true);
            }
            else
            {
                PlayerUIManager.GetInstance().SetServerRoleText(false);
            }

        }
        PV.RPC("RPC_NewPlayerInfo", RpcTarget.MasterClient, PV.ViewID, myRole == "PlayerAvatar");
    }

    [PunRPC]
    void RPC_NewPlayerInfo(int viewID, bool prop)
    {
        RoundSystem.GetInstance().NewPlayerInfo(viewID, prop);
    }

    [PunRPC]
    void RPC_ChangeRole(int ViewID, bool prop)
    {
        RoundSystem.GetInstance().PlayerRoleChange(ViewID, prop);
    }

    public void SpawnPlayer()
    {
        if (PV.IsMine && !characterSelected)
        {
            Transform spawnTrans = GameSetup.GetInstance().SpawnPoints[Room.GetInstance().myPlayerID];

            Avatar = PhotonNetwork.Instantiate(Path.Combine("Prefabs", myRole), spawnTrans.position,
                spawnTrans.rotation, 0);
            

            PV_Avatar = Avatar.GetComponent<PhotonView>();

            PV.RPC("RPC_SetAvatar", RpcTarget.AllBuffered, PV.ViewID, PV_Avatar.ViewID);

            if (myRole == "PlayerAvatar")
                characterSelected = true;

            PV_Avatar.transform.GetChild(0).gameObject.SetActive(true); //TURN ON OUR CAMERA OBJECT BADDD

            PlayerMovement temp = Avatar.GetComponent<PlayerMovement>();
            temp.PV_Owner = PV;

            if (myRole != "PlayerAvatar")
                temp.HunterWaiting = true;
            
            Avatar.GetComponentInChildren<Camera>().enabled = true;
        }
    }


    private void Update()
    {
        if (!PV.IsMine)
            return;
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Player[] players = PhotonNetwork.PlayerList;

                foreach (var VARIABLE in players)
                {
                    PhotonNetwork.CloseConnection(VARIABLE);
                }
            }

            Destroy(Room.GetInstance().gameObject);
            PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.LoadLevel(0);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (myRole == "PlayerAvatar")
            {
                PlayerUIManager.GetInstance().SetServerRoleText(false);
                myRole = "PlayerHunter";
            }
            else
            {
                PlayerUIManager.GetInstance().SetServerRoleText(true);
                myRole = "PlayerAvatar";
            }
            PV.RPC("RPC_ChangeRole", RpcTarget.MasterClient, PV.ViewID, myRole == "PlayerAvatar");
        }

        if (characterSelected)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
                TryToSetProp();
        }
    }

    private void TryToSetProp()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 6f, LayerMask.GetMask("Prop")))
        {
            string newPropName = hit.collider.name;
            Debug.Log(newPropName);
            GameObject newProp = Resources.Load<GameObject>("Prefabs/Props/" + newPropName);
            if (newProp)
            {
                PV.RPC("RPC_SetProp", RpcTarget.AllBuffered, PV.ViewID, newPropName);
            }
        }
    }

    [
        PunRPC]
    void RPC_SetProp(int id, string prop)
    {
        PlayerController Player = PhotonNetwork.GetPhotonView(id).GetComponent<PlayerController>();
        CharacterController capsuleCollider = Player.PV_Avatar.GetComponent<CharacterController>();
        Player.PV_Avatar.gameObject.GetComponent<MeshRenderer>().enabled = false;

        if (Player.Prop)
            Destroy(Player.Prop);

        Player.Prop =
            Instantiate(Resources.Load<GameObject>("Prefabs/Props/" + prop),
                Player.PV_Avatar.transform.position, Quaternion.identity, Player.PV_Avatar.transform);

        Player.Prop.GetComponent<Collider>().enabled = false;
        Renderer rend = Player.Prop.GetComponent<Renderer>();

        Player.Prop.layer = 8;
        capsuleCollider.center = Player.Prop.transform.InverseTransformPoint(rend.bounds.center);
        capsuleCollider.height = rend.bounds.size.y;
        capsuleCollider.radius = rend.bounds.size.z / 2f;

        Player.PV_Avatar.GetComponent<PlayerMovement>().UpdateMeshHealth(rend);

        Vector3 newPos = Player.transform.position;
        RaycastHit hit;

        if (Physics.Raycast(newPos, Vector3.down, out hit, LayerMask.GetMask("Default")))
        {
            newPos.y = hit.point.y + rend.bounds.size.y / 2f;
        }

        Player.transform.position = newPos;
    }

    [
        PunRPC]
    void RPC_SetAvatar(int id, int AvatarID)
    {
        PlayerController Player = PhotonNetwork.GetPhotonView(id).GetComponent<PlayerController>();
        Player.PV_Avatar = PhotonNetwork.GetPhotonView(AvatarID);
    }
}