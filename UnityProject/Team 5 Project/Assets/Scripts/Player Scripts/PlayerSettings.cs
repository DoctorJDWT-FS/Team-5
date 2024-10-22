using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSettings : MonoBehaviour
{

    public Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>();

    [SerializeField] public TMP_Text sprint, slide, dash, jump, moveForward, moveBackward, strafeLeft, strafeRight, enter, exit,
        up, down, left, right, shoot, aim, reload, interact, punch, grenade, displayPlayerStats;

    private GameObject currkey;
    // Start is called before the first frame update
    void Start()
    {
        keys.Add("sprint", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("sprint", KeyCode.LeftShift.ToString())));
        keys.Add("slide", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("slide", KeyCode.C.ToString())));
        keys.Add("dash", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("dash", KeyCode.LeftControl.ToString())));
        keys.Add("jump", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("jump", KeyCode.Space.ToString())));
        keys.Add("moveForward", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("moveForward", KeyCode.W.ToString())));
        keys.Add("moveBackwards", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("moveBackward", KeyCode.S.ToString())));
        keys.Add("strafeLeft", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("strafeLeft", KeyCode.A.ToString())));
        keys.Add("strafeRight", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("strafeRight", KeyCode.D.ToString())));
        keys.Add("enter", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("enter", KeyCode.Return.ToString())));
        keys.Add("exit", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("exit", KeyCode.Escape.ToString())));
        keys.Add("up", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("up", KeyCode.W.ToString())));
        keys.Add("down", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("down", KeyCode.S.ToString())));
        keys.Add("right", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("right", KeyCode.D.ToString())));
        keys.Add("left", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("left", KeyCode.A.ToString())));
        keys.Add("displayPlayerStats", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("displayPlayerStats", KeyCode.Tilde.ToString()))); // Load stat menu key
        keys.Add("shoot", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("shoot", KeyCode.Mouse0.ToString())));
        keys.Add("aim", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("aim", KeyCode.Mouse1.ToString())));
        keys.Add("reload", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("reload", KeyCode.R.ToString())));
        keys.Add("interact", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("interact", KeyCode.E.ToString())));
        keys.Add("punch", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("punch", KeyCode.V.ToString()))); // Load punch key
        keys.Add("grenade", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("grenade", KeyCode.G.ToString()))); // Load grenade key

        if (sprint != null) sprint.text = keys["sprint"].ToString();
        if (slide != null) slide.text = keys["slide"].ToString();
        if (dash != null) dash.text = keys["dash"].ToString();
        if (jump != null) jump.text = keys["jump"].ToString();
        if (moveForward != null) moveForward.text = keys["moveForward"].ToString();
        if (moveBackward != null) moveBackward.text = keys["moveBackwards"].ToString();
        if (strafeLeft != null) strafeLeft.text = keys["strafeLeft"].ToString();
        if (strafeRight != null) strafeRight.text = keys["strafeRight"].ToString();
        if (enter != null) enter.text = keys["enter"].ToString();
        if (exit != null) exit.text = keys["exit"].ToString();
        if (up != null) up.text = keys["up"].ToString();
        if (down != null) down.text = keys["down"].ToString();
        if (left != null) left.text = keys["left"].ToString();
        if (right != null) right.text = keys["right"].ToString();
        if (displayPlayerStats != null) displayPlayerStats.text = keys["displayPlayerStats"].ToString();
        if (shoot != null) shoot.text = keys["shoot"].ToString();
        if (aim != null) aim.text = keys["aim"].ToString();
        if (reload != null) reload.text = keys["reload"].ToString();
        if (interact != null) interact.text = keys["interact"].ToString();
        if (punch != null) punch.text = keys["punch"].ToString();
        if (grenade != null) grenade.text = keys["grenade"].ToString();
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
