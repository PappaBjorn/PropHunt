using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


[AddComponentMenu("Camera-Control Hunter")]
public class PlayerCameraControllerHunter : MonoBehaviour
{
    public float mouseSensitivity = 1.0f;
    private float horizontal;
    private float vertical;
    private CharacterController controller;
    private PhotonView PV;
    public PhotonView PV_Avatar;
    MeshRenderer MyMesh;
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponentInParent<CharacterController>();
        MyMesh = GetComponentInParent<MeshRenderer>();
        PV = GetComponentInParent<PhotonView>();

        if (PV.IsMine)
            MyMesh.enabled = false;
        else
            MyMesh.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(PV.IsMine)
        UpdateRotation();
    }

    void UpdateRotation()
    {
        horizontal = Input.GetAxis("Mouse X") * mouseSensitivity;
        vertical -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        vertical = Mathf.Clamp(vertical, -90, 90);
        
        controller.transform.Rotate(0, horizontal, 0);
        Camera.main.transform.localRotation = Quaternion.Euler(vertical, 0, 0);
    }
}
