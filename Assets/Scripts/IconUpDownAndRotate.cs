using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconUpDownAndRotate : MonoBehaviour
{
    [Header("Set Rotating")]
    [Tooltip("Check this to Rotate")]
    public bool checkToRotate;
    public float rotateSpeed = 15.0f;

    [Header("Set Up/Down Motion")]
    [Tooltip("Check this to Float")]
    public bool checkToFloat;
    public float height = 0.5f;
    public float upDownSpeed = 1f;

    // Position Storage Variables
    Vector3 startingPosition = new Vector3();
    Vector3 tempPosition = new Vector3();

    void OnEnable()
    {
        startingPosition = transform.position;
    }

    void Start()
    {
        startingPosition = transform.position;
    }

    void Update()
    {
        
        UpDownRotate();  
    }

    void UpDownRotate()
    {
        if (checkToRotate)
        {
            transform.Rotate(new Vector3(0f, Time.deltaTime * rotateSpeed, 0f), Space.World);
        }

        if (checkToFloat)
        {
            tempPosition = startingPosition;
            tempPosition.y += Mathf.Sin(Time.fixedTime * Mathf.PI * upDownSpeed) * height;
            transform.position = tempPosition;
        } 
    }

}
