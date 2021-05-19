using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private bool released;
    private KeyCode tempKeyCode;
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        if(!Input.GetKey(tempKeyCode))
        {
            released = true;
            tempKeyCode = KeyCode.None;
        }
    }
    public KeybindManager keybindManager;

    public bool GetKey(KeyCode keyCode)
    {
        if(Input.GetKey(keyCode))
        {
            return true;
        }
        return false;
    }

    public bool GetKeyDown(KeyCode keyCode)
    {
        tempKeyCode = keyCode;
        if(Input.GetKey(keyCode) && released == true)
        {
            released = false;
            return true;
        }
        return false;
    }
    
}
