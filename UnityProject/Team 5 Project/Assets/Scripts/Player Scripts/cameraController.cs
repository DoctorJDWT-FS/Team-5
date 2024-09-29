using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class cameraController : MonoBehaviour
{
    [SerializeField] int sens = 100;
    [SerializeField] int lockVertMin = -60, lockVertMax = 60;
    [SerializeField] public bool invertY = false;

    float rotX;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Camera.main.cullingMask = ~(LayerMask.GetMask("Player Head", "Player Torso"));

        applySettings();
    }

    void Update()
    {
        float mouseY = Input.GetAxis("Mouse Y") * sens * Time.deltaTime;
        float mouseX = Input.GetAxis("Mouse X") * sens * Time.deltaTime;

        if (invertY)
        {
            rotX += mouseY;
        }
        else
        {
            rotX -= mouseY;
        }

        rotX = Mathf.Clamp(rotX, lockVertMin, lockVertMax);

        if (SceneManager.GetActiveScene().buildIndex >= 1)
        {
            transform.localRotation = Quaternion.Euler(rotX, 0, 0);
            transform.parent.Rotate(Vector3.up * mouseX);
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    public void SetSensitivity(int newSensitivity)
    {
        sens = newSensitivity;
    }

    private void applySettings()
    {
        sens = PlayerPrefs.GetInt("Sensitivity", sens);
        invertY = PlayerPrefs.GetInt("InvertY", invertY ? 1 : 0) == 1;
    }
}
