using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

namespace Ezereal
{
    public class EzerealCarController : MonoBehaviour
    {
        [Header("Ezereal References")]
        [SerializeField] EzerealLightController ezerealLightController;
        [SerializeField] EzerealSoundController ezerealSoundController;
        [SerializeField] EzerealWheelFrictionController ezerealWheelFrictionController;
        [SerializeField] CarSurfaceController surfaceController;

        [Header("References")]
        public Rigidbody vehicleRB;
        public WheelCollider frontLeftWheelCollider;
        public WheelCollider frontRightWheelCollider;
        public WheelCollider rearLeftWheelCollider;
        public WheelCollider rearRightWheelCollider;
        WheelCollider[] wheels;

        [SerializeField] Transform frontLeftWheelMesh;
        [SerializeField] Transform frontRightWheelMesh;
        [SerializeField] Transform rearLeftWheelMesh;
        [SerializeField] Transform rearRightWheelMesh;
        [SerializeField] Transform steeringWheel;

        [SerializeField] TMP_Text currentSpeedTMP_UI;
        [SerializeField] TMP_Text currentSpeedTMP_Dashboard;
        [SerializeField] Slider accelerationSlider;

        [Header("Settings")]
        public bool isStarted = true;
        public float maxForwardSpeed = 140f;
        public float maxReverseSpeed = 30f;
        public float horsePower = 700f;
        public float brakePower = 2000f;
        public float handbrakeForce = 3000f;
        public float maxSteerAngle = 15f;
        public float steeringSpeed = 2.5f;
        public float stopThreshold = 1f;
        public float decelerationSpeed = 0.2f;
        public float maxSteeringWheelRotation = 360f;

        [Header("Debug Info")]
        [SerializeField] float currentSpeed = 0f;
        [SerializeField] float currentAccelerationValue = 0f;
        [SerializeField] float currentBrakeValue = 0f;
        [SerializeField] float currentHandbrakeValue = 0f;
        [SerializeField] float currentSteerAngle = 0f;
        [SerializeField] float targetSteerAngle = 0f;
        [SerializeField] float FrontLeftWheelRPM = 0f;
        [SerializeField] float FrontRightWheelRPM = 0f;
        [SerializeField] float RearLeftWheelRPM = 0f;
        [SerializeField] float RearRightWheelRPM = 0f;
        [SerializeField] float speedFactor = 0f;

        private void Awake()
        {
            wheels = new WheelCollider[]
            {
                frontLeftWheelCollider,
                frontRightWheelCollider,
                rearLeftWheelCollider,
                rearRightWheelCollider,
            };

            if (surfaceController == null)
                surfaceController = GetComponent<CarSurfaceController>();

            if (ezerealLightController == null)
                Debug.LogWarning("EzerealLightController reference is missing. Ignore or attach one if you want to have light controls.");

            if (ezerealSoundController == null)
                Debug.LogWarning("EzerealSoundController reference is missing. Ignore or attach one if you want to have engine sounds.");

            if (ezerealWheelFrictionController == null)
                Debug.LogWarning("EzerealWheelFrictionController reference is missing. Ignore or attach one if you want to have friction controls.");

            if (vehicleRB == null)
                Debug.LogError("VehicleRB reference is missing for EzerealCarController!");

            if (isStarted)
            {
                Debug.Log("Car is started.");
                if (ezerealLightController != null)
                    ezerealLightController.MiscLightsOn();
                if (ezerealSoundController != null)
                    ezerealSoundController.TurnOnEngineSound();
            }
        }

        void OnStartCar()
        {
            isStarted = !isStarted;
            if (isStarted)
            {
                Debug.Log("Car started.");
                if (ezerealLightController != null)
                    ezerealLightController.MiscLightsOn();
                if (ezerealSoundController != null)
                    ezerealSoundController.TurnOnEngineSound();
            }
            else
            {
                Debug.Log("Car turned off");
                if (ezerealLightController != null)
                    ezerealLightController.AllLightsOff();
                if (ezerealSoundController != null)
                    ezerealSoundController.TurnOffEngineSound();
                frontLeftWheelCollider.motorTorque = 0;
                frontRightWheelCollider.motorTorque = 0;
                rearLeftWheelCollider.motorTorque = 0;
                rearRightWheelCollider.motorTorque = 0;
            }
        }

        void OnAccelerate(InputValue accelerationValue)
        {
            currentAccelerationValue = accelerationValue.Get<float>();
        }

        void Acceleration()
        {
            if (!isStarted)
                return;

            float speedMul = surfaceController != null ? surfaceController.SpeedMultiplier : 1f;
            float powerMul = surfaceController != null ? surfaceController.PowerMultiplier : 1f;
            float currentMotorTorque = horsePower * powerMul;

            if (currentAccelerationValue > 0f && currentSpeed < maxForwardSpeed)
            {
                frontLeftWheelCollider.motorTorque = currentMotorTorque * currentAccelerationValue;
                frontRightWheelCollider.motorTorque = currentMotorTorque * currentAccelerationValue;
                rearLeftWheelCollider.motorTorque = currentMotorTorque * currentAccelerationValue;
                rearRightWheelCollider.motorTorque = currentMotorTorque * currentAccelerationValue;
            }
            else if (currentAccelerationValue < 0f && currentSpeed > -maxReverseSpeed)
            {
                frontLeftWheelCollider.motorTorque = currentMotorTorque * currentAccelerationValue;
                frontRightWheelCollider.motorTorque = currentMotorTorque * currentAccelerationValue;
                rearLeftWheelCollider.motorTorque = currentMotorTorque * currentAccelerationValue;
                rearRightWheelCollider.motorTorque = currentMotorTorque * currentAccelerationValue;
            }
            else
            {
                frontLeftWheelCollider.motorTorque = 0;
                frontRightWheelCollider.motorTorque = 0;
                rearLeftWheelCollider.motorTorque = 0;
                rearRightWheelCollider.motorTorque = 0;
            }

            UpdateAccelerationSlider();
        }

        void OnBrake(InputValue brakeValue)
        {
            currentBrakeValue = brakeValue.Get<float>();
            if (isStarted && ezerealLightController != null)
            {
                if (currentBrakeValue > 0)
                    ezerealLightController.BrakeLightsOn();
                else
                    ezerealLightController.BrakeLightsOff();
            }
        }

        void Braking()
        {
            if (currentBrakeValue > 0f)
            {
                frontLeftWheelCollider.brakeTorque = currentBrakeValue * brakePower;
                frontRightWheelCollider.brakeTorque = currentBrakeValue * brakePower;
                rearLeftWheelCollider.brakeTorque = currentBrakeValue * brakePower;
                rearRightWheelCollider.brakeTorque = currentBrakeValue * brakePower;
            }
            else
            {
                frontLeftWheelCollider.brakeTorque = 0;
                frontRightWheelCollider.brakeTorque = 0;
                rearLeftWheelCollider.brakeTorque = 0;
                rearRightWheelCollider.brakeTorque = 0;
            }
        }

        void OnHandbrake(InputValue handbrakeValue)
        {
            currentHandbrakeValue = handbrakeValue.Get<float>();
            if (isStarted)
            {
                if (currentHandbrakeValue > 0)
                {
                    if (ezerealWheelFrictionController != null)
                        ezerealWheelFrictionController.StartDrifting(currentHandbrakeValue);
                    if (ezerealLightController != null)
                        ezerealLightController.HandbrakeLightOn();
                }
                else
                {
                    if (ezerealWheelFrictionController != null)
                        ezerealWheelFrictionController.StopDrifting();
                    if (ezerealLightController != null)
                        ezerealLightController.HandbrakeLightOff();
                }
            }
        }

        void Handbraking()
        {
            if (currentHandbrakeValue > 0f)
            {
                rearLeftWheelCollider.motorTorque = 0;
                rearRightWheelCollider.motorTorque = 0;
                rearLeftWheelCollider.brakeTorque = currentHandbrakeValue * handbrakeForce;
                rearRightWheelCollider.brakeTorque = currentHandbrakeValue * handbrakeForce;
            }
            else
            {
                rearLeftWheelCollider.brakeTorque = 0;
                rearRightWheelCollider.brakeTorque = 0;
            }
        }

        void OnSteer(InputValue turnValue)
        {
            targetSteerAngle = turnValue.Get<float>() * maxSteerAngle;
        }

        void Steering()
        {
            float adjustedspeedFactor = Mathf.InverseLerp(20, maxForwardSpeed, currentSpeed);
            float adjustedTurnAngle = targetSteerAngle * (1 - adjustedspeedFactor);
            currentSteerAngle = Mathf.Lerp(currentSteerAngle, adjustedTurnAngle, Time.deltaTime * steeringSpeed);

            frontLeftWheelCollider.steerAngle = currentSteerAngle;
            frontRightWheelCollider.steerAngle = currentSteerAngle;

            UpdateWheel(frontLeftWheelCollider, frontLeftWheelMesh);
            UpdateWheel(frontRightWheelCollider, frontRightWheelMesh);
            UpdateWheel(rearLeftWheelCollider, rearLeftWheelMesh);
            UpdateWheel(rearRightWheelCollider, rearRightWheelMesh);
        }

        void Slowdown()
        {
            if (vehicleRB != null)
            {
                if (currentAccelerationValue == 0 && currentBrakeValue == 0 && currentHandbrakeValue == 0)
                {
#if UNITY_6000_0_OR_NEWER
                    vehicleRB.linearVelocity = Vector3.Lerp(vehicleRB.linearVelocity, Vector3.zero, Time.deltaTime * decelerationSpeed);
#else
                    vehicleRB.velocity = Vector3.Lerp(vehicleRB.velocity, Vector3.zero, Time.deltaTime * decelerationSpeed);
#endif
                }
            }
        }

        private void FixedUpdate()
        {
            Acceleration();
            Braking();
            Handbraking();
            Steering();
            Slowdown();
            RotateSteeringWheel();

            if (vehicleRB != null)
            {
#if UNITY_6000_0_OR_NEWER
                currentSpeed = Vector3.Dot(vehicleRB.gameObject.transform.forward, vehicleRB.linearVelocity);
                currentSpeed *= 3.6f;
                UpdateSpeedText(currentSpeed);
#else
                currentSpeed = Vector3.Dot(vehicleRB.gameObject.transform.forward, vehicleRB.velocity);
                currentSpeed *= 3.6f;
                UpdateSpeedText(currentSpeed);
#endif
            }

            FrontLeftWheelRPM = frontLeftWheelCollider.rpm;
            FrontRightWheelRPM = frontRightWheelCollider.rpm;
            RearLeftWheelRPM = rearLeftWheelCollider.rpm;
            RearRightWheelRPM = rearRightWheelCollider.rpm;
        }

        private void UpdateWheel(WheelCollider col, Transform mesh)
        {
            col.GetWorldPose(out Vector3 position, out Quaternion rotation);
            mesh.SetPositionAndRotation(position, rotation);
        }

        void RotateSteeringWheel()
        {
            float currentXAngle = steeringWheel.transform.localEulerAngles.x;
            float normalizedSteerAngle = Mathf.Clamp(frontLeftWheelCollider.steerAngle, -maxSteerAngle, maxSteerAngle);
            float rotation = Mathf.Lerp(maxSteeringWheelRotation, -maxSteeringWheelRotation, (normalizedSteerAngle + maxSteerAngle) / (2 * maxSteerAngle));
            steeringWheel.localRotation = Quaternion.Euler(currentXAngle, 0, rotation);
        }

        void UpdateSpeedText(float speed)
        {
            speed = Mathf.Abs(speed);
            currentSpeedTMP_UI.text = speed.ToString("F0");
            currentSpeedTMP_Dashboard.text = speed.ToString("F0");
        }

        void UpdateAccelerationSlider()
        {
            accelerationSlider.value = Mathf.Lerp(accelerationSlider.value, Mathf.Abs(currentAccelerationValue), Time.deltaTime * 15f);
        }

        public bool InAir()
        {
            foreach (WheelCollider wheel in wheels)
            {
                if (wheel.GetGroundHit(out _))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
