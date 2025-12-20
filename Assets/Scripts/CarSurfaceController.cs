using UnityEngine;

public class CarSurfaceController : MonoBehaviour
{
    public float SpeedMultiplier { get; private set; } = 1f;
    public float PowerMultiplier { get; private set; } = 1f;

    [SerializeField] WheelCollider[] wheels;
    [SerializeField] SurfaceByLayer surfaceByLayer;

    void Awake()
    {
        if (surfaceByLayer == null)
        {
            surfaceByLayer = FindObjectOfType<SurfaceByLayer>();
        }
    }

    void FixedUpdate()
    {
        UpdateSurface();
    }

    void UpdateSurface()
    {
        float speedSum = 0f;
        float powerSum = 0f;
        int grounded = 0;

        foreach (WheelCollider wheel in wheels)
        {
            if (wheel.GetGroundHit(out WheelHit hit))
            {
                surfaceByLayer.GetMultipliers(hit.collider.gameObject.layer,
                    out float speed,
                    out float power);

                speedSum += speed;
                powerSum += power;
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
