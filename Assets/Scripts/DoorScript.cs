using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    private GameObject[] propPlayers;
    private GameObject[] huntPlayers;
    private Collider triggerZone;

    private Vector3 originPos;
    private Vector3 maxPos;
    private Vector3 currentPos;
    
    public float transLength;
    public float doorTime;

    // Start is called before the first frame update
    void Start()
    {
        propPlayers = GameObject.FindGameObjectsWithTag("Prop");
        huntPlayers = GameObject.FindGameObjectsWithTag("Hunter");

        originPos = this.gameObject.transform.position;
        maxPos = originPos;
        maxPos.y += transLength;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OpenDoor()
    {
        if (originPos.y >= (originPos.y + transLength)) 
            this.gameObject.transform.position += Vector3.Lerp(originPos, maxPos, doorTime);
    }
}
