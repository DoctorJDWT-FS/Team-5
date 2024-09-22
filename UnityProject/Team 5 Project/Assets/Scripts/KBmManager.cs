using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class KBmManager : MonoBehaviour
{
    public Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>();

    public TMP_Text sprint, slide, dash, jump, moveForward, moveBackward, strafeLeft, strafeRight, enter, exit,
        up, down, left, right, shoot, aim, reload, interact, punch, grenade, displayPlayerStats;

    private GameObject currkey;
    // Start is called before the first frame update
    void Start()
    {
        keys.Add("sprint", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("sprint", sprint.ToString())));
        keys.Add("slide", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("slide", slide.ToString())));
        keys.Add("dash", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("dash", dash.ToString())));
        keys.Add("jump", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("jump", jump.ToString())));
        keys.Add("moveForeward", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("moveForward", moveForward.ToString())));
        keys.Add("moveBackwards", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("moveBackward", moveBackward.ToString())));
        keys.Add("strafeLeft", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("strafeLeft", strafeLeft.ToString())));
        keys.Add("strafeRight", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("strafeRight", strafeRight.ToString())));
        keys.Add("enter", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("enter", enter.ToString())));
        keys.Add("exit", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("exit", exit.ToString())));
        keys.Add("up", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("up", up.ToString())));
        keys.Add("down", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("down", down.ToString())));
        keys.Add("right", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("right", right.ToString())));
        keys.Add("left", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("left", left.ToString())));
        keys.Add("displayPlayerStats", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("displayPlayerStats", displayPlayerStats.ToString()))); // Load stat menu key
        keys.Add("shoot", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("shoot", shoot.ToString())));
        keys.Add("aim", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("aim", aim.ToString())));
        keys.Add("reload", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("reload", reload.ToString())));
        keys.Add("interact", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("interact", interact.ToString())));
        keys.Add("punch", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("punch", punch.ToString()))); // Load punch key
        keys.Add("grenade", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("grenade", grenade.ToString()))); // Load grenade key

        sprint.text = keys["sprint"].ToString();
        slide.text = keys["slide"].ToString();
        dash.text = keys["dash"].ToString();
        jump.text = keys["jump"].ToString();
        moveForward.text = keys["moveForward"].ToString();
        moveBackward.text = keys["moveBackwards"].ToString();
        strafeLeft.text = keys["strafeLeft"].ToString();
        strafeRight.text = keys["strafeRight"].ToString();
        enter.text = keys["enter"].ToString();
        exit.text = keys["exit"].ToString();
        up.text = keys["up"].ToString();
        down.text = keys["down"].ToString();
        left.text = keys["left"].ToString();
        right.text = keys["right"].ToString();
        displayPlayerStats.text = keys["displayPlayerStats"].ToString();
        shoot.text = keys["shoot"].ToString();
        aim.text = keys["aim"].ToString();
        reload.text = keys["reload"].ToString();
        interact.text = keys["interact"].ToString();
        punch.text = keys["punch"].ToString();
        grenade.text = keys["grenade"].ToString();

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnGUI()
    {
        if (currkey != null)
        {
            Event e = Event.current;
            if (e.isKey)
            {
                keys[currkey.name] = e.keyCode;
                currkey.transform.GetChild(0).GetComponent<TMP_Text>().text = e.keyCode.ToString();
                currkey = null;
            }
            if (e.isMouse)
            {
                if (e.IsRightMouseButton())
                {
                    keys[currkey.name] = KeyCode.Mouse1;
                    currkey = null;
                }
                if (e.isMouse && !e.IsRightMouseButton())
                {
                    keys[currkey.name] = KeyCode.Mouse0;
                    currkey = null;
                }

            }
        }
    }
    public void ChangeKey(GameObject clicked)
    {
        currkey = clicked;
    }

    public void SaveKeys()
    {
        foreach (var key in keys)
        {
            // Save each setting to player preferences
            PlayerPrefs.SetString(key.Key, key.Value.ToString());
        }

        // Ensure the settings are saved
        PlayerPrefs.Save();
    }
}

