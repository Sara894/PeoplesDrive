using UnityEngine;

public class CarSurfaceController : MonoBehaviour
{
    public float SpeedMultiplier { get; private set; } = 1f;
    public float PowerMultiplier { get; private set; } = 1f;

    [Header("Wheel Setup")]
    [SerializeField] WheelCollider[] wheels;

    void FixedUpdate()
    {
        UpdateSurface();
    }

    void UpdateSurface()
    {
        float speedSum = 0f;
        float powerSum = 0f;
        int grounded = 0;

        foreach (var wheel in wheels)
        {
            if (wheel.GetGroundHit(out WheelHit hit))
            {
                SurfaceData surface = hit.collider.GetComponent<SurfaceData>();
                if (surface != null)
                {
                    speedSum += surface.speedMultiplier;
                    powerSum += surface.powerMultiplier;
                }
                else
                {
                    speedSum += 1f;
                    powerSum += 1f;
                }

                grounded++;
            }
        }

        if (grounded > 0)
        {
            SpeedMultiplier = speedSum / grounded;
            PowerMultiplier = powerSum / grounded;
        }
        else
        {
            SpeedMultiplier = 1f;
            PowerMultiplier = 1f;
        }
    }
}

