using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeNickname : MonoBehaviour
{
    public string newNickname;
    InputField iField;
    public void Start()
    {
        iField = GetComponent<InputField>();
    }
    public void ChangeNicknameFunction()
    {
        GameObject.Find("GameManager").GetComponent<Room>().ChangeNickName(iField.text);
    }
    
}
