using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Spawn Configuration")]
    [Tooltip("Optional: Leave empty to use player's initial position in scene")]
    [SerializeField] private Transform spawnPoint;
    
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Transform playerTransform;
    
    private void Awake()
    {
        if (spawnPoint != null)
        {
            startPosition = spawnPoint.position;
            startRotation = spawnPoint.rotation;
        }
    }
    
    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            
            if (spawnPoint == null)
            {
                startPosition = playerTransform.position;
                startRotation = playerTransform.rotation;
                Debug.Log($"PlayerSpawner: Captured initial player position: {startPosition}");
            }
            else
            {
                Debug.Log($"PlayerSpawner: Using spawn point at position: {startPosition}");
            }
        }
        else
        {
            Debug.LogError("PlayerSpawner: No GameObject with 'Player' tag found!");
        }
    }
    
    public void RespawnPlayer()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                Debug.LogError("PlayerSpawner: Cannot respawn - no player found!");
                return;
            }
        }
        
        playerTransform.position = startPosition;
        playerTransform.rotation = startRotation;
        
        Rigidbody rb = playerTransform.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        
        Debug.Log($"PlayerSpawner: Player respawned at {startPosition}");
    }
}
