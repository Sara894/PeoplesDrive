using UnityEngine;

public class SurfaceByLayer : MonoBehaviour
{
    [Header("Layer Settings")]
    public float mainRoadSpeed = 1f;
    public float mainRoadPower = 1f;

    public float sideRoadSpeed = 0.8f;
    public float sideRoadPower = 0.85f;

    public float offRoadSpeed = 0.55f;
    public float offRoadPower = 0.65f;

    public void GetMultipliers(int layer, out float speed, out float power)
    {
        if (layer == LayerMask.NameToLayer("MainRoad"))
        {
            speed = mainRoadSpeed;
            power = mainRoadPower;
            return;
        }

        if (layer == LayerMask.NameToLayer("SideRoad"))
        {
            speed = sideRoadSpeed;
            power = sideRoadPower;
            return;
        }

        if (layer == LayerMask.NameToLayer("OffRoad"))
        {
            speed = offRoadSpeed;
            power = offRoadPower;
            return;
        }

        // Default
        speed = 1f;
        power = 1f;
    }
}

