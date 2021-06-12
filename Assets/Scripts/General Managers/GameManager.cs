using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Range(60, 90)]
    public int fov;

    private Camera playerCamera;
    private Camera gunCamera;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        if(SceneManager.GetActiveScene().name == "Test Scene"){
            playerCamera = GameObject.Find("Reg Camera").GetComponent<Camera>();
            gunCamera = GameObject.Find("Gun Camera").GetComponent<Camera>();
            playerCamera.fieldOfView = fov;
        }
        
    }
    public KeybindManager keybindManager;
}
