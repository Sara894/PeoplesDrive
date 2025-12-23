using UnityEngine;
using UnityEngine.InputSystem;

namespace Ezereal
{
    public class EzerealCarController : MonoBehaviour
    {
        [Header("References")]
        public Rigidbody vehicleRB;
        public WheelCollider frontLeftWheelCollider;
        public WheelCollider frontRightWheelCollider;
        public WheelCollider rearLeftWheelCollider;
        public WheelCollider rearRightWheelCollider;

        public Transform frontLeftWheelMesh;
        public Transform frontRightWheelMesh;
        public Transform rearLeftWheelMesh;
        public Transform rearRightWheelMesh;

        [Header("Settings")]
        public float maxMotorTorque = 400f;   // Lower for gentle acceleration
        public float maxSteerAngle = 5f;      // Lower for gentle steering
        public float brakeTorque = 2000f;

        public InputAction moveAction;   // Vector2: y=forward/back, x=left/right
        public InputAction brakeAction;  // Button: brake

        private float motorInput;
        private float steerInput;
        private bool brakeInput;

        private PlayerInput playerInput;

        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            moveAction = playerInput.actions["Move"];
            brakeAction = playerInput.actions["Brake"];
        }

        private void OnEnable()
        {
            moveAction?.Enable();
            brakeAction?.Enable();
        }

        private void OnDisable()
        {
            moveAction?.Disable();
            brakeAction?.Disable();
        }

        private void Update()
        {
            Vector2 move = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;
            motorInput = move.y;   // W/S or Up/Down
            steerInput = move.x;   // A/D or Left/Right
            brakeInput = brakeAction != null && brakeAction.ReadValue<float>() > 0.5f;
        }

        private void FixedUpdate()
        {
            // Motor torque (all wheels for simplicity)
            float motor = maxMotorTorque * motorInput;
            frontLeftWheelCollider.motorTorque = motor;
            frontRightWheelCollider.motorTorque = motor;
            rearLeftWheelCollider.motorTorque = motor;
            rearRightWheelCollider.motorTorque = motor;

            // Steering (front wheels only)
            float steerAngle = maxSteerAngle * steerInput;
            frontLeftWheelCollider.steerAngle = steerAngle;
            frontRightWheelCollider.steerAngle = steerAngle;

            // Brake (all wheels)
            float appliedBrake = brakeInput ? brakeTorque : 0f;
            frontLeftWheelCollider.brakeTorque = appliedBrake;
            frontRightWheelCollider.brakeTorque = appliedBrake;
            rearLeftWheelCollider.brakeTorque = appliedBrake;
            rearRightWheelCollider.brakeTorque = appliedBrake;

            // Update wheel meshes for suspension visuals
            UpdateWheelPose(frontLeftWheelCollider, frontLeftWheelMesh);
            UpdateWheelPose(frontRightWheelCollider, frontRightWheelMesh);
            UpdateWheelPose(rearLeftWheelCollider, rearLeftWheelMesh);
            UpdateWheelPose(rearRightWheelCollider, rearRightWheelMesh);
        }

        private void UpdateWheelPose(WheelCollider collider, Transform mesh)
        {
            collider.GetWorldPose(out Vector3 pos, out Quaternion quat);
            mesh.SetPositionAndRotation(pos, quat);
        }
    }
}
