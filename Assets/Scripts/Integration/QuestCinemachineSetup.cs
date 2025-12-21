using UnityEngine;
using Cinemachine;

public class QuestCinemachineSetup : MonoBehaviour
{
    [Header("Virtual Camera Settings")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Transform vehicleTransform;

    [Header("Camera Follow Settings")]
    [SerializeField] private Vector3 followOffset = new Vector3(0, 3, -8);
    [SerializeField] private float cameraDistance = 8f;
    [SerializeField] private float cameraHeight = 3f;
    [SerializeField] private float damping = 1f;

    [Header("Auto Setup")]
    [SerializeField] private bool findVehicleAutomatically = true;
    [SerializeField] private string vehicleTag = "Player";

    private CinemachineTransposer transposer;

    private void Start()
    {
        if (virtualCamera == null)
        {
            virtualCamera = GetComponent<CinemachineVirtualCamera>();
        }

        if (findVehicleAutomatically && vehicleTransform == null)
        {
            GameObject vehicle = GameObject.FindGameObjectWithTag(vehicleTag);
            if (vehicle != null)
            {
                vehicleTransform = vehicle.transform;
                Debug.Log($"QuestCinemachineSetup: Found vehicle '{vehicle.name}' with tag '{vehicleTag}'");
            }
            else
            {
                Debug.LogWarning($"QuestCinemachineSetup: No GameObject with tag '{vehicleTag}' found!");
            }
        }

        SetupCamera();
    }

    private void SetupCamera()
    {
        if (virtualCamera == null || vehicleTransform == null)
        {
            Debug.LogError("QuestCinemachineSetup: Missing VirtualCamera or Vehicle Transform!");
            return;
        }

        virtualCamera.Follow = vehicleTransform;
        virtualCamera.LookAt = vehicleTransform;

        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        if (transposer != null)
        {
            transposer.m_FollowOffset = followOffset;
            transposer.m_XDamping = damping;
            transposer.m_YDamping = damping;
            transposer.m_ZDamping = damping;
        }
        else
        {
            virtualCamera.AddCinemachineComponent<CinemachineTransposer>();
            transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            if (transposer != null)
            {
                transposer.m_FollowOffset = followOffset;
                transposer.m_XDamping = damping;
                transposer.m_YDamping = damping;
                transposer.m_ZDamping = damping;
            }
        }

        Debug.Log($"QuestCinemachineSetup: Camera configured to follow '{vehicleTransform.name}'");
    }

    public void SetFollowTarget(Transform target)
    {
        vehicleTransform = target;
        SetupCamera();
    }

    public void SetCameraDistance(float distance)
    {
        cameraDistance = distance;
        followOffset.z = -distance;
        if (transposer != null)
        {
            transposer.m_FollowOffset = followOffset;
        }
    }

    public void SetCameraHeight(float height)
    {
        cameraHeight = height;
        followOffset.y = height;
        if (transposer != null)
        {
            transposer.m_FollowOffset = followOffset;
        }
    }
}
