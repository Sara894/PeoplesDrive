using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Car Settings")]
    public float speed = 10f;
    public float turnSpeed = 50f;

    [Header("Acceleration & Deceleration")]
    public float acceleration = 5f;
    public float deceleration = 2f;
    public float brakeDeceleration = 8f;

    [Header("Input Actions")]
    public InputActionReference moveAction;
    public InputActionReference brakeAction;

    [SerializeField] private Rigidbody rb;
    private Vector2 moveInput;

    private float currentSpeed = 0f;
    private bool isBraking = false;

    private void OnEnable()
    {
        moveAction.action.Enable();
        moveAction.action.performed += OnMove;
        moveAction.action.canceled += OnMove;

        brakeAction.action.Enable();
        brakeAction.action.performed += OnBrake;
        brakeAction.action.canceled += OnBrake;
    }

    private void OnDisable()
    {
        moveAction.action.performed -= OnMove;
        moveAction.action.canceled -= OnMove;
        moveAction.action.Disable();

        brakeAction.action.performed -= OnBrake;
        brakeAction.action.canceled -= OnBrake;
        brakeAction.action.Disable();
    }

    private void FixedUpdate()
    {
        float targetSpeed = moveInput.y * speed;

        if (!isBraking)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);

            if (moveInput.y == 0)
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.fixedDeltaTime);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, brakeDeceleration * Time.fixedDeltaTime);
        }

        Vector3 movement = transform.forward * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        float turn = moveInput.x * turnSpeed * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turn, 0f));
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnBrake(InputAction.CallbackContext context)
    {
        isBraking = context.performed;
    }
}
