using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerSettings : MonoBehaviour
{
    [Header("----- Movement Controls -----")]
    public KeyCode sprint = KeyCode.LeftShift;
    public KeyCode slide = KeyCode.C;
    public KeyCode dash = KeyCode.LeftControl;
    public KeyCode jump = KeyCode.Space;
    public KeyCode moveForward = KeyCode.W;
    public KeyCode moveBackward = KeyCode.S;
    public KeyCode strafeLeft = KeyCode.A;
    public KeyCode strafeRight = KeyCode.D;

    [Header("----- Menu Navigation Controls -----")]
    public KeyCode enter = KeyCode.Return;
    public KeyCode exit = KeyCode.Escape;
    public KeyCode up = KeyCode.W;
    public KeyCode down = KeyCode.S;
    public KeyCode left = KeyCode.A;
    public KeyCode right = KeyCode.D;
    public KeyCode displayPlayerStats = KeyCode.Tilde; // added stat screen menu

    [Header("----- Combat Controls -----")]
    public KeyCode shoot = KeyCode.Mouse0;
    public KeyCode aim = KeyCode.Mouse1;
    public KeyCode reload = KeyCode.R;
    public KeyCode interact = KeyCode.E;
    public KeyCode punch = KeyCode.V; // Added punch key
    public KeyCode grenade = KeyCode.G; // Added grenade key



    void Start()
    {
        // Load settings from player preferences
        LoadSettings();
    }

    public void LoadSettings()
    {
        // Load each setting from player preferences, or use the default if not set
        sprint = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("sprint", sprint.ToString()));
        slide = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("slide", slide.ToString()));
        dash = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("dash", dash.ToString()));
        jump = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("jump", jump.ToString()));
        moveForward = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("moveForward", moveForward.ToString()));
        moveBackward = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("moveBackward", moveBackward.ToString()));
        strafeLeft = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("strafeLeft", strafeLeft.ToString()));
        strafeRight = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("strafeRight", strafeRight.ToString()));
        enter = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("enter", enter.ToString()));
        exit = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("exit", exit.ToString()));
        up = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("up", up.ToString()));
        down = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("down", down.ToString()));
        left = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("left", left.ToString()));
        right = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("right", right.ToString()));
        shoot = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("shoot", shoot.ToString()));
        aim = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("aim", aim.ToString()));
        reload = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("reload", reload.ToString()));
        interact = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("interact", interact.ToString()));
        punch = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("punch", punch.ToString())); // Load punch key
        grenade = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("grenade", grenade.ToString())); // Load grenade key
        displayPlayerStats = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("displayPlayerStats", displayPlayerStats.ToString())); // Load stat menu key
    }


}
