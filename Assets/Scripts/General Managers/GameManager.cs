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
    public GameObject pauseMenu;

    private bool inGame;
    public bool menuOpen;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        if(playerCamera != null){
            playerCamera.GetComponent<MouseLook>().enabled = !menuOpen;
        }
        menuOpen = pauseMenu.activeInHierarchy;
        if(inGame == false){
            Cursor.lockState = CursorLockMode.Confined;
        }
        if(menuOpen == true && inGame == true){
            Cursor.lockState = CursorLockMode.Confined;
        } else if(menuOpen == false && inGame == true){
            Cursor.lockState = CursorLockMode.Locked;
        }
        if(SceneManager.GetActiveScene().name == "Test Scene"){
            inGame = true;
            playerCamera = GameObject.Find("Reg Camera").GetComponent<Camera>();
            gunCamera = GameObject.Find("Gun Camera").GetComponent<Camera>();
            playerCamera.fieldOfView = fov;
        } else {
            inGame = false;
        }
        
        if(inGame == true){
            if(Input.GetKeyDown(keybindManager.pause.primaryBind) || Input.GetKeyDown(keybindManager.pause.altBind)){
                pauseMenu.SetActive(!pauseMenu.activeInHierarchy);
            }
        } else if(inGame == false){
            pauseMenu.SetActive(false);
        }
    }
    public KeybindManager keybindManager;
}
