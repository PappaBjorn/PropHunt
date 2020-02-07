using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class CameraFollowThirdPerson : MonoBehaviour
{
    [SerializeField] private GameObject Player;
    [SerializeField] private LayerMask PlayerLayerMask;
    [SerializeField] private float distance = 2.0f;
    [SerializeField] private float xMouseSensitivity = 2.0f;
    [SerializeField] private float yMouseSensitivity = 3.0f;
    [SerializeField] private float preferedDistance = 3.0f;
    [SerializeField] private float yMinLimit = -90f;
    [SerializeField] private float yMaxLimit = 90f;
    [SerializeField] private float distanceMin = 10f;
    [SerializeField] private float distanceMax = 10f;
    [SerializeField] private float ZoomSpeed = 2f;
    float rotationYAxis = 0.0f;
    private PhotonView PV;

    float rotationXAxis = 0.0f;
    float velocityX = 0.0f;
    float velocityY = 0.0f;
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        preferedDistance = distance;
        //Cursor.visible = false;
        PV = GetComponentInParent<PhotonView>();


    }
    public void Start()
    {

        //  transform.position = Player.GetComponent<Renderer>().bounds.center;
    }
    public void UpdatePlayerPosition(Transform playerTransform)
    {
        transform.position = playerTransform.GetComponent<Renderer>().bounds.center;
    }
    private void LateUpdate()
    {
        if (Player.transform && PV.IsMine)
        {
            
            velocityX += xMouseSensitivity * Input.GetAxis("Mouse X");
            velocityY += yMouseSensitivity * Input.GetAxis("Mouse Y");

            rotationYAxis += velocityX;
            rotationXAxis -= velocityY;
            rotationXAxis = ClampAngle(rotationXAxis, yMinLimit, yMaxLimit);
            Quaternion fromRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
            Quaternion toRotation = Quaternion.Euler(rotationXAxis, rotationYAxis, 0);
            Quaternion rotation = toRotation;//Quaternion.Lerp(fromRotation,toRotation,Time.deltaTime);
            if (Input.GetAxis("Mouse ScrollWheel") > 0 || Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);
                preferedDistance = distance;

            }
            //new Vector3(Player.transform.position.x, Player.transform.position.y + (Player.GetComponent<Renderer>().bounds.size.y / 2), Player.transform.position.z)
            RaycastHit hit;
            Debug.DrawLine(transform.position, Player.transform.position, Color.red);

            if (Physics.Linecast(Player.transform.position, transform.position, out hit, PlayerLayerMask))
            {
                distance = hit.distance;
            }
            else
            {
               
                if(distance < preferedDistance && preferedDistance < distanceMax)
                distance += (Time.deltaTime * ZoomSpeed);


            }

            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + Player.transform.position;

            transform.rotation = rotation;
            transform.position = position;

            velocityX = velocityX * Time.deltaTime;
            velocityY = velocityY * Time.deltaTime;
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
