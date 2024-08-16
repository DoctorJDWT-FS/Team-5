using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{
    [SerializeField] int sens = 100;
    [SerializeField] int lockVertMin = -60, lockVertMax = 60;
    [SerializeField] public bool invertY = false;

    float rotX;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Set the camera to ignore the PlayerHead and PlayerTorso layers
        Camera.main.cullingMask = ~(LayerMask.GetMask("Player Head", "Player Torso"));
    }

    // Update is called once per frame
    void Update()
    {
        // Normal camera controls
        float mouseY = Input.GetAxis("Mouse Y") * sens * Time.deltaTime;
        float mouseX = Input.GetAxis("Mouse X") * sens * Time.deltaTime;

        if (invertY)
            rotX += mouseY;
        else
            rotX -= mouseY;

        rotX = Mathf.Clamp(rotX, lockVertMin, lockVertMax);

        // Rotate the camera on the x-axis
        transform.localRotation = Quaternion.Euler(rotX, 0, 0);

        // Rotate the PLAYER on the y-axis
        transform.parent.Rotate(Vector3.up * mouseX);
    }

    public void SetSensitivity(int newSensitivity)
    {
        sens = newSensitivity;
    }
}
