using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

//Settings Menu Items
[System.Serializable]
public class SliderSettings {
        public Text settingText;
        public Slider settingSlider;
}
[System.Serializable]
public class BoolSettings {
        public Text settingText;
        public Slider settingBool;
}
[System.Serializable]
public class DropdownSettings {
        public Text settingText;
        public Slider settingDropdown;
}

//Actual Class
public class GameManager : MonoBehaviour
{

    private Camera playerCamera;

    [Header("Pause Menu")]
    public GameObject pauseMenu;
    public GameObject[] pauseMenuItems;

    [Header("Settings")]
    public SliderSettings fovSetting;
    public SliderSettings sensSetting;
    public SliderSettings volumeSetting;
    public AudioMixer volumeMixer;

    private bool inGame;
    public bool menuOpen;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    //Get The Settings
    private void Start()
    {
        
        fovSetting.settingSlider.value = PlayerPrefs.GetFloat("FOV");
        sensSetting.settingSlider.value = PlayerPrefs.GetFloat("Sensitivity");
        volumeSetting.settingSlider.value = PlayerPrefs.GetFloat("Volume");
    }
    private void Update()
    {
        //Saving Settings
        PlayerPrefs.SetFloat("FOV", fovSetting.settingSlider.value);
        PlayerPrefs.SetFloat("Sensitivity", sensSetting.settingSlider.value);
        PlayerPrefs.SetFloat("Volume", volumeSetting.settingSlider.value);

        //Camera settings
        if(playerCamera != null){
            playerCamera.GetComponent<MouseLook>().enabled = !menuOpen;
            playerCamera.GetComponent<MouseLook>().mouseSensitivity = Mathf.Round(sensSetting.settingSlider.value*10f)/10f;
            playerCamera.fieldOfView = Mathf.Round(fovSetting.settingSlider.value*10f)/10f;
        }
        menuOpen = pauseMenu.activeInHierarchy;
        //Volume settings
        volumeMixer.SetFloat("volume", -80f + volumeSetting.settingSlider.value*(.9f));
        //Locking the mouse
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

    public void MenuSwitch(GameObject Menu){
        for(int i = 0; i < pauseMenuItems.Length; i++){
            if(pauseMenuItems[i] != Menu){
                pauseMenuItems[i].SetActive(false);
            } else {
                pauseMenuItems[i].SetActive(true);
            }
        }
    }
    public KeybindManager keybindManager;
}
