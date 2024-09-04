using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class rotateObject : MonoBehaviour
{
    // the degree it will rotate each second 
    [SerializeField] float degreeRotation;
    //the rotation speed modifer 
    [SerializeField] float rotationSpeed;
    void Update()
    {
        float step = degreeRotation * Time.deltaTime * rotationSpeed;
        //sets new y rotation from above
        Quaternion newRotation = Quaternion.Euler(0, step, 0);
        //rotates on the Y axis
        transform.rotation = newRotation * transform.rotation;
    }
}
