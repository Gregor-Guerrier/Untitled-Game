using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Binds Profile", menuName = "Keybinds/Profiles", order = 1)]

[System.Serializable]
public class Keybinds {
        public KeyCode primaryBind;
        public KeyCode altBind;
    }
public class KeybindManager : ScriptableObject
{   
    [Header ("Profile Details")]
    public string profileName;
    [Header("Movement")]
    public Keybinds forward;
    public Keybinds backward;
    public Keybinds left;
    public Keybinds right;
    public Keybinds sprint;
    public Keybinds jump;
    public Keybinds crouch;
    public Keybinds prone;
    public Keybinds leanLeft;
    public Keybinds leanRight;

    [Header("Weapons and Utility")]
    public Keybinds shoot;
    public Keybinds aimDownSights;
    public Keybinds mount;
    public Keybinds firemode;
    public Keybinds reload;
    public Keybinds useGrenade;


}
