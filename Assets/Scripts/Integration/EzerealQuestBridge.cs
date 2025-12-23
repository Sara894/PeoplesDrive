using UnityEngine;
using UnityEngine.InputSystem;
using Ezereal;

public class EzerealQuestBridge : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EzerealCarController carController;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Rigidbody vehicleRB;

    [Header("Movement Lock Settings")]
    [SerializeField] private bool lockPhysicsOnStop = true;
    [SerializeField] private bool lockInputOnStop = true;

    private bool isMovementLocked = false;
    private RigidbodyConstraints originalConstraints;

    private void Awake()
    {
        if (carController == null)
            carController = GetComponent<EzerealCarController>();

        if (playerInput == null)
            playerInput = GetComponent<PlayerInput>();

        if (vehicleRB == null)
            vehicleRB = carController != null ? carController.vehicleRB : null;

        if (vehicleRB != null)
        {
            originalConstraints = vehicleRB.constraints;
        }
    }

    private void Start()
    {
        SubscribeToEvents();

        if (!CompareTag("Player"))
        {
            Debug.LogWarning("EzerealQuestBridge: GameObject is not tagged as 'Player'! Quest triggers will not work. Please set tag to 'Player'.");
        }
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.playerEvents.onDisablePlayerMovement += StopMovement;
            GameEventsManager.instance.playerEvents.onEnablePlayerMovement += ResumeMovement;
            Debug.Log("EzerealQuestBridge: Subscribed to player movement events");
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.playerEvents.onDisablePlayerMovement -= StopMovement;
            GameEventsManager.instance.playerEvents.onEnablePlayerMovement -= ResumeMovement;
        }
    }

    public void StopMovement()
    {
        if (isMovementLocked) return;

        isMovementLocked = true;

        if (lockInputOnStop && playerInput != null)
        {
            playerInput.DeactivateInput();
        }

        if (lockPhysicsOnStop && vehicleRB != null)
        {
            vehicleRB.velocity = Vector3.zero;
            vehicleRB.angularVelocity = Vector3.zero;
            vehicleRB.constraints = RigidbodyConstraints.FreezeAll;
        }

        Debug.Log("EzerealQuestBridge: Vehicle movement stopped (dialogue/quest)");
    }

    public void ResumeMovement()
    {
        if (!isMovementLocked) return;

        isMovementLocked = false;

        if (lockInputOnStop && playerInput != null)
        {
            playerInput.ActivateInput();
        }

        if (lockPhysicsOnStop && vehicleRB != null)
        {
            vehicleRB.constraints = originalConstraints;
        }

        Debug.Log("EzerealQuestBridge: Vehicle movement resumed");
    }

    public bool IsMovementLocked()
    {
        return isMovementLocked;
    }

    public Vector3 GetVehiclePosition()
    {
        return transform.position;
    }

    public Transform GetVehicleTransform()
    {
        return transform;
    }
}
