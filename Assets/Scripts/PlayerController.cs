using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Input Actions")]
    public InputActionReference moveAction;

    [Header("Wheel Colliders")]
    public WheelCollider wheelFL;
    public WheelCollider wheelFR;
    public WheelCollider wheelRL;
    public WheelCollider wheelRR;

    [Header("Wheel Meshes")]
    public Transform meshFL;
    public Transform meshFR;
    public Transform meshRL;
    public Transform meshRR;

    [Header("Car Settings")]
    public float maxMotorTorque = 300f;
    public float maxSteerAngle = 25f;
    public float brakeForce = 500f;

    private Vector2 moveInput;
    [SerializeField] private Rigidbody rb;

    void Start()
    {
        rb.centerOfMass = new Vector3(0, -0.5f, 0);
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
        moveAction.action.performed += OnMove;
        moveAction.action.canceled += OnMove;
    }

    private void OnDisable()
    {
        moveAction.action.performed -= OnMove;
        moveAction.action.canceled -= OnMove;
        moveAction.action.Disable();
    }

    private void FixedUpdate()
    {
        ApplySteering();
        ApplyMotor();
        UpdateWheelMeshes();
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    private void ApplySteering()
    {
        float steer = moveInput.x * maxSteerAngle;

        wheelFL.steerAngle = steer;
        wheelFR.steerAngle = steer;
    }

    private void ApplyMotor()
    {
        float motor = moveInput.y * maxMotorTorque;

        wheelRL.motorTorque = motor;
        wheelRR.motorTorque = motor;

        if (moveInput.y == 0f)
        {
            wheelRL.brakeTorque = brakeForce;
            wheelRR.brakeTorque = brakeForce;
        }
        else
        {
            wheelRL.brakeTorque = 0f;
            wheelRR.brakeTorque = 0f;
        }
    }

    private void UpdateWheelMeshes()
    {
        UpdateSingleWheel(wheelFL, meshFL);
        UpdateSingleWheel(wheelFR, meshFR);
        UpdateSingleWheel(wheelRL, meshRL);
        UpdateSingleWheel(wheelRR, meshRR);
    }

    private void UpdateSingleWheel(WheelCollider col, Transform mesh)
    {
        col.GetWorldPose(out Vector3 pos, out Quaternion rot);

        mesh.position = pos;
        mesh.rotation = rot;
    }
}
