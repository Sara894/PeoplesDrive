using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

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
        public float maxMotorTorque = 200f;
        public float maxSteerAngle = 5f;
        public float brakeTorque = 2000f;
        public float maxSpeed = 50f;

        public InputAction moveAction;
        public InputAction brakeAction;

        private float motorInput;
        private float steerInput;
        private bool brakeInput;

        private PlayerInput playerInput;

        public TMPro.TextMeshProUGUI speedText;

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
            motorInput = move.y; 
            steerInput = move.x;
            brakeInput = brakeAction != null && brakeAction.ReadValue<float>() > 0.5f;

             if (speedText != null)
            {
                float speedKmh = vehicleRB.velocity.magnitude * 3.6f;
                speedText.text = $"{speedKmh:F0} km/h";
            }
        }

        private void FixedUpdate()
        {
            float speed = vehicleRB.velocity.magnitude;

            float motor = 0f;
            if (Mathf.Abs(motorInput) > 0.01f && speed < maxSpeed)
            {
                // High torque at low speed, low torque at high speed
                float accelFactor = Mathf.Lerp(1.5f, 0.1f, speed / maxSpeed);
                motor = maxMotorTorque * motorInput * accelFactor;
            }
            frontLeftWheelCollider.motorTorque = motor;
            frontRightWheelCollider.motorTorque = motor;
            rearLeftWheelCollider.motorTorque = motor;
            rearRightWheelCollider.motorTorque = motor;

            float steerAngle = maxSteerAngle * steerInput;
            frontLeftWheelCollider.steerAngle = steerAngle;
            frontRightWheelCollider.steerAngle = steerAngle;

            float appliedBrake = 0f;
            if (brakeInput)
            {
                appliedBrake = brakeTorque;
            }
            else if (Mathf.Approximately(motorInput, 0f))
            {
                appliedBrake = brakeTorque;
            }
            frontLeftWheelCollider.brakeTorque = appliedBrake;
            frontRightWheelCollider.brakeTorque = appliedBrake;
            rearLeftWheelCollider.brakeTorque = appliedBrake;
            rearRightWheelCollider.brakeTorque = appliedBrake;

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
