using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] float speed = 25f;
    [SerializeField] float rotationSpeed = 15;
    private Vector3 movementInputsY;
    private Vector3 movementInputsX;

    private float originalSpeed;
    private float originalRotationSpeed;
    private bool isMovementLocked = false;

    void Start()
    {
        originalSpeed = speed;
        originalRotationSpeed = rotationSpeed;

        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.playerEvents.onDisablePlayerMovement += StopMovement;
            GameEventsManager.instance.playerEvents.onEnablePlayerMovement += ResumeMovement;
            Debug.Log("Movement: Subscribed to player movement events");
        }
    }

    void OnDestroy()
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.playerEvents.onDisablePlayerMovement -= StopMovement;
            GameEventsManager.instance.playerEvents.onEnablePlayerMovement -= ResumeMovement;
        }
    }

    void Update()
    {
        MovementLogic();
        RotationLogic();
        MovementInput();
    }
    
    public void MovementLogic()
    {
        transform.Translate(movementInputsY.normalized * speed * Time.deltaTime, Space.Self);
    }

    public void RotationLogic()
    {
       transform.Rotate(0, movementInputsX.normalized.x * rotationSpeed * Time.deltaTime, 0);
    }

    public void MovementInput()
    {
        movementInputsY = new Vector3(0, 0, Input.GetAxisRaw("Vertical"));
        movementInputsX = new Vector3(Input.GetAxisRaw("Horizontal"), 0, 0);
    }

    public void StopMovement()
    {
        if (!isMovementLocked)
        {
            originalSpeed = speed;
            originalRotationSpeed = rotationSpeed;
            isMovementLocked = true;
        }

        speed = 0f;
        rotationSpeed = 0f;
        Debug.Log("Movement stopped! Vehicle locked in place.");
    }

    public void ResumeMovement()
    {
        if (isMovementLocked)
        {
            speed = originalSpeed;
            rotationSpeed = originalRotationSpeed;
            isMovementLocked = false;
            Debug.Log($"Movement resumed! Speed: {speed}, Rotation: {rotationSpeed}");
        }
    }
}
