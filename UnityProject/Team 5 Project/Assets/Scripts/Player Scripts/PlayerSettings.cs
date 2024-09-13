using System.Collections;
using System.Collections.Generic;
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

    [Header("----- Combat Controls -----")]
    public KeyCode shoot = KeyCode.Mouse0;
    public KeyCode aim = KeyCode.Mouse1;
    public KeyCode reload = KeyCode.R;
    public KeyCode interact = KeyCode.E;

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
    }

    public void SaveSettings()
    {
        // Save each setting to player preferences
        PlayerPrefs.SetString("sprint", sprint.ToString());
        PlayerPrefs.SetString("slide", slide.ToString());
        PlayerPrefs.SetString("dash", dash.ToString());
        PlayerPrefs.SetString("jump", jump.ToString());
        PlayerPrefs.SetString("moveForward", moveForward.ToString());
        PlayerPrefs.SetString("moveBackward", moveBackward.ToString());
        PlayerPrefs.SetString("strafeLeft", strafeLeft.ToString());
        PlayerPrefs.SetString("strafeRight", strafeRight.ToString());
        PlayerPrefs.SetString("enter", enter.ToString());
        PlayerPrefs.SetString("exit", exit.ToString());
        PlayerPrefs.SetString("up", up.ToString());
        PlayerPrefs.SetString("down", down.ToString());
        PlayerPrefs.SetString("left", left.ToString());
        PlayerPrefs.SetString("right", right.ToString());
        PlayerPrefs.SetString("shoot", shoot.ToString());
        PlayerPrefs.SetString("aim", aim.ToString());
        PlayerPrefs.SetString("reload", reload.ToString());
        PlayerPrefs.SetString("interact", interact.ToString());

        // Ensure the settings are saved
        PlayerPrefs.Save();
    }
}